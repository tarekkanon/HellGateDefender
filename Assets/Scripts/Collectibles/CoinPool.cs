using UnityEngine;

/// <summary>
/// Manages object pooling for coins.
/// Provides singleton access to spawn and despawn coins efficiently.
/// </summary>
public class CoinPool : MonoBehaviour
{
    #region Singleton

    private static CoinPool _instance;

    public static CoinPool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject poolObj = new GameObject("CoinPool");
                _instance = poolObj.AddComponent<CoinPool>();
                DontDestroyOnLoad(poolObj);
            }
            return _instance;
        }
    }

    #endregion

    #region Inspector Fields

    [Header("Pool Configuration")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int initialPoolSize = 30;
    [SerializeField] private int maxPoolSize = 100;
    [SerializeField] private bool expandable = true;

    #endregion

    #region Private Fields

    private ObjectPool<Coin> _coinPool;
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
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize the coin pool with a prefab
    /// Call this once at game start with your coin prefab
    /// </summary>
    /// <param name="prefab">Coin prefab to pool</param>
    public void Initialize(GameObject prefab)
    {
        if (_initialized)
        {
            Debug.LogWarning("CoinPool: Already initialized!");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError("CoinPool: Cannot initialize with null prefab!");
            return;
        }

        coinPrefab = prefab;
        _coinPool = new ObjectPool<Coin>(coinPrefab, initialPoolSize, maxPoolSize, expandable, transform);
        _initialized = true;

        Debug.Log("CoinPool: Initialized successfully");
    }

    /// <summary>
    /// Auto-initialize if prefab is assigned in inspector
    /// </summary>
    private void Start()
    {
        if (!_initialized && coinPrefab != null)
        {
            Initialize(coinPrefab);
        }
    }

    /// <summary>
    /// Spawn a coin from the pool
    /// </summary>
    /// <param name="position">Position to spawn at</param>
    /// <param name="rotation">Rotation to spawn with</param>
    /// <param name="value">Value of the coin</param>
    /// <returns>Spawned coin, or null if failed</returns>
    public Coin SpawnCoin(Vector3 position, Quaternion rotation, int value)
    {
        if (!_initialized)
        {
            Debug.LogError("CoinPool: Not initialized! Call Initialize() first or assign prefab in inspector.");
            return null;
        }

        Coin coin = _coinPool.Get(position, rotation);
        if (coin != null)
        {
            coin.Initialize(value);
        }

        return coin;
    }

    /// <summary>
    /// Spawn a coin with default rotation
    /// </summary>
    public Coin SpawnCoin(Vector3 position, int value)
    {
        return SpawnCoin(position, Quaternion.identity, value);
    }

    /// <summary>
    /// Return a coin to the pool
    /// </summary>
    /// <param name="coin">Coin to return</param>
    public void DespawnCoin(Coin coin)
    {
        if (coin == null)
        {
            return;
        }

        if (_coinPool != null)
        {
            _coinPool.Return(coin);
        }
        else
        {
            Debug.LogWarning("CoinPool: Pool not initialized, destroying coin instead");
            Destroy(coin.gameObject);
        }
    }

    /// <summary>
    /// Return all active coins to the pool
    /// </summary>
    public void DespawnAll()
    {
        _coinPool?.ReturnAll();
    }

    /// <summary>
    /// Clear the pool and destroy all coins
    /// </summary>
    public void ClearPool()
    {
        _coinPool?.Clear();
        _initialized = false;
    }

    #endregion

    #region Debug

    /// <summary>
    /// Get debug information about the pool
    /// </summary>
    public string GetDebugInfo()
    {
        if (!_initialized || _coinPool == null)
        {
            return "CoinPool: Not initialized";
        }

        return "CoinPool Status:\n  " + _coinPool.GetDebugInfo();
    }

    #endregion
}
