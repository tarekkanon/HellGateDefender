# Quick Start Checklist
## Inspector-Based Level Setup in 10 Minutes

Follow these steps to create your first level with the pooling system!

---

## ✅ 10-Minute Setup Checklist

### Step 1: Scene Setup (2 minutes)

- [ ] Create GameObject: "EnemyPool"
- [ ] Create GameObject: "LevelManager"
- [ ] Create 4 GameObjects under "SpawnPoints" folder:
  - [ ] SpawnPoint_North (position: 0, 0, 12)
  - [ ] SpawnPoint_East (position: 12, 0, 0)
  - [ ] SpawnPoint_South (position: 0, 0, -12)
  - [ ] SpawnPoint_West (position: -12, 0, 0)

### Step 2: Enemy Prefabs (2 minutes)

If you don't have enemy prefabs yet:
- [ ] Create 3 Capsule GameObjects named: Scout, Grunt, Tank
- [ ] Add "Enemy" component to each
- [ ] Configure stats (see values below)
- [ ] Add "Nav Mesh Agent" component
- [ ] Save as prefabs in Assets/Prefabs/Enemies/

**Quick Stats:**
```
Scout: Health=20, Speed=4.0, Damage=5, Coins=5
Grunt: Health=50, Speed=2.5, Damage=15, Coins=10
Tank:  Health=120, Speed=1.5, Damage=30, Coins=25
```

### Step 3: Configure EnemyPool (1 minute)

- [ ] Select "EnemyPool" GameObject
- [ ] Add Component → "Enemy Pool"
- [ ] Set Enemy Type Configurations (size: 3):
  - [ ] [0] Id="Scout", Prefab=ScoutPrefab, Initial=20, Max=50
  - [ ] [1] Id="Grunt", Prefab=GruntPrefab, Initial=10, Max=30
  - [ ] [2] Id="Tank", Prefab=TankPrefab, Initial=5, Max=15

### Step 4: Create Wave Configurations (2 minutes)

- [ ] Right-click → Create → Game → Wave Configuration
- [ ] Name: "Wave_1_ScoutRush"
- [ ] Configure:
  ```
  Wave Name: "Wave 1 - Scout Rush"
  Wave Start Delay: 2
  Spawn Interval: 1.5
  Enemy Spawns (size: 1):
    [0] Enemy Type Id: Scout, Count: 5
  ```

- [ ] Create "Wave_2_Mixed":
  ```
  Enemy Spawns (size: 2):
    [0] Enemy Type Id: Scout, Count: 5
    [1] Enemy Type Id: Grunt, Count: 3
  ```

- [ ] Create "Wave_3_HeavyAssault":
  ```
  Enemy Spawns (size: 3):
    [0] Enemy Type Id: Scout, Count: 6
    [1] Enemy Type Id: Grunt, Count: 4
    [2] Enemy Type Id: Tank, Count: 1
  ```

### Step 5: Create Level Configuration (1 minute)

- [ ] Right-click → Create → Game → Level Configuration
- [ ] Name: "Level_1_Grasslands"
- [ ] Configure:
  ```
  Level Number: 1
  Level Name: "Grasslands"

  Level Enemy Pools (size: 3):
    [0] Scout prefab, Initial=15, Max=30
    [1] Grunt prefab, Initial=8, Max=20
    [2] Tank prefab, Initial=5, Max=15

  Waves (size: 3):
    [0] Wave_1_ScoutRush
    [1] Wave_2_Mixed
    [2] Wave_3_HeavyAssault

  Time Between Waves: 5
  ```

### Step 6: Configure LevelManager (1 minute)

- [ ] Select "LevelManager" GameObject
- [ ] Add Component → "Level Manager"
- [ ] Drag "Level_1_Grasslands" to **Current Level** field
- [ ] Set **Spawn Points** array (size: 4):
  - [ ] [0] Drag SpawnPoint_North
  - [ ] [1] Drag SpawnPoint_East
  - [ ] [2] Drag SpawnPoint_South
  - [ ] [3] Drag SpawnPoint_West

### Step 7: Test! (1 minute)

- [ ] Press Play
- [ ] Check Console for:
  ```
  EnemyPool: Initialized with 3 enemy types
  Level 'Grasslands': Initialized enemy pool
  Starting Level: Grasslands
  Starting Wave: Wave 1 - Scout Rush
  ```
- [ ] Verify enemies spawn from spawn points
- [ ] Verify waves progress automatically

---

## 🎯 Quick Verification

Right-click on **LevelManager** component and select:
- **"Print Level Info"** - See wave and enemy counts
- **"Validate Configuration"** - Check for errors

---

## 🚀 You're Ready!

Your level is now running with:
- ✅ Object pooling for performance
- ✅ 3 enemy types with different stats
- ✅ 3 waves with mixed enemy compositions
- ✅ Automatic wave progression

---

## 📝 Next Steps

### Add More Waves
1. Duplicate existing wave configuration
2. Change enemy counts and types
3. Add to Level Configuration waves list

### Create Level 2
1. Duplicate Level_1_Grasslands
2. Rename to Level_2_Forest
3. Adjust enemy pools and waves
4. Change LevelManager's Current Level field

### Add New Enemy Type
1. Create enemy prefab with Enemy component
2. Add to EnemyPool Enemy Type Configurations
3. Use in wave configurations with enemy Type Id

---

## ⚠️ Common Issues

**Enemies not spawning?**
→ Check Enemy Type Id spelling matches exactly in all configs!

**"Pool not initialized" error?**
→ Verify EnemyPool GameObject has EnemyPool component

**Waves not progressing?**
→ Check LevelManager has spawn points assigned

---

## 📚 Full Documentation

For detailed guides, see:
- **InspectorBased_Setup_Guide.md** - Complete step-by-step guide
- **EnemyPooling_Guide.md** - Pool system documentation
- **DynamicEnemyPool_Examples.md** - Code examples

---

**Setup Time: ~10 minutes**
**No code changes required!**
**Ready for new enemy types and levels!**
