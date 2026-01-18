using UnityEngine;
using UnityEngine.SceneManagement;
using BaseDefender.Core;
using BaseDefender.VFX;

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

    [Header("Ambient Atmosphere VFX")]
    [Tooltip("Enable ambient atmosphere VFX (floating embers, wisps)")]
    [SerializeField] private bool enableAmbientAtmosphere = true;
    [Tooltip("Center position for ambient atmosphere (usually map center)")]
    [SerializeField] private Vector3 ambientAtmospherePosition = Vector3.zero;
    [Tooltip("Size of the spawn volume for ambient particles")]
    [SerializeField] private Vector3 ambientAtmosphereVolume = new Vector3(20f, 10f, 20f);

    // VFX tracking
    private ParticleSystem _ambientAtmosphereVFX;

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

        // Initialize ambient atmosphere VFX
        InitializeAmbientAtmosphere();

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
    /// Initialize ambient atmosphere VFX (floating embers, dark wisps, energy motes)
    /// </summary>
    private void InitializeAmbientAtmosphere()
    {
        if (!enableAmbientAtmosphere) return;

        // Stop existing atmosphere if any
        if (_ambientAtmosphereVFX != null)
        {
            VFXHelper.StopEffect(_ambientAtmosphereVFX);
        }

        // Play new ambient atmosphere with custom volume
        _ambientAtmosphereVFX = VFXHelper.PlayAmbientAtmosphere(ambientAtmospherePosition, ambientAtmosphereVolume);
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

        // Clear VFX before scene reload
        CleanupGameVFX();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Return to the main menu scene
    /// </summary>
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        // Clear VFX before scene transition
        CleanupGameVFX();

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Cleanup all game VFX before scene transitions
    /// </summary>
    private void CleanupGameVFX()
    {
        // Stop ambient atmosphere
        if (_ambientAtmosphereVFX != null)
        {
            VFXHelper.StopEffect(_ambientAtmosphereVFX);
            _ambientAtmosphereVFX = null;
        }

        // Clear all active VFX effects
        VFXHelper.ClearAllEffects();
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
