# AI Agent Instructions: Base Defender MVP
## Unity Script Development Guide

---

## Overview

You are an AI agent tasked with building the scripts for a Unity mobile game called **Base Defender**. This is a 3D isometric game where players control a hero that auto-fires at enemies while collecting coins to activate defensive turrets.

**Read the full Game Design Document (GDD) located at:** `Assets/Documentation/GDD_BaseDefender_MVP.md`

---

## Project Context

- **Engine:** Unity 6.3 LTS (6000.3.2f1)
- **Render Pipeline:** URP (Universal Render Pipeline)
- **Platform:** Mobile (iOS/Android, Portrait mode)
- **Language:** C#
- **Architecture:** Component-based with Singleton managers

---

## Task: Build All Scripts

Create all C# scripts in the following order. Each script should be complete, well-commented, and follow Unity best practices.

---

## Script Directory Structure

Create scripts in this folder structure under `Assets/Scripts/`:

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── WaveManager.cs
│   └── GameEvents.cs
├── Player/
│   ├── PlayerController.cs
│   └── PlayerShooting.cs
├── Enemy/
│   ├── Enemy.cs
│   └── EnemySpawner.cs
├── Defense/
│   ├── Base.cs
│   ├── Turret.cs
│   └── Projectile.cs
├── Collectibles/
│   └── Coin.cs
├── UI/
│   ├── HUDManager.cs
│   ├── TurretPrompt.cs
│   ├── MainMenuUI.cs
│   └── GameOverUI.cs
└── Input/
    └── FloatingJoystick.cs
```

---

## Implementation Order

Follow this exact order to ensure dependencies are met:

### Phase 1: Core Systems

#### 1. GameEvents.cs
Static event system for decoupled communication.

```
Requirements:
- Static class with C# events/actions
- Events needed:
  - OnGameStateChanged(GameState newState)
  - OnCoinsChanged(int totalCoins)
  - OnWaveStarted(int waveNumber, int totalWaves)
  - OnWaveCompleted(int waveNumber)
  - OnEnemyKilled(Vector3 position, int coinValue)
  - OnBaseHealthChanged(int currentHealth, int maxHealth)
  - OnBaseDamaged(int damage)
  - OnBaseDestroyed()
  - OnTurretActivated(Turret turret)
  - OnGameVictory()
  - OnGameDefeat()
```

#### 2. GameManager.cs
Singleton managing game state and coins.

```
Requirements:
- Singleton pattern (Instance property)
- GameState enum: MainMenu, Playing, Paused, Victory, Defeat
- Properties:
  - int Coins (current coins)
  - GameState CurrentState
- Methods:
  - StartGame() - initialize and start wave 1
  - PauseGame() / ResumeGame()
  - AddCoins(int amount) - fires OnCoinsChanged
  - SpendCoins(int amount) - returns bool, fires OnCoinsChanged
  - TriggerVictory() - called when all waves complete
  - TriggerDefeat() - called when base destroyed
  - RestartGame() - reload scene
  - ReturnToMenu() - load main menu scene
- Subscribe to GameEvents.OnBaseDestroyed and OnAllWavesComplete
- Use Time.timeScale for pause
```

#### 3. WaveManager.cs
Controls enemy wave spawning.

```
Requirements:
- Serializable WaveData class:
  - int scoutCount
  - int gruntCount
  - int tankCount
- Inspector fields:
  - WaveData[] waves (configure 5 waves per GDD)
  - Transform[] spawnPoints (4 points)
  - GameObject scoutPrefab, gruntPrefab, tankPrefab
  - float delayBetweenWaves = 5f
  - float spawnInterval = 1.5f
- Properties:
  - int CurrentWave
  - int EnemiesRemaining
  - bool IsSpawning
- Methods:
  - StartWaves() - begin wave sequence
  - SpawnWave(int waveIndex) - coroutine
  - OnEnemyDied() - decrement count, check wave complete
- Fire events: OnWaveStarted, OnWaveCompleted
- When all waves complete and no enemies remain, fire OnGameVictory via GameManager
```

---

### Phase 2: Player Systems

#### 4. PlayerController.cs
Handles player movement via joystick.

```
Requirements:
- Inspector fields:
  - float moveSpeed = 5f
  - FloatingJoystick joystick (reference)
  - Rigidbody rb (optional, can use transform)
- Movement:
  - Read joystick.Horizontal and joystick.Vertical
  - Convert to world direction (isometric not needed if camera handles it)
  - Move using transform.position or Rigidbody
  - Rotate player to face movement direction
- Boundary clamping:
  - Keep player within map bounds (configurable min/max)
- Only process input when GameState is Playing
```

#### 5. PlayerShooting.cs
Auto-targeting and firing system.

```
Requirements:
- Inspector fields:
  - float damage = 10f
  - float fireRate = 2f (shots per second)
  - float range = 8f
  - GameObject projectilePrefab
  - Transform firePoint
  - LayerMask enemyLayer
- Private fields:
  - float fireTimer
  - Transform currentTarget
- Methods:
  - FindNearestEnemy() - use Physics.OverlapSphere, find closest
  - CanShoot() - check timer and target exists
  - Shoot() - instantiate projectile toward target
  - RotateTowardsTarget() - smoothly rotate upper body or whole player
- Update loop:
  - Find target
  - Rotate towards target
  - Fire when timer ready
  - Decrement timer
- Only shoot when GameState is Playing
```

---

### Phase 3: Combat Entities

#### 6. Projectile.cs
Projectile behavior for player and turrets.

```
Requirements:
- Inspector fields:
  - float speed = 12f
  - float lifetime = 2f
  - float damage (set by shooter)
- Behavior:
  - Move forward each frame
  - Destroy after lifetime
  - OnTriggerEnter with Enemy: deal damage, destroy self
- Method:
  - Initialize(float damage, Vector3 direction) - called by shooter
```

#### 7. Enemy.cs
Base enemy behavior using NavMesh.

```
Requirements:
- Inspector fields:
  - int maxHealth = 20
  - float moveSpeed = 4f
  - int damageToBase = 5
  - int coinDrop = 5
- Components needed:
  - NavMeshAgent
  - Collider (trigger for projectile hits)
- Private fields:
  - int currentHealth
  - Transform baseTarget
  - bool isDead
- Methods:
  - Initialize() - find base, set destination
  - TakeDamage(int damage) - reduce health, check death
  - Die() - fire OnEnemyKilled event with position and coinDrop, destroy
  - OnReachBase() - deal damage to base, destroy self
- Use OnTriggerEnter to detect base collision (tag: "Base")
- NavMeshAgent.SetDestination to base position
- Stop agent when dead
```

#### 8. EnemySpawner.cs
Simple spawner utility (used by WaveManager).

```
Requirements:
- Static or instance methods:
  - SpawnEnemy(GameObject prefab, Vector3 position) - instantiate and initialize
- Can be merged into WaveManager if preferred
- Ensure spawned enemies are added to a list for tracking
```

---

### Phase 4: Defense Systems

#### 9. Base.cs
The structure players must defend.

```
Requirements:
- Inspector fields:
  - int maxHealth = 500
- Properties:
  - int CurrentHealth
  - float HealthPercentage
- Methods:
  - TakeDamage(int damage) - reduce health, fire events
  - Heal(int amount) - optional, not used in MVP
- Fire events:
  - OnBaseHealthChanged(current, max)
  - OnBaseDamaged(damage)
  - OnBaseDestroyed() when health <= 0
- Visual feedback (optional):
  - Reference to child objects for damage states
```

#### 10. Turret.cs
Activatable defensive turret.

```
Requirements:
- Inspector fields:
  - int activationCost = 50
  - float damage = 8f
  - float fireRate = 1.5f
  - float range = 6f
  - float activationRadius = 2f (how close player must be)
  - GameObject projectilePrefab
  - Transform firePoint
  - Transform turretHead (part that rotates)
  - GameObject inactiveVisual
  - GameObject activeVisual
  - LayerMask enemyLayer
- Properties:
  - bool IsActive
  - bool PlayerInRange
- Methods:
  - CheckPlayerDistance() - is player within activationRadius?
  - TryActivate() - check coins, spend coins, activate
  - Activate() - enable shooting, swap visuals, fire event
  - FindTarget() - same as PlayerShooting
  - Shoot() - same as PlayerShooting
- Update loop (when active):
  - Find target
  - Rotate turret head
  - Fire when ready
- Show/hide activation prompt based on PlayerInRange and !IsActive
```

---

### Phase 5: Collectibles

#### 11. Coin.cs
Collectible coin with magnet behavior.

```
Requirements:
- Inspector fields:
  - int value = 5
  - float magnetRadius = 2.5f
  - float flySpeed = 10f
  - float lifetime = 30f
- Private fields:
  - Transform player
  - bool isCollecting
  - float timer
- Methods:
  - Initialize(int value) - set coin value
  - Update():
    - If not collecting, check distance to player
    - If within magnetRadius, start collecting
    - If collecting, move toward player
    - If reached player, collect
    - Increment timer, destroy if lifetime exceeded
  - Collect() - GameManager.AddCoins(value), destroy
- Find player in Start() using tag "Player"
- Add slight bobbing animation (optional)
```

---

### Phase 6: Input

#### 12. FloatingJoystick.cs
Touch-based floating joystick.

```
Requirements:
- Implements IPointerDownHandler, IDragHandler, IPointerUpHandler
- Inspector fields:
  - RectTransform background
  - RectTransform handle
  - float handleRange = 50f
- Properties:
  - float Horizontal (read-only)
  - float Vertical (read-only)
  - Vector2 Direction (read-only)
- Methods:
  - OnPointerDown(PointerEventData):
    - Position background at touch point
    - Show joystick
    - Process input
  - OnDrag(PointerEventData):
    - Calculate handle position clamped to range
    - Update Horizontal/Vertical values (-1 to 1)
  - OnPointerUp(PointerEventData):
    - Reset handle to center
    - Hide joystick
    - Reset Horizontal/Vertical to 0
- Start hidden, appear on touch
- Works in screen space overlay canvas
```

---

### Phase 7: UI

#### 13. HUDManager.cs
Main gameplay HUD.

```
Requirements:
- Inspector fields:
  - TextMeshProUGUI coinsText
  - TextMeshProUGUI waveText
  - Slider baseHealthSlider (or Image fill)
  - TextMeshProUGUI baseHealthText
  - GameObject waveNotification
  - TextMeshProUGUI waveNotificationText
- Methods:
  - UpdateCoins(int coins)
  - UpdateWave(int current, int total)
  - UpdateBaseHealth(int current, int max)
  - ShowWaveNotification(int waveNumber) - coroutine, show for 2 seconds
- Subscribe to GameEvents in OnEnable, unsubscribe in OnDisable:
  - OnCoinsChanged
  - OnWaveStarted
  - OnBaseHealthChanged
```

#### 14. TurretPrompt.cs
World-space UI for turret activation.

```
Requirements:
- Attached to each Turret as child world-space canvas
- Inspector fields:
  - TextMeshProUGUI costText
  - Button activateButton
  - GameObject promptPanel
  - Turret parentTurret (auto-find in parent)
- Methods:
  - Show() - enable panel, update cost text
  - Hide() - disable panel
  - OnActivateClicked() - call parentTurret.TryActivate()
  - UpdateAffordability() - gray out if can't afford
- Billboard: face camera each frame
- Button interactable only if player has enough coins
- Listen to OnCoinsChanged to update affordability
```

#### 15. MainMenuUI.cs
Main menu screen.

```
Requirements:
- Inspector fields:
  - Button playButton
  - Button quitButton
- Methods:
  - OnPlayClicked() - load Game scene
  - OnQuitClicked() - Application.Quit()
- Hook up buttons in Start() or Inspector
```

#### 16. GameOverUI.cs
Victory/Defeat screen.

```
Requirements:
- Inspector fields:
  - GameObject victoryPanel
  - GameObject defeatPanel
  - TextMeshProUGUI waveReachedText (defeat only)
  - Button playAgainButton
  - Button mainMenuButton
- Methods:
  - ShowVictory()
  - ShowDefeat(int waveReached)
  - OnPlayAgainClicked() - GameManager.RestartGame()
  - OnMainMenuClicked() - GameManager.ReturnToMenu()
- Subscribe to OnGameVictory and OnGameDefeat
- Disable/enable panels accordingly
```

---

## Coding Standards

### General Rules
1. Use `#region` blocks to organize code sections
2. Add XML documentation comments for public methods
3. Use `[SerializeField]` for private inspector fields
4. Use `[Header("Section")]` to organize inspector
5. Cache component references in Awake()
6. Use CompareTag() instead of == for tag comparison
7. Null-check before accessing potentially null references

### Naming Conventions
- **Classes:** PascalCase (e.g., `GameManager`)
- **Public methods:** PascalCase (e.g., `StartGame()`)
- **Private methods:** PascalCase (e.g., `HandleInput()`)
- **Public properties:** PascalCase (e.g., `CurrentHealth`)
- **Private fields:** camelCase with underscore prefix (e.g., `_fireTimer`)
- **Serialized fields:** camelCase (e.g., `moveSpeed`)
- **Constants:** UPPER_SNAKE_CASE (e.g., `MAX_ENEMIES`)
- **Events:** PascalCase with On prefix (e.g., `OnDeath`)

### Unity Best Practices
1. Prefer `TryGetComponent<T>()` over `GetComponent<T>()`
2. Use object pooling comments where applicable (mark with `// TODO: Implement pooling`)
3. Avoid `Find()` methods in Update - cache references
4. Use `Time.deltaTime` for frame-independent movement
5. Use layers for physics queries (OverlapSphere, Raycast)
6. Keep Update() methods lean - delegate to other methods

### Event Pattern
```csharp
// In GameEvents.cs
public static event Action<int> OnCoinsChanged;
public static void CoinsChanged(int coins) => OnCoinsChanged?.Invoke(coins);

// In subscriber
private void OnEnable() => GameEvents.OnCoinsChanged += HandleCoinsChanged;
private void OnDisable() => GameEvents.OnCoinsChanged -= HandleCoinsChanged;
private void HandleCoinsChanged(int coins) { /* ... */ }
```

---

## Configuration Values Reference

From GDD - use these exact values:

### Player
```csharp
moveSpeed = 5f;
fireRate = 2f;
damage = 10f;
range = 8f;
```

### Enemies
```csharp
// Scout
maxHealth = 20; moveSpeed = 4f; damageToBase = 5; coinDrop = 5;

// Grunt  
maxHealth = 50; moveSpeed = 2.5f; damageToBase = 15; coinDrop = 10;

// Tank
maxHealth = 120; moveSpeed = 1.5f; damageToBase = 30; coinDrop = 25;
```

### Turret
```csharp
activationCost = 50;
damage = 8f;
fireRate = 1.5f;
range = 6f;
```

### Base
```csharp
maxHealth = 500;
```

### Waves
```csharp
delayBetweenWaves = 5f;
spawnInterval = 1.5f;

// Wave 1: 5 scouts
// Wave 2: 5 scouts, 3 grunts
// Wave 3: 6 scouts, 4 grunts, 1 tank
// Wave 4: 4 scouts, 6 grunts, 2 tanks
// Wave 5: 5 scouts, 5 grunts, 4 tanks
```

### Coins
```csharp
magnetRadius = 2.5f;
flySpeed = 10f;
lifetime = 30f;
```

---

## Validation Checklist

After creating all scripts, verify:

- [ ] All scripts compile without errors
- [ ] No missing `using` statements
- [ ] All public fields have appropriate attributes
- [ ] Events are properly subscribed/unsubscribed
- [ ] Singleton patterns are correctly implemented
- [ ] LayerMask fields exist for physics queries
- [ ] Tags used: "Player", "Enemy", "Base", "Turret", "Coin"
- [ ] Scenes to reference: "MainMenu", "Game"

---

## Output Format

For each script, output:
1. Full file path
2. Complete C# code
3. Brief description of dependencies (what it needs to work)

Begin with `GameEvents.cs` and proceed in the order listed above.

---

## Notes for AI Agent

- Do NOT skip any script
- Do NOT simplify or remove features from the GDD
- Include ALL methods listed, even if implementation seems trivial
- Add `// TODO:` comments for future enhancements (like object pooling)
- Test compilation mentally - ensure all type references exist
- Cross-reference the GDD for any clarification needed

**Begin implementation now.**
