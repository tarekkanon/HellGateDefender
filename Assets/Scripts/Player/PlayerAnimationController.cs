using UnityEngine;

/// <summary>
/// Controls player animations based on player state.
/// Manages transitions between idle, running, and attack animations.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    #region Inspector Fields

    [Header("Animation Settings")]
    [Tooltip("The Animator component (auto-assigned if not set)")]
    [SerializeField] private Animator animator;

    [Header("Movement Detection")]
    [Tooltip("Velocity threshold to start running animation")]
    [SerializeField] private float runStartThreshold = 0.2f;

    [Tooltip("Velocity threshold to stop running animation (should be lower than start threshold)")]
    [SerializeField] private float runStopThreshold = 0.05f;

    #endregion

    #region Animation Parameter Names

    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int Attack = Animator.StringToHash("Attack");

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
                Debug.LogError($"PlayerAnimationController: No Animator component found on {gameObject.name}!");
            }
        }

    }


    #endregion


    #region Public Methods

    /// <summary>
    /// Trigger the attack animation
    /// </summary>
    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger(Attack);
        }
    }

    public void SetIsRunning(bool _IsRunning)
    {
        if (animator == null) return;

        animator.SetBool(IsRunning, _IsRunning);
    }

    public bool GetRunningState() => animator.GetBool(IsRunning);

    /// <summary>
    /// Check if the attack animation is currently playing
    /// </summary>
    /// <returns>True if attack animation is playing</returns>
    public bool IsAttacking()
    {
        if (animator == null) return false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Attack") && stateInfo.normalizedTime < 1.0f;
    }

    #endregion
}
