# Inspector-Based Setup Guide
## Complete Level Configuration with ScriptableObjects and Pooling

This guide shows you how to create a complete level using only the Unity Inspector - no code changes needed!

---

## 📋 Table of Contents

1. [Scene Setup](#step-1-scene-setup)
2. [Creating Enemy Prefabs](#step-2-creating-enemy-prefabs)
3. [Configuring Enemy Pool](#step-3-configuring-enemy-pool)
4. [Creating Wave Configurations](#step-4-creating-wave-configurations)
5. [Creating Level Configuration](#step-5-creating-level-configuration)
6. [Setting Up Level Manager](#step-6-setting-up-level-manager)
7. [Testing Your Level](#step-7-testing-your-level)
8. [Creating Multiple Levels](#step-8-creating-multiple-levels)

---

## Step 1: Scene Setup

### 1.1 Create Base GameObject Hierarchy

In your Unity scene, create the following hierarchy:

```
Game Scene
├── GameManager (empty GameObject)
├── EnemyPool (empty GameObject)
├── LevelManager (empty GameObject)
├── SpawnPoints (empty GameObject)
│   ├── SpawnPoint_North (empty GameObject)
│   ├── SpawnPoint_East (empty GameObject)
│   ├── SpawnPoint_South (empty GameObject)
│   └── SpawnPoint_West (empty GameObject)
├── Player
├── Base
└── UI (Canvas)
```

### 1.2 Position Spawn Points

1. Select each SpawnPoint GameObject
2. Move them to the edges of your play area
3. Example positions:
   - North: (0, 0, 12)
   - East: (12, 0, 0)
   - South: (0, 0, -12)
   - West: (-12, 0, 0)

---

## Step 2: Creating Enemy Prefabs

### 2.1 Create Scout Enemy Prefab

1. **Create GameObject**: Right-click in Hierarchy → 3D Object → Capsule
2. **Rename**: "Scout"
3. **Add Component**: "Enemy" script
4. **Configure Enemy Component**:
   ```
   Max Health: 20
   Move Speed: 4.0
   Damage To Base: 5
   Coin Drop: 5
   Coin Prefab: [Assign your coin prefab]
   ```
5. **Add Component**: "Nav Mesh Agent"
6. **Configure Nav Mesh Agent**:
   ```
   Speed: 4.0
   Angular Speed: 360
   Acceleration: 8
   ```
7. **Drag to Project**: Drag "Scout" from Hierarchy to Assets/Prefabs/Enemies folder
8. **Delete from scene**

### 2.2 Repeat for Other Enemy Types

**Grunt Enemy:**
```
Max Health: 50
Move Speed: 2.5
Damage To Base: 15
Coin Drop: 10
```

**Tank Enemy:**
```
Max Health: 120
Move Speed: 1.5
Damage To Base: 30
Coin Drop: 25
```

---

## Step 3: Configuring Enemy Pool

### 3.1 Add EnemyPool Component

1. Select **EnemyPool** GameObject in Hierarchy
2. Click **Add Component** → Search for "Enemy Pool"
3. Click **Add Component**

### 3.2 Configure Default Pool Settings

In the EnemyPool Inspector:

```
Default Pool Configuration
├─ Default Initial Pool Size: 15
├─ Default Max Pool Size: 50
└─ Default Expandable: ✓
```

### 3.3 Configure Enemy Type Configurations

Click the **"+"** button under "Enemy Type Configurations" to add entries:

**Entry [0] - Scout:**
```
Enemy Type Id: "Scout"
Prefab: [Drag ScoutPrefab here]
Initial Pool Size: 20
Max Pool Size: 50
Expandable: ✓
```

**Entry [1] - Grunt:**
```
Enemy Type Id: "Grunt"
Prefab: [Drag GruntPrefab here]
Initial Pool Size: 10
Max Pool Size: 30
Expandable: ✓
```

**Entry [2] - Tank:**
```
Enemy Type Id: "Tank"
Prefab: [Drag TankPrefab here]
Initial Pool Size: 5
Max Pool Size: 15
Expandable: ✓
```

### 3.4 Visual Reference

```
┌─────────────────────────────────────┐
│ Enemy Pool (Script)                 │
├─────────────────────────────────────┤
│ Default Pool Configuration          │
│   Default Initial Pool Size: 15     │
│   Default Max Pool Size: 50         │
│   Default Expandable: ☑             │
│                                     │
│ Enemy Type Configurations           │
│   Size: 3                           │
│   ┌─ Element 0                      │
│   │   Enemy Type Id: Scout          │
│   │   Prefab: Scout                 │
│   │   Initial Pool Size: 20         │
│   │   Max Pool Size: 50             │
│   │   Expandable: ☑                 │
│   ├─ Element 1                      │
│   │   Enemy Type Id: Grunt          │
│   │   Prefab: Grunt                 │
│   │   Initial Pool Size: 10         │
│   │   Max Pool Size: 30             │
│   │   Expandable: ☑                 │
│   └─ Element 2                      │
│       Enemy Type Id: Tank           │
│       Prefab: Tank                  │
│       Initial Pool Size: 5          │
│       Max Pool Size: 15             │
│       Expandable: ☑                 │
└─────────────────────────────────────┘
```

---

## Step 4: Creating Wave Configurations

### 4.1 Create Wave 1 ScriptableObject

1. **Right-click** in Project → **Create** → **Game** → **Wave Configuration**
2. **Rename**: "Wave_1_ScoutRush"
3. **Select** the asset in Project view

### 4.2 Configure Wave 1 in Inspector

```
┌─────────────────────────────────────┐
│ Wave Configuration                  │
├─────────────────────────────────────┤
│ Wave Settings                       │
│   Wave Name: "Wave 1 - Scout Rush"  │
│   Wave Start Delay: 2               │
│   Spawn Interval: 1.5               │
│                                     │
│ Enemy Composition                   │
│   Enemy Spawns                      │
│   Size: 1                           │
│   ┌─ Element 0                      │
│   │   Enemy Type Id: Scout          │
│   │   Count: 5                      │
│   │   Specific Spawn Points         │
│   │     Size: 0                     │
│   │   Spawn Weight: 1               │
└───┴─────────────────────────────────┘
```

### 4.3 Create Wave 2 ScriptableObject

1. **Right-click** in Project → **Create** → **Game** → **Wave Configuration**
2. **Rename**: "Wave_2_Mixed"
3. **Configure**:

```
Wave Settings
  Wave Name: "Wave 2 - Mixed Forces"
  Wave Start Delay: 2
  Spawn Interval: 1.5

Enemy Composition
  Enemy Spawns
  Size: 2
  ┌─ Element 0
  │   Enemy Type Id: Scout
  │   Count: 5
  │   Spawn Weight: 1
  └─ Element 1
      Enemy Type Id: Grunt
      Count: 3
      Spawn Weight: 1
```

### 4.4 Create Wave 3 ScriptableObject

**Rename**: "Wave_3_HeavyAssault"

```
Wave Settings
  Wave Name: "Wave 3 - Heavy Assault"
  Wave Start Delay: 2
  Spawn Interval: 1.5

Enemy Composition
  Enemy Spawns
  Size: 3
  ┌─ Element 0 (Scout)
  │   Enemy Type Id: Scout
  │   Count: 6
  ├─ Element 1 (Grunt)
  │   Enemy Type Id: Grunt
  │   Count: 4
  └─ Element 2 (Tank)
      Enemy Type Id: Tank
      Count: 1
```

### 4.5 Organize Wave Assets

Create folder structure:
```
Assets/
└── Configurations/
    └── Waves/
        ├── Level1/
        │   ├── Wave_1_ScoutRush.asset
        │   ├── Wave_2_Mixed.asset
        │   └── Wave_3_HeavyAssault.asset
        └── Level2/
            └── (future waves)
```

---

## Step 5: Creating Level Configuration

### 5.1 Create Level ScriptableObject

1. **Right-click** in Project → **Create** → **Game** → **Level Configuration**
2. **Rename**: "Level_1_Grasslands"
3. **Move** to: Assets/Configurations/Levels/

### 5.2 Configure Level 1 in Inspector

```
┌─────────────────────────────────────────────┐
│ Level Configuration                         │
├─────────────────────────────────────────────┤
│ Level Info                                  │
│   Level Number: 1                           │
│   Level Name: "Grasslands"                  │
│                                             │
│ Enemy Pool Configuration                    │
│   Level Enemy Pools                         │
│   Size: 2                                   │
│   ┌─ Element 0                              │
│   │   Enemy Type Id: Scout                  │
│   │   Prefab: Scout [Drag prefab here]      │
│   │   Initial Pool Size: 15                 │
│   │   Max Pool Size: 30                     │
│   │   Expandable: ☑                         │
│   └─ Element 1                              │
│       Enemy Type Id: Grunt                  │
│       Prefab: Grunt [Drag prefab here]      │
│       Initial Pool Size: 8                  │
│       Max Pool Size: 20                     │
│       Expandable: ☑                         │
│                                             │
│ Wave Configurations                         │
│   Waves                                     │
│   Size: 3                                   │
│   ├─ Element 0: Wave_1_ScoutRush            │
│   ├─ Element 1: Wave_2_Mixed                │
│   └─ Element 2: Wave_3_HeavyAssault         │
│                                             │
│ Level Settings                              │
│   Time Between Waves: 5                     │
└─────────────────────────────────────────────┘
```

### 5.3 Auto-Configure Pool Sizes (Optional)

1. **Right-click** on the Level Configuration asset
2. Select **"Auto-Configure Pool Sizes"**
3. This automatically calculates optimal pool sizes based on your wave configurations

---

## Step 6: Setting Up Level Manager

### 6.1 Add LevelManager Component

1. Select **LevelManager** GameObject in Hierarchy
2. Click **Add Component** → Search for "Level Manager"
3. Click **Add Component**

### 6.2 Configure LevelManager in Inspector

```
┌─────────────────────────────────────┐
│ Level Manager (Script)              │
├─────────────────────────────────────┤
│ Level Configuration                 │
│   Current Level:                    │
│     [Drag Level_1_Grasslands here]  │
│                                     │
│ Spawn Points                        │
│   Size: 4                           │
│   ├─ Element 0: SpawnPoint_North    │
│   ├─ Element 1: SpawnPoint_East     │
│   ├─ Element 2: SpawnPoint_South    │
│   └─ Element 3: SpawnPoint_West     │
│                                     │
│ UI References (Optional)            │
│   Wave Number Text: (none)          │
│   Wave Name Text: (none)            │
└─────────────────────────────────────┘
```

### 6.3 Assign Spawn Points

1. In the **Spawn Points** array, set **Size: 4**
2. Drag spawn point GameObjects from Hierarchy:
   - **Element 0**: Drag "SpawnPoint_North"
   - **Element 1**: Drag "SpawnPoint_East"
   - **Element 2**: Drag "SpawnPoint_South"
   - **Element 3**: Drag "SpawnPoint_West"

---

## Step 7: Testing Your Level

### 7.1 Pre-Flight Checklist

Before pressing Play, verify:

- [ ] EnemyPool GameObject has EnemyPool component
- [ ] EnemyPool has 3 enemy types configured
- [ ] LevelManager GameObject has LevelManager component
- [ ] LevelManager has Level Configuration assigned
- [ ] LevelManager has 4 spawn points assigned
- [ ] All enemy prefabs are in Assets/Prefabs/Enemies
- [ ] All wave configs are created and configured
- [ ] Level config has waves assigned

### 7.2 Run the Game

1. **Press Play** in Unity
2. **Watch Console** for initialization messages:
   ```
   EnemyPool: Initialized with 2 enemy types from configs
   Level 'Grasslands': Initialized enemy pool with 2 enemy types
   Starting Level: Grasslands
   Starting Wave: Wave 1 - Scout Rush
   ```

### 7.3 Debug Information

While playing, you can right-click on the **LevelManager** component and select **"Print Level Info"** to see:

```
=== Level: Grasslands ===
Waves: 3
Total Enemies: 19
Enemy Types: Scout, Grunt, Tank
EnemyPool Status:
  [Scout] Pool<Enemy>: Active=3, Available=12, Total=15
  [Grunt] Pool<Enemy>: Active=0, Available=8, Total=8
```

---

## Step 8: Creating Multiple Levels

### 8.1 Create Level 2 Configuration

1. **Duplicate** Level_1_Grasslands
2. **Rename**: "Level_2_DarkForest"
3. **Configure**:

```
Level Info
  Level Number: 2
  Level Name: "Dark Forest"

Enemy Pool Configuration
  Level Enemy Pools
  Size: 3
  ├─ Scout (Initial: 20, Max: 40)
  ├─ Grunt (Initial: 15, Max: 30)
  └─ Tank (Initial: 8, Max: 20)

Waves
  Size: 4
  ├─ Wave 1: 8 Scouts
  ├─ Wave 2: 6 Scouts, 5 Grunts
  ├─ Wave 3: 5 Scouts, 6 Grunts, 2 Tanks
  └─ Wave 4: 5 Scouts, 5 Grunts, 4 Tanks

Level Settings
  Time Between Waves: 5
```

### 8.2 Switch Levels at Runtime

To change levels, simply:

1. Select **LevelManager** in Hierarchy
2. Drag **Level_2_DarkForest** into the **Current Level** field
3. Press Play

Or use this in code:
```csharp
public LevelConfiguration level2;
levelManager.LoadLevel(level2);
```

---

## 🎯 Quick Reference: Creating a New Enemy Type

### Adding a "FlyingDemon" enemy:

1. **Create Prefab**:
   - Create GameObject → Name: "FlyingDemon"
   - Add Enemy component, configure stats
   - Save as prefab

2. **Add to EnemyPool**:
   - Select EnemyPool GameObject
   - Enemy Type Configurations → Click "+"
   - Enemy Type Id: "FlyingDemon"
   - Prefab: Drag FlyingDemon prefab
   - Initial Pool Size: 8
   - Max Pool Size: 20

3. **Add to EnemyTypes.cs** (optional):
   ```csharp
   public const string FlyingDemon = "FlyingDemon";
   ```

4. **Use in Wave Configuration**:
   - Open any Wave Configuration
   - Enemy Spawns → Click "+"
   - Enemy Type Id: "FlyingDemon"
   - Count: 3

5. **Done!** No other code changes needed!

---

## 🎨 Visual Workflow Summary

```
1. Create Enemy Prefabs
   └─> Save to Assets/Prefabs/Enemies/

2. Configure EnemyPool (Scene)
   └─> Add enemy types with pool sizes

3. Create Wave Configurations (Assets)
   └─> Save to Assets/Configurations/Waves/
       ├─ Define enemy composition
       └─ Set spawn intervals

4. Create Level Configuration (Assets)
   └─> Save to Assets/Configurations/Levels/
       ├─ Assign enemy pool configs
       └─ Assign wave sequence

5. Setup LevelManager (Scene)
   └─> Assign Level Configuration
   └─> Assign Spawn Points

6. Press Play!
```

---

## 📊 Example Level Progression

### Level 1: Grasslands (Tutorial)
- Enemies: Scout, Grunt only
- Waves: 3 waves
- Total Enemies: 13
- Pool Sizes: Small (Scout: 15, Grunt: 8)

### Level 5: Dark Forest (Medium)
- Enemies: Scout, Grunt, Tank
- Waves: 4 waves
- Total Enemies: 28
- Pool Sizes: Medium (Scout: 20, Grunt: 15, Tank: 8)

### Level 10: Volcanic Wasteland (Hard)
- Enemies: All types + Boss
- Waves: 5 waves
- Total Enemies: 45
- Pool Sizes: Large (Scout: 30, Grunt: 20, Tank: 15, Boss: 3)

---

## 🔧 Troubleshooting

### "EnemyPool not initialized" error
- **Solution**: Check that EnemyPool GameObject exists in scene
- Verify Enemy Type Configurations are filled out

### Enemies not spawning
- **Solution**: Check enemy Type IDs match exactly (case-sensitive!)
- Verify Level Configuration has waves assigned
- Check spawn points are assigned to LevelManager

### Pool runs out of enemies
- **Solution**: Increase Max Pool Size in EnemyPool configuration
- Or enable "Expandable" option

### Wrong enemies spawning
- **Solution**: Verify Enemy Type Id spelling matches in:
  - EnemyPool configuration
  - Wave Configuration
  - Level Configuration

---

## ✅ Best Practices

1. **Naming Convention**:
   - Enemy Types: "Scout", "Grunt", "Tank" (PascalCase)
   - Waves: "Wave_1_ScoutRush" (descriptive names)
   - Levels: "Level_1_Grasslands" (number + theme)

2. **Organization**:
   ```
   Assets/
   ├── Prefabs/
   │   └── Enemies/
   ├── Configurations/
   │   ├── Waves/
   │   │   ├── Level1/
   │   │   └── Level2/
   │   └── Levels/
   └── Scripts/
   ```

3. **Pool Sizing**:
   - Initial Size = ~50% of max concurrent enemies
   - Max Size = 2x Initial Size
   - Use "Auto-Configure Pool Sizes" feature

4. **Testing**:
   - Test each wave individually first
   - Use "Print Level Info" for debugging
   - Monitor console for pool warnings

---

## 🚀 You're Done!

You now have a complete, inspector-based level system that:
- ✅ Uses object pooling for performance
- ✅ Supports unlimited enemy types
- ✅ Configurable via ScriptableObjects
- ✅ No code changes needed for new levels
- ✅ Scalable and maintainable

Create new levels by duplicating configurations and adjusting values in the Inspector!
