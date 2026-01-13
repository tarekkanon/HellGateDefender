using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseDefender.Core;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Centralized VFX management system for the game.
    /// Handles particle effect pooling, spawning, audio synchronization, and performance optimization.
    /// Singleton pattern ensures single instance throughout the game.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }

        [Header("VFX Library")]
        [Tooltip("ScriptableObject containing all VFX effect definitions")]
        [SerializeField] private VFXLibrary vfxLibrary;

        [Header("Performance Settings")]
        [Tooltip("Maximum total particles allowed on screen at once")]
        [SerializeField] private int maxParticlesOnScreen = 200;

        [Tooltip("Enable performance-based effect culling")]
        [SerializeField] private bool enablePerformanceCulling = true;

        [Tooltip("Enable distance-based LOD for effects")]
        [SerializeField] private bool enableDistanceLOD = true;

        [Header("Audio Integration")]
        [Tooltip("Enable automatic audio synchronization")]
        [SerializeField] private bool enableAudioSync = true;

        [Tooltip("Master volume multiplier for all VFX audio")]
        [Range(0f, 1f)]
        [SerializeField] private float vfxAudioVolume = 1f;

        [Header("Debug")]
        [Tooltip("Show debug information in console")]
        [SerializeField] private bool showDebugInfo = false;

        // Pool management
        private Dictionary<VFXType, ObjectPool<ParticleSystem>> _effectPools;
        private Dictionary<ParticleSystem, VFXType> _activeEffects;
        private Dictionary<ParticleSystem, Coroutine> _returnCoroutines;

        // Performance tracking
        private int _currentActiveParticles = 0;
        private Camera _mainCamera;

        #region Initialization

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeVFXSystem();
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogWarning("VFXManager: Main camera not found! Distance-based LOD will be disabled.");
            }
        }

        private void InitializeVFXSystem()
        {
            if (vfxLibrary == null)
            {
                Debug.LogError("VFXManager: VFX Library not assigned! Please assign a VFXLibrary in the inspector.");
                return;
            }

            // Initialize library
            vfxLibrary.Initialize();

            // Initialize collections
            _effectPools = new Dictionary<VFXType, ObjectPool<ParticleSystem>>();
            _activeEffects = new Dictionary<ParticleSystem, VFXType>();
            _returnCoroutines = new Dictionary<ParticleSystem, Coroutine>();

            // Create pools for all effects in library
            CreateAllPools();

            if (showDebugInfo)
            {
                Debug.Log($"VFXManager: Initialized with {_effectPools.Count} effect pools.");
            }
        }

        private void CreateAllPools()
        {
            // Get all VFX types and create pools
            foreach (VFXType vfxType in System.Enum.GetValues(typeof(VFXType)))
            {
                VFXData effectData = vfxLibrary.GetEffect(vfxType);
                if (effectData != null && effectData.IsValid())
                {
                    CreatePool(vfxType, effectData);
                }
            }
        }

        private void CreatePool(VFXType vfxType, VFXData effectData)
        {
            // Create pool using the generic ObjectPool
            ObjectPool<ParticleSystem> pool = new ObjectPool<ParticleSystem>(
                prefab: effectData.prefab.gameObject,
                initialSize: effectData.initialPoolSize,
                maxSize: effectData.maxPoolSize,
                expandable: true,
                parent: transform
            );

            _effectPools[vfxType] = pool;

            if (showDebugInfo)
            {
                Debug.Log($"VFXManager: Created pool for '{vfxType}' with initial size {effectData.initialPoolSize}");
            }
        }

        #endregion

        #region Public Play Methods

        /// <summary>
        /// Play a VFX effect at the specified position
        /// </summary>
        /// <param name="vfxType">Type of VFX to play</param>
        /// <param name="position">World position to spawn effect</param>
        /// <param name="rotation">Rotation of the effect</param>
        /// <returns>The spawned ParticleSystem, or null if failed</returns>
        public ParticleSystem PlayEffect(VFXType vfxType, Vector3 position, Quaternion rotation = default)
        {
            VFXData effectData = vfxLibrary.GetEffect(vfxType);
            if (effectData == null)
            {
                return null;
            }

            // Check if effect should be played based on performance and distance
            if (!ShouldPlayEffect(position, effectData))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"VFXManager: Skipping effect '{vfxType}' due to performance/distance culling.");
                }
                return null;
            }

            // Get particle system from pool
            ParticleSystem ps = GetFromPool(vfxType, position, rotation);
            if (ps == null)
            {
                return null;
            }

            // Play audio if enabled
            if (enableAudioSync && effectData.syncWithAudio && effectData.audioClip != null)
            {
                PlayEffectAudio(effectData, position);
            }

            // Track active effect
            _activeEffects[ps] = vfxType;
            _currentActiveParticles += GetMaxParticles(ps);

            // Setup auto-return to pool
            Coroutine returnCoroutine = StartCoroutine(ReturnToPoolAfterDuration(ps, vfxType));
            _returnCoroutines[ps] = returnCoroutine;

            if (showDebugInfo)
            {
                Debug.Log($"VFXManager: Playing effect '{vfxType}' at {position}. Active particles: {_currentActiveParticles}");
            }

            return ps;
        }

        /// <summary>
        /// Play a VFX effect with color override
        /// </summary>
        public ParticleSystem PlayEffect(VFXType vfxType, Vector3 position, Color color)
        {
            ParticleSystem ps = PlayEffect(vfxType, position);
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = color;
            }
            return ps;
        }

        /// <summary>
        /// Play a VFX effect attached to a transform (follows the transform)
        /// </summary>
        public ParticleSystem PlayEffectAttached(VFXType vfxType, Transform parent, Vector3 localPosition = default)
        {
            ParticleSystem ps = PlayEffect(vfxType, parent.position + localPosition, parent.rotation);
            if (ps != null)
            {
                ps.transform.SetParent(parent);
                ps.transform.localPosition = localPosition;
            }
            return ps;
        }

        /// <summary>
        /// Play a VFX effect with audio synchronization - unified method
        /// Plays both VFX and audio together (calls AudioManager)
        /// </summary>
        public ParticleSystem PlayEffectWithAudio(VFXType vfxType, Vector3 position, System.Action audioCallback = null)
        {
            ParticleSystem ps = PlayEffect(vfxType, position);

            // If effect has custom audio callback, invoke it
            if (audioCallback != null)
            {
                VFXData effectData = vfxLibrary.GetEffect(vfxType);
                if (effectData != null && effectData.audioDelay > 0)
                {
                    StartCoroutine(InvokeAudioCallbackDelayed(audioCallback, effectData.audioDelay));
                }
                else
                {
                    audioCallback.Invoke();
                }
            }

            return ps;
        }

        /// <summary>
        /// Stop a specific active effect early
        /// </summary>
        public void StopEffect(ParticleSystem ps)
        {
            if (ps == null) return;

            if (_returnCoroutines.TryGetValue(ps, out Coroutine coroutine))
            {
                // Only stop the coroutine if it's not null (could be null if it already completed/yielded break)
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                _returnCoroutines.Remove(ps);
            }

            if (_activeEffects.TryGetValue(ps, out VFXType vfxType))
            {
                ReturnToPool(ps, vfxType);
            }
        }

        /// <summary>
        /// Stop all active effects of a specific type
        /// </summary>
        public void StopAllEffectsOfType(VFXType vfxType)
        {
            // Create a temporary list to avoid modifying collection during iteration
            List<ParticleSystem> effectsToStop = new List<ParticleSystem>();

            foreach (var kvp in _activeEffects)
            {
                if (kvp.Value == vfxType)
                {
                    effectsToStop.Add(kvp.Key);
                }
            }

            foreach (var ps in effectsToStop)
            {
                StopEffect(ps);
            }
        }

        /// <summary>
        /// Clear all active effects immediately
        /// </summary>
        public void ClearAllEffects()
        {
            // Stop all return coroutines
            foreach (var coroutine in _returnCoroutines.Values)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            _returnCoroutines.Clear();

            // Return all active effects to pools
            List<ParticleSystem> activeList = new List<ParticleSystem>(_activeEffects.Keys);
            foreach (var ps in activeList)
            {
                if (ps != null && _activeEffects.TryGetValue(ps, out VFXType vfxType))
                {
                    ReturnToPool(ps, vfxType);
                }
            }

            _activeEffects.Clear();
            _currentActiveParticles = 0;

            if (showDebugInfo)
            {
                Debug.Log("VFXManager: Cleared all active effects.");
            }
        }

        #endregion

        #region Pool Management

        private ParticleSystem GetFromPool(VFXType vfxType, Vector3 position, Quaternion rotation)
        {
            if (!_effectPools.TryGetValue(vfxType, out ObjectPool<ParticleSystem> pool))
            {
                Debug.LogWarning($"VFXManager: Pool not found for VFX type '{vfxType}'");
                return null;
            }

            // Check particle budget
            VFXData effectData = vfxLibrary.GetEffect(vfxType);
            if (enablePerformanceCulling && _currentActiveParticles >= maxParticlesOnScreen)
            {
                // Only allow critical and high priority effects when over budget
                if (effectData.GetPriority() > VFXPriority.High)
                {
                    if (showDebugInfo)
                    {
                        Debug.LogWarning($"VFXManager: Particle budget exceeded. Skipping low-priority effect '{vfxType}'.");
                    }
                    return null;
                }
            }

            // Get from pool
            if (rotation == default)
            {
                rotation = Quaternion.identity;
            }

            ParticleSystem ps = pool.Get(position, rotation);
            if (ps != null)
            {
                ps.Play(true); // Play with children
            }

            return ps;
        }

        private void ReturnToPool(ParticleSystem ps, VFXType vfxType)
        {
            if (ps == null) return;

            if (!_effectPools.TryGetValue(vfxType, out ObjectPool<ParticleSystem> pool))
            {
                Debug.LogWarning($"VFXManager: Pool not found for VFX type '{vfxType}' when returning.");
                return;
            }

            // Update particle count
            _currentActiveParticles = Mathf.Max(0, _currentActiveParticles - GetMaxParticles(ps));

            // Clear tracking
            _activeEffects.Remove(ps);
            _returnCoroutines.Remove(ps);

            // Stop and return to pool
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            pool.Return(ps);

            if (showDebugInfo)
            {
                Debug.Log($"VFXManager: Returned effect '{vfxType}' to pool. Active particles: {_currentActiveParticles}");
            }
        }

        private IEnumerator ReturnToPoolAfterDuration(ParticleSystem ps, VFXType vfxType)
        {
            if (ps == null)
            {
                yield break;
            }

            // Calculate total duration
            float duration = ps.main.duration;
            float startLifetime = ps.main.startLifetime.constantMax;
            float totalTime = duration + startLifetime + 0.5f; // Add small buffer

            // If looping, don't auto-return (needs manual stop)
            // Clean up the coroutine reference since we're exiting early
            if (ps.main.loop)
            {
                _returnCoroutines.Remove(ps);
                yield break;
            }

            // Wait for effect to complete
            yield return new WaitForSeconds(totalTime);

            // Ensure all particles are cleared
            while (ps != null && ps.particleCount > 0)
            {
                yield return null;
            }

            // Return to pool
            ReturnToPool(ps, vfxType);
        }

        #endregion

        #region Audio Integration

        private void PlayEffectAudio(VFXData effectData, Vector3 position)
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("VFXManager: AudioManager not found! Cannot play audio.");
                return;
            }

            if (effectData.audioDelay > 0)
            {
                StartCoroutine(PlayAudioDelayed(effectData.audioClip, effectData.audioVolume, effectData.audioDelay, position));
            }
            else
            {
                PlayAudioImmediate(effectData.audioClip, effectData.audioVolume);
            }
        }

        private void PlayAudioImmediate(AudioClip clip, float volume)
        {
            // Play through AudioManager's SFX source
            // Note: This assumes AudioManager has a public method to play clips
            // You may need to adjust based on your AudioManager implementation
            if (AudioManager.Instance != null)
            {
                // Temporary workaround: play through reflection or add public method to AudioManager
                // For now, we'll create a temporary audio source
                CreateOneShotAudioSource(clip, volume);
            }
        }

        private IEnumerator PlayAudioDelayed(AudioClip clip, float volume, float delay, Vector3 position)
        {
            yield return new WaitForSeconds(delay);
            PlayAudioImmediate(clip, volume);
        }

        private IEnumerator InvokeAudioCallbackDelayed(System.Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        private void CreateOneShotAudioSource(AudioClip clip, float volume)
        {
            GameObject tempAudio = new GameObject("TempVFXAudio");
            tempAudio.transform.position = transform.position;
            AudioSource source = tempAudio.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume * vfxAudioVolume;
            source.spatialBlend = 0f; // 2D audio
            source.Play();
            Destroy(tempAudio, clip.length + 0.1f);
        }

        #endregion

        #region Performance & Culling

        private bool ShouldPlayEffect(Vector3 position, VFXData effectData)
        {
            // Always play critical effects
            if (effectData.GetPriority() == VFXPriority.Critical)
            {
                return true;
            }

            // Check distance-based LOD
            if (enableDistanceLOD && effectData.maxDistance > 0 && _mainCamera != null)
            {
                float distance = Vector3.Distance(_mainCamera.transform.position, position);
                if (distance > effectData.maxDistance)
                {
                    return false;
                }
            }

            // Check performance budget
            if (enablePerformanceCulling && _currentActiveParticles >= maxParticlesOnScreen)
            {
                // Only allow high priority or above when over budget
                if (effectData.GetPriority() > VFXPriority.High)
                {
                    return false;
                }
            }

            return true;
        }

        private int GetMaxParticles(ParticleSystem ps)
        {
            if (ps == null) return 0;

            int maxParticles = ps.main.maxParticles;

            // Include child particle systems
            ParticleSystem[] children = ps.GetComponentsInChildren<ParticleSystem>();
            foreach (var child in children)
            {
                if (child != ps)
                {
                    maxParticles += child.main.maxParticles;
                }
            }

            return maxParticles;
        }

        #endregion

        #region Public Getters

        /// <summary>
        /// Get current active particle count for performance monitoring
        /// </summary>
        public int GetActiveParticleCount()
        {
            return _currentActiveParticles;
        }

        /// <summary>
        /// Get the number of active effects
        /// </summary>
        public int GetActiveEffectCount()
        {
            return _activeEffects.Count;
        }

        /// <summary>
        /// Get pool statistics for a specific VFX type
        /// </summary>
        public string GetPoolDebugInfo(VFXType vfxType)
        {
            if (_effectPools.TryGetValue(vfxType, out ObjectPool<ParticleSystem> pool))
            {
                return pool.GetDebugInfo();
            }
            return $"Pool for {vfxType} not found.";
        }

        /// <summary>
        /// Check if VFX system is initialized
        /// </summary>
        public bool IsInitialized()
        {
            return _effectPools != null && _effectPools.Count > 0;
        }

        #endregion

        #region Debug

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Box("VFX Manager Debug");
            GUILayout.Label($"Active Effects: {GetActiveEffectCount()}");
            GUILayout.Label($"Active Particles: {_currentActiveParticles} / {maxParticlesOnScreen}");
            GUILayout.Label($"Pools Created: {_effectPools?.Count ?? 0}");

            if (GUILayout.Button("Clear All Effects"))
            {
                ClearAllEffects();
            }

            GUILayout.EndArea();
        }
#endif

        #endregion
    }
}
