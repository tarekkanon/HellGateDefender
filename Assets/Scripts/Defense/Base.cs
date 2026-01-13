using UnityEngine;
using BaseDefender.Core;

/// <summary>
/// The base structure that players must defend.
/// Tracks health and fires events when damaged or destroyed.
/// </summary>
public class Base : MonoBehaviour
{
    #region Inspector Fields

    [Header("Base Stats")]
    [SerializeField] private int maxHealth = 500;

    [Header("Visual Feedback (Optional)")]
    [SerializeField] private GameObject[] damageStates; // Optional: different models for damage levels

    #endregion

    #region Private Fields

    private int _currentHealth;

    #endregion

    #region Properties

    /// <summary>
    /// Current health of the base
    /// </summary>
    public int CurrentHealth => _currentHealth;

    /// <summary>
    /// Maximum health of the base
    /// </summary>
    public int MaxHealth => maxHealth;

    /// <summary>
    /// Health as a percentage (0-1)
    /// </summary>
    public float HealthPercentage => (float)_currentHealth / maxHealth;

    /// <summary>
    /// Whether the base has been destroyed
    /// </summary>
    public bool IsDestroyed => _currentHealth <= 0;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        Initialize();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initialize the base
    /// </summary>
    public void Initialize()
    {
        _currentHealth = maxHealth;
        UpdateVisuals();

        // Fire initial health changed event
        GameEvents.BaseHealthChanged(_currentHealth, maxHealth);
    }

    /// <summary>
    /// Take damage from enemies
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        if (IsDestroyed)
        {
            return;
        }

        if (damage < 0)
        {
            Debug.LogWarning("Base: Attempted to take negative damage!");
            return;
        }

        // Reduce health
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);

        // Play base hit sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBaseHit();
        }

        // Fire events
        GameEvents.BaseDamaged(damage);
        GameEvents.BaseHealthChanged(_currentHealth, maxHealth);

        // Update visuals
        UpdateVisuals();

        // Check if destroyed
        if (_currentHealth <= 0)
        {
            OnDestroyed();
        }
    }

    /// <summary>
    /// Heal the base (optional, not used in MVP)
    /// </summary>
    /// <param name="amount">Amount to heal</param>
    public void Heal(int amount)
    {
        if (IsDestroyed)
        {
            return;
        }

        if (amount < 0)
        {
            Debug.LogWarning("Base: Attempted to heal with negative amount!");
            return;
        }

        // Increase health (capped at max)
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth);

        // Fire event
        GameEvents.BaseHealthChanged(_currentHealth, maxHealth);

        // Update visuals
        UpdateVisuals();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handle base destruction
    /// </summary>
    private void OnDestroyed()
    {
        // Fire destroyed event
        GameEvents.BaseDestroyed();

        // Optional: Play destruction animation or effects
        Debug.Log("Base has been destroyed!");
    }

    /// <summary>
    /// Update visual state based on health percentage
    /// </summary>
    private void UpdateVisuals()
    {
        if (damageStates == null || damageStates.Length == 0)
        {
            return;
        }

        // Disable all damage states first
        foreach (GameObject state in damageStates)
        {
            if (state != null)
            {
                state.SetActive(false);
            }
        }

        // Calculate which damage state to show based on health percentage
        float healthPercent = HealthPercentage;

        if (healthPercent > 0.75f && damageStates.Length > 0 && damageStates[0] != null)
        {
            // Healthy state (75-100%)
            damageStates[0].SetActive(true);
        }
        else if (healthPercent > 0.5f && damageStates.Length > 1 && damageStates[1] != null)
        {
            // Light damage (50-75%)
            damageStates[1].SetActive(true);
        }
        else if (healthPercent > 0.25f && damageStates.Length > 2 && damageStates[2] != null)
        {
            // Medium damage (25-50%)
            damageStates[2].SetActive(true);
        }
        else if (healthPercent > 0f && damageStates.Length > 3 && damageStates[3] != null)
        {
            // Heavy damage (0-25%)
            damageStates[3].SetActive(true);
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Draw health bar above base
        if (Application.isPlaying)
        {
            Vector3 healthBarPos = transform.position + Vector3.up * 5f;
            float healthPercent = HealthPercentage;

            // Background (red)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 2f, healthBarPos + Vector3.right * 2f);

            // Foreground (green)
            Gizmos.color = Color.green;
            float healthWidth = healthPercent * 4f - 2f;
            Gizmos.DrawLine(healthBarPos - Vector3.right * 2f, healthBarPos + Vector3.right * healthWidth);

            // Health text
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(healthBarPos + Vector3.up * 0.5f, $"{_currentHealth} / {maxHealth}");
            #endif
        }
    }

    #endregion
}
