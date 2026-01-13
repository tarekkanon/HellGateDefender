using UnityEngine;

/// <summary>
/// Controls enemy animations based on enemy state.
/// Manages transitions between running and death animations.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Animation Settings")]
    [Tooltip("The Animator component (auto-assigned if not set)")]
    [SerializeField] private Animator animator;

    #endregion

    #region Animation Parameter Names

    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Auto-assign Animator if not set
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"EnemyAnimationController: No Animator component found on {gameObject.name}!");
            }
        }

    }

    private void Start()
    {
        // Start with running animation
        SetRunning(true);
    }


    #endregion

    #region Public Methods

    /// <summary>
    /// Set the running animation state
    /// </summary>
    /// <param name="isRunning">Whether the enemy is running</param>
    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            animator.SetBool(IsRunning, isRunning);
        }
    }

    /// <summary>
    /// Trigger the death animation
    /// </summary>
    public void SetDead()
    {
        if (animator != null)
        {
            animator.SetBool(IsRunning, false);
            animator.SetBool(IsDead, true);
        }
    }

    #endregion
}
