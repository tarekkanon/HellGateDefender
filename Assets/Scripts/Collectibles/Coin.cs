using UnityEngine;
using BaseDefender.Core;

/// <summary>
/// Collectible coin that drops from defeated enemies.
/// Features magnet behavior that pulls coins toward the player when nearby.
/// </summary>
public class Coin : MonoBehaviour
{
    #region Inspector Fields

    [Header("Coin Settings")]
    [SerializeField] private int value = 5;

    [Header("Magnet Behavior")]
    [SerializeField] private float magnetRadius = 2.5f;
    [SerializeField] private float flySpeed = 10f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 30f;

    [Header("Visual (Optional)")]
    [SerializeField] private float bobHeight = 0.3f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float rotationSpeed = 180f;

    #endregion

    #region Private Fields

    private Transform _player;
    private bool _isCollecting = false;
    private float _timer = 0f;
    private Vector3 _initialPosition;
    private float _bobOffset = 0f;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        FindPlayer();
        _initialPosition = transform.position;
        _bobOffset = Random.Range(0f, Mathf.PI * 2f); // Random starting phase for bobbing
    }

    private void Update()
    {
        // Increment timer
        _timer += Time.deltaTime;

        // Check if lifetime expired
        if (_timer >= lifetime)
        {
            DestroyCoin();
            return;
        }

        // If not collecting, check distance to player
        if (!_isCollecting)
        {
            CheckPlayerDistance();
            ApplyBobbing();
            ApplyRotation();
        }
        else
        {
            // Move toward player
            MoveTowardPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if collected by player
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize the coin with a specific value
    /// </summary>
    /// <param name="coinValue">Value of this coin</param>
    public void Initialize(int coinValue)
    {
        if (coinValue < 0)
        {
            Debug.LogWarning("Coin: Attempted to initialize with negative value!");
            coinValue = 0;
        }

        value = coinValue;
        _timer = 0f;
        _isCollecting = false;
        _initialPosition = transform.position;
        _bobOffset = Random.Range(0f, Mathf.PI * 2f);

        // Find player if not already cached
        if (_player == null)
        {
            FindPlayer();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Find the player in the scene
    /// </summary>
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Coin: Player not found! Make sure player has 'Player' tag.");
        }
    }

    /// <summary>
    /// Check distance to player and start collecting if within magnet radius
    /// </summary>
    private void CheckPlayerDistance()
    {
        if (_player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= magnetRadius)
        {
            _isCollecting = true;
        }
    }

    /// <summary>
    /// Move the coin toward the player
    /// </summary>
    private void MoveTowardPlayer()
    {
        if (_player == null)
        {
            return;
        }

        // Move toward player
        Vector3 direction = (_player.position - transform.position).normalized;
        transform.position += direction * flySpeed * Time.deltaTime;

        // Check if reached player
        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance < 0.5f)
        {
            Collect();
        }
    }

    /// <summary>
    /// Collect the coin and add value to player
    /// </summary>
    private void Collect()
    {
        // Add coins to game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(value);
        }

        // Play collection sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCoinCollect();
        }

        // Destroy the coin
        DestroyCoin();
    }

    /// <summary>
    /// Apply bobbing animation to the coin
    /// </summary>
    private void ApplyBobbing()
    {
        if (bobHeight <= 0f)
        {
            return;
        }

        // Calculate bob offset
        float bobY = Mathf.Sin((Time.time * bobSpeed) + _bobOffset) * bobHeight;

        // Apply to position
        transform.position = _initialPosition + Vector3.up * bobY;
    }

    /// <summary>
    /// Apply rotation animation to the coin
    /// </summary>
    private void ApplyRotation()
    {
        if (rotationSpeed <= 0f)
        {
            return;
        }

        // Rotate around Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Return coin to pool or destroy it
    /// </summary>
    private void DestroyCoin()
    {
        // Return to pool if available, otherwise destroy
        if (CoinPool.Instance != null)
        {
            CoinPool.Instance.DespawnCoin(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Draw magnet radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);

        // Draw line to player if collecting
        if (_isCollecting && _player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _player.position);
        }
    }

    #endregion
}
