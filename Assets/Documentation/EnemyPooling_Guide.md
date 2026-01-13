# Enemy Pooling System Guide

## Overview

The dynamic enemy pooling system allows you to register any number of enemy types with custom pool configurations. This makes it easy to add new enemy types, adjust pool sizes per level, and optimize performance.

## Setup Methods

### Method 1: Inspector Configuration (Recommended)

1. **Find the EnemyPool GameObject** in your scene (or it will be created automatically)

2. **Configure in Inspector:**
   - Set **Default Pool Configuration** values (used when enemy configs don't specify values)
   - Add entries to **Enemy Type Configurations** list:
     - **Enemy Type Id**: Unique identifier (e.g., "Scout", "Grunt", "Tank", "BossGoblin")
     - **Prefab**: The enemy prefab to pool
     - **Initial Pool Size**: Pre-created objects (0 = use default)
     - **Max Pool Size**: Maximum pool capacity (0 = use default)
     - **Expandable**: Can pool grow beyond initial size

3. **Auto-Initialization**: Pools initialize automatically on Start()

**Example Inspector Setup:**
```
Enemy Type Configurations:
├─ [0]
│  ├─ Enemy Type Id: "Scout"
│  ├─ Prefab: ScoutEnemyPrefab
│  ├─ Initial Pool Size: 20
│  ├─ Max Pool Size: 50
│  └─ Expandable: ✓
├─ [1]
│  ├─ Enemy Type Id: "Grunt"
│  ├─ Prefab: GruntEnemyPrefab
│  ├─ Initial Pool Size: 10
│  ├─ Max Pool Size: 30
│  └─ Expandable: ✓
└─ [2]
   ├─ Enemy Type Id: "Tank"
   ├─ Prefab: TankEnemyPrefab
   ├─ Initial Pool Size: 5
   ├─ Max Pool Size: 15
   └─ Expandable: ✓
```

### Method 2: Runtime Registration

```csharp
// Register enemy types at runtime
EnemyPool.Instance.RegisterEnemyType("Scout", scoutPrefab, initialPoolSize: 20, maxPoolSize: 50);
EnemyPool.Instance.RegisterEnemyType("Grunt", gruntPrefab, initialPoolSize: 10, maxPoolSize: 30);
EnemyPool.Instance.RegisterEnemyType("Tank", tankPrefab, initialPoolSize: 5, maxPoolSize: 15);

// Add new enemy type mid-game
EnemyPool.Instance.RegisterEnemyType("BossGoblin", bossGoblinPrefab, initialPoolSize: 2);
```

## Spawning Enemies

### Basic Spawning

```csharp
// Spawn by type ID
Enemy scout = EnemySpawner.SpawnEnemyPooled("Scout", spawnPosition);
Enemy grunt = EnemySpawner.SpawnEnemyPooled("Grunt", spawnPosition, spawnRotation);
```

### Wave System Example

```csharp
[System.Serializable]
public class WaveEnemyConfig
{
    public string enemyTypeId;  // "Scout", "Grunt", "Tank", etc.
    public int count;
}

[System.Serializable]
public class WaveData
{
    public List<WaveEnemyConfig> enemies;
    public float spawnInterval;
}

// Example: Spawn wave with custom enemy ratios
public IEnumerator SpawnWave(WaveData wave)
{
    foreach (var enemyConfig in wave.enemies)
    {
        for (int i = 0; i < enemyConfig.count; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPoint();
            EnemySpawner.SpawnEnemyPooled(enemyConfig.enemyTypeId, spawnPos);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }
}
```

### Level-Specific Enemy Ratios

```csharp
// Configure different enemy types per level
public class LevelConfig : ScriptableObject
{
    public List<EnemyPoolConfig> levelEnemies;
}

// Initialize pool for specific level
public void InitializeLevel(LevelConfig level)
{
    // Clear previous pools
    EnemyPool.Instance.ClearAllPools();

    // Register enemies for this level
    foreach (var config in level.levelEnemies)
    {
        EnemyPool.Instance.RegisterEnemyType(
            config.enemyTypeId,
            config.prefab,
            config.initialPoolSize,
            config.maxPoolSize,
            config.expandable
        );
    }
}
```

## Adding New Enemy Types

### Step 1: Create Enemy Prefab
- Create your new enemy GameObject with Enemy component
- Configure stats, animations, visuals

### Step 2: Register in Pool

**Option A: Add to Inspector**
```
Add new entry to EnemyPool → Enemy Type Configurations:
- Enemy Type Id: "FlyingDemon"
- Prefab: FlyingDemonPrefab
- Initial Pool Size: 8
```

**Option B: Register at Runtime**
```csharp
EnemyPool.Instance.RegisterEnemyType("FlyingDemon", flyingDemonPrefab, 8);
```

### Step 3: Spawn in Game
```csharp
Enemy demon = EnemySpawner.SpawnEnemyPooled("FlyingDemon", position);
```

## Dynamic Pool Management

### Check if Enemy Type Exists
```csharp
if (EnemyPool.Instance.IsEnemyTypeRegistered("MiniBoss"))
{
    // Spawn mini boss
}
```

### Get All Registered Types
```csharp
List<string> enemyTypes = EnemyPool.Instance.GetRegisteredEnemyTypes();
foreach (string type in enemyTypes)
{
    Debug.Log($"Available enemy type: {type}");
}
```

### Unregister Enemy Type
```csharp
// Remove an enemy type and clear its pool
EnemyPool.Instance.UnregisterEnemyType("OldEnemyType");
```

## Performance Optimization Tips

### 1. Set Appropriate Initial Pool Sizes
```csharp
// Common enemies: Larger initial pool
RegisterEnemyType("Scout", scoutPrefab, initialPoolSize: 25);

// Rare enemies: Smaller initial pool
RegisterEnemyType("Boss", bossPrefab, initialPoolSize: 2);
```

### 2. Level-Specific Pool Configurations
```csharp
// Early levels: Only basic enemies
Level 1: Scout (20), Grunt (10)

// Late levels: More variety, larger pools
Level 10: Scout (30), Grunt (20), Tank (15), BossGoblin (5)
```

### 3. Use Max Pool Size for Memory Management
```csharp
// Prevent unlimited pool growth on long-running levels
RegisterEnemyType("Scout", scoutPrefab, initialPoolSize: 20, maxPoolSize: 40);
```

## Example: Wave System with Dynamic Enemies

```csharp
[System.Serializable]
public class Wave
{
    public WaveEnemyConfig[] enemyComposition;
}

[System.Serializable]
public class WaveEnemyConfig
{
    public string enemyTypeId;
    public int count;
}

public class WaveManager : MonoBehaviour
{
    public Wave[] waves;

    void Start()
    {
        // Waves automatically use whatever enemy types are registered
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        foreach (Wave wave in waves)
        {
            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (var config in wave.enemyComposition)
        {
            for (int i = 0; i < config.count; i++)
            {
                if (EnemyPool.Instance.IsEnemyTypeRegistered(config.enemyTypeId))
                {
                    Vector3 pos = GetSpawnPoint();
                    EnemySpawner.SpawnEnemyPooled(config.enemyTypeId, pos);
                    yield return new WaitForSeconds(1.5f);
                }
                else
                {
                    Debug.LogWarning($"Enemy type '{config.enemyTypeId}' not registered!");
                }
            }
        }
    }
}
```

## Inspector Configuration Example for Different Levels

```
=== LEVEL 1 CONFIG ===
Scout: InitialSize=15, MaxSize=30
Grunt: InitialSize=5,  MaxSize=15

=== LEVEL 5 CONFIG ===
Scout: InitialSize=20, MaxSize=40
Grunt: InitialSize=15, MaxSize=30
Tank:  InitialSize=8,  MaxSize=20

=== BOSS LEVEL CONFIG ===
Scout: InitialSize=25, MaxSize=50
Grunt: InitialSize=20, MaxSize=40
Tank:  InitialSize=10, MaxSize=25
Boss:  InitialSize=1,  MaxSize=3
```

## Debugging

```csharp
// Get debug info
Debug.Log(EnemyPool.Instance.GetDebugInfo());

// Output example:
// EnemyPool Status:
//   [Scout] Pool<Enemy>: Active=12, Available=8, Total=20
//   [Grunt] Pool<Enemy>: Active=5, Available=5, Total=10
//   [Tank] Pool<Enemy>: Active=2, Available=3, Total=5
```

## Migration from Old System

**Old Code:**
```csharp
Enemy enemy = EnemySpawner.SpawnEnemyPooled(EnemyType.Scout, position);
```

**New Code:**
```csharp
Enemy enemy = EnemySpawner.SpawnEnemyPooled("Scout", position);
```

The string-based system is more flexible and allows runtime registration of new enemy types without code changes!
