using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages object pooling for enemies.
/// Provides singleton access to spawn and despawn enemies efficiently.
/// Dynamic system that supports any number of enemy types.
/// </summary>
public class EnemyPool : MonoBehaviour
{
    #region Singleton

    private static EnemyPool _instance;

    public static EnemyPool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject poolObj = new GameObject("EnemyPool");
                _instance = poolObj.AddComponent<EnemyPool>();
                DontDestroyOnLoad(poolObj);
            }
            return _instance;
        }
    }

    #endregion

    #region Inspector Fields

    [Header("Default Pool Configuration")]
    [SerializeField] private int defaultInitialPoolSize = 15;
    [SerializeField] private int defaultMaxPoolSize = 50;
    [SerializeField] private bool defaultExpandable = true;

    [Header("Enemy Type Configurations")]
    [SerializeField] private List<EnemyPoolConfig> enemyConfigs = new List<EnemyPoolConfig>();

    #endregion

    #region Private Fields

    private Dictionary<string, ObjectPool<Enemy>> _enemyPools;
    private Dictionary<string, GameObject> _enemyPrefabs;
    private bool _initialized = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Enforce singleton
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize dictionaries
        _enemyPools = new Dictionary<string, ObjectPool<Enemy>>();
        _enemyPrefabs = new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        // Auto-initialize from inspector configs if not manually initialized
        if (!_initialized && enemyConfigs.Count > 0)
        {
            InitializeFromConfigs();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize pools from inspector configurations
    /// </summary>
    public void InitializeFromConfigs()
    {
        if (_initialized)
        {
            Debug.LogWarning("EnemyPool: Already initialized!");
            return;
        }

        foreach (var config in enemyConfigs)
        {
            if (config != null && config.prefab != null)
            {
                RegisterEnemyType(
                    config.enemyTypeId,
                    config.prefab,
                    config.initialPoolSize,
                    config.maxPoolSize,
                    config.expandable
                );
            }
        }

        _initialized = true;
        Debug.Log($"EnemyPool: Initialized with {_enemyPools.Count} enemy types from configs");
    }

    /// <summary>
    /// Register a new enemy type for pooling
    /// </summary>
    /// <param name="enemyTypeId">Unique identifier for this enemy type</param>
    /// <param name="prefab">Enemy prefab</param>
    /// <param name="initialPoolSize">Initial pool size (0 to use default)</param>
    /// <param name="maxPoolSize">Max pool size (0 to use default)</param>
    /// <param name="expandable">Can pool expand (null to use default)</param>
    public void RegisterEnemyType(string enemyTypeId, GameObject prefab, int initialPoolSize = 0, int maxPoolSize = 0, bool? expandable = null)
    {
        if (string.IsNullOrEmpty(enemyTypeId))
        {
            Debug.LogError("EnemyPool: Enemy type ID cannot be null or empty!");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError($"EnemyPool: Cannot register enemy type '{enemyTypeId}' with null prefab!");
            return;
        }

        if (prefab.GetComponent<Enemy>() == null)
        {
            Debug.LogError($"EnemyPool: Prefab '{prefab.name}' doesn't have an Enemy component!");
            return;
        }

        if (_enemyPools.ContainsKey(enemyTypeId))
        {
            Debug.LogWarning($"EnemyPool: Enemy type '{enemyTypeId}' is already registered. Skipping.");
            return;
        }

        // Use provided values or fall back to defaults
        int poolInitialSize = initialPoolSize > 0 ? initialPoolSize : defaultInitialPoolSize;
        int poolMaxSize = maxPoolSize > 0 ? maxPoolSize : defaultMaxPoolSize;
        bool poolExpandable = expandable ?? defaultExpandable;

        // Create pool for this enemy type
        ObjectPool<Enemy> pool = new ObjectPool<Enemy>(prefab, poolInitialSize, poolMaxSize, poolExpandable, transform);
        _enemyPools[enemyTypeId] = pool;
        _enemyPrefabs[enemyTypeId] = prefab;

        Debug.Log($"EnemyPool: Registered enemy type '{enemyTypeId}' with pool size {poolInitialSize}");
    }

    /// <summary>
    /// Unregister an enemy type (clears its pool)
    /// </summary>
    public void UnregisterEnemyType(string enemyTypeId)
    {
        if (_enemyPools.ContainsKey(enemyTypeId))
        {
            _enemyPools[enemyTypeId].Clear();
            _enemyPools.Remove(enemyTypeId);
            _enemyPrefabs.Remove(enemyTypeId);
            Debug.Log($"EnemyPool: Unregistered enemy type '{enemyTypeId}'");
        }
    }

    /// <summary>
    /// Spawn an enemy from the pool by type ID
    /// </summary>
    /// <param name="enemyTypeId">Type ID of enemy to spawn</param>
    /// <param name="position">Position to spawn at</param>
    /// <param name="rotation">Rotation to spawn with</param>
    /// <returns>Spawned enemy, or null if failed</returns>
    public Enemy SpawnEnemy(string enemyTypeId, Vector3 position, Quaternion rotation)
    {
        if (!_enemyPools.ContainsKey(enemyTypeId))
        {
            Debug.LogError($"EnemyPool: Enemy type '{enemyTypeId}' is not registered! Call RegisterEnemyType() first.");
            return null;
        }

        ObjectPool<Enemy> pool = _enemyPools[enemyTypeId];
        Enemy enemy = pool.Get(position, rotation);

        if (enemy != null)
        {
            enemy.Initialize();
        }

        return enemy;
    }

    /// <summary>
    /// Spawn an enemy at a position (default rotation)
    /// </summary>
    public Enemy SpawnEnemy(string enemyTypeId, Vector3 position)
    {
        return SpawnEnemy(enemyTypeId, position, Quaternion.identity);
    }

    /// <summary>
    /// Check if an enemy type is registered
    /// </summary>
    public bool IsEnemyTypeRegistered(string enemyTypeId)
    {
        return _enemyPools.ContainsKey(enemyTypeId);
    }

    /// <summary>
    /// Get all registered enemy type IDs
    /// </summary>
    public List<string> GetRegisteredEnemyTypes()
    {
        return new List<string>(_enemyPools.Keys);
    }

    /// <summary>
    /// Return an enemy to the pool
    /// </summary>
    /// <param name="enemy">Enemy to return</param>
    public void DespawnEnemy(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        // Remove from active enemies tracker
        EnemySpawner.RemoveEnemy(enemy);

        // Find which pool this enemy belongs to by checking prefab name
        string enemyName = enemy.gameObject.name.Replace("(Pooled)", "").Replace("(Clone)", "").Trim();

        foreach (var kvp in _enemyPrefabs)
        {
            if (enemyName.Contains(kvp.Value.name))
            {
                if (_enemyPools.TryGetValue(kvp.Key, out ObjectPool<Enemy> pool))
                {
                    pool.Return(enemy);
                    return;
                }
            }
        }

        // If we couldn't find the pool, destroy the enemy
        Debug.LogWarning($"EnemyPool: Could not find pool for enemy '{enemy.name}', destroying instead");
        Destroy(enemy.gameObject);
    }

    /// <summary>
    /// Return all active enemies to their pools
    /// </summary>
    public void DespawnAll()
    {
        foreach (var pool in _enemyPools.Values)
        {
            pool.ReturnAll();
        }
    }

    /// <summary>
    /// Clear all pools and destroy objects
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in _enemyPools.Values)
        {
            pool.Clear();
        }

        _enemyPools.Clear();
        _enemyPrefabs.Clear();
        _initialized = false;
    }

    #endregion

    #region Private Methods

    // No private methods needed in dynamic version

    #endregion

    #region Debug

    /// <summary>
    /// Get debug information about all pools
    /// </summary>
    public string GetDebugInfo()
    {
        if (_enemyPools.Count == 0)
        {
            return "EnemyPool: No enemy types registered";
        }

        string info = "EnemyPool Status:\n";
        foreach (var kvp in _enemyPools)
        {
            info += $"  [{kvp.Key}] {kvp.Value.GetDebugInfo()}\n";
        }

        return info.TrimEnd();
    }

    #endregion
}

/// <summary>
/// Configuration for an enemy type pool
/// </summary>
[System.Serializable]
public class EnemyPoolConfig
{
    [Tooltip("Unique identifier for this enemy type (e.g., 'Scout', 'Grunt', 'Tank', 'Boss1')")]
    public string enemyTypeId;

    [Tooltip("Enemy prefab to pool")]
    public GameObject prefab;

    [Tooltip("Initial number of objects to pre-create (0 to use default)")]
    public int initialPoolSize = 0;

    [Tooltip("Maximum pool size (0 to use default)")]
    public int maxPoolSize = 0;

    [Tooltip("Can the pool grow beyond initial size?")]
    public bool expandable = true;
}
