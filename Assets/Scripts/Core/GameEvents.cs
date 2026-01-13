using System;
using UnityEngine;

/// <summary>
/// Static event system for decoupled communication between game systems.
/// All game events are centralized here to prevent tight coupling between components.
/// </summary>
public static class GameEvents
{
    #region Game State Events

    /// <summary>
    /// Fired when the game state changes (MainMenu, Playing, Paused, Victory, Defeat)
    /// </summary>
    public static event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// Invoke the game state changed event
    /// </summary>
    public static void GameStateChanged(GameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }

    #endregion

    #region Economy Events

    /// <summary>
    /// Fired when the player's coin count changes
    /// </summary>
    public static event Action<int> OnCoinsChanged;

    /// <summary>
    /// Invoke the coins changed event
    /// </summary>
    public static void CoinsChanged(int totalCoins)
    {
        OnCoinsChanged?.Invoke(totalCoins);
    }

    #endregion

    #region Wave Events

    /// <summary>
    /// Fired when a new wave starts
    /// </summary>
    public static event Action<int, int> OnWaveStarted;

    /// <summary>
    /// Invoke the wave started event
    /// </summary>
    public static void WaveStarted(int waveNumber, int totalWaves)
    {
        OnWaveStarted?.Invoke(waveNumber, totalWaves);
    }

    /// <summary>
    /// Fired when a wave is completed (all enemies defeated)
    /// </summary>
    public static event Action<int> OnWaveCompleted;

    /// <summary>
    /// Invoke the wave completed event
    /// </summary>
    public static void WaveCompleted(int waveNumber)
    {
        OnWaveCompleted?.Invoke(waveNumber);
    }

    #endregion

    #region Combat Events

    /// <summary>
    /// Fired when an enemy is killed
    /// </summary>
    public static event Action<Vector3, int> OnEnemyKilled;

    /// <summary>
    /// Invoke the enemy killed event
    /// </summary>
    public static void EnemyKilled(Vector3 position, int coinValue)
    {
        OnEnemyKilled?.Invoke(position, coinValue);
    }

    /// <summary>
    /// Fired when an enemy reaches the base and is removed from the game
    /// </summary>
    public static event Action OnEnemyReachedBase;

    /// <summary>
    /// Invoke the enemy reached base event
    /// </summary>
    public static void EnemyReachedBase()
    {
        OnEnemyReachedBase?.Invoke();
    }

    #endregion

    #region Base Events

    /// <summary>
    /// Fired when the base health changes
    /// </summary>
    public static event Action<int, int> OnBaseHealthChanged;

    /// <summary>
    /// Invoke the base health changed event
    /// </summary>
    public static void BaseHealthChanged(int currentHealth, int maxHealth)
    {
        OnBaseHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Fired when the base takes damage
    /// </summary>
    public static event Action<int> OnBaseDamaged;

    /// <summary>
    /// Invoke the base damaged event
    /// </summary>
    public static void BaseDamaged(int damage)
    {
        OnBaseDamaged?.Invoke(damage);
    }

    /// <summary>
    /// Fired when the base is destroyed (health reaches 0)
    /// </summary>
    public static event Action OnBaseDestroyed;

    /// <summary>
    /// Invoke the base destroyed event
    /// </summary>
    public static void BaseDestroyed()
    {
        OnBaseDestroyed?.Invoke();
    }

    #endregion

    #region Defense Events

    /// <summary>
    /// Fired when a turret is activated
    /// </summary>
    public static event Action<Turret> OnTurretActivated;

    /// <summary>
    /// Invoke the turret activated event
    /// </summary>
    public static void TurretActivated(Turret turret)
    {
        OnTurretActivated?.Invoke(turret);
    }

    #endregion

    #region Victory/Defeat Events

    /// <summary>
    /// Fired when the player wins the game (all waves completed)
    /// </summary>
    public static event Action OnGameVictory;

    /// <summary>
    /// Invoke the game victory event
    /// </summary>
    public static void GameVictory()
    {
        OnGameVictory?.Invoke();
    }

    /// <summary>
    /// Fired when the player loses the game (base destroyed)
    /// </summary>
    public static event Action OnGameDefeat;

    /// <summary>
    /// Invoke the game defeat event
    /// </summary>
    public static void GameDefeat()
    {
        OnGameDefeat?.Invoke();
    }

    #endregion
}

/// <summary>
/// Enum representing the current state of the game
/// </summary>
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Victory,
    Defeat
}
