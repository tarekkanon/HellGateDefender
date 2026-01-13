using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu UI controller.
/// Handles play and quit button functionality.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    #region Inspector Fields

    [Header("UI Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "Game";

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        Initialize();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the main menu
    /// </summary>
    private void Initialize()
    {
        // Setup button listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }
        else
        {
            Debug.LogError("MainMenuUI: Play button not assigned!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        else
        {
            Debug.LogWarning("MainMenuUI: Quit button not assigned!");
        }

        // Ensure time scale is normal (in case returning from paused game)
        Time.timeScale = 1f;
    }

    #endregion

    #region Button Handlers

    /// <summary>
    /// Handle play button click - load game scene
    /// </summary>
    private void OnPlayClicked()
    {
        Debug.Log("MainMenuUI: Loading game scene...");

        // Load the game scene
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("MainMenuUI: Game scene name not set!");
        }
    }

    /// <summary>
    /// Handle quit button click - exit application
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("MainMenuUI: Quitting application...");

        #if UNITY_EDITOR
        // In editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // In build, quit application
        Application.Quit();
        #endif
    }

    #endregion
}
