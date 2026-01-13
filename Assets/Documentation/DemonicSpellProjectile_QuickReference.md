# Demonic Spell Projectile - Quick Reference Card
## Essential Values at a Glance

---

## Particle System Configuration

### Main Module
| Property | Value |
|----------|-------|
| Duration | 5.00s (continuous) |
| Looping | ✓ Enabled |
| Start Lifetime | 0.5s |
| Start Speed | 0 |
| Start Size | Random: 0.2 - 0.3 |
| Start Rotation | Random: 0 - 360° |
| Start Color | RGB(255, 69, 0) - OrangeRed |
| Gravity Modifier | 0 |
| Simulation Space | **World** (important!) |
| Max Particles | 20 |

### Emission Module
| Property | Value |
|----------|-------|
| Rate over Time | 15 particles/sec |

### Shape Module
| Property | Value |
|----------|-------|
| Shape Type | Sphere |
| Radius | 0.1 |
| Radius Thickness | 1 |
| Arc | 360° |

### Color over Lifetime (Gradient)
| Time | Color | Alpha |
|------|-------|-------|
| 0% | RGB(255, 69, 0) - OrangeRed | 255 (100%) |
| 40% | RGB(220, 20, 60) - Crimson | 255 (100%) |
| 70% | RGB(75, 0, 130) - Purple | 200 (78%) |
| 100% | RGB(75, 0, 130) - Purple | 0 (0%) |

### Size over Lifetime (Curve)
| Time | Size Multiplier |
|------|-----------------|
| 0.0 | 1.0 |
| 0.5 | 0.8 |
| 1.0 | 0.2 |

### Noise Module
| Property | Value |
|----------|-------|
| Enabled | ✓ Yes |
| Strength | 0.3 - 0.5 (curve) |
| Frequency | 1.5 |
| Scroll Speed | 0.5 |
| Octaves | 2 |

### Velocity over Lifetime
| Axis | Value |
|------|-------|
| Linear X/Y/Z | Random: -0.5 to 0.5 |
| Orbital X/Y/Z | Random: -1.0 to 1.0 |
| Space | World |

### Rotation over Lifetime
| Property | Value |
|----------|-------|
| Angular Velocity | Random: -90° to 90° |

---

## Trail Renderer

| Property | Value |
|----------|-------|
| Time | 0.3s |
| Min Vertex Distance | 0.1 |
| Width (Start → End) | 0.15 → 0.0 |
| Material | MAT_Trail_Red (Additive) |

### Trail Color Gradient
| Time | Color | Alpha |
|------|-------|-------|
| 0% | RGB(255, 69, 0) - Bright Red | 255 |
| 50% | RGB(220, 20, 60) - Crimson | 150 |
| 100% | RGB(75, 0, 130) - Purple | 0 |

---

## Point Light (Optional)

| Property | Value |
|----------|-------|
| Type | Point |
| Color | RGB(255, 69, 0) - Red-Orange |
| Intensity | 2.0 |
| Range | 3.0 |
| Shadows | None (performance) |

**Performance Note:** Disable for mobile low-end devices

---

## Materials

### MAT_Particle_Additive_Red
```
Shader: URP/Particles/Unlit
Surface: Transparent
Blend: Additive
Render Face: Both
Base Color: White (255, 255, 255)
Vertex Alpha: ✓ Enabled
GPU Instancing: ✓ Enabled
```

### MAT_Trail_Red
```
Shader: URP/Particles/Unlit
Surface: Transparent
Blend: Additive
Render Face: Both
Base Color: White (255, 255, 255)
Vertex Alpha: ✓ Enabled
```

---

## VFXLibrary Configuration

| Setting | Value |
|---------|-------|
| VFX Type | PlayerSpellProjectile |
| Priority | 0 (Critical) |
| Sync With Audio | ✓ Enabled |
| Initial Pool Size | 10 |
| Max Pool Size | 20 |
| Max Distance | 30 |

---

## Color Hex Codes

### Demonic Palette
```css
Start Color:   #FF4500  /* OrangeRed */
Mid Color:     #DC143C  /* Crimson */
End Color:     #4B0082  /* Indigo/Purple */
Light Color:   #FF4500  /* Red-Orange */
```

### RGB Values
```
Start: (255, 69, 0)
Mid:   (220, 20, 60)
End:   (75, 0, 130)
```

---

## Performance Targets

| Metric | Target | Current |
|--------|--------|---------|
| Max Particles per Effect | 20 | 20 ✓ |
| Emission Rate | 15/sec | 15 ✓ |
| Particle Lifetime | 0.5s | 0.5s ✓ |
| Trail Duration | 0.3s | 0.3s ✓ |
| Theoretical Max Particles | ~8 | Within budget ✓ |

**Budget Calculation:**
- Emission: 15 particles/sec
- Lifetime: 0.5s
- Max: 15 × 0.5 = 7.5 particles (rounded to 8)
- Cap: 20 particles (safety margin)

---

## Integration Code Snippets

### Spawn at Position
```csharp
using BaseDefender.VFX;

VFXHelper.PlayPlayerSpell(position, rotation);
```

### Attach to Projectile
```csharp
ParticleSystem vfx = VFXHelper.PlayEffectAttached(
    VFXType.PlayerSpellProjectile,
    projectileTransform
);
```

### Stop Effect
```csharp
VFXHelper.StopEffect(vfxEffect);
```

---

## Test Shortcuts

### Play Test (Add to any script)
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.T))
    {
        VFXHelper.PlayPlayerSpell(Camera.main.transform.position);
    }
}
```

### Validation Commands
```csharp
// In Unity Inspector, right-click DemonicSpellProjectileConfig:
- "Apply Configuration" - Auto-setup all values
- "Validate Settings" - Check current settings
- "Print Configuration" - Output all values to console
- "Show Performance Stats" - Check particle budget
```

---

## Common Issues Quick Fix

| Problem | Quick Fix |
|---------|-----------|
| No particles visible | Check material assigned, verify Start Color alpha |
| Particles don't follow | Set Simulation Space to **World** |
| No trail | Assign MAT_Trail_Red, set Time to 0.3 |
| Too slow performance | Reduce Emission to 12, disable Point Light |
| Particles stay at spawn | Ensure Inherit Velocity is **disabled** |

---

## File Locations

```
Prefab:     Assets/VFX/Prefabs/Player/FX_Player_DarkSpell.prefab
Materials:  Assets/VFX/Materials/MAT_Particle_Additive_Red.mat
            Assets/VFX/Materials/MAT_Trail_Red.mat
Textures:   Assets/VFX/Textures/TEX_Particle_Sphere_Soft.png
Library:    Assets/ScriptableObjects/VFX/VFXLibrary.asset
Config:     Assets/Scripts/VFX/DemonicSpellProjectileConfig.cs
```

---

## Module Checklist

Quick reference for which modules should be enabled:

```
✓ Main Module
✓ Emission Module
✓ Shape Module (Sphere, 0.1 radius)
✓ Velocity over Lifetime (Linear + Orbital)
✗ Limit Velocity over Lifetime (optional)
✗ Inherit Velocity
✓ Noise Module (Strength 0.4, Freq 1.5)
✓ Color over Lifetime (Red → Purple gradient)
✗ Color by Speed
✓ Size over Lifetime (1.0 → 0.2 curve)
✗ Size by Speed
✓ Rotation over Lifetime (±90°)
✗ Rotation by Speed
✗ External Forces
✗ Collision
✗ Triggers
✗ Sub Emitters
✗ Texture Sheet Animation
✗ Lights (use separate Point Light component)
✗ Trails (use separate Trail Renderer component)
✗ Custom Data
✓ Renderer (Billboard, Additive material)
```

---

## Quick Start Steps

1. Create GameObject: `FX_Player_DarkSpell`
2. Add ParticleSystem component
3. Add DemonicSpellProjectileConfig component
4. Click "Apply Configuration" in Inspector
5. Add TrailRenderer component
6. Create and assign materials
7. (Optional) Add Point Light child
8. Save as prefab in `Assets/VFX/Prefabs/Player/`
9. Assign to VFXLibrary
10. Test with `VFXHelper.PlayPlayerSpell()`

---

**Version:** 1.0
**Last Updated:** 2026-01-10
**Compatibility:** Unity 2020.3+ URP

*Print this page for quick reference during implementation!*
