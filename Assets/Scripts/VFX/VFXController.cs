using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Component attached to VFX prefabs for enhanced control and callbacks.
    /// Allows VFX to trigger events, play audio, and handle custom behavior.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXController : MonoBehaviour
    {
        [Header("VFX Settings")]
        [Tooltip("Type of this VFX effect")]
        [SerializeField] private VFXType vfxType;

        [Tooltip("Should this effect auto-destroy when finished?")]
        [SerializeField] private bool autoDestroy = false;

        [Tooltip("Custom lifetime override (0 = use particle system duration)")]
        [SerializeField] private float customLifetime = 0f;

        [Header("Audio")]
        [Tooltip("Play audio clip on start")]
        [SerializeField] private bool playAudioOnStart = false;

        [SerializeField] private AudioClip audioClip;

        [Range(0f, 1f)]
        [SerializeField] private float audioVolume = 1f;

        [Header("Effects")]
        [Tooltip("Apply screen shake on start")]
        [SerializeField] private bool applyScreenShake = false;

        [Range(0f, 1f)]
        [SerializeField] private float shakeIntensity = 0.1f;

        [SerializeField] private float shakeDuration = 0.2f;

        [Header("Callbacks")]
        [Tooltip("Enable event callbacks")]
        [SerializeField] private bool enableCallbacks = false;

        // Events
        public System.Action OnVFXStarted;
        public System.Action OnVFXStopped;
        public System.Action OnVFXCompleted;

        private ParticleSystem _particleSystem;
        private float _startTime;
        private bool _hasPlayed = false;

        #region Properties

        public VFXType VFXType => vfxType;
        public ParticleSystem ParticleSystem => _particleSystem;
        public bool IsPlaying => _particleSystem != null && _particleSystem.isPlaying;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            if (_particleSystem == null)
            {
                Debug.LogError($"VFXController on '{gameObject.name}' requires a ParticleSystem component!");
            }
        }

        private void OnEnable()
        {
            _startTime = Time.time;
            _hasPlayed = false;

            if (_particleSystem != null && !_particleSystem.isPlaying)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            if (_hasPlayed && enableCallbacks)
            {
                OnVFXStopped?.Invoke();
            }
        }

        private void Update()
        {
            // Handle auto-destroy
            if (autoDestroy && _particleSystem != null)
            {
                float lifetime = customLifetime > 0 ? customLifetime : _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;

                if (Time.time - _startTime >= lifetime && _particleSystem.particleCount == 0)
                {
                    if (enableCallbacks)
                    {
                        OnVFXCompleted?.Invoke();
                    }

                    Destroy(gameObject);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Play the VFX effect
        /// </summary>
        public void Play()
        {
            if (_particleSystem == null) return;

            _particleSystem.Play(true);
            _hasPlayed = true;
            _startTime = Time.time;

            // Handle audio
            if (playAudioOnStart && audioClip != null)
            {
                PlayAudio();
            }

            // Handle screen shake
            if (applyScreenShake)
            {
                // TODO: Integrate with camera shake system if available
                // CameraShake.Instance?.Shake(shakeIntensity, shakeDuration);
            }

            // Invoke callback
            if (enableCallbacks)
            {
                OnVFXStarted?.Invoke();
            }
        }

        /// <summary>
        /// Stop the VFX effect
        /// </summary>
        public void Stop(bool clearParticles = true)
        {
            if (_particleSystem == null) return;

            if (clearParticles)
            {
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            else
            {
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            if (enableCallbacks)
            {
                OnVFXStopped?.Invoke();
            }
        }

        /// <summary>
        /// Pause the VFX effect
        /// </summary>
        public void Pause()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Pause(true);
            }
        }

        /// <summary>
        /// Resume the VFX effect
        /// </summary>
        public void Resume()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Play(true);
            }
        }

        /// <summary>
        /// Set the color of the particle system
        /// </summary>
        public void SetColor(Color color)
        {
            if (_particleSystem == null) return;

            var main = _particleSystem.main;
            main.startColor = color;
        }

        /// <summary>
        /// Set the scale of the particle system
        /// </summary>
        public void SetScale(float scale)
        {
            if (_particleSystem == null) return;

            var main = _particleSystem.main;
            main.startSizeMultiplier = scale;
        }

        /// <summary>
        /// Set the emission rate multiplier
        /// </summary>
        public void SetEmissionMultiplier(float multiplier)
        {
            if (_particleSystem == null) return;

            var emission = _particleSystem.emission;
            emission.rateOverTimeMultiplier = multiplier;
        }

        #endregion

        #region Private Methods

        private void PlayAudio()
        {
            if (audioClip == null) return;

            // Create a temporary audio source for one-shot playback
            GameObject audioObject = new GameObject($"VFXAudio_{vfxType}");
            audioObject.transform.position = transform.position;
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.clip = audioClip;
            source.volume = audioVolume;
            source.spatialBlend = 0.5f; // Semi-spatial
            source.Play();

            Destroy(audioObject, audioClip.length + 0.1f);
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        [ContextMenu("Play VFX")]
        private void EditorPlayVFX()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            if (_particleSystem != null)
            {
                _particleSystem.Play();
                Debug.Log($"Playing VFX: {vfxType}");
            }
        }

        [ContextMenu("Stop VFX")]
        private void EditorStopVFX()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            if (_particleSystem != null)
            {
                _particleSystem.Stop();
                _particleSystem.Clear();
                Debug.Log($"Stopped VFX: {vfxType}");
            }
        }

        private void OnValidate()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            // Auto-name the GameObject based on VFX type
            if (gameObject.name != $"FX_{vfxType}")
            {
                gameObject.name = $"FX_{vfxType}";
            }
        }
#endif

        #endregion
    }
}
