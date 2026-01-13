using UnityEngine;
using BaseDefender.Core;

/// <summary>
/// Activatable defensive turret that auto-targets and shoots enemies.
/// Players can activate turrets by spending coins when nearby.
/// </summary>
public class Turret : MonoBehaviour
{
    #region Inspector Fields

    [Header("Activation")]
    [SerializeField] private int activationCost = 50;
    [SerializeField] private float activationRadius = 2f;

    [Header("Combat Stats")]
    [SerializeField] private float damage = 8f;
    [SerializeField] private float fireRate = 1.5f; // Shots per second
    [SerializeField] private float range = 6f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Visuals")]
    [SerializeField] private Transform turretHead; // Part that rotates to aim
    [SerializeField] private GameObject inactiveVisual;
    [SerializeField] private GameObject activeVisual;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("UI")]
    [SerializeField] private TurretPrompt turretPrompt; // Optional: auto-find in children

    #endregion

    #region Private Fields

    private bool _isActive = false;
    private bool _playerInRange = false;
    private float _fireTimer = 0f;
    private Transform _currentTarget;
    private Enemy _targetEnemy;
    private Transform _player;

    #endregion

    #region Properties

    /// <summary>
    /// Whether the turret is active
    /// </summary>
    public bool IsActive => _isActive;

    /// <summary>
    /// Whether the player is in activation range
    /// </summary>
    public bool PlayerInRange => _playerInRange;

    /// <summary>
    /// Cost to activate this turret
    /// </summary>
    public int ActivationCost => activationCost;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // Only operate when game is playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        // Check player distance
        CheckPlayerDistance();

        // If active, perform targeting and shooting
        if (_isActive)
        {
            FindTarget();
            RotateTurretHead();

            if (CanShoot())
            {
                Shoot();
            }

            // Decrement fire timer
            if (_fireTimer > 0f)
            {
                _fireTimer -= Time.deltaTime;
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Try to activate the turret (called by player or UI)
    /// </summary>
    /// <returns>True if activation succeeded, false otherwise</returns>
    public bool TryActivate()
    {
        if (_isActive)
        {
            Debug.Log("Turret: Already active!");
            return false;
        }

        if (!_playerInRange)
        {
            Debug.Log("Turret: Player not in range!");
            return false;
        }

        // Try to spend coins
        if (GameManager.Instance.SpendCoins(activationCost))
        {
            Activate();
            return true;
        }
        else
        {
            Debug.Log("Turret: Not enough coins!");
            return false;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initialize the turret
    /// </summary>
    private void Initialize()
    {
        _isActive = false;
        _playerInRange = false;

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Turret: Player not found! Make sure player has 'Player' tag.");
        }

        // Find turret prompt if not assigned
        if (turretPrompt == null)
        {
            turretPrompt = GetComponentInChildren<TurretPrompt>(true);
        }

        // Set initial visual state
        UpdateVisuals();

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Validate that all required configuration is present
    /// </summary>
    private void ValidateConfiguration()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Turret: Projectile prefab not assigned!");
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Turret: Fire point not assigned, using turret position.");
            firePoint = transform;
        }

        if (turretHead == null)
        {
            Debug.LogWarning("Turret: Turret head not assigned, turret won't rotate to aim.");
        }

        if (enemyLayer == 0)
        {
            Debug.LogWarning("Turret: Enemy layer not set! Targeting may not work correctly.");
        }
    }

    /// <summary>
    /// Check if player is within activation radius
    /// </summary>
    private void CheckPlayerDistance()
    {
        if (_player == null)
        {
            _playerInRange = false;
            UpdatePromptVisibility();
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.position);
        bool wasInRange = _playerInRange;
        _playerInRange = distance <= activationRadius;

        // Update prompt visibility if range status changed
        if (wasInRange != _playerInRange)
        {
            UpdatePromptVisibility();
        }
    }

    /// <summary>
    /// Activate the turret
    /// </summary>
    private void Activate()
    {
        _isActive = true;

        // Update visuals
        UpdateVisuals();

        // Hide prompt
        UpdatePromptVisibility();

        // Fire activation event
        GameEvents.TurretActivated(this);

        // Play activation sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTurretActivate();
        }

        Debug.Log("Turret activated!");
    }

    /// <summary>
    /// Update visual state (active/inactive)
    /// </summary>
    private void UpdateVisuals()
    {
        if (inactiveVisual != null)
        {
            inactiveVisual.SetActive(!_isActive);
        }

        if (activeVisual != null)
        {
            activeVisual.SetActive(_isActive);
        }
    }

    /// <summary>
    /// Update turret prompt visibility
    /// </summary>
    private void UpdatePromptVisibility()
    {
        if (turretPrompt != null)
        {
            if (_playerInRange && !_isActive)
            {
                turretPrompt.Show();
            }
            else
            {
                turretPrompt.Hide();
            }
        }
    }

    /// <summary>
    /// Find the nearest enemy target
    /// </summary>
    private void FindTarget()
    {
        // Clear current target if it's dead or null
        if (_currentTarget == null || _targetEnemy == null || _targetEnemy.IsDead)
        {
            _currentTarget = null;
            _targetEnemy = null;
        }

        // Find all enemies in range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, enemyLayer);

        if (enemiesInRange.Length == 0)
        {
            _currentTarget = null;
            _targetEnemy = null;
            return;
        }

        // Find the closest enemy
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        Enemy closestEnemyComponent = null;

        foreach (Collider col in enemiesInRange)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, col.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = col.transform;
                closestEnemyComponent = enemy;
            }
        }

        _currentTarget = closestEnemy;
        _targetEnemy = closestEnemyComponent;
    }

    /// <summary>
    /// Check if the turret can shoot
    /// </summary>
    private bool CanShoot()
    {
        return _currentTarget != null && _fireTimer <= 0f && projectilePrefab != null;
    }

    /// <summary>
    /// Shoot a projectile toward the current target
    /// </summary>
    private void Shoot()
    {
        if (firePoint == null || _currentTarget == null)
        {
            return;
        }

        // Calculate direction to target
        Vector3 direction = (_currentTarget.position - firePoint.position).normalized;

        Projectile projectile = null;

        // Try to use ProjectilePool first for better performance
        if (ProjectilePool.Instance != null)
        {
            projectile = ProjectilePool.Instance.SpawnProjectile(firePoint.position, damage, direction);
        }
        // Fallback to instantiation if pool not available
        else if (projectilePrefab != null)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
            projectile = projectileObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Initialize(damage, direction);
            }
        }

        // Reset fire timer
        _fireTimer = 1f / fireRate;

        // Play shoot sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTurretShoot();
        }
    }

    /// <summary>
    /// Rotate turret head to face target
    /// </summary>
    private void RotateTurretHead()
    {
        if (turretHead == null || _currentTarget == null)
        {
            return;
        }

        // Calculate direction to target (ignore Y axis)
        Vector3 direction = _currentTarget.position - turretHead.position;
        direction.y = 0f;

        if (direction.magnitude < 0.01f)
        {
            return;
        }

        // Rotate turret head
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.RotateTowards(
            turretHead.rotation,
            targetRotation,
            360f * Time.deltaTime
        );
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Draw activation radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        // Draw shooting range
        Gizmos.color = _isActive ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(transform.position, range);

        // Draw line to current target
        if (_currentTarget != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint != null ? firePoint.position : transform.position, _currentTarget.position);
        }
    }

    #endregion
}
