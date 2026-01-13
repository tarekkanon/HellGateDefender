using BaseDefender.VFX;
using UnityEngine;

/// <summary>
/// Projectile behavior for player and turret projectiles.
/// Moves forward, deals damage on hit, and self-destructs after lifetime.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    #region Inspector Fields

    [Header("Projectile Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;

    #endregion

    #region Private Fields

    private float _damage = 10f;
    private Vector3 _direction = Vector3.forward;
    private float _lifeTimer = 0f;
    private bool _hasHit = false;
    private Rigidbody _rb;
    private ParticleSystem vfxEffect;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Cache Rigidbody component
        if (!TryGetComponent<Rigidbody>(out _rb))
        {
            Debug.LogError("Projectile: Rigidbody component not found!");
        }

        // Configure Rigidbody
        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void Start()
    {
        _lifeTimer = lifetime;
    }

    private void Update()
    {
        // Move forward
        transform.position += _direction * speed * Time.deltaTime;

        // Count down lifetime
        _lifeTimer -= Time.deltaTime;

        // Destroy after lifetime expires
        if (_lifeTimer <= 0f)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore if already hit something
        if (_hasHit)
        {
            return;
        }

        // Check if hit an enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.IsDead)
        {
            // Deal damage to enemy
            enemy.TakeDamage(Mathf.RoundToInt(_damage));

            _hasHit = true;
            DestroyProjectile();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize the projectile with damage and direction
    /// </summary>
    /// <param name="damage">Damage to deal on hit</param>
    /// <param name="direction">Direction to move in (should be normalized)</param>
    public void Initialize(float damage, Vector3 direction)
    {
        _damage = damage;
        _direction = direction.normalized;
        _lifeTimer = lifetime;
        _hasHit = false;

        // Orient the projectile to face the direction
        if (_direction.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(_direction);
            // Play VFX attached to this projectile
            vfxEffect = VFXHelper.PlayEffectAttached(
                VFXType.PlayerSpellProjectile,
                transform,
                Vector3.zero
            );
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Return the projectile to pool or destroy it
    /// </summary>
    private void DestroyProjectile()
    {
        // Stop VFX when projectile is disabled/pooled
        if (vfxEffect != null)
        {
            VFXHelper.StopEffect(vfxEffect);
        }
        
        // Return to pool if available, otherwise destroy
        if (ProjectilePool.Instance != null)
        {
            ProjectilePool.Instance.DespawnProjectile(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    #endregion
}
