using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the main gameplay HUD (Heads-Up Display).
/// Updates coin count, wave progress, base health, and displays notifications.
/// </summary>
public class HUDManager : MonoBehaviour
{
    #region Inspector Fields

    [Header("Coin Display")]
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("Wave Display")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Base Health Display")]
    [SerializeField] private Slider baseHealthSlider;
    [SerializeField] private TextMeshProUGUI baseHealthText;
    [SerializeField] private Image baseHealthFill; // Optional: for color changes

    [Header("Wave Notification")]
    [SerializeField] private GameObject waveNotification;
    [SerializeField] private TextMeshProUGUI waveNotificationText;
    [SerializeField] private float notificationDuration = 2f;

    [Header("Health Bar Colors (Optional)")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color damagedColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    #endregion

    #region Private Fields

    private Coroutine _notificationCoroutine;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        // Subscribe to game events
        GameEvents.OnCoinsChanged += UpdateCoins;
        GameEvents.OnWaveStarted += HandleWaveStarted;
        GameEvents.OnBaseHealthChanged += UpdateBaseHealth;
    }

    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnCoinsChanged -= UpdateCoins;
        GameEvents.OnWaveStarted -= HandleWaveStarted;
        GameEvents.OnBaseHealthChanged -= UpdateBaseHealth;
    }

    private void Start()
    {
        Initialize();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the HUD
    /// </summary>
    private void Initialize()
    {
        // Hide wave notification initially
        if (waveNotification != null)
        {
            waveNotification.SetActive(false);
        }

        // Set initial values
        UpdateCoins(0);

        // Validate components
        ValidateComponents();
    }

    /// <summary>
    /// Validate that all required components are assigned
    /// </summary>
    private void ValidateComponents()
    {
        if (coinsText == null)
        {
            Debug.LogWarning("HUDManager: Coins text not assigned!");
        }

        if (waveText == null)
        {
            Debug.LogWarning("HUDManager: Wave text not assigned!");
        }

        if (baseHealthSlider == null && baseHealthText == null)
        {
            Debug.LogWarning("HUDManager: Neither base health slider nor text assigned!");
        }

        if (waveNotification == null || waveNotificationText == null)
        {
            Debug.LogWarning("HUDManager: Wave notification components not assigned!");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Update the coin display
    /// </summary>
    public void UpdateCoins(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {coins}";
        }
    }

    /// <summary>
    /// Update the wave display
    /// </summary>
    public void UpdateWave(int current, int total)
    {
        if (waveText != null)
        {
            Debug.Log("UpdateWave IN HUD total : " + total + " , current : " + current);
            if (total > 0)
            {
                waveText.text = $"Wave: {current}/{total}";
            }
            else
            {
                waveText.text = "Wave: --";
            }
        }
    }

    /// <summary>
    /// Update the base health display
    /// </summary>
    public void UpdateBaseHealth(int current, int max)
    {
        // Update slider
        if (baseHealthSlider != null)
        {
            baseHealthSlider.maxValue = max;
            baseHealthSlider.value = current;
        }

        // Update text
        if (baseHealthText != null)
        {
            baseHealthText.text = $"{current} / {max}";
        }

        // Update health bar color based on percentage
        UpdateHealthBarColor(current, max);
    }

    /// <summary>
    /// Show wave notification for a duration
    /// </summary>
    public void ShowWaveNotification(int waveNumber)
    {
        if (waveNotification == null || waveNotificationText == null)
        {
            return;
        }

        // Stop existing notification coroutine if running
        if (_notificationCoroutine != null)
        {
            StopCoroutine(_notificationCoroutine);
        }

        // Start new notification
        _notificationCoroutine = StartCoroutine(WaveNotificationCoroutine(waveNumber));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update health bar fill color based on health percentage
    /// </summary>
    private void UpdateHealthBarColor(int current, int max)
    {
        if (baseHealthFill == null)
        {
            return;
        }

        float healthPercent = (float)current / max;

        if (healthPercent > 0.6f)
        {
            baseHealthFill.color = healthyColor;
        }
        else if (healthPercent > 0.3f)
        {
            baseHealthFill.color = damagedColor;
        }
        else
        {
            baseHealthFill.color = criticalColor;
        }
    }

    /// <summary>
    /// Coroutine to show wave notification for a duration
    /// </summary>
    private IEnumerator WaveNotificationCoroutine(int waveNumber)
    {
        // Set notification text
        waveNotificationText.text = $"Wave {waveNumber}";

        // Show notification
        waveNotification.SetActive(true);

        // Wait for duration
        yield return new WaitForSeconds(notificationDuration);

        // Hide notification
        waveNotification.SetActive(false);

        _notificationCoroutine = null;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle wave started event
    /// </summary>
    private void HandleWaveStarted(int waveNumber, int totalWaves)
    {
        Debug.Log("HandleWaveStarted IN HUD total : " + totalWaves);
        UpdateWave(waveNumber, totalWaves);
        ShowWaveNotification(waveNumber);
    }

    #endregion
}
