using UnityEngine;

/// <summary>
/// Controls player movement using a floating joystick input.
/// Handles movement, rotation, and boundary clamping.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private PlayerAnimationController playerAnimationController;

    [Header("Input")]
    [SerializeField] private FloatingJoystick joystick;

    [Header("Boundaries")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 720f;

    #endregion

    #region Private Fields

    private Rigidbody _rb;
    private Vector3 _moveDirection;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Cache Rigidbody component
        if (!TryGetComponent<Rigidbody>(out _rb))
        {
            Debug.LogError("PlayerController: Rigidbody component not found!");
        }

        // Configure Rigidbody for kinematic movement
        if (_rb != null)
        {
            _rb.useGravity = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                              RigidbodyConstraints.FreezeRotationZ |
                              RigidbodyConstraints.FreezePositionY;
        }
    }

    private void Start()
    {
        ValidateConfiguration();
    }

    private void Update()
    {
        // Only process input when game is playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        HandleInput();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        // Only move when game is playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        HandleMovement();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validate that all required configuration is present
    /// </summary>
    private void ValidateConfiguration()
    {
        if (joystick == null)
        {
            Debug.LogWarning("PlayerController: Joystick reference not assigned! Attempting to find in scene...");
            joystick = FindObjectOfType<FloatingJoystick>();

            if (joystick == null)
            {
                Debug.LogError("PlayerController: FloatingJoystick not found in scene!");
            }
        }
    }

    /// <summary>
    /// Read input from the joystick
    /// </summary>
    private void HandleInput()
    {
        if (joystick == null)
        {
            _moveDirection = Vector3.zero;
            return;
        }

        // Get input from joystick
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Convert to world space direction
        // For isometric view, we use X and Z axes
        _moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    /// <summary>
    /// Move the player using Rigidbody
    /// </summary>
    private void HandleMovement()
    {
        if (_rb == null || _moveDirection.magnitude < 0.01f)
        {
            HandleMovementAnimation(false);
            return;
        }

        // Calculate new position
        Vector3 movement = _moveDirection * moveSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = _rb.position + movement;

        // Clamp position to boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);

        // Apply movement
        _rb.MovePosition(newPosition);
        HandleMovementAnimation(true);
    }
    
    private void HandleMovementAnimation(bool IsRunning)
    {
        if (playerAnimationController.GetRunningState() == IsRunning) return;

        playerAnimationController.SetIsRunning(IsRunning);
    }

    /// <summary>
    /// Rotate the player to face movement direction
    /// </summary>
    private void HandleRotation()
    {
        if (_moveDirection.magnitude < 0.01f)
        {
            return;
        }

        // Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

        // Smoothly rotate towards target
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Visualize movement boundaries
        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) * 0.5f,
            transform.position.y,
            (minBounds.y + maxBounds.y) * 0.5f
        );

        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            0.1f,
            maxBounds.y - minBounds.y
        );

        Gizmos.DrawWireCube(center, size);
    }

    #endregion
}
