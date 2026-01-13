# VFX System - Setup and Usage Guide

## Overview

The VFX System provides a comprehensive, performant, and easy-to-use solution for managing visual effects in your game. It features:

- **Object Pooling** - Efficient reuse of particle systems
- **Audio Synchronization** - Built-in integration with AudioManager
- **Performance Optimization** - Particle budgeting and distance-based LOD
- **Inspector-Friendly** - Add new effects without writing code
- **Priority System** - Ensures critical effects always play

---

## Quick Start

### 1. Create VFX Library

1. In Unity, right-click in Project window
2. Select `Create > Base Defender > VFX Library`
3. Name it `VFXLibrary`
4. Click the library asset and select `Generate Default Effect Entries` from the context menu (right-click)
5. Assign particle system prefabs to each VFX entry in the inspector

### 2. Setup VFX Manager

1. Create an empty GameObject in your scene named "VFXManager"
2. Add the `VFXManager` component
3. Assign the VFXLibrary you created
4. Configure performance settings (defaults are good for mobile)

### 3. Create VFX Prefabs

For each effect in your VFXLibrary:

1. Create a GameObject with a ParticleSystem
2. Optionally add `VFXController` component for enhanced control
3. Configure the particle system according to the VFX Implementation Guide
4. Save as prefab in `Assets/VFX/Prefabs/`
5. Assign the prefab to the corresponding entry in VFXLibrary

---

## Usage Examples

### Basic Usage - Play Effect

```csharp
using BaseDefender.VFX;

// Play a VFX effect at a position
VFXManager.Instance.PlayEffect(VFXType.PlayerSpellProjectile, transform.position);

// Play with rotation
VFXManager.Instance.PlayEffect(VFXType.TowerMuzzleFlash, firePoint.position, firePoint.rotation);

// Play attached to a transform
VFXManager.Instance.PlayEffectAttached(VFXType.TowerIdleGlow, towerTransform);
```

### Using VFXHelper (Recommended)

The VFXHelper class provides convenient methods with audio synchronization:

```csharp
using BaseDefender.VFX;

// In PlayerShooting.cs
private void Shoot()
{
    // Plays both VFX and audio automatically
    VFXHelper.PlayPlayerSpell(firePoint.position, firePoint.rotation);
}

// In Turret.cs
public void Activate()
{
    // Tower activation with synced audio
    VFXHelper.PlayTowerActivation(transform.position);
}

// In Enemy.cs
private void Die()
{
    // Death effect with audio
    VFXHelper.PlayAngelDeath(transform.position);

    // Spawn coin with idle glow
    GameObject coin = SpawnCoin();
    VFXHelper.PlayCoinIdle(coin.transform);
}

// In Coin.cs - Collection
private void OnCollected()
{
    // Collection burst with satisfying audio
    VFXHelper.PlayCoinCollect(transform.position);
}
```

### Advanced Usage - Custom Audio Callback

```csharp
// Play VFX with custom audio logic
VFXManager.Instance.PlayEffectWithAudio(
    VFXType.TowerActivation,
    transform.position,
    () => {
        AudioManager.Instance.PlayTurretActivate();
        // Additional audio or logic here
    }
);
```

### Stop Effects

```csharp
// Stop a specific effect
ParticleSystem effect = VFXHelper.PlayPlayerSpell(position);
VFXHelper.StopEffect(effect);

// Stop all effects of a type
VFXManager.Instance.StopAllEffectsOfType(VFXType.AmbientAtmosphere);

// Clear all active effects (useful for scene transitions)
VFXHelper.ClearAllEffects();
```

---

## Integration with Existing Code

### Player Shooting

```csharp
// In PlayerShooting.cs - Shoot() method
private void Shoot()
{
    // OLD: AudioManager.Instance.PlayPlayerShoot();

    // NEW: Play muzzle flash with audio
    VFXHelper.PlayPlayerMuzzleFlash(firePoint.position, firePoint.rotation);

    // Create projectile
    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

    // Attach projectile trail VFX
    VFXHelper.PlayEffectAttached(VFXType.PlayerSpellProjectile, projectile.transform);
}
```

### Turret Activation

```csharp
// In Turret.cs - Activate() method
public void Activate()
{
    isActive = true;

    // Play activation sequence (includes audio)
    VFXHelper.PlayTowerActivation(transform.position);

    // Wait for activation to complete, then add idle glow
    StartCoroutine(EnableIdleGlowAfterDelay());
}

private IEnumerator EnableIdleGlowAfterDelay()
{
    yield return new WaitForSeconds(2.0f); // Match activation duration
    VFXHelper.PlayTowerIdleGlow(transform);
}
```

### Enemy Death

```csharp
// In Enemy.cs - Die() method
private void Die()
{
    // OLD: AudioManager.Instance.PlayEnemyDeath();

    // NEW: Play death VFX with audio
    VFXHelper.PlayAngelDeath(transform.position);

    // Spawn coin
    SpawnCoin();

    // Disable enemy
    gameObject.SetActive(false);
}
```

### Projectile Hits

```csharp
// In Projectile.cs - OnTriggerEnter() method
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Enemy"))
    {
        // Play impact effect
        VFXHelper.PlayDemonicHit(transform.position);

        // Apply damage
        Enemy enemy = other.GetComponent<Enemy>();
        enemy?.TakeDamage(damage);

        // Return projectile to pool
        ReturnToPool();
    }
}
```

### Coin Collection

```csharp
// In Coin.cs - OnCollected() method
public void OnCollected()
{
    // Play collection effect with audio
    VFXHelper.PlayCoinCollect(transform.position);

    // Add currency to player
    GameManager.Instance.AddCoins(coinValue);

    // Return to pool or destroy
    ReturnToPool();
}
```

---

## Adding New VFX Effects

### Step 1: Add to VFXType Enum

```csharp
// In VFXType.cs
public enum VFXType
{
    // ... existing types

    // Add your new type here
    PlayerDashTrail,
    BossDeathExplosion,
    PowerUpCollect
}
```

### Step 2: Create Particle System Prefab

1. Create new GameObject with ParticleSystem
2. Configure particle system settings
3. Optionally add VFXController component
4. Save as prefab

### Step 3: Add to VFX Library

1. Open your VFXLibrary asset
2. Find the appropriate category (Player, Tower, Combat, etc.)
3. Add a new entry to the list
4. Configure the VFXData:
   - **VFX Type**: Select your new type
   - **Prefab**: Assign your particle system prefab
   - **Priority**: Set importance (0=Critical, 3=Low)
   - **Audio Sync**: Enable if it should play audio
   - **Audio Clip**: Assign audio clip if using sync
   - **Pool Settings**: Configure initial/max pool size

### Step 4: Add Helper Method (Optional)

```csharp
// In VFXHelper.cs - Add a convenience method
public static ParticleSystem PlayPlayerDashTrail(Vector3 position)
{
    return VFXManager.Instance?.PlayEffect(VFXType.PlayerDashTrail, position);
}
```

---

## Performance Tips

### Particle Budgeting

The VFX system automatically manages particle count to maintain performance:

- **Max Particles**: Set in VFXManager (default: 200 for mobile)
- **Priority System**: Critical effects always play, low-priority may be culled
- **Distance LOD**: Far effects are automatically skipped

### Priority Guidelines

- **Critical (0)**: Player attacks, major hits, important feedback
- **High (1)**: Tower attacks, enemy deaths, significant events
- **Medium (2)**: Ambient effects, trails, minor impacts
- **Low (3)**: Optional polish, dust, decorative particles

### Best Practices

1. **Use Object Pooling**: Always use VFXManager.PlayEffect(), never Instantiate() VFX prefabs directly
2. **Keep Particle Counts Low**: Mobile target is 20-30 particles per effect
3. **Use Additive Shaders**: More performant than alpha blending
4. **Limit Active Effects**: The system tracks this automatically
5. **Test on Target Device**: Always profile on mobile devices

---

## Debug Tools

### Inspector Debug Mode

Enable "Show Debug Info" in VFXManager inspector to see:
- Active effects count
- Active particles count
- Pool statistics
- Button to clear all effects

### Code Debug Methods

```csharp
// Check if system is ready
if (VFXHelper.IsVFXSystemReady())
{
    Debug.Log("VFX System initialized!");
}

// Get performance stats
int particleCount = VFXHelper.GetActiveParticleCount();
int effectCount = VFXHelper.GetActiveEffectCount();
Debug.Log($"Active: {effectCount} effects, {particleCount} particles");

// Get pool info for specific effect
string poolInfo = VFXManager.Instance.GetPoolDebugInfo(VFXType.PlayerSpellProjectile);
Debug.Log(poolInfo);
```

---

## Audio Synchronization

### Automatic Sync

VFX effects can automatically play audio when enabled in VFXLibrary:

1. Enable "Sync With Audio" in VFXData
2. Assign audio clip
3. Set audio volume (0-1)
4. Set audio delay for timing adjustments

### Manual Integration

For custom audio control:

```csharp
// Play VFX and audio separately
VFXManager.Instance.PlayEffect(VFXType.PlayerSpellProjectile, position);
AudioManager.Instance.PlayPlayerShoot();

// Or use helper with custom callback
VFXManager.Instance.PlayEffectWithAudio(
    VFXType.TowerActivation,
    position,
    () => {
        // Your custom audio logic
        AudioManager.Instance.PlayTurretActivate();
    }
);
```

---

## Troubleshooting

### Effects Not Playing

1. Check VFXManager is in scene and initialized
2. Verify VFXLibrary is assigned in VFXManager
3. Check prefab is assigned in VFXLibrary for that VFX type
4. Ensure particle budget isn't exceeded (check debug info)

### Audio Not Playing

1. Verify AudioManager is in scene
2. Check "Enable Audio Sync" in VFXManager
3. Confirm audio clip assigned in VFXData
4. Check AudioManager has sound clips assigned

### Performance Issues

1. Check active particle count (should be < 200 for mobile)
2. Lower max particles in VFXManager settings
3. Reduce priority of non-critical effects
4. Enable distance LOD culling
5. Reduce initial pool sizes

### Pool Exhaustion Warnings

1. Increase max pool size in VFXData
2. Check for effects not being returned to pool (infinite loops)
3. Consider reducing effect lifetime

---

## File Structure

```
Assets/
├── Scripts/
│   └── VFX/
│       ├── VFXType.cs              (Effect type enum)
│       ├── VFXData.cs              (Effect configuration)
│       ├── VFXLibrary.cs           (ScriptableObject)
│       ├── VFXManager.cs           (Main manager)
│       ├── VFXController.cs        (Component for prefabs)
│       ├── VFXHelper.cs            (Convenience methods)
│       └── README_VFX_SYSTEM.md    (This file)
│
├── VFX/
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Towers/
│   │   ├── Combat/
│   │   ├── Collection/
│   │   └── Environment/
│   │
│   └── Materials/
│       └── (VFX materials and shaders)
│
└── ScriptableObjects/
    └── VFXLibrary.asset           (Your VFX library asset)
```

---

## Support

For VFX creation guidelines and specifications, refer to:
- `VFX_Implementation_Guide.md` - Comprehensive VFX specifications

For code questions or issues:
1. Check this README first
2. Inspect debug output with "Show Debug Info" enabled
3. Verify setup steps were followed correctly

---

**Version**: 1.0
**Last Updated**: 2026-01-10
**Compatibility**: Unity 2020.3+ with URP
