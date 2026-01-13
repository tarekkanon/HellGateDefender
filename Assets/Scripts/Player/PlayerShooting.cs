using UnityEngine;
using BaseDefender.Core;

/// <summary>
/// Auto-targeting and firing system for the player.
/// Automatically finds and shoots the nearest enemy within range.
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    #region Inspector Fields

    [Header("Combat Stats")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 2f; // Shots per second
    [SerializeField] private float range = 8f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateWholeBody = false;
    [SerializeField] private Transform upperBody; // Optional: only rotate upper body
    [SerializeField] private float rotationSpeed = 360f;

    #endregion

    #region Private Fields

    private float _fireTimer = 0f;
    private Transform _currentTarget;
    private Enemy _targetEnemy;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        ValidateConfiguration();
    }

    private void Update()
    {
        // Only shoot when game is playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        FindNearestEnemy();
        RotateTowardsTarget();

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

    #endregion

    #region Private Methods

    /// <summary>
    /// Validate that all required configuration is present
    /// </summary>
    private void ValidateConfiguration()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("PlayerShooting: Projectile prefab not assigned!");
        }

        if (firePoint == null)
        {
            Debug.LogWarning("PlayerShooting: Fire point not assigned, using player position.");
            firePoint = transform;
        }

        if (enemyLayer == 0)
        {
            Debug.LogWarning("PlayerShooting: Enemy layer not set! Auto-targeting may not work correctly.");
        }
    }

    /// <summary>
    /// Find the nearest enemy within range
    /// </summary>
    private void FindNearestEnemy()
    {
        // Clear current target if it's dead or null
        if (_currentTarget == null || _targetEnemy == null)
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
    /// Check if the player can shoot
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
            else
            {
                Debug.LogError("PlayerShooting: Projectile prefab doesn't have a Projectile component!");
            }
        }

        // Reset fire timer
        _fireTimer = 1f / fireRate;

        // Play shoot sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerShoot();
        }
    }

    /// <summary>
    /// Rotate towards the current target
    /// </summary>
    private void RotateTowardsTarget()
    {
        if (_currentTarget == null)
        {
            return;
        }

        // Calculate direction to target (ignore Y axis for rotation)
        Vector3 direction = _currentTarget.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.01f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Rotate the appropriate transform
        if (rotateWholeBody || upperBody == null)
        {
            // Rotate the entire player
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Rotate only the upper body
            upperBody.rotation = Quaternion.RotateTowards(
                upperBody.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Visualize shooting range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        // Draw line to current target
        if (_currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _currentTarget.position);
        }
    }

    #endregion
}
