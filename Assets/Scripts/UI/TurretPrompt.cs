using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// World-space UI prompt for turret activation.
/// Displays activation cost and provides a button to activate the turret.
/// </summary>
public class TurretPrompt : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI Components")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button activateButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Billboard Settings")]
    [SerializeField] private bool faceCamera = true;

    [Header("Affordability Colors")]
    [SerializeField] private Color canAffordColor = Color.white;
    [SerializeField] private Color cannotAffordColor = Color.red;

    #endregion

    #region Private Fields

    private Turret _parentTurret;
    private Camera _mainCamera;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Find parent turret
        _parentTurret = GetComponentInParent<Turret>();
        if (_parentTurret == null)
        {
            Debug.LogError("TurretPrompt: Parent Turret component not found!");
        }

        // Get main camera
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogWarning("TurretPrompt: Main camera not found!");
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        // Subscribe to coin changes to update affordability
        GameEvents.OnCoinsChanged += HandleCoinsChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        GameEvents.OnCoinsChanged -= HandleCoinsChanged;
    }

    private void Update()
    {
        // Billboard: face camera
        if (faceCamera && _mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                            _mainCamera.transform.rotation * Vector3.up);
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the turret prompt
    /// </summary>
    private void Initialize()
    {
        // Setup button click listener
        if (activateButton != null)
        {
            activateButton.onClick.AddListener(OnActivateClicked);
        }
        else
        {
            Debug.LogError("TurretPrompt: Activate button not assigned!");
        }

        // Update cost text
        UpdateCostText();

        // Hide initially
        Hide();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show the prompt
    /// </summary>
    public void Show()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
            UpdateCostText();
            UpdateAffordability();
        }
    }

    /// <summary>
    /// Hide the prompt
    /// </summary>
    public void Hide()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update the cost text display
    /// </summary>
    private void UpdateCostText()
    {
        if (_parentTurret == null || costText == null)
        {
            return;
        }

        costText.text = $"Cost: {_parentTurret.ActivationCost}";
    }

    /// <summary>
    /// Update button interactability and visual state based on affordability
    /// </summary>
    private void UpdateAffordability()
    {
        if (_parentTurret == null || activateButton == null)
        {
            return;
        }

        bool canAfford = GameManager.Instance != null &&
                        GameManager.Instance.Coins >= _parentTurret.ActivationCost;

        // Update button interactability
        activateButton.interactable = canAfford;

        // Update text color
        if (costText != null)
        {
            costText.color = canAfford ? canAffordColor : cannotAffordColor;
        }

        // Update button text
        if (buttonText != null)
        {
            buttonText.text = canAfford ? "Activate" : "Not Enough Coins";
        }
    }

    /// <summary>
    /// Handle activate button click
    /// </summary>
    private void OnActivateClicked()
    {
        if (_parentTurret == null)
        {
            Debug.LogError("TurretPrompt: Cannot activate - parent turret not found!");
            return;
        }

        // Try to activate the turret
        bool success = _parentTurret.TryActivate();

        if (success)
        {
            // Hide prompt on successful activation
            Hide();
        }
        else
        {
            // Update affordability to reflect failed attempt
            UpdateAffordability();
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle coin count changes to update affordability
    /// </summary>
    private void HandleCoinsChanged(int coins)
    {
        // Only update if prompt is visible
        if (promptPanel != null && promptPanel.activeSelf)
        {
            UpdateAffordability();
        }
    }

    #endregion
}
