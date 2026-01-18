using BaseDefender.VFX;
using UnityEngine;

/// <summary>
/// Manages object pooling for projectiles.
/// Provides singleton access to spawn and despawn projectiles efficiently.
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    #region Singleton

    private static ProjectilePool _instance;

    public static ProjectilePool Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject poolObj = new GameObject("ProjectilePool");
                _instance = poolObj.AddComponent<ProjectilePool>();
                DontDestroyOnLoad(poolObj);
            }
            return _instance;
        }
    }

    #endregion

    #region Inspector Fields

    [Header("Pool Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int initialPoolSize = 30;
    [SerializeField] private int maxPoolSize = 100;
    [SerializeField] private bool expandable = true;

    #endregion

    #region Private Fields

    private ObjectPool<Projectile> _projectilePool;
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
    /// Initialize the projectile pool with a prefab
    /// Call this once at game start with your projectile prefab
    /// </summary>
    /// <param name="prefab">Projectile prefab to pool</param>
    public void Initialize(GameObject prefab)
    {
        if (_initialized)
        {
            Debug.LogWarning("ProjectilePool: Already initialized!");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError("ProjectilePool: Cannot initialize with null prefab!");
            return;
        }

        projectilePrefab = prefab;
        _projectilePool = new ObjectPool<Projectile>(projectilePrefab, initialPoolSize, maxPoolSize, expandable, transform);
        _initialized = true;

        Debug.Log("ProjectilePool: Initialized successfully");
    }

    /// <summary>
    /// Auto-initialize if prefab is assigned in inspector
    /// </summary>
    private void Start()
    {
        if (!_initialized && projectilePrefab != null)
        {
            Initialize(projectilePrefab);
        }
    }

    /// <summary>
    /// Spawn a projectile from the pool
    /// </summary>
    /// <param name="position">Position to spawn at</param>
    /// <param name="rotation">Rotation to spawn with</param>
    /// <param name="damage">Damage the projectile deals</param>
    /// <param name="direction">Direction the projectile travels</param>
    /// <param name="isPlayerSpawn">Origin of projectile spawn</param>
    /// <returns>Spawned projectile, or null if failed</returns>
    public Projectile SpawnProjectile(Vector3 position, Quaternion rotation, float damage, Vector3 direction, bool isPlayerSpawn)
    {
        if (!_initialized)
        {
            Debug.LogError("ProjectilePool: Not initialized! Call Initialize() first or assign prefab in inspector.");
            return null;
        }

        Projectile projectile = _projectilePool.Get(position, rotation);
        if (projectile != null)
        {
            projectile.Initialize(damage, direction);
            // Play VFX attached to this projectile and store the reference
            ParticleSystem vfx = VFXHelper.PlayEffectAttached(
                isPlayerSpawn ? VFXType.PlayerSpellProjectile : VFXType.TowerSpellProjectile,
                projectile.transform,
                Vector3.zero
            );
            projectile.SetVFXEffect(vfx);
        }

        return projectile;
    }

    /// <summary>
    /// Spawn a projectile with calculated rotation from direction
    /// </summary>
    public Projectile SpawnProjectile(Vector3 position, float damage, Vector3 direction, bool isPlayerSpawn)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        return SpawnProjectile(position, rotation, damage, direction, isPlayerSpawn);
    }

    /// <summary>
    /// Return a projectile to the pool
    /// </summary>
    /// <param name="projectile">Projectile to return</param>
    public void DespawnProjectile(Projectile projectile)
    {
        if (projectile == null)
        {
            return;
        }

        if (_projectilePool != null)
        {
            _projectilePool.Return(projectile);
        }
        else
        {
            Debug.LogWarning("ProjectilePool: Pool not initialized, destroying projectile instead");
            Destroy(projectile.gameObject);
        }
    }

    /// <summary>
    /// Return all active projectiles to the pool
    /// </summary>
    public void DespawnAll()
    {
        _projectilePool?.ReturnAll();
    }

    /// <summary>
    /// Clear the pool and destroy all projectiles
    /// </summary>
    public void ClearPool()
    {
        _projectilePool?.Clear();
        _initialized = false;
    }

    #endregion

    #region Debug

    /// <summary>
    /// Get debug information about the pool
    /// </summary>
    public string GetDebugInfo()
    {
        if (!_initialized || _projectilePool == null)
        {
            return "ProjectilePool: Not initialized";
        }

        return "ProjectilePool Status:\n  " + _projectilePool.GetDebugInfo();
    }

    #endregion
}
