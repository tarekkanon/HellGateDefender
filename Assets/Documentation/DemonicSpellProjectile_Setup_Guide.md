# Demonic Spell Projectile - Unity Setup Guide
## Complete Implementation for Hell Gate Defender

---

## Overview

This guide provides step-by-step instructions to create the **Player Demonic Spell Projectile** VFX in Unity, fully integrated with your existing VFX system.

**Effect Type:** `VFXType.PlayerSpellProjectile`
**Visual Style:** Crimson red core with dark purple trailing edges, pulsing energy with spiraling dark embers
**Performance:** Mobile-optimized, max 20 particles
**Duration:** Attached to projectile (follows movement)

---

## Part 1: Create the Particle System Prefab

### Step 1: Create Base GameObject

1. In Unity Hierarchy, right-click and select **Create Empty**
2. Name it: `FX_Player_DarkSpell`
3. Reset Transform (Position: 0,0,0 | Rotation: 0,0,0 | Scale: 1,1,1)

### Step 2: Add Main Particle System

1. Right-click `FX_Player_DarkSpell` → **Effects** → **Particle System**
2. Rename the child to `MainParticles`
3. Configure the following modules:

---

## Part 2: Configure Particle System Modules

### Main Module Settings

**⚠️ CRITICAL: Looping MUST be enabled for projectile-attached VFX!**

```
Duration: 5.00 (continuous emission while attached)
Looping: ✓ Checked (CRITICAL - must be enabled!)
Start Delay: 0
Start Lifetime: 0.5
Start Speed: 0
3D Start Size: ✓ Checked
Start Size: Random Between Two Constants
  - Min: 0.2
  - Max: 0.3
3D Start Rotation: ✗ Unchecked
Start Rotation: Random Between Two Constants (0 to 360)
Start Color: Color (255, 69, 0) - OrangeRed RGB
Gravity Modifier: 0
Simulation Space: World
Simulation Speed: 1
Delta Time: Scaled
Scaling Mode: Hierarchy
Play On Awake: ✓ Checked
Emitter Velocity: Transform
Max Particles: 20
Auto Random Seed: ✓ Checked
Stop Action: Disable
Culling Mode: Automatic
Ring Buffer Mode: Disabled
```

### Emission Module

```
✓ Enabled
Rate over Time: 15
Rate over Distance: 0
Bursts: None
```

### Shape Module

```
✓ Enabled
Shape: Sphere
Radius: 0.1
Radius Thickness: 1
Arc: 360
Arc Mode: Random
Arc Spread: 0
Arc Speed: 1
Randomize Direction: 0
Spherize Direction: 0
Randomize Position: 0
Position: (0, 0, 0)
Rotation: (0, 0, 0)
Scale: (1, 1, 1)
Align To Direction: ✗ Unchecked
Texture: None
```

### Velocity over Lifetime Module

```
✓ Enabled
Linear:
  - X: Random Between Two Constants (-0.5 to 0.5)
  - Y: Random Between Two Constants (-0.5 to 0.5)
  - Z: Random Between Two Constants (-0.5 to 0.5)
Space: World

Orbital:
  - X: Random Between Two Constants (-1 to 1)
  - Y: Random Between Two Constants (-1 to 1)
  - Z: Random Between Two Constants (-1 to 1)

Offset:
  - X: 0
  - Y: 0
  - Z: 0

Radial: 0
Speed Modifier: Curve (starts at 1, ends at 0.5)
```

### Limit Velocity over Lifetime Module

```
✓ Enabled
Separate Axes: ✗ Unchecked
Speed: Constant (2)
Dampen: 0.5
Drag: 0
Multiply by Size: ✗ Unchecked
Multiply by Velocity: ✗ Unchecked
```

### Inherit Velocity Module

```
✗ Disabled (we want particles to stay in world space behind projectile)
```

### Noise Module (for spiral/chaotic movement)

```
✓ Enabled
Separate Axes: ✗ Unchecked
Strength: Curve (0.3 at start, 0.5 at end)
Frequency: 1.5
Scroll Speed: 0.5
Damping: ✓ Checked
Octaves: 2
Octave Multiplier: 0.5
Octave Scale: 2
Quality: High
Remap: (0 to 1)
Remap Curve: Default
Position Amount: 1
Rotation Amount: 0
Size Amount: 0
```

### Color over Lifetime Module

```
✓ Enabled
Color: Gradient
  - Time 0%: RGB(255, 69, 0) Alpha 255 - OrangeRed
  - Time 40%: RGB(220, 20, 60) Alpha 255 - Crimson
  - Time 70%: RGB(75, 0, 130) Alpha 200 - Indigo/Purple
  - Time 100%: RGB(75, 0, 130) Alpha 0 - Transparent Purple
```

**Gradient Setup:**
1. Click on the Color field
2. Add color stops by clicking below the gradient bar
3. Add alpha stops by clicking above the gradient bar
4. Set each stop's values as specified above

### Color by Speed Module

```
✗ Disabled
```

### Size over Lifetime Module

```
✓ Enabled
Separate Axes: ✗ Unchecked
Size: Curve
  - Time 0: Value 1.0
  - Time 0.5: Value 0.8
  - Time 1.0: Value 0.2
```

**Curve Setup:**
1. Click the curve dropdown
2. Select the downward curve preset or manually create it
3. Ensure it starts at 1.0 and ends at 0.2

### Size by Speed Module

```
✗ Disabled
```

### Rotation over Lifetime Module

```
✓ Enabled
Angular Velocity: Random Between Two Constants (-90 to 90)
```

### Rotation by Speed Module

```
✗ Disabled
```

### External Forces Module

```
✗ Disabled
```

### Collision Module

```
✗ Disabled
```

### Triggers Module

```
✗ Disabled
```

### Sub Emitters Module

```
✗ Disabled
```

### Texture Sheet Animation Module

```
✗ Disabled (we'll use simple material)
```

### Lights Module

```
✗ Disabled (we'll add point light separately)
```

### Trails Module

```
✗ Disabled (we'll use Trail Renderer component instead)
```

### Custom Data Module

```
✗ Disabled
```

### Renderer Module

```
✓ Enabled
Render Mode: Billboard
Material: MAT_Particle_Additive_Red (we'll create this next)
Trail Material: None
Sort Mode: By Distance
Sorting Fudge: 0
Min Particle Size: 0
Max Particle Size: 0.5
Render Alignment: View
Flip: (0, 0, 0)
Allow Roll: ✓ Checked
Pivot: (0, 0, 0)
Visualize Pivot: ✗ Unchecked
Masking: No Masking
Custom Vertex Streams: None
Cast Shadows: Off
Receive Shadows: ✗ Unchecked
Shadow Bias: 0
Motion Vectors: Camera Motion Only
Sorting Layer ID: Default
Order in Layer: 0
Light Probes: Off
Reflection Probes: Off
```

---

## Part 3: Create Materials

### Material 1: Particle Material (Additive Red)

1. Navigate to `Assets/VFX/Materials/`
2. Right-click → **Create** → **Material**
3. Name it: `MAT_Particle_Additive_Red`

**Material Settings:**
```
Shader: Universal Render Pipeline/Particles/Unlit
Render Face: Both
Blend Mode: Additive
Color Mode: Multiply
Surface Type: Transparent
Render Face: Both

Base Map: TEX_Particle_Sphere_Soft (create texture next)
Base Color: White (255, 255, 255, 255)

Alpha Clipping: ✗ Disabled
Vertex Alpha: ✓ Checked

Advanced:
  - GPU Instancing: ✓ Enabled
  - Double Sided Global Illumination: ✗ Disabled
```

### Texture: Soft Sphere Particle

If you don't have a soft sphere texture, you can:

**Option A: Use Unity's Default Particle**
1. In Material's Base Map, search for "Default-Particle"
2. Select the built-in Unity particle texture

**Option B: Create Custom Texture** (Recommended)
1. Use image editing software (Photoshop, GIMP, etc.)
2. Create 256x256 image
3. Black background
4. White radial gradient sphere in center (soft edges)
5. Save as PNG with transparency
6. Import to `Assets/VFX/Textures/`
7. Name it: `TEX_Particle_Sphere_Soft`
8. Import Settings:
   - Texture Type: Default
   - Alpha Source: From Gray Scale
   - Alpha Is Transparency: ✓ Checked
   - Wrap Mode: Clamp
   - Filter Mode: Bilinear
   - Max Size: 256
   - Compression: Normal Quality

---

## Part 4: Add Trail Renderer Component

1. Select `FX_Player_DarkSpell` GameObject (root)
2. Click **Add Component** → **Effects** → **Trail Renderer**

**Trail Renderer Settings:**
```
Cast Shadows: Off
Receive Shadows: ✗ Unchecked
Motion Vectors: Camera Motion Only
Dynamic Occlusion: ✓ Enabled
Materials:
  - Element 0: MAT_Trail_Red (create this material)

Time: 0.3
Min Vertex Distance: 0.1
Autodestruct: ✗ Unchecked
Emitting: ✓ Checked

Width:
  - Curve: Linear decrease
  - Start: 0.15
  - End: 0.0

Color:
  - Gradient:
    - Time 0%: RGB(255, 69, 0) Alpha 255 - Bright Red
    - Time 50%: RGB(220, 20, 60) Alpha 150 - Crimson
    - Time 100%: RGB(75, 0, 130) Alpha 0 - Transparent Purple

Corner Vertices: 4
End Cap Vertices: 4
Alignment: View
Texture Mode: Stretch
Shadow Bias: 0.5
Generate Lighting Data: ✗ Unchecked
```

### Material 2: Trail Material

1. Navigate to `Assets/VFX/Materials/`
2. Right-click → **Create** → **Material**
3. Name it: `MAT_Trail_Red`

**Material Settings:**
```
Shader: Universal Render Pipeline/Particles/Unlit
Surface Type: Transparent
Blending Mode: Additive
Render Face: Both

Base Map: None (solid color trail)
Base Color: White (255, 255, 255, 255)

Alpha Clipping: ✗ Disabled
Vertex Alpha: ✓ Checked

Advanced:
  - GPU Instancing: ✓ Enabled
```

---

## Part 5: Add Optional Point Light

1. Right-click `FX_Player_DarkSpell` → **Light** → **Point Light**
2. Rename to `ProjectileLight`

**Point Light Settings:**
```
Type: Point
Mode: Realtime
Intensity: 2.0
Indirect Multiplier: 1
Range: 3.0
Color: RGB(255, 69, 0) - Red-Orange

Rendering:
  - Render Mode: Auto
  - Culling Mask: Everything

Shadows:
  - Shadow Type: No Shadows (performance)
```

**Performance Note:** This light is optional. For best mobile performance:
- Disable lights when more than 4 projectiles are on screen
- Or remove completely for low-end devices

---

## Part 6: Add VFXController Component

1. Select `FX_Player_DarkSpell` GameObject (root)
2. Click **Add Component** → Type "VFXController"
3. Select **BaseDefender.VFX.VFXController**

**VFXController Settings:**
```
VFX Settings:
  - VFX Type: PlayerSpellProjectile
  - Auto Destroy: ✗ Unchecked (managed by pool)
  - Custom Lifetime: 0

Audio:
  - Play Audio On Start: ✗ Unchecked (handled by VFXHelper)
  - Audio Clip: None
  - Audio Volume: 1

Effects:
  - Apply Screen Shake: ✗ Unchecked
  - Shake Intensity: 0
  - Shake Duration: 0

Callbacks:
  - Enable Callbacks: ✗ Unchecked
```

---

## Part 7: Save as Prefab

1. Drag `FX_Player_DarkSpell` from Hierarchy to `Assets/VFX/Prefabs/Player/`
2. Confirm prefab creation
3. Delete the instance from the Hierarchy (VFXManager will spawn from pool)

---

## Part 8: Configure VFXLibrary

1. Open `Assets/ScriptableObjects/VFX/VFXLibrary.asset` in Inspector
2. Find the **Player Effects** section
3. Locate the entry for `PlayerSpellProjectile`

**VFXData Configuration:**
```
Identification:
  - VFX Type: PlayerSpellProjectile

VFX Settings:
  - Prefab: FX_Player_DarkSpell (assign your prefab)
  - Priority: 0 (Critical - always plays)
  - Sync With Audio: ✓ Checked

Audio Sync Settings:
  - Audio Clip: (assign player shoot sound if available)
  - Audio Volume: 0.8
  - Audio Delay: 0

Pool Settings:
  - Initial Pool Size: 10
  - Max Pool Size: 20

Performance:
  - Max Distance: 30
  - Scale With Performance: ✗ Unchecked
```

---

## Part 9: Integration with Projectile System

### Option A: Attach to Existing Projectile Prefab

If you already have a projectile GameObject:

1. Open your projectile prefab (e.g., `Assets/Prefabs/Projectiles/Projectile.prefab`)
2. Add this code to your projectile script:

```csharp
using BaseDefender.VFX;

public class Projectile : MonoBehaviour
{
    private ParticleSystem vfxEffect;

    private void OnEnable()
    {
        // Play VFX attached to this projectile
        vfxEffect = VFXHelper.PlayEffectAttached(
            VFXType.PlayerSpellProjectile,
            transform,
            Vector3.zero
        );
    }

    private void OnDisable()
    {
        // Stop VFX when projectile is disabled/pooled
        if (vfxEffect != null)
        {
            VFXHelper.StopEffect(vfxEffect);
        }
    }
}
```

### Option B: Spawn at Fire Point

If you spawn VFX at the fire point instead:

```csharp
using BaseDefender.VFX;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Transform firePoint;

    private void Shoot()
    {
        // Play muzzle flash
        VFXHelper.PlayPlayerMuzzleFlash(firePoint.position, firePoint.rotation);

        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Attach trail VFX to projectile
        VFXHelper.PlayEffectAttached(VFXType.PlayerSpellProjectile, projectile.transform);
    }
}
```

---

## Part 10: Testing & Validation

### Test Checklist

1. **VFX System Initialized**
   - Ensure VFXManager GameObject exists in scene
   - VFXLibrary is assigned in VFXManager
   - FX_Player_DarkSpell prefab assigned in VFXLibrary

2. **Play Mode Test**
   ```csharp
   // Add this to any test script
   void Update()
   {
       if (Input.GetKeyDown(KeyCode.Space))
       {
           // Test spawn at camera position
           VFXHelper.PlayPlayerSpell(Camera.main.transform.position);
       }
   }
   ```

3. **Visual Verification**
   - ✓ Crimson red core particles spawning
   - ✓ Particles transition red → purple → transparent
   - ✓ Particles spiral chaotically
   - ✓ Trail fades from red to transparent
   - ✓ Size decreases over lifetime
   - ✓ Maximum ~20 particles visible

4. **Performance Check**
   - Open Unity Profiler (Window → Analysis → Profiler)
   - Check Rendering tab
   - Verify particle count stays under budget
   - Test with multiple projectiles on screen

5. **Pool Test**
   - Fire multiple projectiles rapidly
   - Check Console for "Pool exhausted" warnings
   - Adjust Initial Pool Size if needed

---

## Part 11: Troubleshooting

### Particles Not Visible

**Issue:** Effect spawns but no particles appear

**Solutions:**
1. Check material is assigned in Renderer module
2. Verify material shader is URP/Particles/Unlit
3. Ensure Render Mode is set to Billboard
4. Check Start Color alpha is not 0
5. Verify Max Particles is not 0

### Particles Don't Follow Projectile

**Issue:** Particles stay at spawn point

**Solutions:**
1. Ensure Simulation Space is set to **World** (not Local)
2. Check Inherit Velocity module is **disabled**
3. Verify effect is attached with `PlayEffectAttached()`

### Trail Not Showing

**Issue:** No trail behind projectile

**Solutions:**
1. Check Trail Renderer component is enabled
2. Verify MAT_Trail_Red material is assigned
3. Ensure Time > 0 (0.3 recommended)
4. Check Min Vertex Distance is not too high

### Too Many Particles / Performance Issues

**Issue:** Frame rate drops with multiple projectiles

**Solutions:**
1. Reduce Emission Rate (try 12 instead of 15)
2. Lower Max Particles to 15
3. Disable Point Light component
4. Reduce Initial Pool Size in VFXLibrary
5. Increase priority to allow culling

### Effect Doesn't Stop When Projectile Disabled

**Issue:** VFX continues playing after projectile destroyed

**Solutions:**
1. Ensure you're calling `VFXHelper.StopEffect(vfxEffect)` in OnDisable
2. Check VFXManager is properly managing pools
3. Verify effect is not set to Loop indefinitely (it should be, but controlled by attachment)

### Audio Not Playing

**Issue:** No sound with spell effect

**Solutions:**
1. Check AudioManager exists in scene
2. Verify "Sync With Audio" is checked in VFXLibrary
3. Ensure audio clip is assigned in VFXData
4. Check VFXManager has "Enable Audio Sync" enabled
5. Verify AudioManager has player shoot sound configured

---

## Part 12: Optimization Tips

### Mobile Performance Guidelines

**Target Specs:**
- 60 FPS on iPhone 12 / Galaxy S21
- Max 20 particles per effect
- Max 5 projectiles on screen simultaneously
- Total particle budget: 200 particles

**Optimization Checklist:**

1. **Particle Count**
   - Keep emission rate at 15 or lower
   - Use Max Particles: 20 cap
   - Short particle lifetime (0.5s)

2. **Rendering**
   - Use simple unlit additive shader
   - No texture (or simple soft circle)
   - Disable shadows
   - Disable receive shadows

3. **Physics**
   - No collision detection
   - No external forces
   - Minimal noise computation

4. **Lighting**
   - Disable point light for low-end devices
   - Or limit to 2 lights max on screen

5. **Pooling**
   - Use VFXManager pools (never Instantiate)
   - Initial Pool: 10
   - Max Pool: 20
   - Auto-returns to pool

### Quality Settings Tiers

**High Quality (Desktop/High-End Mobile):**
- Point Light: Enabled
- Emission Rate: 15
- Trail Width: 0.15

**Medium Quality (Mid-Range Mobile):**
- Point Light: Enabled (limit 2 on screen)
- Emission Rate: 12
- Trail Width: 0.12

**Low Quality (Low-End Mobile):**
- Point Light: Disabled
- Emission Rate: 10
- Trail Width: 0.10

---

## Part 13: Variations & Enhancements

### Color Variations

For different spell types, create material variants:

**Dark Purple Spell:**
```
Start Color: RGB(75, 0, 130) - Purple
Color over Lifetime: Purple → Dark Red → Black
Trail Color: Purple gradient
```

**Toxic Green Spell (Tower):**
```
Start Color: RGB(50, 205, 50) - Lime Green
Color over Lifetime: Bright Green → Dark Purple → Transparent
Trail Color: Green to purple gradient
Point Light: Green
```

### Enhanced Effects (Optional)

**Add Core Sphere:**
1. Add child ParticleSystem: `CoreGlow`
2. Emission: 1 particle (continuous)
3. Start Size: 0.4
4. Color: Bright white-red
5. Render: Additive, higher intensity

**Add Ember Sparks:**
1. Add child ParticleSystem: `Embers`
2. Emission: 3 particles/sec
3. Start Size: 0.05-0.1
4. Color: Orange-red
5. Gravity: 0.5 (falls slightly)
6. Adds chaotic, fiery feel

---

## Summary

You've created a mobile-optimized demonic spell projectile VFX with:

✅ Crimson red to dark purple color transition
✅ Spiral/chaotic particle movement
✅ Smooth trailing effect
✅ Mobile performance (max 20 particles)
✅ Full integration with VFX pooling system
✅ Audio synchronization ready
✅ Low-poly aesthetic matching game style

**Next Steps:**
1. Create similar effects for towers (green variation)
2. Add muzzle flash effects
3. Create impact hit effects
4. Test with full gameplay loop

**Quick Play Test:**
```csharp
// Test in any script Update()
if (Input.GetKeyDown(KeyCode.T))
{
    VFXHelper.PlayPlayerSpell(Camera.main.transform.position);
    Debug.Log("Spawned demonic spell VFX!");
}
```

---

**Document Version:** 1.0
**Created:** 2026-01-10
**For:** Hell Gate Defender - Knight Adventure
**VFX System Version:** 1.0

---

*For questions or issues, refer to:*
- `VFX_Implementation_Guide.md` - Comprehensive VFX specifications
- `README_VFX_SYSTEM.md` - VFX system usage guide
