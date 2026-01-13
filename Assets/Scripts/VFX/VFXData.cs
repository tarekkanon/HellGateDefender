using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Data structure for a VFX effect with optional audio synchronization.
    /// Allows each effect to have its own settings and audio integration.
    /// </summary>
    [System.Serializable]
    public class VFXData
    {
        [Header("Identification")]
        [Tooltip("The type of VFX effect")]
        public VFXType vfxType;

        [Header("VFX Settings")]
        [Tooltip("The particle system prefab for this effect")]
        public ParticleSystem prefab;

        [Tooltip("Priority level for this effect (0=Critical, 1=High, 2=Medium, 3=Low)")]
        [Range(0, 3)]
        public int priority = 2;

        [Tooltip("Should this effect play audio when spawned?")]
        public bool syncWithAudio = false;

        [Header("Audio Sync Settings")]
        [Tooltip("The audio clip to play with this effect")]
        public AudioClip audioClip;

        [Tooltip("Audio volume for this effect (0-1)")]
        [Range(0f, 1f)]
        public float audioVolume = 1f;

        [Tooltip("Delay in seconds before playing audio (for sync timing)")]
        [Range(0f, 2f)]
        public float audioDelay = 0f;

        [Header("Pool Settings")]
        [Tooltip("Initial number of instances to pre-create")]
        [Range(1, 50)]
        public int initialPoolSize = 5;

        [Tooltip("Maximum pool size (0 = unlimited)")]
        [Range(0, 100)]
        public int maxPoolSize = 20;

        [Header("Performance")]
        [Tooltip("Maximum distance from camera to play this effect (0 = no limit)")]
        [Range(0f, 50f)]
        public float maxDistance = 30f;

        [Tooltip("Should this effect scale emission based on performance?")]
        public bool scaleWithPerformance = false;

        /// <summary>
        /// Validates that the VFX data is properly configured
        /// </summary>
        public bool IsValid()
        {
            if (prefab == null)
            {
                Debug.LogWarning($"VFXData for {vfxType} has no prefab assigned!");
                return false;
            }

            if (syncWithAudio && audioClip == null)
            {
                Debug.LogWarning($"VFXData for {vfxType} has audio sync enabled but no audio clip assigned!");
            }

            return true;
        }

        /// <summary>
        /// Gets the priority as an enum for easier comparison
        /// </summary>
        public VFXPriority GetPriority()
        {
            return (VFXPriority)priority;
        }
    }

    /// <summary>
    /// Priority levels for VFX effects - used for performance budgeting
    /// </summary>
    public enum VFXPriority
    {
        Critical = 0,   // Always play (player attacks, major hits)
        High = 1,       // Usually play (tower attacks, deaths)
        Medium = 2,     // Play if budget allows (ambient, trails)
        Low = 3         // Optional polish (dust, minor particles)
    }
}
