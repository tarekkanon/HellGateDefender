using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BaseDefender.VFX;

/// <summary>
/// Utility class for spawning and tracking enemies.
/// Provides helper methods for enemy instantiation and management.
/// Supports spawn portal VFX for dramatic enemy entrances.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    #region Static Fields

    private static List<Enemy> _activeEnemies = new List<Enemy>();

    #endregion

    #region Properties

    /// <summary>
    /// Get a read-only list of all active enemies
    /// </summary>
    public static IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies.AsReadOnly();

    /// <summary>
    /// Get the number of active enemies
    /// </summary>
    public static int ActiveEnemyCount => _activeEnemies.Count;

    #endregion

    #region Static Methods

    /// <summary>
    /// Spawn an enemy at a specific position
    /// </summary>
    /// <param name="prefab">Enemy prefab to spawn</param>
    /// <param name="position">World position to spawn at</param>
    /// <returns>The spawned enemy instance, or null if spawn failed</returns>
    public static Enemy SpawnEnemy(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemySpawner: Cannot spawn null prefab!");
            return null;
        }

        return SpawnEnemy(prefab, position, Quaternion.identity);
    }

    /// <summary>
    /// Spawn an enemy at a specific position and rotation
    /// </summary>
    /// <param name="prefab">Enemy prefab to spawn</param>
    /// <param name="position">World position to spawn at</param>
    /// <param name="rotation">World rotation to spawn with</param>
    /// <returns>The spawned enemy instance, or null if spawn failed</returns>
    public static Enemy SpawnEnemy(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemySpawner: Cannot spawn null prefab!");
            return null;
        }

        // Instantiate the enemy
        GameObject enemyObj = Instantiate(prefab, position, rotation);

        // Get the Enemy component
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError($"EnemySpawner: Prefab '{prefab.name}' doesn't have an Enemy component!");
            Destroy(enemyObj);
            return null;
        }

        // Initialize the enemy
        enemy.Initialize();

        // Add to tracking list
        _activeEnemies.Add(enemy);

        return enemy;
    }

    /// <summary>
    /// Spawn an enemy using the object pool (RECOMMENDED for performance)
    /// </summary>
    /// <param name="enemyTypeId">Type ID of enemy to spawn (e.g., "Scout", "Grunt", "Tank")</param>
    /// <param name="position">World position to spawn at</param>
    /// <param name="rotation">World rotation to spawn with</param>
    /// <returns>The spawned enemy instance, or null if spawn failed</returns>
    public static Enemy SpawnEnemyPooled(string enemyTypeId, Vector3 position, Quaternion rotation)
    {
        if (EnemyPool.Instance == null)
        {
            Debug.LogError("EnemySpawner: EnemyPool not initialized! Cannot spawn pooled enemy.");
            return null;
        }

        // Get enemy from pool
        Enemy enemy = EnemyPool.Instance.SpawnEnemy(enemyTypeId, position, rotation);

        if (enemy != null)
        {
            // Add to tracking list
            _activeEnemies.Add(enemy);
        }

        return enemy;
    }

    /// <summary>
    /// Spawn an enemy using the object pool (default rotation)
    /// </summary>
    public static Enemy SpawnEnemyPooled(string enemyTypeId, Vector3 position)
    {
        return SpawnEnemyPooled(enemyTypeId, position, Quaternion.identity);
    }

    /// <summary>
    /// Spawn an enemy with a spawn portal VFX effect (for dramatic entrances)
    /// </summary>
    /// <param name="prefab">Enemy prefab to spawn</param>
    /// <param name="position">World position to spawn at</param>
    /// <param name="onSpawnComplete">Callback when spawn is complete</param>
    public static void SpawnEnemyWithPortal(GameObject prefab, Vector3 position, System.Action<Enemy> onSpawnComplete = null)
    {
        if (prefab == null)
        {
            Debug.LogError("EnemySpawner: Cannot spawn null prefab!");
            onSpawnComplete?.Invoke(null);
            return;
        }

        // Play spawn portal effect with callback
        VFXHelper.PlaySpawnPortal(position,
            onEnemySpawn: () =>
            {
                // Spawn the enemy when portal is ready
                Enemy enemy = SpawnEnemy(prefab, position);
                onSpawnComplete?.Invoke(enemy);
            }
        );
    }

    /// <summary>
    /// Spawn a pooled enemy with a spawn portal VFX effect
    /// </summary>
    /// <param name="enemyTypeId">Type ID of enemy to spawn</param>
    /// <param name="position">World position to spawn at</param>
    /// <param name="onSpawnComplete">Callback when spawn is complete</param>
    public static void SpawnEnemyPooledWithPortal(string enemyTypeId, Vector3 position, System.Action<Enemy> onSpawnComplete = null)
    {
        // Play spawn portal effect with callback
        VFXHelper.PlaySpawnPortal(position,
            onEnemySpawn: () =>
            {
                // Spawn the enemy when portal is ready
                Enemy enemy = SpawnEnemyPooled(enemyTypeId, position);
                onSpawnComplete?.Invoke(enemy);
            }
        );
    }

    /// <summary>
    /// Remove an enemy from the active list (called when enemy dies or is destroyed)
    /// </summary>
    /// <param name="enemy">Enemy to remove</param>
    public static void RemoveEnemy(Enemy enemy)
    {
        if (enemy != null && _activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
        }
    }

    /// <summary>
    /// Clear all active enemies from the tracking list
    /// </summary>
    public static void ClearAllEnemies()
    {
        // Destroy all active enemies
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            if (_activeEnemies[i] != null)
            {
                Destroy(_activeEnemies[i].gameObject);
            }
        }

        _activeEnemies.Clear();
    }

    /// <summary>
    /// Remove null references from the active enemies list
    /// </summary>
    public static void CleanupDestroyedEnemies()
    {
        _activeEnemies.RemoveAll(enemy => enemy == null);
    }

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        GameEvents.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
    }

    private void Update()
    {
        // Periodically clean up destroyed enemies
        // This handles cases where enemies are destroyed without calling RemoveEnemy
        if (Time.frameCount % 60 == 0) // Every 60 frames (~1 second)
        {
            CleanupDestroyedEnemies();
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle enemy killed event
    /// </summary>
    private void HandleEnemyKilled(Vector3 position, int coinValue)
    {
        // Cleanup happens automatically via Update
    }

    #endregion

    #region Debug

    /// <summary>
    /// Get debug info about active enemies
    /// </summary>
    public static string GetDebugInfo()
    {
        CleanupDestroyedEnemies();
        return $"Active Enemies: {_activeEnemies.Count}";
    }

    #endregion
}
