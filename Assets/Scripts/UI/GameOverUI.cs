using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Game over UI controller for victory and defeat screens.
/// Displays appropriate panel and provides restart/menu options.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    #region Inspector Fields

    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [Header("Defeat Panel Components")]
    [SerializeField] private TextMeshProUGUI waveReachedText;

    [Header("Victory Panel Components (Optional)")]
    [SerializeField] private TextMeshProUGUI victoryMessageText;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        // Subscribe to game events
        GameEvents.OnGameVictory += HandleGameVictory;
        GameEvents.OnGameDefeat += HandleGameDefeat;
    }

    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnGameVictory -= HandleGameVictory;
        GameEvents.OnGameDefeat -= HandleGameDefeat;
    }

    private void Start()
    {
        Initialize();
    }

    #endregion


    #region Private Attributes

    private Image image;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the game over UI
    /// </summary>
    private void Initialize()
    {
        // Setup button listeners
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }
        else
        {
            Debug.LogError("GameOverUI: Play Again button not assigned!");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
        else
        {
            Debug.LogError("GameOverUI: Main Menu button not assigned!");
        }

        image = GetComponent<Image>();

        // Hide both panels initially
        HideAllPanels();

        // Validate components
        ValidateComponents();
    }

    /// <summary>
    /// Validate that required components are assigned
    /// </summary>
    private void ValidateComponents()
    {
        if (victoryPanel == null)
        {
            Debug.LogWarning("GameOverUI: Victory panel not assigned!");
        }

        if (defeatPanel == null)
        {
            Debug.LogWarning("GameOverUI: Defeat panel not assigned!");
        }

        if (waveReachedText == null)
        {
            Debug.LogWarning("GameOverUI: Wave reached text not assigned!");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show victory screen
    /// </summary>
    public void ShowVictory()
    {
        HideAllPanels();

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryMessageText != null)
        {
            victoryMessageText.text = "You Win!\nAll waves defeated!";
        }

        ToggleButtons(true);

        Debug.Log("GameOverUI: Showing victory screen");
    }

    /// <summary>
    /// Show defeat screen with wave reached info
    /// </summary>
    public void ShowDefeat()
    {
        HideAllPanels();

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }

        if (waveReachedText != null)
        {
            waveReachedText.text = $"Game Over";
        }

        ToggleButtons(true);

        Debug.Log($"GameOverUI: Showing defeat screen");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Hide all panels
    /// </summary>
    private void HideAllPanels()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (defeatPanel != null)
        {
            defeatPanel.SetActive(false);
        }

        ToggleButtons(false);
    }

    private void ToggleButtons(bool show)
    {
        if (image != null)
        {
            image.raycastTarget = show;
        }

        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(show);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(show);
        }
    }

    #endregion

    #region Button Handlers

    /// <summary>
    /// Handle play again button click
    /// </summary>
    private void OnPlayAgainClicked()
    {
        Debug.Log("GameOverUI: Restarting game...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            Debug.LogError("GameOverUI: GameManager instance not found!");
        }
    }

    /// <summary>
    /// Handle main menu button click
    /// </summary>
    private void OnMainMenuClicked()
    {
        Debug.Log("GameOverUI: Returning to main menu...");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMenu();
        }
        else
        {
            Debug.LogError("GameOverUI: GameManager instance not found!");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle game victory event
    /// </summary>
    private void HandleGameVictory()
    {
        ShowVictory();
    }

    /// <summary>
    /// Handle game defeat event
    /// </summary>
    private void HandleGameDefeat()
    {
        // Get current wave from WaveManager if available

        ShowDefeat();
    }

    #endregion
}
