# Evil Tower Effects - Quick Reference Card
## Essential Values at a Glance

---

## 1. Inactive Tower - Dormant State

### Particle System Configuration
| Property | Value |
|----------|-------|
| Duration | 5.00s (continuous) |
| Looping | ✓ Enabled |
| Start Lifetime | 2.5s |
| Start Speed | 0.5 (upward drift) |
| Start Size | Random: 0.15 - 0.25 |
| Start Color | Purple RGB(75, 0, 130) Alpha 0.3 |
| Gravity Modifier | 0 |
| Simulation Space | **World** |
| Max Particles | 10 |

### Emission
| Property | Value |
|----------|-------|
| Rate over Time | 2.5 particles/sec |

### Shape
| Property | Value |
|----------|-------|
| Shape Type | Circle |
| Radius | 0.5 |
| Radius Thickness | 0.2 |

### Color over Lifetime
| Time | Color | Alpha |
|------|-------|-------|
| 0% | Purple | 0% (fade in) |
| 20% | Purple | 40% (peak) |
| 70% | Purple | 40% (hold) |
| 100% | Purple | 0% (fade out) |

### Velocity
| Axis | Value |
|------|-------|
| X/Z | Random: -0.3 to 0.3 (wander) |
| Y | 0 (drift in start speed) |

---

## 2. Tower Activation Sequence

### Phase 1: Ground Eruption (0.0-0.5s)

| Property | Value |
|----------|-------|
| Duration | 0.5s (one-shot) |
| Lifetime | 0.6s |
| Start Speed | Random: 4-6 |
| Start Size | Random: 0.3 - 0.5 |
| Gravity | 0.5 |
| Max Particles | 20 |

**Emission:**
- Burst: 15-20 particles at time 0

**Shape:**
- Cone, 15° angle, pointing upward

**Colors:**
- Crimson RGB(220, 20, 60) → Purple RGB(75, 0, 130)

### Phase 2: Energy Spiral (0.5-1.5s)

| Property | Value |
|----------|-------|
| Duration | 1.0s |
| Start Delay | 0.5s |
| Lifetime | 1.2s |
| Start Speed | 2.0 |
| Start Size | Random: 0.2 - 0.4 |
| Gravity | -0.5 (upward) |
| Max Particles | 40 |

**Emission:**
- Rate: 30 particles/sec

**Shape:**
- Circle, radius 0.4

**Velocity (Orbital):**
- Y Orbital: 2.0 (creates spiral)
- Linear Y: 3.0 (strong upward)

**Colors:**
- Red RGB(220, 20, 60) → Purple RGB(75, 0, 130) → Green RGB(50, 205, 50)

### Phase 3: Power Surge (1.5-2.0s)

| Property | Value |
|----------|-------|
| Duration | 0.5s |
| Start Delay | 1.5s |
| Lifetime | 0.5s |
| Start Speed | Random: 2-4 |
| Start Size | Random: 0.5 - 0.7 |
| Max Particles | 15 |

**Emission:**
- Burst: 8-10 particles at time 0

**Shape:**
- Sphere, radius 0.5

**Size Curve:**
- 0.0 → 1.0
- 0.2 → 2.5 (rapid expansion)
- 1.0 → 0.2

**Colors:**
- White → Bright Green-White → Green

### Point Light
| Property | Value |
|----------|-------|
| Color | Green RGB(50, 205, 50) |
| Intensity | 0 → 5.0 (fade in over 2s) |
| Range | 6.0 |

---

## 3. Active Tower - Idle Glow

### Particle System Configuration
| Property | Value |
|----------|-------|
| Duration | 5.00s (continuous) |
| Looping | ✓ Enabled |
| Start Lifetime | 2.0s |
| Start Speed | 0.5 |
| Start Size | Random: 0.1 - 0.2 |
| Start Color | Green RGB(50, 205, 50) |
| Simulation Space | **World** |
| Max Particles | 15 |

### Emission
| Property | Value |
|----------|-------|
| Rate over Time | 4 particles/sec |

### Shape
| Property | Value |
|----------|-------|
| Shape Type | Circle |
| Radius | 0.8 (orbit radius) |
| Radius Thickness | 0.1 (thin ring) |

### Velocity (Orbital Motion)
| Axis | Value |
|------|-------|
| Orbital Y | 0.5 (orbit speed) |
| Linear Y | 0.3 (upward drift) |

### Color over Lifetime
| Time | Color | Alpha |
|------|-------|-------|
| 0% | Green | 0% (fade in) |
| 20% | Green | 60% (peak) |
| 70% | Purple | 60% (hold) |
| 100% | Purple | 0% (fade out) |

### Material Pulse (Tower Mesh)
| Property | Value |
|----------|-------|
| Emission Color | Green RGB(50, 205, 50) |
| Min Intensity | 1.5 |
| Max Intensity | 2.5 |
| Pulse Frequency | 2.0s per cycle |

---

## 4. Tower Spell Projectile

### Particle System Configuration
| Property | Value |
|----------|-------|
| Duration | 5.00s (continuous) |
| Looping | ✓ Enabled |
| Start Lifetime | 0.8s |
| Start Speed | 0 |
| Start Size | Random: 0.25 - 0.4 |
| Start Color | Bright Green RGB(50, 205, 50) |
| Simulation Space | **World** |
| Max Particles | 30 |

### Emission
| Property | Value |
|----------|-------|
| Rate over Time | 25 particles/sec |

### Shape
| Property | Value |
|----------|-------|
| Shape Type | Sphere |
| Radius | 0.15 |

### Color over Lifetime (Gradient)
| Time | Color | Alpha |
|------|-------|-------|
| 0% | Bright Green RGB(50, 205, 50) | 100% |
| 40% | Green RGB(38, 179, 38) | 100% |
| 70% | Purple RGB(75, 0, 130) | 78% |
| 100% | Purple RGB(75, 0, 130) | 0% |

### Size over Lifetime
| Time | Size Multiplier |
|------|-----------------|
| 0.0 | 1.0 |
| 0.5 | 0.85 |
| 1.0 | 0.3 |

### Noise Module
| Property | Value |
|----------|-------|
| Strength | 0.5 |
| Frequency | 2.0 |
| Scroll Speed | 0.8 |
| Octaves | 2 |

### Trail Renderer
| Property | Value |
|----------|-------|
| Time | 0.4s (60% longer than player) |
| Min Vertex Distance | 0.1 |
| Width (Start → End) | 0.25 → 0.0 |

### Trail Color Gradient
| Time | Color | Alpha |
|------|-------|-------|
| 0% | Bright Green | 255 |
| 50% | Green | 179 |
| 100% | Purple | 0 |

### Point Light
| Property | Value |
|----------|-------|
| Color | Green RGB(50, 205, 50) |
| Intensity | 3.0 (50% brighter than player) |
| Range | 4.0 |

### Comparison to Player Projectile
| Aspect | Player | Tower | Difference |
|--------|--------|-------|------------|
| Visual Size | 0.3 | 0.5 | +60% |
| Trail Width | 0.15 | 0.25 | +67% |
| Trail Duration | 0.3s | 0.4s | +33% |
| Light Intensity | 2.0 | 3.0 | +50% |
| Color | Red-Purple | Green-Purple | Different |

---

## 5. Tower Muzzle Flash

### Particle System Configuration
| Property | Value |
|----------|-------|
| Duration | 0.2s (one-shot) |
| Looping | ✗ Disabled |
| Start Lifetime | 0.2s |
| Start Speed | Random: 3-5 |
| Start Size | Random: 0.6 - 1.0 |
| Start Color | White-Green RGB(230, 255, 230) |
| Max Particles | 10 |

### Emission
| Property | Value |
|----------|-------|
| Rate over Time | 0 (burst only) |
| Burst | 6 particles at time 0 |

### Shape
| Property | Value |
|----------|-------|
| Shape Type | Cone |
| Angle | 25° |
| Radius | 0.1 |

### Color over Lifetime
| Time | Color | Alpha |
|------|-------|-------|
| 0% | White-Green | 100% |
| 30% | Bright Green | 90% |
| 70% | Purple | 50% |
| 100% | Purple | 0% |

### Size over Lifetime
| Time | Size Multiplier |
|------|-----------------|
| 0.0 | 1.0 |
| 0.15 | 1.3 (quick expansion) |
| 1.0 | 0.0 (fade out) |

### Flash Light
| Property | Value |
|----------|-------|
| Color | Green RGB(50, 205, 50) |
| Max Intensity | 4.0 |
| Range | 5.0 |
| Duration | 0.15s pulse |

---

## 📊 Performance Summary

| Effect | Max Particles | Emission | Lifetime | Budget Impact |
|--------|--------------|----------|----------|---------------|
| Inactive | 10 | 2.5/s | 2.5s | Very Low |
| Activation (total) | 75 | Varies | 2.0s | Medium (one-time) |
| - Phase 1 | 20 | Burst | 0.6s | - |
| - Phase 2 | 40 | 30/s | 1.2s | - |
| - Phase 3 | 15 | Burst | 0.5s | - |
| Idle Glow | 15 | 4/s | 2.0s | Low |
| Projectile | 30 | 25/s | 0.8s | Medium |
| Muzzle Flash | 10 | Burst | 0.2s | Very Low |

**Total Simultaneous Budget (4 Active Towers):**
- 4 × Idle: 60 particles
- 4 × Projectile: 120 particles
- 4 × Muzzle: 40 particles (brief)
- **Peak: ~220 particles** ✓ Within mobile budget

---

## 🎨 Color Hex Codes

### Tower Palette
```css
/* Primary Colors */
Toxic Green:       #32CD32  RGB(50, 205, 50)
Dark Purple:       #4B0082  RGB(75, 0, 130)
Crimson Red:       #DC143C  RGB(220, 20, 60)

/* Flash Colors */
Bright White:      #FFFFFF  RGB(255, 255, 255)
White-Green:       #E6FFE6  RGB(230, 255, 230)

/* Gradient Mids */
Mid Green:         #26B326  RGB(38, 179, 38)
Faint Purple:      #4B00824D RGB(75, 0, 130, Alpha 30%)
```

---

## 🔧 Material Settings

### MAT_Particle_Additive_Green
```
Shader: URP/Particles/Unlit
Surface: Transparent
Blend: Additive
Base Color: White (255, 255, 255)
Vertex Alpha: ✓ Enabled
GPU Instancing: ✓ Enabled
```

### MAT_Particle_Additive_Purple
```
Shader: URP/Particles/Unlit
Surface: Transparent
Blend: Additive
Base Color: White (255, 255, 255)
Vertex Alpha: ✓ Enabled
GPU Instancing: ✓ Enabled
```

### MAT_Trail_Green
```
Shader: URP/Particles/Unlit
Surface: Transparent
Blend: Additive
Base Color: White (255, 255, 255)
Vertex Alpha: ✓ Enabled
```

---

## 📁 File Locations

```
Scripts:
Assets/Scripts/VFX/TowerInactiveConfig.cs
Assets/Scripts/VFX/TowerActivationConfig.cs
Assets/Scripts/VFX/TowerIdleGlowConfig.cs
Assets/Scripts/VFX/TowerSpellProjectileConfig.cs
Assets/Scripts/VFX/TowerMuzzleFlashConfig.cs

Prefabs:
Assets/VFX/Prefabs/Towers/FX_Tower_Inactive.prefab
Assets/VFX/Prefabs/Towers/FX_Tower_Activation.prefab
Assets/VFX/Prefabs/Towers/FX_Tower_IdleGlow.prefab
Assets/VFX/Prefabs/Towers/FX_Tower_Spell.prefab
Assets/VFX/Prefabs/Towers/FX_Tower_MuzzleFlash.prefab

Materials:
Assets/VFX/Materials/MAT_Particle_Additive_Green.mat
Assets/VFX/Materials/MAT_Particle_Additive_Purple.mat
Assets/VFX/Materials/MAT_Trail_Green.mat
```

---

## 🎯 Quick Start Steps

1. Create GameObjects with ParticleSystem
2. Add appropriate Config script
3. Assign materials (Green or Purple)
4. Right-click config → **Apply Configuration**
5. Right-click config → **Validate Settings**
6. Save as prefab
7. Test in Scene view

---

## ⚡ Context Menu Commands

### All Config Scripts:
- **Apply Configuration** - Auto-setup
- **Validate Settings** - Check values
- **Print Configuration** - Console output
- **Show Performance Stats** - Budget check

### TowerActivationConfig Only:
- **Play Activation Sequence** - Test full effect

### TowerMuzzleFlashConfig Only:
- **Play Flash** - Test muzzle effect

---

## 🐛 Common Issues Quick Fix

| Problem | Solution |
|---------|----------|
| No particles | Check material assigned, verify Alpha > 0 |
| Wrong colors | Re-apply configuration |
| Particles static | Set Simulation Space to **World** |
| No trail | Add Trail Renderer, assign material |
| Activation phases wrong | Check child assignments in config |
| Material pulse not working | Assign Tower Renderer, enable emission |

---

## 📊 Module Checklist

### Inactive Tower
```
✓ Main Module
✓ Emission (2.5/s)
✓ Shape (Circle 0.5)
✓ Color over Lifetime (Fade in/out)
✓ Size over Lifetime
✓ Velocity over Lifetime (Wander)
✓ Noise (Gentle 0.2)
✓ Renderer (Billboard, Additive)
```

### Tower Activation
```
Phase 1:
✓ Burst emission (15-20)
✓ Cone shape upward
✓ Red → Purple gradient

Phase 2:
✓ Continuous emission (30/s)
✓ Orbital velocity (spiral)
✓ Red → Purple → Green

Phase 3:
✓ Burst emission (8-10)
✓ Rapid size expansion
✓ White → Green flash

✓ Point Light (fade in)
```

### Idle Glow
```
✓ Main Module
✓ Emission (4/s)
✓ Shape (Circle 0.8)
✓ Orbital velocity
✓ Color over Lifetime (Green → Purple)
✓ Material Pulse (Update loop)
```

### Projectile
```
✓ Main Module (World space)
✓ Emission (25/s)
✓ Shape (Sphere 0.15)
✓ Noise (0.5 strength)
✓ Color over Lifetime (Green → Purple)
✓ Size over Lifetime
✓ Trail Renderer (0.4s)
✓ Point Light (optional)
```

### Muzzle Flash
```
✓ Main Module (one-shot)
✓ Burst emission (6)
✓ Shape (Cone 25°)
✓ Color over Lifetime (White-Green → Purple)
✓ Size expansion curve
✓ Flash Light (0.15s pulse)
```

---

## 🔗 Integration Code Snippets

### VFXManager Registration
```csharp
CreatePool("TowerInactive", towerInactivePrefab);
CreatePool("TowerActivation", towerActivationPrefab);
CreatePool("TowerSpell", towerSpellPrefab);
CreatePool("TowerMuzzle", towerMuzzleFlashPrefab);
// Note: Don't pool TowerIdleGlow - use persistent instance
```

### Usage Examples
```csharp
// Show inactive state
VFXManager.Instance.PlayEffect("TowerInactive", towerBase);

// Activate tower
var activation = VFXManager.Instance.PlayEffect("TowerActivation", towerPos);
activation.GetComponent<TowerActivationConfig>().PlayActivationSequence();

// Enable idle glow (persistent)
var glow = Instantiate(idleGlowPrefab, towerTop, Quaternion.identity);
glow.GetComponent<TowerIdleGlowConfig>().towerRenderer = towerMesh;

// Fire spell
VFXManager.Instance.PlayEffect("TowerSpell", firePoint.position);
VFXManager.Instance.PlayEffect("TowerMuzzle", firePoint.position, fireDirection);
```

---

**Version:** 1.0
**Last Updated:** 2026-01-16
**Compatibility:** Unity 2020.3+ URP

---

*Print this reference card for quick lookups during implementation!*
