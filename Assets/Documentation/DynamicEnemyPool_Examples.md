# Dynamic Enemy Pool - Usage Examples

## Example 1: Simple Setup with Constants

```csharp
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameObject scoutPrefab;
    public GameObject gruntPrefab;
    public GameObject tankPrefab;

    void Start()
    {
        // Register enemy types using constants (prevents typos)
        EnemyPool.Instance.RegisterEnemyType(EnemyTypes.Scout, scoutPrefab, 20, 50);
        EnemyPool.Instance.RegisterEnemyType(EnemyTypes.Grunt, gruntPrefab, 10, 30);
        EnemyPool.Instance.RegisterEnemyType(EnemyTypes.Tank, tankPrefab, 5, 15);
    }
}
```

## Example 2: Spawning Enemies with Different Ratios

```csharp
using UnityEngine;

public class SimpleWaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        StartCoroutine(SpawnWave1());
    }

    IEnumerator SpawnWave1()
    {
        // Wave 1: 5 Scouts only
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            EnemySpawner.SpawnEnemyPooled(EnemyTypes.Scout, pos);
            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(5f);

        // Wave 2: 5 Scouts, 3 Grunts
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            EnemySpawner.SpawnEnemyPooled(EnemyTypes.Scout, pos);
            yield return new WaitForSeconds(1.5f);
        }

        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            EnemySpawner.SpawnEnemyPooled(EnemyTypes.Grunt, pos);
            yield return new WaitForSeconds(1.5f);
        }
    }
}
```

## Example 3: Using WaveConfiguration ScriptableObjects

```csharp
using UnityEngine;
using System.Collections;

public class WaveSystemManager : MonoBehaviour
{
    public WaveConfiguration[] waves;
    public Transform[] spawnPoints;

    private int currentWaveIndex = 0;

    void Start()
    {
        StartCoroutine(RunAllWaves());
    }

    IEnumerator RunAllWaves()
    {
        foreach (var wave in waves)
        {
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(5f); // Delay between waves
        }

        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnWave(WaveConfiguration wave)
    {
        Debug.Log($"Starting {wave.waveName}");

        yield return new WaitForSeconds(wave.waveStartDelay);

        foreach (var enemySpawn in wave.enemySpawns)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                // Use specific spawn point if configured, otherwise random
                Vector3 spawnPos;
                if (enemySpawn.specificSpawnPoints.Count > 0)
                {
                    int index = Random.Range(0, enemySpawn.specificSpawnPoints.Count);
                    spawnPos = enemySpawn.specificSpawnPoints[index].position;
                }
                else
                {
                    int index = Random.Range(0, spawnPoints.Length);
                    spawnPos = spawnPoints[index].position;
                }

                EnemySpawner.SpawnEnemyPooled(enemySpawn.enemyTypeId, spawnPos);
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }
}
```

**Create WaveConfiguration Assets:**

1. Right-click in Project → Create → Game → Wave Configuration
2. Name it "Wave_1_Scouts"
3. Configure:
   - Wave Name: "Wave 1 - Scout Rush"
   - Spawn Interval: 1.5
   - Enemy Spawns:
     - [0] Enemy Type Id: "Scout", Count: 5

4. Create "Wave_2_Mixed":
   - Wave Name: "Wave 2 - Mixed Forces"
   - Spawn Interval: 1.5
   - Enemy Spawns:
     - [0] Enemy Type Id: "Scout", Count: 5
     - [1] Enemy Type Id: "Grunt", Count: 3

## Example 4: Level-Based Enemy Pool Configuration

```csharp
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelConfiguration currentLevel;

    void Start()
    {
        InitializeLevel(currentLevel);
    }

    void InitializeLevel(LevelConfiguration level)
    {
        // Validate configuration
        if (!level.Validate())
        {
            Debug.LogError("Level configuration is invalid!");
            return;
        }

        // Initialize enemy pool for this level
        level.InitializeEnemyPool();

        // Start wave system
        StartCoroutine(RunLevelWaves(level));
    }

    IEnumerator RunLevelWaves(LevelConfiguration level)
    {
        foreach (var wave in level.waves)
        {
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(level.timeBetweenWaves);
        }

        Debug.Log($"Level {level.levelName} completed!");
    }

    IEnumerator SpawnWave(WaveConfiguration wave)
    {
        // ... (same as Example 3)
    }
}
```

**Create Level Configurations:**

**Level 1 Config:**
```
Level Number: 1
Level Name: "Grasslands"

Level Enemy Pools:
├─ [0] Enemy Type Id: "Scout"
│      Prefab: ScoutPrefab
│      Initial Pool Size: 15
│      Max Pool Size: 30
└─ [1] Enemy Type Id: "Grunt"
       Prefab: GruntPrefab
       Initial Pool Size: 5
       Max Pool Size: 15

Waves:
├─ Wave_1_Scouts (5 scouts)
└─ Wave_2_Mixed (5 scouts, 3 grunts)
```

**Level 5 Config:**
```
Level Number: 5
Level Name: "Dark Forest"

Level Enemy Pools:
├─ [0] Enemy Type Id: "Scout", Initial: 20, Max: 40
├─ [1] Enemy Type Id: "Grunt", Initial: 15, Max: 30
└─ [2] Enemy Type Id: "Tank", Initial: 8, Max: 20

Waves:
├─ Wave_1 (6 scouts, 4 grunts)
├─ Wave_2 (4 scouts, 6 grunts, 1 tank)
└─ Wave_3 (5 scouts, 5 grunts, 4 tanks)
```

## Example 5: Adding New Enemy Type at Runtime

```csharp
using UnityEngine;

public class BossLevelManager : MonoBehaviour
{
    public GameObject bossGoblinPrefab;

    void Start()
    {
        // Add a new boss enemy type dynamically
        EnemyPool.Instance.RegisterEnemyType("BossGoblin", bossGoblinPrefab,
            initialPoolSize: 1,
            maxPoolSize: 2);

        // Now you can spawn it
        SpawnBoss();
    }

    void SpawnBoss()
    {
        Vector3 bossSpawnPoint = new Vector3(0, 0, 20);
        Enemy boss = EnemySpawner.SpawnEnemyPooled("BossGoblin", bossSpawnPoint);

        Debug.Log("Boss has arrived!");
    }
}
```

## Example 6: Dynamic Difficulty Adjustment

```csharp
using UnityEngine;

public class DynamicDifficultyManager : MonoBehaviour
{
    public GameObject[] enemyVariants; // Different difficulty variants
    private int currentDifficulty = 1;

    void Start()
    {
        RegisterEnemiesForDifficulty(currentDifficulty);
    }

    void RegisterEnemiesForDifficulty(int difficulty)
    {
        // Clear existing
        EnemyPool.Instance.ClearAllPools();

        // Register appropriate enemy variants based on difficulty
        if (difficulty == 1)
        {
            // Easy: Only scouts and grunts
            EnemyPool.Instance.RegisterEnemyType("Scout", enemyVariants[0], 15);
            EnemyPool.Instance.RegisterEnemyType("Grunt", enemyVariants[1], 8);
        }
        else if (difficulty == 2)
        {
            // Medium: Add tanks, increase pool sizes
            EnemyPool.Instance.RegisterEnemyType("Scout", enemyVariants[0], 20);
            EnemyPool.Instance.RegisterEnemyType("Grunt", enemyVariants[1], 12);
            EnemyPool.Instance.RegisterEnemyType("Tank", enemyVariants[2], 6);
        }
        else if (difficulty == 3)
        {
            // Hard: Add elite variants
            EnemyPool.Instance.RegisterEnemyType("ScoutElite", enemyVariants[3], 25);
            EnemyPool.Instance.RegisterEnemyType("GruntElite", enemyVariants[4], 15);
            EnemyPool.Instance.RegisterEnemyType("TankElite", enemyVariants[5], 8);
        }
    }

    public void IncreaseDifficulty()
    {
        currentDifficulty++;
        RegisterEnemiesForDifficulty(currentDifficulty);
        Debug.Log($"Difficulty increased to {currentDifficulty}");
    }
}
```

## Example 7: Checking Available Enemies

```csharp
using UnityEngine;

public class WaveValidator : MonoBehaviour
{
    public WaveConfiguration wave;

    void ValidateWave()
    {
        foreach (var enemySpawn in wave.enemySpawns)
        {
            if (!EnemyPool.Instance.IsEnemyTypeRegistered(enemySpawn.enemyTypeId))
            {
                Debug.LogError($"Enemy type '{enemySpawn.enemyTypeId}' is not registered in pool!");
            }
        }

        // List all available enemy types
        var availableTypes = EnemyPool.Instance.GetRegisteredEnemyTypes();
        Debug.Log("Available enemy types: " + string.Join(", ", availableTypes));
    }
}
```

## Example 8: Debug and Monitoring

```csharp
using UnityEngine;

public class PoolDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Print pool status
            Debug.Log(EnemyPool.Instance.GetDebugInfo());
        }
    }
}

// Output example:
// EnemyPool Status:
//   [Scout] Pool<Enemy>: Active=12, Available=8, Total=20
//   [Grunt] Pool<Enemy>: Active=5, Available=5, Total=10
//   [Tank] Pool<Enemy>: Active=2, Available=3, Total=5
```

## Best Practices

1. **Use EnemyTypes Constants**: Always use `EnemyTypes.Scout` instead of `"Scout"` to prevent typos
2. **Validate Configurations**: Call `level.Validate()` before loading
3. **Set Appropriate Pool Sizes**: Use `GetRecommendedPoolSizes()` to auto-configure
4. **Clear Between Levels**: Call `ClearAllPools()` when changing levels
5. **Check Registration**: Use `IsEnemyTypeRegistered()` before spawning
6. **Monitor Performance**: Use `GetDebugInfo()` to track pool usage

## Common Patterns

### Pattern: Percentage-Based Spawning
```csharp
void SpawnEnemyByPercentage()
{
    float roll = Random.value;

    if (roll < 0.6f)  // 60% scouts
        EnemySpawner.SpawnEnemyPooled(EnemyTypes.Scout, pos);
    else if (roll < 0.9f)  // 30% grunts
        EnemySpawner.SpawnEnemyPooled(EnemyTypes.Grunt, pos);
    else  // 10% tanks
        EnemySpawner.SpawnEnemyPooled(EnemyTypes.Tank, pos);
}
```

### Pattern: Progressive Wave Difficulty
```csharp
void SpawnProgressiveWave(int waveNumber)
{
    int scoutCount = 5 + waveNumber;
    int gruntCount = waveNumber / 2;
    int tankCount = Mathf.Max(0, (waveNumber - 2) / 2);

    StartCoroutine(SpawnEnemies(EnemyTypes.Scout, scoutCount));
    StartCoroutine(SpawnEnemies(EnemyTypes.Grunt, gruntCount));
    StartCoroutine(SpawnEnemies(EnemyTypes.Tank, tankCount));
}
```
