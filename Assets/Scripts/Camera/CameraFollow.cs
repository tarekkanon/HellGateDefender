using UnityEngine;

/// <summary>
/// Makes the camera smoothly follow a target (player) while maintaining a fixed perspective.
/// Uses LateUpdate to ensure smooth following after player movement.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    #region Inspector Fields

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [Tooltip("The offset from the target position (relative to target)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 25, 0f);

    [Header("Follow Settings")]
    [SerializeField]
    [Range(0.01f, 1f)]
    [Tooltip("How quickly the camera follows the target. Lower = smoother but slower")]
    private float smoothSpeed = 0.125f;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        ValidateConfiguration();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validate that all required configuration is present
    /// </summary>
    private void ValidateConfiguration()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Target not assigned! Attempting to find player...");

            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
                Debug.Log("CameraFollow: Found player and assigned as target.");
            }
            else
            {
                Debug.LogError("CameraFollow: No target assigned and could not find PlayerController in scene!");
            }
        }
    }

    /// <summary>
    /// Smoothly follow the target while maintaining the offset
    /// </summary>
    private void FollowTarget()
    {
        if (target == null)
        {
            return;
        }

        // Calculate the desired position
        Vector3 targetPosition = target.position + offset;

        // Smoothly interpolate to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set a new target for the camera to follow
    /// </summary>
    /// <param name="newTarget">The transform to follow</param>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// Set the camera offset from the target
    /// </summary>
    /// <param name="newOffset">The new offset vector</param>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    /// <summary>
    /// Set the smooth follow speed
    /// </summary>
    /// <param name="speed">Speed value between 0.01 and 1</param>
    public void SetSmoothSpeed(float speed)
    {
        smoothSpeed = Mathf.Clamp(speed, 0.01f, 1f);
    }

    #endregion
}
