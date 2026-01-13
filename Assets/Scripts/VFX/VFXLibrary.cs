using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// ScriptableObject that holds references to all VFX effects in the game.
    /// Organizes effects by category and allows easy inspector-based configuration.
    /// Create via: Assets > Create > Base Defender > VFX Library
    /// </summary>
    [CreateAssetMenu(fileName = "VFXLibrary", menuName = "Base Defender/VFX Library", order = 2)]
    public class VFXLibrary : ScriptableObject
    {
        [Header("Player Effects")]
        [Tooltip("VFX effects related to the player character")]
        public List<VFXData> playerEffects = new List<VFXData>();

        [Header("Tower Effects")]
        [Tooltip("VFX effects related to defense towers")]
        public List<VFXData> towerEffects = new List<VFXData>();

        [Header("Combat Effects")]
        [Tooltip("VFX effects for impacts and combat")]
        public List<VFXData> combatEffects = new List<VFXData>();

        [Header("Collection Effects")]
        [Tooltip("VFX effects for collectibles and pickups")]
        public List<VFXData> collectionEffects = new List<VFXData>();

        [Header("Environment Effects")]
        [Tooltip("VFX effects for ambient atmosphere and environment")]
        public List<VFXData> environmentEffects = new List<VFXData>();

        [Header("UI Effects")]
        [Tooltip("VFX effects for user interface feedback")]
        public List<VFXData> uiEffects = new List<VFXData>();

        // Cache all effects in a dictionary for fast lookup
        private Dictionary<VFXType, VFXData> _effectCache;

        /// <summary>
        /// Initialize the VFX library and validate all effects
        /// </summary>
        public void Initialize()
        {
            BuildEffectCache();
            ValidateEffects();
        }

        /// <summary>
        /// Build a dictionary cache of all effects for fast lookup
        /// </summary>
        private void BuildEffectCache()
        {
            _effectCache = new Dictionary<VFXType, VFXData>();

            // Combine all effect lists
            var allEffects = new List<VFXData>();
            allEffects.AddRange(playerEffects);
            allEffects.AddRange(towerEffects);
            allEffects.AddRange(combatEffects);
            allEffects.AddRange(collectionEffects);
            allEffects.AddRange(environmentEffects);
            allEffects.AddRange(uiEffects);

            // Build cache
            foreach (var effect in allEffects)
            {
                if (effect != null && effect.prefab != null)
                {
                    if (_effectCache.ContainsKey(effect.vfxType))
                    {
                        Debug.LogWarning($"VFXLibrary: Duplicate VFXType found - {effect.vfxType}. Using first occurrence.");
                    }
                    else
                    {
                        _effectCache[effect.vfxType] = effect;
                    }
                }
            }

            Debug.Log($"VFXLibrary: Initialized with {_effectCache.Count} effects.");
        }

        /// <summary>
        /// Get VFX data for a specific effect type
        /// </summary>
        /// <param name="vfxType">The type of VFX to retrieve</param>
        /// <returns>VFXData if found, null otherwise</returns>
        public VFXData GetEffect(VFXType vfxType)
        {
            if (_effectCache == null)
            {
                BuildEffectCache();
            }

            if (_effectCache.TryGetValue(vfxType, out VFXData effect))
            {
                return effect;
            }

            Debug.LogWarning($"VFXLibrary: Effect type '{vfxType}' not found in library!");
            return null;
        }

        /// <summary>
        /// Check if a specific VFX type exists in the library
        /// </summary>
        public bool HasEffect(VFXType vfxType)
        {
            if (_effectCache == null)
            {
                BuildEffectCache();
            }

            return _effectCache.ContainsKey(vfxType);
        }

        /// <summary>
        /// Get all effects of a specific priority level
        /// </summary>
        public List<VFXData> GetEffectsByPriority(VFXPriority priority)
        {
            if (_effectCache == null)
            {
                BuildEffectCache();
            }

            return _effectCache.Values
                .Where(effect => effect.GetPriority() == priority)
                .ToList();
        }

        /// <summary>
        /// Get the total count of registered effects
        /// </summary>
        public int GetEffectCount()
        {
            if (_effectCache == null)
            {
                BuildEffectCache();
            }

            return _effectCache.Count;
        }

        #region Validation

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Rebuild cache when values change in inspector
            BuildEffectCache();
            ValidateEffects();
        }
#endif

        private void ValidateEffects()
        {
            int missingPrefabs = 0;
            int missingAudioClips = 0;

            // Validate all effect lists
            var allEffects = new List<VFXData>();
            allEffects.AddRange(playerEffects);
            allEffects.AddRange(towerEffects);
            allEffects.AddRange(combatEffects);
            allEffects.AddRange(collectionEffects);
            allEffects.AddRange(environmentEffects);
            allEffects.AddRange(uiEffects);

            foreach (var effect in allEffects)
            {
                if (effect != null)
                {
                    if (effect.prefab == null)
                    {
                        missingPrefabs++;
                        Debug.LogWarning($"VFXLibrary: '{effect.vfxType}' is missing a prefab!");
                    }

                    if (effect.syncWithAudio && effect.audioClip == null)
                    {
                        missingAudioClips++;
                    }
                }
            }

            if (missingPrefabs > 0 || missingAudioClips > 0)
            {
                Debug.LogWarning($"VFXLibrary '{name}': {missingPrefabs} effects missing prefabs, {missingAudioClips} effects missing audio clips.");
            }
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        /// <summary>
        /// Helper method to auto-populate effect lists with default VFXData entries
        /// Call this from a custom editor button
        /// </summary>
        [ContextMenu("Generate Default Effect Entries")]
        private void GenerateDefaultEffectEntries()
        {
            // Clear existing lists
            playerEffects.Clear();
            towerEffects.Clear();
            combatEffects.Clear();
            collectionEffects.Clear();
            environmentEffects.Clear();
            uiEffects.Clear();

            // Create entries for each VFX type
            foreach (VFXType vfxType in System.Enum.GetValues(typeof(VFXType)))
            {
                VFXData newEffect = new VFXData
                {
                    vfxType = vfxType,
                    priority = 2, // Default to Medium
                    initialPoolSize = 5,
                    maxPoolSize = 20,
                    maxDistance = 30f,
                    audioVolume = 1f
                };

                // Categorize by VFX type name
                string typeName = vfxType.ToString();
                if (typeName.Contains("Player"))
                {
                    playerEffects.Add(newEffect);
                }
                else if (typeName.Contains("Tower"))
                {
                    towerEffects.Add(newEffect);
                }
                else if (typeName.Contains("Hit") || typeName.Contains("Death"))
                {
                    combatEffects.Add(newEffect);
                }
                else if (typeName.Contains("Coin"))
                {
                    collectionEffects.Add(newEffect);
                }
                else if (typeName.Contains("Ambient") || typeName.Contains("Shield") || typeName.Contains("Portal"))
                {
                    environmentEffects.Add(newEffect);
                }
                else
                {
                    uiEffects.Add(newEffect);
                }
            }

            Debug.Log($"VFXLibrary: Generated {playerEffects.Count + towerEffects.Count + combatEffects.Count + collectionEffects.Count + environmentEffects.Count + uiEffects.Count} default effect entries.");
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        #endregion
    }
}
