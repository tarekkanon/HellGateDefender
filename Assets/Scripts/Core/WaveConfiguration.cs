using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject for configuring wave composition with dynamic enemy types and ratios.
/// Create multiple instances for different levels with varying enemy compositions.
/// </summary>
[CreateAssetMenu(fileName = "WaveConfig", menuName = "Game/Wave Configuration")]
public class WaveConfiguration : ScriptableObject
{
    [Header("Wave Settings")]
    [Tooltip("Name/description of this wave configuration")]
    public string waveName = "Wave 1";

    [Tooltip("Delay before this wave starts (seconds)")]
    public float waveStartDelay = 2f;

    [Tooltip("Delay between spawning each enemy (seconds)")]
    public float spawnInterval = 1.5f;

    [Header("Enemy Composition")]
    [Tooltip("List of enemy types and their spawn counts for this wave")]
    public List<WaveEnemySpawn> enemySpawns = new List<WaveEnemySpawn>();

    /// <summary>
    /// Get total number of enemies in this wave
    /// </summary>
    public int TotalEnemyCount
    {
        get
        {
            int total = 0;
            foreach (var spawn in enemySpawns)
            {
                total += spawn.count;
            }
            return total;
        }
    }

    /// <summary>
    /// Get all unique enemy types in this wave
    /// </summary>
    public List<string> GetUniqueEnemyTypes()
    {
        List<string> types = new List<string>();
        foreach (var spawn in enemySpawns)
        {
            if (!types.Contains(spawn.enemyTypeId))
            {
                types.Add(spawn.enemyTypeId);
            }
        }
        return types;
    }

    /// <summary>
    /// Validate wave configuration
    /// </summary>
    public bool Validate()
    {
        if (enemySpawns.Count == 0)
        {
            Debug.LogWarning($"WaveConfiguration '{waveName}': No enemy spawns defined!");
            return false;
        }

        foreach (var spawn in enemySpawns)
        {
            if (string.IsNullOrEmpty(spawn.enemyTypeId))
            {
                Debug.LogWarning($"WaveConfiguration '{waveName}': Enemy spawn with empty type ID!");
                return false;
            }

            if (spawn.count <= 0)
            {
                Debug.LogWarning($"WaveConfiguration '{waveName}': Enemy '{spawn.enemyTypeId}' has count <= 0!");
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// Configuration for spawning a specific enemy type in a wave
/// </summary>
[System.Serializable]
public class WaveEnemySpawn
{
    [Tooltip("Enemy type ID (e.g., 'Scout', 'Grunt', 'Tank')")]
    public string enemyTypeId;

    [Tooltip("Number of this enemy type to spawn in this wave")]
    [Min(1)]
    public int count = 1;

    [Tooltip("Optional: Specific spawn points for this enemy type (empty = use random)")]
    public List<Transform> specificSpawnPoints = new List<Transform>();

    [Tooltip("Optional: Weight for random spawning (higher = more likely)")]
    [Range(0f, 10f)]
    public float spawnWeight = 1f;
}
