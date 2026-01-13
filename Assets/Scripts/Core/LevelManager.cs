using UnityEngine;
using System.Collections;
using BaseDefender.Core;

/// <summary>
/// Manages level initialization and wave progression using ScriptableObject configurations.
/// Set up everything in the inspector - no code changes needed for new levels!
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Inspector Fields

    [Header("Level Configuration")]
    [Tooltip("Current level configuration (ScriptableObject)")]
    [SerializeField] private LevelConfiguration currentLevel;

    [Header("Spawn Points")]
    [Tooltip("Available spawn points for enemies")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("UI References (Optional)")]
    [SerializeField] private UnityEngine.UI.Text waveNumberText;
    [SerializeField] private UnityEngine.UI.Text waveNameText;

    #endregion

    #region Private Fields

    private int _currentWaveIndex = 0;
    private bool _levelActive = false;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        // Subscribe to base destroyed event
        GameEvents.OnBaseDestroyed += HandleBaseDestroyed;
    }

    private void OnDisable()
    {
        // Unsubscribe from base destroyed event
        GameEvents.OnBaseDestroyed -= HandleBaseDestroyed;
    }

    private void Start()
    {
        if (currentLevel != null)
        {
            StartLevel();
        }
        else
        {
            Debug.LogError("LevelManager: No level configuration assigned!");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start the current level
    /// </summary>
    public void StartLevel()
    {
        // Validate configuration
        if (!ValidateSetup())
        {
            return;
        }

        // Validate level configuration
        if (!currentLevel.Validate())
        {
            Debug.LogError("LevelManager: Level configuration validation failed!");
            return;
        }

        // Initialize enemy pool for this level
        currentLevel.InitializeEnemyPool();

        // Start wave progression
        _levelActive = true;
        _currentWaveIndex = 0;
        StartCoroutine(RunLevel());
    }

    /// <summary>
    /// Load and start a different level
    /// </summary>
    public void LoadLevel(LevelConfiguration newLevel)
    {
        if (newLevel == null)
        {
            Debug.LogError("LevelManager: Cannot load null level!");
            return;
        }

        // Stop current level
        StopAllCoroutines();
        _levelActive = false;

        // Clear enemies
        EnemySpawner.ClearAllEnemies();
        EnemyPool.Instance.DespawnAll();

        // Load new level
        currentLevel = newLevel;
        StartLevel();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validate that all required components are set up
    /// </summary>
    private bool ValidateSetup()
    {
        if (currentLevel == null)
        {
            Debug.LogError("LevelManager: Current level configuration is not assigned!");
            return false;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("LevelManager: No spawn points assigned!");
            return false;
        }

        if (EnemyPool.Instance == null)
        {
            Debug.LogError("LevelManager: EnemyPool not found in scene!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Run the entire level (all waves)
    /// </summary>
    private IEnumerator RunLevel()
    {
        Debug.Log($"Starting Level: {currentLevel.levelName}");

        for (int i = 0; i < currentLevel.waves.Count; i++)
        {
            if (!_levelActive) yield break;

            WaveConfiguration wave = currentLevel.waves[i];
            if (wave == null)
            {
                Debug.LogWarning($"LevelManager: Wave {i} is null, skipping!");
                continue;
            }

            _currentWaveIndex = i;
            UpdateWaveUI(i + 1, wave.waveName);

            // Spawn the wave
            yield return StartCoroutine(SpawnWave(wave));

            // Wait for all enemies to be defeated
            yield return StartCoroutine(WaitForWaveCompletion());

            // Check if level is still active (base might have been destroyed)
            if (!_levelActive) yield break;

            // Fire wave completed event
            GameEvents.WaveCompleted(i + 1);

            // Delay before next wave (except after last wave)
            if (i < currentLevel.waves.Count - 1)
            {
                Debug.Log($"Wave {i + 1} completed! Next wave in {currentLevel.timeBetweenWaves} seconds...");
                yield return new WaitForSeconds(currentLevel.timeBetweenWaves);
            }
        }

        OnLevelComplete();
    }

    /// <summary>
    /// Spawn all enemies for a wave
    /// </summary>
    private IEnumerator SpawnWave(WaveConfiguration wave)
    {
        Debug.Log($"Starting Wave: {wave.waveName}");

        // Show wave notification
        GameEvents.WaveStarted(_currentWaveIndex + 1, currentLevel.WaveCount);

        // Play wave start sound effect
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWaveStart();
        }

        // Wait for wave start delay
        yield return new WaitForSeconds(wave.waveStartDelay);

        // Spawn each enemy type
        foreach (var enemySpawn in wave.enemySpawns)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                if (!_levelActive) yield break;

                // Get spawn position
                Vector3 spawnPosition = GetSpawnPosition(enemySpawn);

                // Spawn enemy from pool
                Enemy enemy = EnemySpawner.SpawnEnemyPooled(
                    enemySpawn.enemyTypeId,
                    spawnPosition
                );

                if (enemy == null)
                {
                    Debug.LogWarning($"Failed to spawn enemy: {enemySpawn.enemyTypeId}");
                }

                // Wait before spawning next enemy
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        Debug.Log($"Wave {wave.waveName}: All enemies spawned");
    }

    /// <summary>
    /// Get spawn position for an enemy
    /// </summary>
    private Vector3 GetSpawnPosition(WaveEnemySpawn enemySpawn)
    {
        // Use specific spawn points if configured
        if (enemySpawn.specificSpawnPoints != null && enemySpawn.specificSpawnPoints.Count > 0)
        {
            int index = Random.Range(0, enemySpawn.specificSpawnPoints.Count);
            return enemySpawn.specificSpawnPoints[index].position;
        }

        // Otherwise use random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }

    /// <summary>
    /// Wait until all enemies are defeated
    /// </summary>
    private IEnumerator WaitForWaveCompletion()
    {
        // Wait a bit for enemies to spawn
        yield return new WaitForSeconds(1f);

        // Wait until no active enemies remain
        while (EnemySpawner.ActiveEnemyCount > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("All enemies defeated!");
    }

    /// <summary>
    /// Update wave UI
    /// </summary>
    private void UpdateWaveUI(int waveNumber, string waveName)
    {
        if (waveNumberText != null)
        {
            waveNumberText.text = $"Wave {waveNumber}/{currentLevel.WaveCount}";
        }

        if (waveNameText != null)
        {
            waveNameText.text = waveName;
        }
    }

    /// <summary>
    /// Called when level is completed
    /// </summary>
    private void OnLevelComplete()
    {
        _levelActive = false;

        // Check if base is still alive
        GameObject baseObj = GameObject.FindGameObjectWithTag("Base");
        if (baseObj != null)
        {
            Base baseComponent = baseObj.GetComponent<Base>();
            if (baseComponent != null && !baseComponent.IsDestroyed)
            {
                // Base is alive, player wins!
                Debug.Log($"Level Complete: {currentLevel.levelName}");
                //GameEvents.LevelCompleted(currentLevel.levelNumber);
                GameEvents.GameVictory();
                return;
            }
        }

        // If we get here, base was destroyed
        //Debug.Log("All enemies defeated, but base was destroyed!");
        //GameEvents.GameDefeat();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle base destroyed event
    /// </summary>
    private void HandleBaseDestroyed()
    {
        // Stop all wave processing
        StopAllCoroutines();
        _levelActive = false;

        Debug.Log("Base destroyed! Game Over.");

        // Fire game defeat event
        //GameEvents.GameDefeat();
    }

    #endregion

    #region Debug

    [ContextMenu("Print Level Info")]
    private void PrintLevelInfo()
    {
        if (currentLevel == null)
        {
            Debug.Log("No level configuration assigned");
            return;
        }

        Debug.Log($"=== Level: {currentLevel.levelName} ===");
        Debug.Log($"Waves: {currentLevel.WaveCount}");
        Debug.Log($"Total Enemies: {currentLevel.TotalEnemyCount}");
        Debug.Log($"Enemy Types: {string.Join(", ", currentLevel.GetAllEnemyTypes())}");

        if (EnemyPool.Instance != null)
        {
            Debug.Log(EnemyPool.Instance.GetDebugInfo());
        }
    }

    [ContextMenu("Validate Configuration")]
    private void ValidateConfiguration()
    {
        if (currentLevel == null)
        {
            Debug.LogWarning("No level configuration assigned");
            return;
        }

        bool valid = currentLevel.Validate();
        if (valid)
        {
            Debug.Log("Level configuration is valid!");
        }
        else
        {
            Debug.LogError("Level configuration has errors!");
        }
    }

    #endregion
}
