# Game Design Document — MVP
## Base Defender
### Version 1.0 (MVP)

---

## 1. Game Overview

### 1.1 High Concept
A casual 3D isometric mobile game where players control a hero that auto-fires at enemies while collecting coins to activate defensive turrets protecting their base.

### 1.2 Core Loop
```
MOVE → KILL ENEMIES → COLLECT COINS → ACTIVATE TURRETS → SURVIVE WAVES
```

### 1.3 MVP Scope
- Single level with configurable waves
- One hero (no upgrades)
- One turret type
- Three enemy types
- Coin collection and turret activation
- Win/Lose conditions

---

## 2. Platform & Controls

### 2.1 Platform
- iOS and Android (Portrait mode)
- Unity 2022.3 LTS with URP

### 2.2 Controls
- Virtual joystick (floating, appears on touch)
- No other input required (auto-fire handles combat)

---

## 3. Gameplay Mechanics

### 3.1 Player Hero

The player controls a hero that moves on a flat surface and automatically fires at the nearest enemy.

```
HERO_STATS:
  move_speed: 5 units/second
  fire_rate: 2 shots/second
  damage: 10
  range: 8 units
```

**Behavior:**
- Player is invincible (enemies ignore player, only attack base)
- Auto-targets nearest enemy within range
- Fires projectile toward target automatically
- Collects coins by moving near them

### 3.2 Enemies

Enemies spawn at map edges and walk directly toward the base. They do not attack the player.

| Type | Health | Speed | Damage to Base | Coin Drop |
|------|--------|-------|----------------|-----------|
| Scout | 20 | 4.0 | 5 | 5 |
| Grunt | 50 | 2.5 | 15 | 10 |
| Tank | 120 | 1.5 | 30 | 25 |

**Enemy Behavior:**
- Spawn at designated spawn points
- Use NavMesh to path toward base
- Attack base on contact (deal damage, then destroy self)
- Drop coins at death position

### 3.3 Base

The base is a stationary structure at the center of the map.

```
BASE_STATS:
  health: 500
  size: 3x3 units
```

**Win Condition:** Survive all waves
**Lose Condition:** Base health reaches 0

### 3.4 Defensive Turrets

Pre-placed turret posts around the map start inactive. Player activates them by spending coins.

```
TURRET_STATS:
  activation_cost: 50 coins
  damage: 8
  fire_rate: 1.5 shots/second
  range: 6 units
```

**Turret Behavior:**
- Inactive until player pays cost while nearby (within 2 units)
- Once active, auto-targets nearest enemy in range
- Cannot be destroyed or upgraded (MVP)

**Turret Count:** 4 posts placed around the base

### 3.5 Coins

```
COIN_BEHAVIOR:
  magnet_radius: 2.5 units
  fly_speed: 10 units/second
  lifetime: 30 seconds (then despawn)
```

- Spawn at enemy death location
- Fly toward player when player is within magnet radius
- Add to player's coin total on collection

### 3.6 Wave System

```
WAVE_CONFIG:
  total_waves: 5
  delay_between_waves: 5 seconds
  spawn_interval: 1.5 seconds
```

| Wave | Scouts | Grunts | Tanks | Total Enemies |
|------|--------|--------|-------|---------------|
| 1 | 5 | 0 | 0 | 5 |
| 2 | 5 | 3 | 0 | 8 |
| 3 | 6 | 4 | 1 | 11 |
| 4 | 4 | 6 | 2 | 12 |
| 5 | 5 | 5 | 4 | 14 |

**Spawn Points:** 4 points at North, East, South, West edges of map

---

## 4. Game Flow

```
Main Menu → Gameplay → Victory/Defeat Screen → Main Menu
```

### 4.1 Main Menu
- Play Button
- Quit Button

### 4.2 Gameplay
- Waves spawn automatically after countdown
- Player defends until all waves cleared or base destroyed

### 4.3 End Screen
- Victory: "You Win!" + Play Again button
- Defeat: "Game Over" + wave reached + Retry button

---

## 5. User Interface

### 5.1 HUD Layout (Portrait)

```
+---------------------------+
|  Coins: 150    Wave: 3/5  |
|  Base HP: [██████████]    |
+---------------------------+
|                           |
|                           |
|      GAME VIEWPORT        |
|                           |
|                           |
+---------------------------+
|  [PAUSE]                  |
|                           |
|    (Joystick Zone)        |
+---------------------------+
```

### 5.2 Turret Activation Prompt

When player is near an inactive turret:
```
+-------------------+
|  Activate Turret  |
|    Cost: 50       |
|   [ACTIVATE]      |
+-------------------+
```

Appears as world-space UI above the turret.

### 5.3 Wave Notification

"Wave X" text appears center screen for 2 seconds at wave start.

---

## 6. 3D Art Style

### 6.1 Visual Direction
- Low-poly stylized 3D
- Bright, readable colors
- Minimal textures (vertex colors or simple gradients)

### 6.2 Asset List

**Models Required:**

| Asset | Description | Poly Target |
|-------|-------------|-------------|
| Hero | Simple humanoid character | 500-800 |
| Scout Enemy | Small, fast creature | 300-500 |
| Grunt Enemy | Medium humanoid | 500-800 |
| Tank Enemy | Large, bulky creature | 600-1000 |
| Base | Castle/fortress structure | 1000-1500 |
| Turret (Inactive) | Powered-down turret | 400-600 |
| Turret (Active) | Same with glow/effects | 400-600 |
| Coin | Simple coin shape | 50-100 |
| Projectile | Small sphere/bolt | 50 |
| Ground Tile | Flat plane with texture | 2 |

**Animations Required:**

| Character | Animations |
|-----------|------------|
| Hero | Idle, Run, Shoot (upper body) |
| Scout | Run, Death |
| Grunt | Run, Attack, Death |
| Tank | Run, Attack, Death |
| Turret | Activate, Shoot |

### 6.3 Camera Setup

```
CAMERA_CONFIG:
  type: Orthographic (or Perspective with fixed angle)
  rotation: X=45, Y=45, Z=0
  height: Fixed, showing full play area
  follow: Soft follow on player (optional) or fixed
```

---

## 7. Technical Specification

### 7.1 Unity Project Structure

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   └── WaveManager.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   └── PlayerShooting.cs
│   ├── Enemy/
│   │   ├── Enemy.cs
│   │   └── EnemySpawner.cs
│   ├── Defense/
│   │   ├── Base.cs
│   │   └── Turret.cs
│   ├── Collectibles/
│   │   └── Coin.cs
│   └── UI/
│       ├── HUDManager.cs
│       ├── TurretPrompt.cs
│       └── GameOverScreen.cs
├── Prefabs/
│   ├── Player.prefab
│   ├── Enemies/
│   │   ├── Scout.prefab
│   │   ├── Grunt.prefab
│   │   └── Tank.prefab
│   ├── Turret.prefab
│   ├── Base.prefab
│   ├── Coin.prefab
│   └── Projectile.prefab
├── Scenes/
│   ├── MainMenu.unity
│   └── Game.unity
├── Materials/
├── Models/
├── Animations/
└── Audio/
```

### 7.2 Core Scripts Overview

#### GameManager.cs
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int coins;
    public GameState state; // Menu, Playing, Paused, Victory, Defeat
    
    public void AddCoins(int amount);
    public bool SpendCoins(int amount);
    public void OnBaseDestroyed();
    public void OnAllWavesComplete();
}
```

#### WaveManager.cs
```csharp
public class WaveManager : MonoBehaviour
{
    public WaveData[] waves;
    public Transform[] spawnPoints;
    
    public int currentWave;
    public int enemiesRemaining;
    
    public void StartNextWave();
    private IEnumerator SpawnWave(WaveData wave);
    public void OnEnemyKilled();
}

[System.Serializable]
public class WaveData
{
    public int scoutCount;
    public int gruntCount;
    public int tankCount;
    public float spawnInterval;
}
```

#### PlayerController.cs
```csharp
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick joystick; // Reference to joystick UI
    
    private void Update()
    {
        Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
```

#### PlayerShooting.cs
```csharp
public class PlayerShooting : MonoBehaviour
{
    public float damage = 10f;
    public float fireRate = 2f;
    public float range = 8f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    
    private float fireTimer;
    private Enemy currentTarget;
    
    private void Update()
    {
        FindNearestEnemy();
        if (currentTarget != null && fireTimer <= 0)
        {
            Shoot();
            fireTimer = 1f / fireRate;
        }
        fireTimer -= Time.deltaTime;
    }
    
    private void FindNearestEnemy();
    private void Shoot();
}
```

#### Enemy.cs
```csharp
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public float moveSpeed;
    public int damageToBase;
    public int coinDrop;
    
    private int currentHealth;
    private NavMeshAgent agent;
    private Transform baseTarget;
    
    public void TakeDamage(int damage);
    private void Die();
    private void OnReachBase();
}
```

#### Turret.cs
```csharp
public class Turret : MonoBehaviour
{
    public int activationCost = 50;
    public float damage = 8f;
    public float fireRate = 1.5f;
    public float range = 6f;
    
    public bool isActive = false;
    
    public void Activate();
    private void FindTarget();
    private void Shoot();
}
```

#### Base.cs
```csharp
public class Base : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;
    
    public event System.Action<int, int> OnHealthChanged;
    public event System.Action OnDestroyed;
    
    public void TakeDamage(int damage);
}
```

#### Coin.cs
```csharp
public class Coin : MonoBehaviour
{
    public int value = 1;
    public float magnetRadius = 2.5f;
    public float flySpeed = 10f;
    public float lifetime = 30f;
    
    private Transform player;
    private bool isCollecting = false;
    
    private void Update();
    private void OnCollected();
}
```

### 7.3 Scene Setup (Game.unity)

```
Hierarchy:
├── GameManager
├── WaveManager
├── Cameras
│   └── Main Camera (Isometric angle)
├── Lighting
│   └── Directional Light
├── Environment
│   ├── Ground (Plane with NavMesh)
│   └── Boundaries (Invisible colliders)
├── Player
├── Base
├── Turrets
│   ├── Turret_North
│   ├── Turret_East
│   ├── Turret_South
│   └── Turret_West
├── SpawnPoints
│   ├── Spawn_North
│   ├── Spawn_East
│   ├── Spawn_South
│   └── Spawn_West
├── UI
│   ├── Canvas (Screen Space)
│   │   ├── HUD
│   │   ├── WaveText
│   │   ├── PauseButton
│   │   └── Joystick
│   └── WorldCanvas (World Space)
│       └── TurretPrompts
└── Pools
    ├── EnemyPool
    ├── ProjectilePool
    └── CoinPool
```

### 7.4 NavMesh Setup

- Bake NavMesh on ground plane
- Base is NavMesh obstacle (enemies path around it)
- Turrets are small obstacles

### 7.5 Layer Setup

| Layer | Purpose |
|-------|---------|
| Default | Environment |
| Player | Player character |
| Enemy | All enemy types |
| Turret | Defensive turrets |
| Projectile | All projectiles |
| Coin | Collectible coins |

### 7.6 Tags

| Tag | Objects |
|-----|---------|
| Player | Player character |
| Enemy | All enemies |
| Base | Base structure |
| Turret | All turrets |
| Coin | All coins |

---

## 8. Audio (Minimal)

### 8.1 Sound Effects
- player_shoot.wav
- turret_shoot.wav
- enemy_death.wav
- coin_collect.wav
- base_hit.wav
- turret_activate.wav
- wave_start.wav
- victory.wav
- defeat.wav

### 8.2 Music
- gameplay_loop.wav (single track)

---

## 9. Development Checklist

### Phase 1: Core Setup
- [ ] Unity project with URP
- [ ] Isometric camera setup
- [ ] Ground plane with NavMesh
- [ ] Player movement with joystick
- [ ] Basic placeholder models (cubes/capsules)

### Phase 2: Combat
- [ ] Player auto-targeting
- [ ] Player shooting projectiles
- [ ] Enemy spawning
- [ ] Enemy pathfinding to base
- [ ] Enemy health and death
- [ ] Base health system

### Phase 3: Economy
- [ ] Coin spawning on enemy death
- [ ] Coin collection (magnet behavior)
- [ ] Coin counter UI
- [ ] Turret activation with coins
- [ ] Turret shooting

### Phase 4: Game Loop
- [ ] Wave system
- [ ] Wave UI
- [ ] Win condition
- [ ] Lose condition
- [ ] Main menu
- [ ] Game over screen

### Phase 5: Polish
- [ ] Replace placeholder art
- [ ] Add animations
- [ ] Add sound effects
- [ ] Mobile build and test
- [ ] Performance optimization

---

## 10. Map Layout

```
        [Spawn North]
             |
    +--------+--------+
    |        |        |
    |   [T]  |  [T]   |      T = Turret Post
    |        |        |
[Spawn]------[BASE]------[Spawn East]
 West |      |        |
    |   [T]  |  [T]   |
    |        |        |
    +--------+--------+
             |
        [Spawn South]

Map Size: 20x20 units
Base Position: Center (0, 0, 0)
Turret Positions: (5,0,5), (-5,0,5), (5,0,-5), (-5,0,-5)
Spawn Distance: 12 units from center
```

---

## 11. Balance Reference

### Expected Economy Flow

| Wave | Enemies | Max Coins | Cumulative | Turrets Affordable |
|------|---------|-----------|------------|-------------------|
| 1 | 5 | 25 | 25 | 0 |
| 2 | 8 | 65 | 90 | 1 |
| 3 | 11 | 120 | 210 | 4 |
| 4 | 12 | 140 | 350 | 4 (all) |
| 5 | 14 | 195 | 545 | 4 (all) |

Players should be able to activate 1-2 turrets by wave 3, all 4 by wave 4-5.

---

## Appendix: Joystick Implementation

Use a simple floating joystick asset or implement:

```csharp
public class FloatingJoystick : MonoBehaviour
{
    public RectTransform background;
    public RectTransform handle;
    public float handleRange = 50f;
    
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    
    public void OnPointerDown(PointerEventData eventData);
    public void OnDrag(PointerEventData eventData);
    public void OnPointerUp(PointerEventData eventData);
}
```

Recommended: Use "Joystick Pack" from Unity Asset Store (free).

---

*End of MVP Document*
