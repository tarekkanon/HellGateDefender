using UnityEngine;
using UnityEngine.SceneManagement;
using BaseDefender.Core;

/// <summary>
/// Singleton game manager that controls the overall game state and economy system.
/// Manages coins, game flow, and coordinates between major systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;

    /// <summary>
    /// Singleton instance of the GameManager
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    #region Properties

    [SerializeField] 
    private int _coins = 0;
    private GameState _currentState = GameState.MainMenu;

    /// <summary>
    /// Current number of coins the player has
    /// </summary>
    public int Coins => _coins;

    /// <summary>
    /// Current state of the game
    /// </summary>
    public GameState CurrentState => _currentState;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Enforce singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to game events
        GameEvents.OnBaseDestroyed += HandleBaseDestroyed;
    }

    private void OnDisable()
    {
        // Unsubscribe from game events
        GameEvents.OnBaseDestroyed -= HandleBaseDestroyed;
    }

    #endregion

    #region Game Flow Methods

    /// <summary>
    /// Start the game and initialize wave 1
    /// </summary>
    public void StartGame()
    {
        _coins = 0;
        ChangeGameState(GameState.Playing);
        GameEvents.CoinsChanged(_coins);

        // Start gameplay music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        // Find and start the wave manager
        //WaveManager waveManager = FindObjectOfType<WaveManager>();
        //if (waveManager != null)
        //{
        //    waveManager.StartWaves();
        //}
        //else
        //{
        //    Debug.LogError("GameManager: WaveManager not found in scene!");
        //}
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
        ChangeGameState(GameState.Paused);
    }

    /// <summary>
    /// Resume the game from paused state
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        ChangeGameState(GameState.Playing);
    }

    /// <summary>
    /// Trigger victory state (called when all waves are completed)
    /// </summary>
    public void TriggerVictory()
    {
        ChangeGameState(GameState.Victory);

        // Play victory sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictory();
        }

        GameEvents.GameVictory();
    }

    /// <summary>
    /// Trigger defeat state (called when base is destroyed)
    /// </summary>
    public void TriggerDefeat()
    {
        ChangeGameState(GameState.Defeat);

        // Play defeat sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDefeat();
        }

        GameEvents.GameDefeat();
    }

    /// <summary>
    /// Restart the current game scene
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Return to the main menu scene
    /// </summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Change the game state and notify listeners
    /// </summary>
    private void ChangeGameState(GameState newState)
    {
        _currentState = newState;
        GameEvents.GameStateChanged(newState);
    }

    #endregion

    #region Economy Methods

    /// <summary>
    /// Add coins to the player's total
    /// </summary>
    /// <param name="amount">Number of coins to add</param>
    public void AddCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("GameManager: Attempted to add negative coins!");
            return;
        }

        _coins += amount;
        GameEvents.CoinsChanged(_coins);
    }

    /// <summary>
    /// Spend coins if the player has enough
    /// </summary>
    /// <param name="amount">Number of coins to spend</param>
    /// <returns>True if coins were spent successfully, false if insufficient funds</returns>
    public bool SpendCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("GameManager: Attempted to spend negative coins!");
            return false;
        }

        if (_coins >= amount)
        {
            _coins -= amount;
            GameEvents.CoinsChanged(_coins);
            return true;
        }

        return false;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle the base destroyed event
    /// </summary>
    private void HandleBaseDestroyed()
    {
        TriggerDefeat();
    }

    #endregion
}
