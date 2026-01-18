using UnityEngine;
using UnityEngine.AI;
using BaseDefender.Core;
using BaseDefender.VFX;

/// <summary>
/// Base enemy behavior using NavMesh pathfinding.
/// Enemies move toward the base, take damage, drop coins on death, and damage the base on contact.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    #region Inspector Fields

    [Header("Stats")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private int damageToBase = 5;
    [SerializeField] private int coinDrop = 5;

    [Header("References")]
    [SerializeField] private GameObject coinPrefab;

    #endregion

    #region Components

    private EnemyAnimationController _animationController;

    #endregion

    #region Private Fields

    private int _currentHealth;
    private NavMeshAgent _agent;
    private Transform _baseTarget;
    private bool _isDead = false;
    private bool _hasReachedBase = false;

    #endregion

    #region Properties

    /// <summary>
    /// Whether the enemy is dead
    /// </summary>
    public bool IsDead => _isDead;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Cache NavMeshAgent component
        if (!TryGetComponent<NavMeshAgent>(out _agent))
        {
            Debug.LogError("Enemy: NavMeshAgent component not found!");
        }

        // Configure NavMeshAgent
        if (_agent != null)
        {
            _agent.speed = moveSpeed;
            _agent.angularSpeed = 360f;
            _agent.acceleration = 8f;
            _agent.stoppingDistance = 0.5f;
        }

        // Cache animation controller
        _animationController = GetComponentInChildren<EnemyAnimationController>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize the enemy (called after spawning)
    /// </summary>
    public void Initialize()
    {
        _currentHealth = maxHealth;
        _isDead = false;
        _hasReachedBase = false;

        FindBase();

        if (_baseTarget != null && _agent != null)
        {
            _agent.SetDestination(_baseTarget.position);
        }
    }

    /// <summary>
    /// Take damage from projectiles or other sources
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        if (_isDead)
        {
            return;
        }

        _currentHealth -= damage;
        VFXHelper.PlayDemonicHit(transform.position);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Find the base in the scene
    /// </summary>
    private void FindBase()
    {
        GameObject baseObj = GameObject.FindGameObjectWithTag("Base");
        if (baseObj != null)
        {
            _baseTarget = baseObj.transform;
        }
        else
        {
            Debug.LogError("Enemy: Base not found! Make sure the base has the 'Base' tag.");
        }
    }

    /// <summary>
    /// Handle enemy death
    /// </summary>
    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        // Stop the agent
        if (_agent != null)
        {
            _agent.isStopped = true;
        }

        // Trigger death animation
        if (_animationController != null)
        {
            _animationController.SetDead();
        }

        // Play death sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyDeath();
        }

        // Play angel death effect with audio
        VFXHelper.PlayAngelDeath(transform.position);

        // Fire enemy killed event
        GameEvents.EnemyKilled(transform.position, coinDrop);

        // Spawn coin at death position
        SpawnCoin();

        // Return to pool after death animation
        // TODO: Get animation length and return after it finishes
        ReturnToPool(2f); // Delay to show death animation
    }

    /// <summary>
    /// Spawn a coin at the enemy's death position
    /// </summary>
    private void SpawnCoin()
    {
        // Use CoinPool if available, otherwise fall back to instantiation
        if (CoinPool.Instance != null)
        {
            Coin coin = CoinPool.Instance.SpawnCoin(transform.position, coinDrop);
            if (coin == null)
            {
                Debug.LogWarning("Enemy: Failed to spawn coin from pool!");
            }
        }
        else if (coinPrefab != null)
        {
            // Fallback to direct instantiation if pool not available
            GameObject coinObj = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            Coin coin = coinObj.GetComponent<Coin>();
            if (coin != null)
            {
                coin.Initialize(coinDrop);
            }
        }
        else
        {
            Debug.LogWarning("Enemy: Coin prefab not assigned and pool not available, no coin will be dropped!");
        }
    }

    /// <summary>
    /// Return enemy to pool after a delay
    /// </summary>
    private void ReturnToPool(float delay)
    {
        Invoke(nameof(ReturnToPoolImmediate), delay);
    }

    /// <summary>
    /// Immediately return enemy to pool
    /// </summary>
    private void ReturnToPoolImmediate()
    {
        // Reset state for reuse
        _isDead = false;
        _hasReachedBase = false;

        if (_agent != null)
        {
            _agent.isStopped = false;
        }

        // Return to pool if available, otherwise destroy
        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.DespawnEnemy(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handle reaching the base
    /// </summary>
    private void OnReachBase()
    {
        if (_hasReachedBase || _isDead)
        {
            return;
        }

        _hasReachedBase = true;

        // Find the base and deal damage
        Base baseComponent = _baseTarget?.GetComponent<Base>();
        if (baseComponent != null)
        {
            baseComponent.TakeDamage(damageToBase);
        }

        // Fire enemy reached base event
        GameEvents.EnemyReachedBase();

        // Return to pool instead of destroying
        ReturnToPoolImmediate();
    }

    #endregion

    #region Collision Detection

    private void OnTriggerEnter(Collider other)
    {
        // Check if we reached the base
        if (other.CompareTag("Base"))
        {
            OnReachBase();
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Draw line to base if assigned
        if (_baseTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _baseTarget.position);
        }

        // Draw health bar
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Vector3 healthBarPos = transform.position + Vector3.up * 2f;
            float healthPercent = (float)_currentHealth / maxHealth;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 0.5f, healthBarPos + Vector3.right * (healthPercent - 0.5f));
        }
    }

    #endregion
}
