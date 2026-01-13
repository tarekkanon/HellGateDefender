using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject for configuring level-specific settings including enemy pool configurations.
/// Allows each level to have different enemy types with custom pool sizes.
/// </summary>
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Configuration")]
public class LevelConfiguration : ScriptableObject
{
    [Header("Level Info")]
    [Tooltip("Level number or identifier")]
    public int levelNumber = 1;

    [Tooltip("Display name for this level")]
    public string levelName = "Level 1";

    [Header("Enemy Pool Configuration")]
    [Tooltip("Enemy types available in this level with their pool configurations")]
    public List<EnemyPoolConfig> levelEnemyPools = new List<EnemyPoolConfig>();

    [Header("Wave Configurations")]
    [Tooltip("Sequence of waves for this level")]
    public List<WaveConfiguration> waves = new List<WaveConfiguration>();

    [Header("Level Settings")]
    [Tooltip("Time between waves (seconds)")]
    public float timeBetweenWaves = 5f;

    /// <summary>
    /// Initialize the enemy pool for this level
    /// Clears existing pools and registers only this level's enemy types
    /// </summary>
    public void InitializeEnemyPool()
    {
        if (EnemyPool.Instance == null)
        {
            Debug.LogError($"LevelConfiguration '{levelName}': EnemyPool instance not available!");
            return;
        }

        // Clear previous level's pools
        EnemyPool.Instance.ClearAllPools();

        // Register enemy types for this level
        foreach (var poolConfig in levelEnemyPools)
        {
            if (poolConfig != null && poolConfig.prefab != null && !string.IsNullOrEmpty(poolConfig.enemyTypeId))
            {
                EnemyPool.Instance.RegisterEnemyType(
                    poolConfig.enemyTypeId,
                    poolConfig.prefab,
                    poolConfig.initialPoolSize,
                    poolConfig.maxPoolSize,
                    poolConfig.expandable
                );
            }
        }

        Debug.Log($"Level '{levelName}': Initialized enemy pool with {levelEnemyPools.Count} enemy types");
    }

    /// <summary>
    /// Get total number of waves in this level
    /// </summary>
    public int WaveCount => waves.Count;

    /// <summary>
    /// Get total number of enemies across all waves
    /// </summary>
    public int TotalEnemyCount
    {
        get
        {
            int total = 0;
            foreach (var wave in waves)
            {
                if (wave != null)
                {
                    total += wave.TotalEnemyCount;
                }
            }
            return total;
        }
    }

    /// <summary>
    /// Get all unique enemy types used in this level
    /// </summary>
    public List<string> GetAllEnemyTypes()
    {
        HashSet<string> uniqueTypes = new HashSet<string>();

        foreach (var poolConfig in levelEnemyPools)
        {
            if (!string.IsNullOrEmpty(poolConfig.enemyTypeId))
            {
                uniqueTypes.Add(poolConfig.enemyTypeId);
            }
        }

        return new List<string>(uniqueTypes);
    }

    /// <summary>
    /// Validate level configuration
    /// </summary>
    public bool Validate()
    {
        bool isValid = true;

        // Check if we have enemy pools defined
        if (levelEnemyPools.Count == 0)
        {
            Debug.LogWarning($"LevelConfiguration '{levelName}': No enemy pool configurations defined!");
            isValid = false;
        }

        // Check if we have waves defined
        if (waves.Count == 0)
        {
            Debug.LogWarning($"LevelConfiguration '{levelName}': No waves defined!");
            isValid = false;
        }

        // Validate each wave
        foreach (var wave in waves)
        {
            if (wave != null)
            {
                if (!wave.Validate())
                {
                    isValid = false;
                }

                // Check if wave uses enemies that are in our pool config
                foreach (var enemyType in wave.GetUniqueEnemyTypes())
                {
                    bool found = false;
                    foreach (var poolConfig in levelEnemyPools)
                    {
                        if (poolConfig.enemyTypeId == enemyType)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Debug.LogWarning($"LevelConfiguration '{levelName}': Wave '{wave.waveName}' uses enemy type '{enemyType}' which is not in level's pool configurations!");
                        isValid = false;
                    }
                }
            }
        }

        return isValid;
    }

    /// <summary>
    /// Get recommended pool sizes based on wave enemy counts
    /// </summary>
    public Dictionary<string, int> GetRecommendedPoolSizes()
    {
        Dictionary<string, int> maxConcurrent = new Dictionary<string, int>();

        foreach (var wave in waves)
        {
            if (wave == null) continue;

            foreach (var spawn in wave.enemySpawns)
            {
                if (!maxConcurrent.ContainsKey(spawn.enemyTypeId))
                {
                    maxConcurrent[spawn.enemyTypeId] = 0;
                }

                // Assume up to 50% of enemies might be alive at once
                int concurrent = Mathf.CeilToInt(spawn.count * 0.5f);
                if (concurrent > maxConcurrent[spawn.enemyTypeId])
                {
                    maxConcurrent[spawn.enemyTypeId] = concurrent;
                }
            }
        }

        return maxConcurrent;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Auto-configure pool sizes based on wave configurations
    /// </summary>
    [ContextMenu("Auto-Configure Pool Sizes")]
    public void AutoConfigurePoolSizes()
    {
        var recommended = GetRecommendedPoolSizes();

        foreach (var poolConfig in levelEnemyPools)
        {
            if (recommended.TryGetValue(poolConfig.enemyTypeId, out int recommendedSize))
            {
                poolConfig.initialPoolSize = recommendedSize;
                poolConfig.maxPoolSize = recommendedSize * 2;
                Debug.Log($"Set {poolConfig.enemyTypeId}: Initial={poolConfig.initialPoolSize}, Max={poolConfig.maxPoolSize}");
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
