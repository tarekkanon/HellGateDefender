using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Component attached to VFX prefabs for enhanced control and callbacks.
    /// Allows VFX to trigger events, play audio, and handle custom behavior.
    /// Supports both single-phase VFX (with ParticleSystem on same GameObject) and
    /// multi-phase VFX (with child ParticleSystems).
    /// </summary>
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
        private ParticleSystem[] _allParticleSystems;
        private float _startTime;
        private bool _hasPlayed = false;
        private bool _isMultiPhase = false;

        #region Properties

        public VFXType VFXType => vfxType;
        public ParticleSystem ParticleSystem => _particleSystem;
        public ParticleSystem[] AllParticleSystems => _allParticleSystems;
        public bool IsMultiPhase => _isMultiPhase;
        public bool IsPlaying
        {
            get
            {
                if (_allParticleSystems == null || _allParticleSystems.Length == 0) return false;

                // Return true if any particle system is playing
                foreach (var ps in _allParticleSystems)
                {
                    if (ps != null && ps.isPlaying)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Try to get ParticleSystem on this GameObject first (single-phase)
            _particleSystem = GetComponent<ParticleSystem>();

            // Get all particle systems (includes this GameObject and children)
            _allParticleSystems = GetComponentsInChildren<ParticleSystem>();

            // Determine if this is a multi-phase effect
            _isMultiPhase = _particleSystem == null && _allParticleSystems.Length > 0;

            if (_isMultiPhase)
            {
                // For multi-phase, use the first child as the main particle system for duration calculations
                _particleSystem = _allParticleSystems[0];
            }

            if (_particleSystem == null && _allParticleSystems.Length == 0)
            {
                Debug.LogError($"VFXController on '{gameObject.name}' requires at least one ParticleSystem component (either on this GameObject or its children)!");
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
            if (autoDestroy && _allParticleSystems != null && _allParticleSystems.Length > 0)
            {
                float lifetime = customLifetime;

                // Calculate lifetime from particle systems if not custom set
                if (customLifetime <= 0 && _particleSystem != null)
                {
                    lifetime = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
                }

                // Check if all particle systems are finished
                bool allFinished = true;
                int totalParticleCount = 0;

                foreach (var ps in _allParticleSystems)
                {
                    if (ps != null)
                    {
                        totalParticleCount += ps.particleCount;
                        if (ps.isPlaying)
                        {
                            allFinished = false;
                        }
                    }
                }

                if (Time.time - _startTime >= lifetime && totalParticleCount == 0 && allFinished)
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
        /// Play the VFX effect (supports both single and multi-phase VFX)
        /// </summary>
        public void Play()
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            // Play all particle systems
            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Play(true);
                }
            }

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
        /// Stop the VFX effect (supports both single and multi-phase VFX)
        /// </summary>
        public void Stop(bool clearParticles = true)
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            var stopBehavior = clearParticles ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting;

            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Stop(true, stopBehavior);
                }
            }

            if (enableCallbacks)
            {
                OnVFXStopped?.Invoke();
            }
        }

        /// <summary>
        /// Pause the VFX effect (supports both single and multi-phase VFX)
        /// </summary>
        public void Pause()
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Pause(true);
                }
            }
        }

        /// <summary>
        /// Resume the VFX effect (supports both single and multi-phase VFX)
        /// </summary>
        public void Resume()
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Play(true);
                }
            }
        }

        /// <summary>
        /// Set the color of all particle systems
        /// </summary>
        public void SetColor(Color color)
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = color;
                }
            }
        }

        /// <summary>
        /// Set the scale of all particle systems
        /// </summary>
        public void SetScale(float scale)
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    var main = ps.main;
                    main.startSizeMultiplier = scale;
                }
            }
        }

        /// <summary>
        /// Set the emission rate multiplier for all particle systems
        /// </summary>
        public void SetEmissionMultiplier(float multiplier)
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0) return;

            foreach (var ps in _allParticleSystems)
            {
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.rateOverTimeMultiplier = multiplier;
                }
            }
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
            if (_allParticleSystems == null || _allParticleSystems.Length == 0)
            {
                _allParticleSystems = GetComponentsInChildren<ParticleSystem>();
            }

            if (_allParticleSystems != null && _allParticleSystems.Length > 0)
            {
                foreach (var ps in _allParticleSystems)
                {
                    if (ps != null)
                    {
                        ps.Play(true);
                    }
                }
                Debug.Log($"Playing VFX: {vfxType} ({_allParticleSystems.Length} particle system{(_allParticleSystems.Length > 1 ? "s" : "")})");
            }
        }

        [ContextMenu("Stop VFX")]
        private void EditorStopVFX()
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0)
            {
                _allParticleSystems = GetComponentsInChildren<ParticleSystem>();
            }

            if (_allParticleSystems != null && _allParticleSystems.Length > 0)
            {
                foreach (var ps in _allParticleSystems)
                {
                    if (ps != null)
                    {
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                        ps.Clear();
                    }
                }
                Debug.Log($"Stopped VFX: {vfxType}");
            }
        }

        private void OnValidate()
        {
            // Update particle system references
            _particleSystem = GetComponent<ParticleSystem>();
            _allParticleSystems = GetComponentsInChildren<ParticleSystem>();
            _isMultiPhase = _particleSystem == null && _allParticleSystems.Length > 0;

            if (_isMultiPhase && _allParticleSystems.Length > 0)
            {
                _particleSystem = _allParticleSystems[0];
            }

            // Auto-name the GameObject based on VFX type
            string expectedName = $"FX_{vfxType}";
            if (!gameObject.name.Contains(vfxType.ToString()))
            {
                gameObject.name = expectedName;
            }

            // Log multi-phase status for clarity
            if (_isMultiPhase)
            {
                Debug.Log($"VFXController: '{gameObject.name}' is a multi-phase VFX with {_allParticleSystems.Length} particle systems.");
            }
        }
#endif

        #endregion
    }
}
