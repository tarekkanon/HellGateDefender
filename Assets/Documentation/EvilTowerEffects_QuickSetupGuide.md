# Evil Tower Effects - Quick Setup Guide
## Complete Implementation for All 5 Tower VFX Effects

---

## 📋 Overview

This guide provides step-by-step instructions for implementing all **EVIL TOWER EFFECTS** from the VFX Implementation Guide using the configuration scripts.

### Effects Included:
1. **Inactive Tower - Dormant State** (P1)
2. **Tower Activation Sequence** (P0)
3. **Active Tower - Idle Glow** (P0)
4. **Tower Spell Projectile** (P0)
5. **Tower Muzzle Flash** (P0)

All effects follow the same implementation pattern as `DemonicSpellProjectile` with automated configuration scripts.

---

## 🚀 Quick Start

### Prerequisites
- Unity 2020.3+ with URP
- Scripts already created in `Assets/Scripts/VFX/`:
  - `TowerInactiveConfig.cs`
  - `TowerActivationConfig.cs`
  - `TowerIdleGlowConfig.cs`
  - `TowerSpellProjectileConfig.cs`
  - `TowerMuzzleFlashConfig.cs`

### Materials Needed
Create these materials in `Assets/VFX/Materials/`:
- `MAT_Particle_Additive_Purple.mat` - For inactive tower
- `MAT_Particle_Additive_Green.mat` - For active effects
- `MAT_Trail_Green.mat` - For tower projectile trail

**Material Settings:**
```
Shader: URP/Particles/Unlit
Surface: Transparent
Blend: Additive
Render Face: Both
Base Color: White (255, 255, 255)
Vertex Alpha: ✓ Enabled
GPU Instancing: ✓ Enabled
```

---

## 1️⃣ Inactive Tower - Dormant State

### Purpose
Subtle ambient effect showing tower is powered down but can be activated.

### Setup Steps

1. **Create GameObject**
   - Right-click in Hierarchy → Create Empty
   - Name: `FX_Tower_Inactive`

2. **Add Particle System**
   - Add Component → Effects → Particle System

3. **Add Configuration Script**
   - Add Component → Scripts → `TowerInactiveConfig`

4. **Configure Materials**
   - Assign `MAT_Particle_Additive_Purple` to Particle System Renderer

5. **Apply Configuration**
   - In Inspector, click **TowerInactiveConfig** component
   - Right-click → **Apply Configuration**
   - Verify with **Validate Settings**

6. **Save as Prefab**
   - Drag to `Assets/VFX/Prefabs/Towers/FX_Tower_Inactive.prefab`

### Key Settings
| Property | Value |
|----------|-------|
| Emission Rate | 2.5 particles/sec |
| Particle Lifetime | 2.5s |
| Size | 0.15 - 0.25 |
| Color | Faint Purple (RGB 75, 0, 130, Alpha 0.3) |
| Movement | Upward drift 0.5 units/sec |
| Max Particles | 10 |

### Usage
```csharp
// Attach to inactive tower
ParticleSystem inactiveEffect = Instantiate(inactiveTowerPrefab, towerPosition, Quaternion.identity);
inactiveEffect.transform.SetParent(towerTransform);
inactiveEffect.Play();
```

---

## 2️⃣ Tower Activation Sequence

### Purpose
Epic 2-second effect when player activates tower with coins. Three distinct phases.

### Setup Steps

1. **Create Parent GameObject**
   - Name: `FX_Tower_Activation`

2. **Create 3 Child Particle Systems**
   - Child 1: `Phase1_GroundEruption`
   - Child 2: `Phase2_EnergySpiral`
   - Child 3: `Phase3_PowerSurge`

3. **Add Configuration Script to Parent**
   - Add Component → `TowerActivationConfig`

4. **Assign References**
   - In TowerActivationConfig Inspector:
     - Drag Phase1 child to `Phase 1 Ground Eruption` field
     - Drag Phase2 child to `Phase 2 Energy Spiral` field
     - Drag Phase3 child to `Phase 3 Power Surge` field

5. **Add Point Light (Optional)**
   - Create child Light object
   - Will auto-assign to config script

6. **Configure Materials**
   - Phase 1 & 2: `MAT_Particle_Additive_Purple.mat`
   - Phase 3: `MAT_Particle_Additive_Green.mat`

7. **Apply Configuration**
   - Right-click TowerActivationConfig → **Apply Configuration**

8. **Test Sequence**
   - Right-click TowerActivationConfig → **Play Activation Sequence**
   - Should see full 2-second effect

9. **Save as Prefab**
   - Drag to `Assets/VFX/Prefabs/Towers/FX_Tower_Activation.prefab`

### Phase Breakdown

**Phase 1: Ground Eruption (0.0 - 0.5s)**
- 15-20 particles burst upward from tower base
- Colors: Dark red → Purple
- Speed: 4-6 units/sec

**Phase 2: Energy Spiral (0.5 - 1.5s)**
- Continuous particles spiral up tower
- 30 particles/sec emission
- Colors: Red → Purple → Toxic Green

**Phase 3: Power Surge (1.5 - 2.0s)**
- Bright flash burst at tower top
- 8-10 particles expanding rapidly
- Colors: White-Green flash

### Usage
```csharp
// Play when tower is activated
public void ActivateTower()
{
    ParticleSystem activation = Instantiate(activationPrefab, towerTop, Quaternion.identity);
    activation.GetComponent<TowerActivationConfig>().PlayActivationSequence();

    // Wait 2 seconds, then enable tower functionality
    StartCoroutine(EnableTowerAfterActivation(2.0f));
}
```

---

## 3️⃣ Active Tower - Idle Glow

### Purpose
Persistent looping effect showing tower is active and ready to fire.

### Setup Steps

1. **Create GameObject**
   - Name: `FX_Tower_IdleGlow`

2. **Add Particle System**
   - Add Component → Particle System

3. **Add Configuration Script**
   - Add Component → `TowerIdleGlowConfig`

4. **Configure Materials**
   - Assign `MAT_Particle_Additive_Green.mat`

5. **Assign Tower Renderer (Optional)**
   - Drag tower mesh renderer to `Tower Renderer` field for pulsing emissive glow
   - Make sure tower material has emission enabled

6. **Apply Configuration**
   - Right-click TowerIdleGlowConfig → **Apply Configuration**
   - Verify with **Validate Settings**

7. **Save as Prefab**
   - Drag to `Assets/VFX/Prefabs/Towers/FX_Tower_IdleGlow.prefab`

### Key Features
- **Floating Energy Orbs**: 3-5 particles/sec orbiting tower top
- **Material Pulse**: Tower mesh pulses green (1.5 → 2.5 → 1.5 intensity, 2s cycle)
- **Low Performance**: Only 15 max particles, always running

### Settings
| Property | Value |
|----------|-------|
| Emission Rate | 4 particles/sec |
| Particle Lifetime | 2.0s |
| Size | 0.1 - 0.2 |
| Colors | Toxic Green & Purple alternating |
| Orbit Radius | 0.8 units |
| Orbit Speed | 0.5 |
| Max Particles | 15 |

### Usage
```csharp
// Attach to active tower (persistent)
public void EnableTowerIdleGlow()
{
    ParticleSystem idleGlow = Instantiate(idleGlowPrefab, towerTop, Quaternion.identity);
    idleGlow.transform.SetParent(towerTransform);

    // Assign tower renderer for material pulse
    var config = idleGlow.GetComponent<TowerIdleGlowConfig>();
    config.towerRenderer = towerMeshRenderer;

    idleGlow.Play(); // Loops forever
}
```

---

## 4️⃣ Tower Spell Projectile

### Purpose
Main attack projectile fired by active tower at angels. More powerful looking than player projectile.

### Setup Steps

1. **Create GameObject**
   - Name: `FX_Tower_Spell`

2. **Add Particle System**
   - Add Component → Particle System

3. **Add Trail Renderer**
   - Add Component → Effects → Trail Renderer
   - Assign `MAT_Trail_Green.mat`

4. **Add Point Light (Optional)**
   - Create child GameObject with Light component

5. **Add Configuration Script**
   - Add Component → `TowerSpellProjectileConfig`

6. **Configure Materials**
   - Particle System: `MAT_Particle_Additive_Green.mat`
   - Trail: `MAT_Trail_Green.mat`

7. **Apply Configuration**
   - Right-click TowerSpellProjectileConfig → **Apply Configuration**
   - Verify with **Validate Settings**

8. **Save as Prefab**
   - Drag to `Assets/VFX/Prefabs/Towers/FX_Tower_Spell.prefab`

### Key Differences from Player Projectile
- **60% Larger**: Size 0.5 units vs 0.3
- **Brighter Colors**: Toxic Green core instead of red
- **Thicker Trail**: 0.25 start width vs 0.15
- **Longer Trail**: 0.4s duration vs 0.3s
- **Higher Budget**: 30 particles vs 20

### Settings
| Property | Value |
|----------|-------|
| Emission Rate | 25 particles/sec |
| Particle Lifetime | 0.8s |
| Size | 0.25 - 0.4 |
| Colors | Bright Green → Green → Purple |
| Trail Width | 0.25 → 0.0 |
| Trail Time | 0.4s |
| Max Particles | 30 |
| Point Light | Intensity 3.0, Range 4.0 |

### Usage
```csharp
// Attach to projectile GameObject
public void FireTowerSpell(Vector3 targetPosition)
{
    GameObject projectile = Instantiate(towerSpellPrefab, firePoint.position, Quaternion.identity);

    // Attach VFX to projectile
    ParticleSystem vfx = projectile.GetComponent<ParticleSystem>();
    vfx.Play();

    // Move projectile toward target
    StartCoroutine(MoveProjectile(projectile, targetPosition, 12f)); // 12 units/sec
}
```

---

## 5️⃣ Tower Muzzle Flash

### Purpose
Brief flash effect when tower fires a spell at firing point.

### Setup Steps

1. **Create GameObject**
   - Name: `FX_Tower_MuzzleFlash`

2. **Add Particle System**
   - Add Component → Particle System

3. **Add Flash Light (Optional)**
   - Create child Light object

4. **Add Configuration Script**
   - Add Component → `TowerMuzzleFlashConfig`

5. **Configure Materials**
   - Assign `MAT_Particle_Additive_Green.mat`

6. **Apply Configuration**
   - Right-click TowerMuzzleFlashConfig → **Apply Configuration**
   - Verify with **Validate Settings**

7. **Test Effect**
   - Right-click TowerMuzzleFlashConfig → **Play Flash**
   - Should see brief 0.2s flash

8. **Save as Prefab**
   - Drag to `Assets/VFX/Prefabs/Towers/FX_Tower_MuzzleFlash.prefab`

### Key Features
- **One-Shot Effect**: 0.2 second duration
- **Burst Only**: 5-8 particles in single burst
- **Cone Directed**: Points toward target
- **Brief Light Pulse**: 0.15s flash at firing point

### Settings
| Property | Value |
|----------|-------|
| Duration | 0.2s (one-shot) |
| Burst Count | 6 particles |
| Size | 0.6 - 1.0 (larger than player) |
| Speed | 3-5 units/sec |
| Cone Angle | 25° |
| Colors | White-Green → Green → Purple |
| Flash Light | Intensity 4.0, Range 5.0 |

### Usage
```csharp
// Spawn at fire point when tower shoots
public void ShowTowerMuzzleFlash(Vector3 firePosition, Quaternion fireRotation)
{
    ParticleSystem flash = Instantiate(muzzleFlashPrefab, firePosition, fireRotation);

    // Auto-plays and destroys after 0.5s
    Destroy(flash.gameObject, 0.5f);
}
```

---

## 🎨 Color Reference

### Tower VFX Color Palette

**Primary Colors:**
```css
Toxic Green:   #32CD32  RGB(50, 205, 50)   - Active towers, projectiles
Dark Purple:   #4B0082  RGB(75, 0, 130)    - Inactive state, trail end
Crimson Red:   #DC143C  RGB(220, 20, 60)   - Activation phase 1
Bright White:  #FFFFFF  RGB(255, 255, 255) - Flash effects
```

**Usage Map:**
| Effect | Start Color | Mid Color | End Color |
|--------|-------------|-----------|-----------|
| Inactive | Purple | Purple | Purple |
| Activation P1 | Crimson | Purple | Purple |
| Activation P2 | Crimson | Purple | Green |
| Activation P3 | White-Green | Green | Green |
| Idle Glow | Green | Green/Purple | Purple |
| Projectile | Bright Green | Green | Purple |
| Muzzle Flash | White-Green | Green | Purple |

---

## 🔧 Integration with VFXManager

### Registering Effects

Add to `VFXManager.cs`:

```csharp
[Header("VFX Prefabs - Tower")]
[SerializeField] private ParticleSystem towerInactivePrefab;
[SerializeField] private ParticleSystem towerActivationPrefab;
[SerializeField] private ParticleSystem towerIdleGlowPrefab;
[SerializeField] private ParticleSystem towerSpellPrefab;
[SerializeField] private ParticleSystem towerMuzzleFlashPrefab;
```

### Initialize Pools

```csharp
private void InitializePools()
{
    // ... existing pools ...

    CreatePool("TowerInactive", towerInactivePrefab);
    CreatePool("TowerActivation", towerActivationPrefab);
    CreatePool("TowerIdleGlow", towerIdleGlowPrefab);
    CreatePool("TowerSpell", towerSpellPrefab);
    CreatePool("TowerMuzzle", towerMuzzleFlashPrefab);
}
```

### Usage Examples

```csharp
// Play inactive tower effect
VFXManager.Instance.PlayEffect("TowerInactive", towerBasePosition);

// Play activation sequence (one-time)
VFXManager.Instance.PlayEffect("TowerActivation", towerPosition);

// Play idle glow (persistent - don't pool this)
ParticleSystem idleGlow = Instantiate(towerIdleGlowPrefab, towerTop, Quaternion.identity);
idleGlow.transform.SetParent(towerTransform);

// Play tower spell projectile
VFXManager.Instance.PlayEffect("TowerSpell", firePoint.position, firePoint.rotation);

// Play muzzle flash
VFXManager.Instance.PlayEffect("TowerMuzzle", firePoint.position, firePoint.rotation);
```

---

## 📊 Performance Budget

### Per-Effect Particle Counts

| Effect | Max Particles | Emission Rate | Lifetime | Impact |
|--------|--------------|---------------|----------|--------|
| Inactive | 10 | 2.5/s | 2.5s | Very Low |
| Activation | 70 (total) | Varies | 2.0s total | Medium (one-time) |
| Idle Glow | 15 | 4/s | 2.0s | Low (continuous) |
| Projectile | 30 | 25/s | 0.8s | Medium |
| Muzzle Flash | 10 | Burst only | 0.2s | Very Low |

### Total Budget Scenarios

**Worst Case (4 Active Towers firing):**
- 4 × Idle Glow: 60 particles
- 4 × Projectile: 120 particles
- 4 × Muzzle Flash: 40 particles (brief)
- **Total: ~220 particles** (within mobile budget)

**Optimization:**
- LOD: Disable Idle Glow if camera distance > 15 units
- Limit simultaneous muzzle flashes to 4
- Disable Point Lights on low-end devices

---

## ✅ Validation Checklist

### For Each Effect:

□ Prefab created in correct folder
□ Configuration script attached
□ Materials assigned (Additive blend)
□ "Apply Configuration" executed successfully
□ "Validate Settings" passes
□ Effect plays correctly in Scene view
□ Performance stats within budget
□ Saved as prefab
□ Registered in VFXManager (if using pooling)

### Testing Scenarios:

□ **Inactive Tower**: Plays subtle wisps continuously
□ **Activation**: Full 2-second sequence plays correctly
□ **Idle Glow**: Particles orbit tower, material pulses
□ **Projectile**: Trail follows projectile smoothly
□ **Muzzle Flash**: Brief flash visible at fire point

---

## 🐛 Troubleshooting

### Common Issues

**No particles visible:**
- Check material is assigned to Renderer
- Verify material uses Additive blend mode
- Check Start Color alpha is not 0
- Ensure Play On Awake is enabled (for looping effects)

**Particles don't follow projectile:**
- Set Simulation Space to **World** (not Local)
- Ensure projectile moves at correct speed (12 units/sec)
- Check Inherit Velocity is disabled

**Trail not showing:**
- Verify Trail Renderer component exists
- Check MAT_Trail_Green is assigned
- Ensure Trail Time > 0 (0.3-0.4s)

**Activation sequence timing off:**
- Check phase start delays:
  - Phase 1: 0s
  - Phase 2: 0.5s delay
  - Phase 3: 1.5s delay
- Verify each ParticleSystem is assigned in config

**Material pulse not working:**
- Ensure tower material has **Emission** enabled
- Check Tower Renderer is assigned in TowerIdleGlowConfig
- Verify `enableMaterialPulse` is checked

**Performance issues:**
- Reduce emission rates by 20-30%
- Disable Point Lights
- Lower max particles caps
- Implement LOD system (disable distant effects)

---

## 📂 File Structure

```
Assets/
├── Scripts/
│   └── VFX/
│       ├── TowerInactiveConfig.cs
│       ├── TowerActivationConfig.cs
│       ├── TowerIdleGlowConfig.cs
│       ├── TowerSpellProjectileConfig.cs
│       └── TowerMuzzleFlashConfig.cs
├── VFX/
│   ├── Prefabs/
│   │   └── Towers/
│   │       ├── FX_Tower_Inactive.prefab
│   │       ├── FX_Tower_Activation.prefab
│   │       ├── FX_Tower_IdleGlow.prefab
│   │       ├── FX_Tower_Spell.prefab
│   │       └── FX_Tower_MuzzleFlash.prefab
│   ├── Materials/
│   │   ├── MAT_Particle_Additive_Purple.mat
│   │   ├── MAT_Particle_Additive_Green.mat
│   │   └── MAT_Trail_Green.mat
│   └── Textures/
│       └── TEX_Particle_Sphere_Soft.png
└── Documentation/
    └── EvilTowerEffects_QuickSetupGuide.md (this file)
```

---

## 🎯 Next Steps

After implementing all Evil Tower Effects:

1. **Integrate with Tower System**
   - Add effect references to Tower prefabs
   - Trigger activation sequence on purchase
   - Enable idle glow on active towers
   - Spawn projectile/muzzle flash on fire

2. **Test Full Tower Lifecycle**
   - Inactive → Activation → Active Idle → Fire → Repeat
   - Verify all effects transition smoothly

3. **Optimize for Mobile**
   - Profile on target devices
   - Adjust particle budgets if needed
   - Implement LOD system

4. **Audio Integration**
   - Sync activation sound with visual phases
   - Add spell fire sound with muzzle flash
   - Background hum for idle glow

5. **Polish**
   - Add camera shake on activation (Phase 3)
   - Screen flash on power surge
   - Particle decals at tower base

---

## 📞 Support

### Context Menu Commands

All config scripts have right-click menu options:

- **Apply Configuration** - Auto-setup all particle system modules
- **Validate Settings** - Check current configuration
- **Print Configuration** - Output settings to console
- **Show Performance Stats** - Display particle budget info
- **Play Activation Sequence** - Test activation (TowerActivationConfig)
- **Play Flash** - Test muzzle flash (TowerMuzzleFlashConfig)

### Debug Tips

**View particle counts in real-time:**
```csharp
// Add to Update() for monitoring
Debug.Log($"Particles: {particleSystem.particleCount}/{particleSystem.main.maxParticles}");
```

**Test individual phases:**
```csharp
// In TowerActivationConfig
phase1_GroundEruption.Play();
phase2_EnergySpiral.Play();
phase3_PowerSurge.Play();
```

---

**Document Version:** 1.0
**Last Updated:** 2026-01-16
**Compatibility:** Unity 2020.3+ URP
**Implementation Time:** ~2-3 hours for all 5 effects

---

*Follow this guide step-by-step to implement all Evil Tower VFX effects with consistent quality and performance!*
