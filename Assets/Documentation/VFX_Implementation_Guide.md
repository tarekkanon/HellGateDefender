# VFX Implementation Guide — Hell Gate Defender
## AI Prompt Reference for Visual Effects Development

---

## 📋 Document Purpose

This guide provides comprehensive prompts and specifications for implementing visual effects (VFX) in a medieval-themed, low-poly mobile game. Use these prompts with AI assistants, VFX artists, or as technical references for Unity particle system implementation.

---

## 🎮 Game Context Overview

### Theme & Setting
**Game Title:** Hell Gate Defender
**Art Style:** Low-poly, stylized medieval fantasy
**Setting:** Demonic hell castle under siege by angelic forces
**Visual Tone:** Dark fantasy with vibrant magical effects

### Core Gameplay Elements
- **Player:** Hell Gate Guard (demonic hero character)
- **Enemies:** Angels, celestial warriors, holy creatures
- **Defense Structures:** Evil towers powered by dark magic
- **Combat System:** Spell-based projectiles (both hero and towers)
- **Platform:** Mobile (iOS/Android) with Unity URP
- **Perspective:** 3D Isometric view

### VFX Importance
Visual effects are **CRITICAL** to gameplay feel. Since combat is spell-based and auto-firing, VFX must:
- Clearly communicate spell types and power
- Provide satisfying visual feedback
- Maintain performance on mobile devices
- Reinforce the demonic vs. angelic theme contrast

---

## 🎨 VFX Style Guidelines

### Color Palette

**Hell/Demonic Side (Player & Towers):**
```
Primary Colors:
- Crimson Red: #DC143C
- Hell Fire Orange: #FF4500
- Dark Purple: #4B0082
- Toxic Green: #32CD32
- Shadow Black: #1C1C1C

Secondary Accents:
- Ember Glow: #FF6347
- Soul Blue: #00CED1
- Bone White: #F5F5DC
```

**Angelic Side (Enemies):**
```
Primary Colors:
- Holy Gold: #FFD700
- Divine White: #FFFFFF
- Sky Blue: #87CEEB
- Light Yellow: #FFFFE0

Secondary Accents:
- Radiant Silver: #C0C0C0
- Blessed Cyan: #40E0D0
```

### Visual Characteristics

**Demonic Effects:**
- Chaotic, irregular particle movement
- Trailing smoke and embers
- Pulsing, unstable energy
- Sharp, aggressive shapes
- Heavy use of additive blending
- Fire, shadow, and corruption themes

**Angelic Effects:**
- Smooth, graceful particle arcs
- Radiant light rays and halos
- Stable, peaceful energy
- Soft, rounded shapes
- Glowing, luminous qualities
- Light, holy, and purification themes

---

## 🔥 VFX Asset Requirements

### Priority Levels
- **P0 (Critical):** Must have for MVP
- **P1 (High):** Strongly recommended
- **P2 (Polish):** Nice to have

---

## 🎯 VFX Catalog with AI Prompts

### 1. PLAYER HERO EFFECTS

#### 1.1 Dark Spell Projectile (P0)
**Description:** Main attack projectile fired by hell gate guard

**AI Prompt:**
```
Create a Unity particle system for a demonic spell projectile with these specs:

TECHNICAL REQUIREMENTS:
- Unity URP compatible
- Mobile-optimized (max 20 particles per projectile)
- Projectile speed: 15 units/second
- Lifetime: 1.5 seconds
- Trail effect attached to moving object

VISUAL STYLE:
- Low-poly aesthetic
- Color: Crimson red core with dark purple trailing edges
- Shape: Elongated sphere (0.3 units) with trailing particle tail
- Effect: Pulsing energy core with spiraling dark embers
- Additive shader for glow effect

PARTICLE SYSTEM MODULES:
- Main: Start size 0.2-0.3, start speed 0, lifetime 0.5s
- Emission: Continuous rate 15 particles/sec
- Shape: Sphere, radius 0.1
- Color over Lifetime: Red (255,69,0) → Purple (75,0,130) → Transparent
- Size over Lifetime: 1.0 → 0.2 (fade out)
- Velocity over Lifetime: Spiral/curl noise for chaotic movement

ADDITIONAL COMPONENTS:
- Trail Renderer: Width 0.15 → 0.0, gradient red to transparent, time 0.3s
- Point Light (optional): Red, intensity 2.0, range 3.0
```

#### 1.2 Muzzle Flash - Dark Casting (P0)
**Description:** Effect when hero fires a spell

**AI Prompt:**
```
Create a spell casting muzzle flash effect for a demonic spellcaster:

TECHNICAL REQUIREMENTS:
- One-shot particle burst
- Duration: 0.15 seconds
- Triggered on spell cast
- Position: Player's hand/staff position

VISUAL STYLE:
- Explosive burst of dark energy
- Color: Bright orange-red core with purple outer particles
- Shape: 3-5 particles radiating outward
- Size: 0.4-0.8 units

PARTICLE SYSTEM SETUP:
- Main Module:
  * Duration: 0.15s
  * Start Lifetime: 0.15s
  * Start Speed: 2-4 (random)
  * Start Size: 0.4-0.8
  * Start Color: Orange to Red gradient

- Emission Module:
  * Burst: 1 burst at time 0
  * Count: 3-5 particles

- Shape Module:
  * Cone shape, angle 30°, radius 0.2

- Color over Lifetime:
  * Bright orange → Dark red → Transparent

- Size over Lifetime:
  * 1.0 → 1.5 → 0 (quick expansion then fade)

OPTIONAL ENHANCEMENTS:
- Add rim light shader on hero hands during cast
- Screen space distortion (heat wave) for 0.1s
```

#### 1.3 Player Footstep Smoke (P2)
**Description:** Dark smoke trail when hero moves

**AI Prompt:**
```
Create subtle demonic footstep particle effect:

REQUIREMENTS:
- Continuous emission while moving
- Very low particle count (5-10 total on screen)
- Fades quickly to avoid clutter

VISUAL:
- Small puffs of dark smoke
- Color: Dark gray #2C2C2C with purple tint
- Size: 0.2-0.3 units
- Spawns at foot position every 0.3 seconds while moving

IMPLEMENTATION:
- Trigger from PlayerController when velocity > 0.1
- Particle lifetime: 0.8s
- Start speed: 0.5 upward, 0.2 random horizontal
- Alpha: 0.3 → 0.0
```

---

### 2. EVIL TOWER EFFECTS

#### 2.1 Inactive Tower - Dormant State (P1)
**Description:** Subtle effect showing tower is powered down but magical

**AI Prompt:**
```
Create ambient particle effect for inactive evil tower:

VISUAL CONCEPT:
- Occasional wisps of residual dark magic
- Very subtle to not distract from gameplay
- Communicates "activatable" state

TECHNICAL SPECS:
- Emission rate: 2-3 particles per second
- Particle lifetime: 2-3 seconds
- Size: 0.15-0.25 units
- Color: Faint purple #4B0082 with alpha 0.2-0.4

PARTICLE BEHAVIOR:
- Spawn at tower base
- Slow upward drift (speed 0.5 units/sec)
- Gentle random horizontal wander
- Fade in first 20% of life, fade out last 30%

SHADER:
- Unlit additive blend
- No texture needed (solid color spheres)
```

#### 2.2 Tower Activation Sequence (P0)
**Description:** Dramatic effect when player activates a tower with coins

**AI Prompt:**
```
Create epic tower activation effect for demonic defense tower:

NARRATIVE:
Player spends coins to awaken ancient evil magic in dormant tower

DURATION:
2.0 seconds total (plays once on activation)

SEQUENCE BREAKDOWN:

PHASE 1 (0.0 - 0.5s): Ground Eruption
- Particle burst from tower base
- 15-20 particles shooting upward
- Color: Dark red and purple mix
- Speed: 4-6 units/sec upward
- Size: 0.3-0.5 units

PHASE 2 (0.5 - 1.5s): Energy Spiral
- Continuous particles spiraling up tower
- Emission: 30 particles/sec
- Spiral shape using Velocity over Lifetime
- Color gradient: Red → Purple → Green (toxic)
- Creates helix pattern around tower body

PHASE 3 (1.5 - 2.0s): Power Surge
- Bright flash at tower top
- Expanding ring particle burst (8-10 particles)
- Size: 0.5 → 2.0 units (rapid expansion)
- Color: Bright green-white flash
- Additive blend for bloom effect

ADDITIONAL EFFECTS:
- Material swap on tower mesh: Add emissive glow material
- Emission color: Toxic green #32CD32, intensity 2.0
- Point light: Green, intensity 5.0, range 6.0, fade in over activation
- Audio cue trigger: Play "tower_activate.wav"
- Camera shake: Slight vibration (amplitude 0.1) during phase 3

OPTIMIZATION:
- Use object pooling - pre-instantiate effect at tower position
- Disable emission modules after completion, don't destroy
```

#### 2.3 Active Tower - Idle Glow (P0)
**Description:** Persistent effect showing tower is active and ready to fire

**AI Prompt:**
```
Create looping ambient effect for active evil tower:

PURPOSE:
- Clearly distinguish active from inactive towers
- Provide visual interest without distraction
- Minimal performance impact (always running)

VISUAL DESIGN:
- Pulsing glow on tower structure
- Occasional particle emissions
- Toxic green and dark purple color scheme

IMPLEMENTATION:

Component 1: Mesh Emissive Glow
- Material property animation (not particles)
- Emission color: #32CD32 (toxic green)
- Emission intensity: Sine wave 1.5 → 2.5 → 1.5
- Pulse frequency: 2 seconds per cycle
- Use MaterialPropertyBlock to avoid material instances

Component 2: Floating Energy Orbs
- Emission: 3-5 particles per second
- Lifetime: 2.0s
- Size: 0.1-0.2 units
- Spawn: Random points around tower top (radius 0.8)
- Movement: Slow orbit around tower crown
- Color: Alternating green and purple particles
- Alpha: 0 → 0.6 → 0 (fade in/out)

Component 3: Ground Runes (Optional P2)
- Decal projector at tower base
- Texture: Circular runic pattern (low-res 256x256)
- Pulsing UV offset animation
- Color: Faint green with additive blend

PERFORMANCE:
- Total particles from all active towers: < 40
- Use LOD: Disable Component 2 & 3 if camera distance > 15 units
```

#### 2.4 Tower Spell Projectile (P0)
**Description:** Magic missile fired by tower at angels

**AI Prompt:**
```
Create tower attack projectile - more powerful looking than player projectile:

CONCEPT:
Evil tower fires larger, more destructive spell bolts

TECHNICAL SPECS:
- Projectile speed: 12 units/second
- Lifetime: 2.0 seconds
- Size: Larger than player projectile (0.5 units core)

VISUAL DESIGN:
- Core: Bright toxic green sphere (#32CD32)
- Aura: Purple energy haze around core
- Trail: Thick particle trail with spiral effect

PARTICLE SYSTEM:

Main Projectile Sphere:
- Single large billboard sprite
- Size: 0.5 units
- Unlit shader with additive blend
- Color: Green with pulsing brightness (use animation curve)
- Attached to projectile GameObject

Particle Trail:
- Emission: 25 particles/second
- Lifetime: 0.8s
- Start size: 0.3 units
- Start speed: 0 (particles stay behind projectile)
- Color over lifetime: Bright green → Dark purple → Transparent
- Inherit velocity: 0% (particles remain in world space)
- Add curl noise for spiral effect

Visual Enhancement:
- Trail Renderer component
- Width: 0.25 → 0.0
- Color: Green gradient with glow
- Time: 0.4s

Lighting:
- Point light attached to projectile
- Color: Green
- Intensity: 3.0
- Range: 4.0
- Note: Disable if >4 projectiles on screen (performance)

COMPARISON TO PLAYER PROJECTILE:
- 60% larger visual size
- Brighter, more saturated colors
- Thicker, longer-lasting trail
- Should feel more powerful
```

#### 2.5 Tower Muzzle Flash (P0)
**Description:** Effect when tower fires a spell

**AI Prompt:**
```
Create spell firing effect for evil tower:

REQUIREMENTS:
- Brief flash at tower's firing point
- Duration: 0.2 seconds
- Bright enough to be visible but not overwhelming

VISUAL:
- Burst of 5-8 particles
- Color: Bright green core with purple sparks
- Size: 0.6-1.0 units (larger than player muzzle flash)
- Shape: Cone directed toward target

PARTICLE SETTINGS:
- Burst emission: 5-8 particles at time 0
- Lifetime: 0.2s
- Start speed: 3-5 units/sec
- Cone angle: 25°
- Color: Gradient from white-green → green → purple
- Size over lifetime: 1.0 → 1.3 → 0

ADDITIONAL:
- Flash light: Brief point light (0.15s) at firing point
  * Intensity: 4.0
  * Range: 5.0
  * Color: Green
```

---

### 3. IMPACT & HIT EFFECTS

#### 3.1 Spell Hit on Angel (P0)
**Description:** Impact effect when demonic spell hits angelic enemy

**AI Prompt:**
```
Create impact VFX for demonic spell hitting holy enemy:

NARRATIVE CONCEPT:
Dark magic corrupting/burning celestial being - should feel like opposing forces colliding

TECHNICAL REQUIREMENTS:
- One-shot effect triggered on projectile collision
- Duration: 0.5-0.8 seconds
- Position: Point of impact on enemy
- Must be visible against white/gold enemy colors

VISUAL DESIGN:

Impact Burst:
- 10-15 particles exploding outward from hit point
- Color mix: Red, orange, and black particles
- Size: 0.2-0.4 units
- Speed: 3-6 units/sec radial
- Lifetime: 0.4s
- Shape: Hemisphere facing outward from surface

Corruption Effect:
- Secondary particle system
- 5-8 smaller particles
- Color: Toxic green and purple
- Behavior: Brief upward spray then fall
- Lifetime: 0.6s
- Simulates "burning" the angel

Visual Flavor:
- Add small "X" shaped sprite particles (2-3)
- Color: Bright red
- Size: 0.3 units
- Rotation: Random
- Purpose: Comic-book style impact marker

PARTICLE SYSTEM MODULES:

System 1 - Main Burst:
- Emission: Burst of 10-15 at time 0
- Shape: Hemisphere, radius 0.2
- Start speed: 3-6
- Start size: 0.2-0.4
- Color over lifetime: Orange → Red → Black transparent
- Gravity modifier: 0.5 (slight fall)

System 2 - Corruption Sparks:
- Emission: Burst of 5-8 at time 0
- Shape: Cone, angle 45°, upward
- Start speed: 2-4
- Start size: 0.1-0.2
- Color: Alternating green/purple particles
- Gravity modifier: 2.0 (falls quickly)

OPTIONAL ENHANCEMENTS:
- Screen flash: Brief red tint on camera (0.1s, subtle)
- Enemy material flash: Set emission on enemy material for 0.15s
- Impact decal: Dark scorch mark on ground if enemy near floor
```

#### 3.2 Angel Projectile Hit on Player/Base (P0)
**Description:** Impact when holy attack hits demonic structures

**AI Prompt:**
```
Create holy impact effect - visual counterpoint to demonic hits:

CONCEPT:
Radiant light/purifying energy striking dark structures

VISUAL:
- Bright white/gold burst
- Smooth, clean particle shapes (spheres)
- Radiating light rays
- Color: Gold (#FFD700) → White → Transparent
- Size: 0.3-0.6 units
- Particles: 8-12 in radial pattern

PARTICLE SETUP:
- Burst emission: 8-12 particles
- Shape: Sphere burst
- Speed: 2-4 units/sec
- Lifetime: 0.4s
- Color: Bright gold fading to white then transparent
- Size: 0.3 → 0.5 → 0 (expand then fade)

LIGHT:
- Point light: Gold color, intensity 5.0, range 4.0
- Duration: 0.2s
- Gives bright flash on impact

CONTRAST NOTE:
This should feel opposite to demonic hits - clean, orderly, radiant vs chaotic, corrupted
```

#### 3.3 Angel Death Effect (P0)
**Description:** Angelic enemy defeated by dark magic

**AI Prompt:**
```
Create angel death/purification defeat effect:

NARRATIVE:
Angel is corrupted and vanquished - body dissolves into dark energy

DURATION: 1.0 second

VISUAL SEQUENCE:

Phase 1 (0.0-0.3s): Corruption Spread
- Small particles of dark energy spreading across angel body
- Emission: 15-20 particles
- Color: Red and purple
- Movement: Spread outward from death point
- Size: 0.15-0.25 units

Phase 2 (0.3-0.7s): Dissolution
- Angel model fades out (material alpha dissolve)
- Upward stream of mixed particles
- Color mix: White/gold (angel essence) + Red/purple (dark magic)
- Emission: 25 particles/second
- Creates visual of angel "burning away"

Phase 3 (0.7-1.0s): Soul Release
- Final burst upward
- 5-8 larger particles rising quickly
- Color: Fading white with purple edges
- Size: 0.4-0.6 units
- Speed: 5-8 units/sec upward
- Represents angel's essence being consumed

PARTICLE SYSTEMS:

System 1 - Corruption:
- Burst: 15-20 particles at time 0
- Shape: Sphere, radius 0.5
- Speed: 1-2 units/sec
- Color: Red/purple mix
- Lifetime: 0.3s

System 2 - Dissolution Stream:
- Emission: 25/sec for 0.4 seconds (total ~10 particles)
- Shape: Cone, narrow (15°), upward
- Speed: 2-4 units/sec
- Color: Gradient white → gold → purple
- Lifetime: 0.6s
- Size: 0.2-0.3 units

System 3 - Soul Burst:
- Burst: 5-8 particles at time 0.7s
- Shape: Cone upward
- Speed: 5-8 units/sec
- Color: White → Purple → Transparent
- Size: 0.4-0.6 units
- Lifetime: 0.3s

SYNCHRONIZATION:
- Trigger dissolve shader on angel mesh at 0.3s
- Full transparency by 0.8s
- Destroy GameObject at 1.0s
- Spawn coin at death position at 0.5s (during dissolution)

AUDIO:
- Play angel defeat sound with reverb
- Add dark magic "crackle" layer
```

---

### 4. COLLECTION & ECONOMY EFFECTS

#### 4.1 Soul Coin Idle (P1)
**Description:** Dropped currency floating/glowing on ground

**AI Prompt:**
```
Create collectible soul coin ambient effect:

CONCEPT:
Coins are corrupted angel souls - should feel magical and valuable

VISUAL:
- Coin mesh: Simple low-poly octagonal disc
- Base color: Dark purple with skull emblem
- Glow: Pulsing green/purple aura

PARTICLE SYSTEM:
- Emission: 2-3 particles per second
- Lifetime: 1.0s
- Size: 0.1-0.15 units
- Spawn: Around coin perimeter (torus shape)
- Movement: Gentle upward float + slow rotation around coin
- Color: Alternating green and purple
- Alpha: 0 → 0.5 → 0

MESH ANIMATION:
- Coin rotates slowly on Y axis (30° per second)
- Gentle hover bob animation (0.1 unit amplitude, 2s cycle)

GLOW:
- Emission on coin material
- Color: Toxic green #32CD32
- Intensity: Pulsing 1.0 → 2.0 → 1.0 (1.5s cycle)

PERFORMANCE:
- Disable particles if >20 coins on screen
- Keep mesh animation always (very cheap)
```

#### 4.2 Coin Collection Effect (P0)
**Description:** Visual feedback when player collects coin

**AI Prompt:**
```
Create satisfying coin collection VFX:

REQUIREMENTS:
- Clear visual confirmation of collection
- Duration: 0.5 seconds
- Feels rewarding and satisfying

VISUAL SEQUENCE:

Pre-Collection (Magnet Phase):
- Coin scales up slightly (1.0 → 1.2) as it flies to player
- Trail particles behind moving coin
- Color: Purple/green streak
- Emission: 10 particles/sec while moving

Collection Moment:
- Particle burst at player position
- 8-10 particles radiating outward then inward
- Color: Bright green flash → Purple
- Size: 0.2-0.4 units
- Pattern: Circle burst, then particles curve back to center

PARTICLE SYSTEMS:

Magnet Trail:
- Emission: 10 particles/sec (continuous during flight)
- Lifetime: 0.3s
- Size: 0.1-0.15 units
- Color: Purple → Transparent
- Speed: 0 (particles stay where spawned in world space)

Collection Burst:
- Burst: 8-10 particles at collection moment
- Speed: 3 units/sec outward
- Lifetime: 0.4s
- Color over lifetime: Bright green → Purple → Transparent
- Size: 0.2 → 0.4 → 0.1
- Custom velocity: Outward 50%, then curve inward using velocity over lifetime

ADDITIONAL FEEDBACK:
- Brief screen flash: Subtle green tint (0.1s)
- UI number popup: "+5" floating text in green
- Audio: Satisfying "ding" sound

FEEL:
Should feel like absorbing dark energy - quick, punchy, rewarding
```

---

### 5. ENVIRONMENTAL & AMBIENT EFFECTS

#### 5.1 Hell Gate Ambient Atmosphere (P1)
**Description:** Subtle environmental particles to set dark mood

**AI Prompt:**
```
Create atmospheric particle system for hell-themed battle arena:

PURPOSE:
- Establish dark fantasy atmosphere
- Not distracting - very subtle
- Fills empty space with visual interest

VISUAL CONCEPT:
- Floating embers and ash
- Occasional dark wisps
- Faint purple energy motes

PARTICLE SETTINGS:

System 1 - Floating Embers:
- Emission: 5 particles per second
- Lifetime: 8-12 seconds (very long)
- Spawn area: Box volume, 20x10x20 units (whole map)
- Size: 0.05-0.15 units (small)
- Color: Orange-red #FF4500 with low alpha (0.3-0.5)
- Movement: Slow upward drift (0.2 units/sec) + gentle horizontal wander
- Rotation: Slow random rotation

System 2 - Dark Wisps:
- Emission: 2-3 particles per second
- Lifetime: 6-8 seconds
- Spawn: Same volume as embers
- Size: 0.3-0.5 units
- Color: Dark purple #4B0082, alpha 0.15-0.25 (very faint)
- Movement: Meandering path using Curl Noise
- Texture: Wispy cloud sprite (optional)

System 3 - Energy Motes (near hell gate):
- Emission: 8-10 particles per second
- Lifetime: 3-5 seconds
- Spawn: Cylinder around base/gate structure
- Size: 0.08-0.12 units
- Color: Toxic green #32CD32, alpha 0.4
- Movement: Spiral upward around gate, use Orbit velocity

OPTIMIZATION:
- Max total particles: 60-80
- Spawn volume follows camera if using soft follow
- Render queue: Background (drawn first, obscured by gameplay)
- Use GPU instancing on material

PLACEMENT:
- Attach to "EnvironmentalVFX" empty GameObject in scene
- Position at map center
- Should run continuously from game start
```

#### 5.2 Base/Hell Gate Energy Field (P2)
**Description:** Protective aura around the hell castle base

**AI Prompt:**
```
Create magical barrier effect around base structure:

CONCEPT:
Dome-shaped energy shield protecting hell gate

VISUAL:
- Semi-transparent dome mesh (low-poly hemisphere)
- Pulsing red/purple energy
- Hexagonal pattern texture scrolling across surface
- Particles at base perimeter

SHADER IMPLEMENTATION:
Create Shader Graph (URP):
- Base: Fresnel effect (edges brighter than center)
- Color: Dark red with purple tint
- Emission: Pulsing intensity (1.0 → 2.0)
- Texture: Hexagon grid pattern, scrolling UV
- Alpha: 0.2-0.4 (mostly transparent)
- Additive blend mode

PARTICLE RING:
- Emission: 15-20 particles/sec
- Spawn: Circle at base of dome
- Movement: Rotate around base + gentle upward drift
- Color: Red and purple alternating
- Size: 0.2-0.3 units
- Lifetime: 2.0s

DAMAGED STATE:
- When base health < 50%: Cracks appear in dome
- Particle emission increases
- Color shifts more toward dark red
- Pulsing becomes erratic (randomized timing)

DESTROYED STATE:
- When base destroyed: Dome shatters
- Large particle burst (30-40 particles)
- Mesh scales down and fades
- Duration: 1.0s
```

#### 5.3 Spawn Portal Effect (P1)
**Description:** Effect when angels spawn into the battle

**AI Prompt:**
```
Create holy portal spawn effect for angel enemies:

CONCEPT:
Angelic enemies appear through heavenly portals - contrast with dark environment

DURATION: 1.5 seconds (portal opens, enemy spawns, portal closes)

SEQUENCE:

Phase 1 (0.0-0.5s): Portal Opening
- Vertical ring of light particles forming circle
- 12-16 particles in circle formation
- Color: Bright gold and white
- Size: 0.3 units
- Position: Form circle, radius 1.5 units
- Rotation: Slow spin around center

Phase 2 (0.5-1.0s): Portal Active
- Vertical disc of light (mesh or billboard)
- Diameter: 2.5 units
- Color: Bright white with gold edges
- Texture: Swirling light pattern with alpha
- Continuous particle emission from edges
- Enemy walks through center

Phase 3 (1.0-1.5s): Portal Closing
- Reverse of opening - particles collapse to center
- Light disc fades and shrinks
- Final flash when fully closed

PARTICLE SYSTEMS:

Opening Ring:
- 12-16 particles placed in circle
- Animated scale: 0 → 1.0 over 0.5s
- Color: Gold → White glow
- Rotation: 30°/second clockwise

Active Portal Particles:
- Emission: 20 particles/sec
- Spawn: Circle edge
- Movement: Inward spiral toward center
- Color: White with gold tint
- Size: 0.15-0.25 units
- Lifetime: 0.8s

MESH EFFECT:
- Vertical quad mesh with portal texture
- Shader: Unlit, additive, scrolling UV
- Scale animation: 0 → 2.5 → 0
- Alpha: 0 → 1.0 → 0

TIMING:
- Play audio at 0.0s (portal opening sound)
- Spawn enemy at 0.7s (mid-portal active phase)
- Complete close by 1.5s
```

---

### 6. UI & FEEDBACK EFFECTS

#### 6.1 Damage Number Popup (P1)
**Description:** Floating damage numbers above enemies

**AI Prompt:**
```
Create floating damage text VFX:

REQUIREMENTS:
- World-space UI text
- Shows damage dealt to enemy
- Rises and fades
- Duration: 0.8s

VISUAL:
- Font: Bold, outlined
- Color: Based on attack type
  * Player attacks: Red #DC143C
  * Tower attacks: Green #32CD32
  * Critical hits: Larger, orange #FF4500
- Size: Base 0.3 units, critical 0.5 units

ANIMATION:
- Spawn at hit position on enemy
- Rise upward 1.5 units over lifetime
- Slight random horizontal offset (-0.3 to +0.3)
- Scale: 1.0 → 1.3 → 0.8 (punch in, then shrink)
- Alpha: 0 → 1.0 (fade in 0.1s) → 1.0 → 0 (fade out 0.4s)
- Rotation: Slight random tilt (-10° to +10°)

IMPLEMENTATION:
- Use TextMeshPro with world space canvas
- Pool damage number prefabs
- Trigger on Enemy.TakeDamage()

OPTIONAL:
- Particle burst behind number (2-3 small particles matching color)
```

#### 6.2 Turret Activation Prompt Glow (P2)
**Description:** Visual indicator when near activatable tower

**AI Prompt:**
```
Create attention-drawing effect for tower activation UI:

WHEN ACTIVE:
- Player within 2 units of inactive tower

VISUAL:
- Glowing outline around tower
- Pulsing arrow or indicator above tower
- Subtle particle stream pointing to tower

OUTLINE SHADER:
- Rim light effect on tower mesh
- Color: Bright green #32CD32
- Width: 0.15 units
- Pulsing intensity: 1.0 → 2.0

INDICATOR:
- Floating icon above tower (down arrow or hand icon)
- Position: 2 units above tower
- Animation: Bob up and down (0.2 units, 1s cycle)
- Scale pulse: 1.0 → 1.1 → 1.0
- Color: Green with high emission

PARTICLES:
- Thin stream from tower to player (optional)
- 5 particles/sec
- Color: Faint green
- Movement: From tower toward player
- Lifetime: 0.5s
```

---

## 🔧 Technical Implementation Framework

### Unity Particle System Structure

```
Assets/VFX/
├── Prefabs/
│   ├── Player/
│   │   ├── FX_Player_DarkSpell.prefab
│   │   ├── FX_Player_MuzzleFlash.prefab
│   │   └── FX_Player_Footsteps.prefab
│   ├── Towers/
│   │   ├── FX_Tower_Inactive.prefab
│   │   ├── FX_Tower_Activation.prefab
│   │   ├── FX_Tower_IdleGlow.prefab
│   │   ├── FX_Tower_Spell.prefab
│   │   └── FX_Tower_MuzzleFlash.prefab
│   ├── Combat/
│   │   ├── FX_Impact_DemonicOnAngel.prefab
│   │   ├── FX_Impact_AngelicOnDark.prefab
│   │   ├── FX_Angel_Death.prefab
│   │   └── FX_DamageNumber.prefab
│   ├── Collection/
│   │   ├── FX_Coin_Idle.prefab
│   │   ├── FX_Coin_Collect.prefab
│   │   └── FX_Coin_Trail.prefab
│   ├── Environment/
│   │   ├── FX_Ambient_Embers.prefab
│   │   ├── FX_Base_Shield.prefab
│   │   └── FX_Spawn_Portal.prefab
│   └── UI/
│       ├── FX_TowerPrompt_Glow.prefab
│       └── FX_DamageNumber.prefab
├── Materials/
│   ├── MAT_Particle_Additive_Red.mat
│   ├── MAT_Particle_Additive_Green.mat
│   ├── MAT_Particle_Additive_Purple.mat
│   ├── MAT_Particle_Additive_Gold.mat
│   ├── MAT_Trail_Red.mat
│   ├── MAT_Trail_Green.mat
│   └── MAT_Shield_Dome.mat
├── Textures/
│   ├── TEX_Particle_Sphere_Soft.png (256x256)
│   ├── TEX_Particle_Wisp.png (256x256)
│   ├── TEX_Particle_Spark.png (128x128)
│   ├── TEX_Portal_Swirl.png (512x512)
│   └── TEX_Shield_Hex.png (512x512)
└── Shaders/
    ├── SHD_Shield_Dome.shadergraph
    └── SHD_Portal_Scroll.shadergraph
```

### VFX Manager Script Template

```csharp
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized VFX management system for Hell Gate Defender
/// Handles particle effect pooling, spawning, and optimization
/// </summary>
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("VFX Prefabs - Player")]
    [SerializeField] private ParticleSystem playerSpellPrefab;
    [SerializeField] private ParticleSystem playerMuzzleFlashPrefab;

    [Header("VFX Prefabs - Tower")]
    [SerializeField] private ParticleSystem towerActivationPrefab;
    [SerializeField] private ParticleSystem towerIdleGlowPrefab;
    [SerializeField] private ParticleSystem towerSpellPrefab;
    [SerializeField] private ParticleSystem towerMuzzleFlashPrefab;

    [Header("VFX Prefabs - Combat")]
    [SerializeField] private ParticleSystem demonicHitPrefab;
    [SerializeField] private ParticleSystem angelicHitPrefab;
    [SerializeField] private ParticleSystem angelDeathPrefab;

    [Header("VFX Prefabs - Collection")]
    [SerializeField] private ParticleSystem coinIdlePrefab;
    [SerializeField] private ParticleSystem coinCollectPrefab;

    [Header("VFX Prefabs - Environment")]
    [SerializeField] private ParticleSystem spawnPortalPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxParticlesOnScreen = 200;

    // Pool dictionaries
    private Dictionary<string, Queue<ParticleSystem>> effectPools;
    private Dictionary<ParticleSystem, string> activeEffects;

    // Performance tracking
    private int currentActiveParticles = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePools();
    }

    private void InitializePools()
    {
        effectPools = new Dictionary<string, Queue<ParticleSystem>>();
        activeEffects = new Dictionary<ParticleSystem, string>();

        // Initialize pools for each effect type
        CreatePool("PlayerSpell", playerSpellPrefab);
        CreatePool("PlayerMuzzle", playerMuzzleFlashPrefab);
        CreatePool("TowerActivation", towerActivationPrefab);
        CreatePool("TowerSpell", towerSpellPrefab);
        CreatePool("TowerMuzzle", towerMuzzleFlashPrefab);
        CreatePool("DemonicHit", demonicHitPrefab);
        CreatePool("AngelicHit", angelicHitPrefab);
        CreatePool("AngelDeath", angelDeathPrefab);
        CreatePool("CoinCollect", coinCollectPrefab);
        CreatePool("SpawnPortal", spawnPortalPrefab);
    }

    private void CreatePool(string poolName, ParticleSystem prefab)
    {
        if (prefab == null) return;

        Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            ParticleSystem ps = Instantiate(prefab, transform);
            ps.gameObject.SetActive(false);
            pool.Enqueue(ps);
        }

        effectPools[poolName] = pool;
    }

    /// <summary>
    /// Play VFX at specified position
    /// </summary>
    public ParticleSystem PlayEffect(string effectName, Vector3 position, Quaternion rotation = default)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            Debug.LogWarning($"VFX pool '{effectName}' not found!");
            return null;
        }

        // Check particle budget
        if (currentActiveParticles >= maxParticlesOnScreen)
        {
            Debug.LogWarning("Max particles on screen reached. Skipping effect.");
            return null;
        }

        Queue<ParticleSystem> pool = effectPools[effectName];
        ParticleSystem ps;

        if (pool.Count > 0)
        {
            ps = pool.Dequeue();
        }
        else
        {
            // Pool exhausted, create new instance
            Debug.LogWarning($"Pool '{effectName}' exhausted. Creating new instance.");
            ps = Instantiate(effectPools[effectName].Peek(), transform);
        }

        ps.transform.position = position;
        ps.transform.rotation = rotation == default ? Quaternion.identity : rotation;
        ps.gameObject.SetActive(true);
        ps.Play();

        activeEffects[ps] = effectName;
        currentActiveParticles += ps.particleCount;

        // Auto-return to pool after duration
        StartCoroutine(ReturnToPoolAfterDuration(ps, effectName));

        return ps;
    }

    /// <summary>
    /// Play VFX with color override
    /// </summary>
    public ParticleSystem PlayEffect(string effectName, Vector3 position, Color color)
    {
        ParticleSystem ps = PlayEffect(effectName, position);
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = color;
        }
        return ps;
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDuration(ParticleSystem ps, string poolName)
    {
        // Wait for particle system to finish
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);

        // Ensure all particles are dead
        while (ps.particleCount > 0)
        {
            yield return null;
        }

        ReturnToPool(ps, poolName);
    }

    private void ReturnToPool(ParticleSystem ps, string poolName)
    {
        if (!effectPools.ContainsKey(poolName)) return;

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
        ps.transform.SetParent(transform);

        effectPools[poolName].Enqueue(ps);
        activeEffects.Remove(ps);

        currentActiveParticles = Mathf.Max(0, currentActiveParticles - ps.particleCount);
    }

    /// <summary>
    /// Get current particle count for performance monitoring
    /// </summary>
    public int GetActiveParticleCount() => currentActiveParticles;

    /// <summary>
    /// Clear all active effects (e.g., on scene transition)
    /// </summary>
    public void ClearAllEffects()
    {
        foreach (var kvp in new Dictionary<ParticleSystem, string>(activeEffects))
        {
            ReturnToPool(kvp.Key, kvp.Value);
        }
    }
}
```

### Usage Examples in Game Scripts

```csharp
// In PlayerShooting.cs
private void Shoot()
{
    // Spawn muzzle flash at fire point
    VFXManager.Instance.PlayEffect("PlayerMuzzle", firePoint.position, firePoint.rotation);

    // Create projectile with trail effect
    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    VFXManager.Instance.PlayEffect("PlayerSpell", firePoint.position);
}

// In Enemy.cs
public void TakeDamage(int damage)
{
    currentHealth -= damage;

    // Show hit effect
    VFXManager.Instance.PlayEffect("DemonicHit", transform.position);

    if (currentHealth <= 0)
    {
        Die();
    }
}

private void Die()
{
    // Play death effect
    VFXManager.Instance.PlayEffect("AngelDeath", transform.position);

    // Spawn coin with idle effect
    SpawnCoin();
}

// In Turret.cs
public void Activate()
{
    isActive = true;

    // Play activation sequence
    VFXManager.Instance.PlayEffect("TowerActivation", transform.position);

    // Enable idle glow
    StartCoroutine(EnableIdleGlowAfterActivation());
}

private IEnumerator EnableIdleGlowAfterActivation()
{
    yield return new WaitForSeconds(2.0f); // Wait for activation effect

    // Spawn persistent idle glow (don't auto-pool this one)
    ParticleSystem idleGlow = Instantiate(towerIdleGlowPrefab, transform);
    idleGlow.Play();
}
```

---

## 📊 Performance Optimization Guidelines

### Mobile Performance Targets
```
TARGET METRICS:
- Total particles on screen: < 200
- Particle systems active: < 15
- Overdraw: < 2.5x per pixel
- Frame rate: Solid 60 FPS on mid-tier devices (iPhone 12, Galaxy S21)

OPTIMIZATION STRATEGIES:
1. Object Pooling: MANDATORY for all VFX
2. LOD System: Disable distant/off-screen effects
3. Particle Budgeting: Track and limit total particle count
4. Texture Atlasing: Combine all VFX textures into single atlas
5. Shader Simplification: Use unlit shaders, avoid complex operations
6. Emission Control: Scale emission based on device performance tier
```

### Effect Priority System

```csharp
public enum VFXPriority
{
    Critical = 0,   // Always play (player attacks, major hits)
    High = 1,       // Usually play (tower attacks, deaths)
    Medium = 2,     // Play if budget allows (ambient, trails)
    Low = 3         // Optional polish (dust, minor particles)
}

// In VFXManager, implement priority-based culling
public ParticleSystem PlayEffect(string effectName, Vector3 position, VFXPriority priority = VFXPriority.Medium)
{
    if (currentActiveParticles >= maxParticlesOnScreen && priority > VFXPriority.High)
    {
        return null; // Skip low-priority effects when over budget
    }

    // ... rest of PlayEffect logic
}
```

### Distance-Based LOD

```csharp
// Disable effects beyond certain distance from camera
private bool ShouldPlayEffect(Vector3 position, VFXPriority priority)
{
    float distanceToCamera = Vector3.Distance(Camera.main.transform.position, position);

    switch (priority)
    {
        case VFXPriority.Critical:
            return distanceToCamera < 30f;
        case VFXPriority.High:
            return distanceToCamera < 20f;
        case VFXPriority.Medium:
            return distanceToCamera < 15f;
        case VFXPriority.Low:
            return distanceToCamera < 10f;
        default:
            return true;
    }
}
```

---

## 🎨 Shader Graph Recommendations

### Basic Particle Shader (Additive)

```
CREATE IN SHADER GRAPH:

Inputs:
- Base Color (Color property, HDR enabled)
- Main Texture (Texture2D, default white)
- Fresnel Power (Float, range 0-5, default 1)

Graph Flow:
1. Sample Texture 2D (Main Texture + UV)
2. Multiply with Vertex Color
3. Multiply with Base Color
4. Fresnel Effect (optional, for sphere particles)
   - Fresnel → Power → Multiply with color
5. Output to Emission (not Base Color)

Blend Mode: Additive
Surface: Transparent
Render Face: Both
```

### Shield Dome Shader

```
SHADER GRAPH SETUP:

Properties:
- Shield Color (Color, HDR)
- Hexagon Texture (Texture2D)
- Scroll Speed (Vector2, default (0.1, 0.1))
- Fresnel Intensity (Float, range 0-10, default 3)
- Pulse Speed (Float, default 1)

Graph:
1. UV Scroll:
   - UV + (Time * Scroll Speed)

2. Hexagon Pattern:
   - Sample Texture (scrolled UV)

3. Fresnel Glow:
   - Fresnel Effect → Power
   - Multiply with Shield Color

4. Pulse Animation:
   - Sine(Time * Pulse Speed) → Remap 0-1
   - Multiply with emission intensity

5. Combine:
   - Hex Pattern + Fresnel
   - Multiply with Pulse
   - Output to Emission

Alpha: Fresnel result * 0.3
Blend: Additive or Alpha Blend
Cull: Off (visible from inside dome)
```

---

## 🔊 Audio-Visual Synchronization

### VFX-Audio Timing Guide

```
CRITICAL SYNC POINTS:

Player Spell Cast:
- Frame 0: Audio start + Muzzle flash spawn
- Frame 2-3: Projectile spawn with trail

Tower Activation:
- 0.0s: Audio "power up" start + Ground particles
- 0.5s: Whoosh sound layer + Spiral begins
- 1.5s: Impact sound + Flash burst
- 2.0s: Audio tail fadeout, VFX complete

Coin Collection:
- Frame 0: Ding sound + Particle burst
- Sync particle expansion with audio attack

Angel Death:
- 0.0s: Corruption sound (crackling)
- 0.3s: Angel cry (reverb) + Dissolution start
- 0.7s: Dark magic whoosh + Soul burst

IMPLEMENTATION:
Always trigger audio and VFX in same frame via unified function:
```

```csharp
public void PlayEffectWithAudio(string effectName, string audioClip, Vector3 position)
{
    VFXManager.Instance.PlayEffect(effectName, position);
    AudioManager.Instance.PlaySFX(audioClip, position);
}
```

---

## 📋 Testing & Iteration Checklist

### VFX Quality Assurance

```
FOR EACH EFFECT, VERIFY:

□ Visual Clarity:
  - Readable against all background colors?
  - Distinguishable from other effects?
  - Correct color theme (demonic vs angelic)?

□ Performance:
  - Particle count within budget?
  - Runs at 60 FPS on test device?
  - No excessive overdraw (check Frame Debugger)?

□ Timing:
  - Effect duration feels appropriate?
  - Doesn't linger too long or fade too quickly?
  - Synced with audio cues?

□ Gameplay Clarity:
  - Doesn't obscure important game elements?
  - Provides clear feedback for player actions?
  - Differentiates between player/tower/enemy effects?

□ Technical:
  - Properly pooled and recycled?
  - No memory leaks or orphaned particles?
  - Handles edge cases (spawn at world bounds, etc.)?

□ Aesthetic:
  - Matches low-poly art style?
  - Color palette consistent with theme?
  - Visual weight appropriate for effect importance?
```

### A/B Testing Recommendations

Test variations of key effects with playtesters:

1. **Projectile Trails**: Thick vs thin, long vs short lifetime
2. **Impact Intensity**: Subtle vs dramatic bursts
3. **Color Saturation**: Vibrant vs muted tones
4. **Ambient Density**: More vs fewer environmental particles

Metrics to track:
- Player ability to identify attack sources
- Visual clutter perception
- "Juiciness" / satisfaction rating
- Performance on target devices

---

## 🎓 Learning Resources

### Recommended Unity Learn Tutorials
- "Mobile VFX Optimization in URP"
- "Shader Graph Fundamentals"
- "Particle System Mastery"
- "Visual Effects Best Practices"

### External References
- Stylized VFX examples: Search "low poly spell effects" on ArtStation
- Mobile performance guides: Unity Mobile Optimization documentation
- Color theory: Study contrast between warm (demonic) and cool (angelic) palettes

---

## 📞 Support & Iteration

### Feedback Integration Process

When receiving VFX feedback:

1. **Document**: Note specific effect, timestamp, and issue
2. **Categorize**: Bug vs enhancement vs new request
3. **Prioritize**: Critical (blocks gameplay) → High (impacts feel) → Low (polish)
4. **Iterate**: Adjust particle values, don't rebuild from scratch
5. **Retest**: Verify fix doesn't break performance budget

### Version Control for VFX

```
NAMING CONVENTION:
FX_[Type]_[Name]_v[IterationNumber].prefab

Examples:
- FX_Player_DarkSpell_v1.prefab
- FX_Player_DarkSpell_v2.prefab (increased trail thickness)
- FX_Player_DarkSpell_v3.prefab (changed color to darker red)

Keep previous versions for A/B comparison
Delete old versions before final build
```

---

## 🚀 Expansion Roadmap (Post-MVP)

### Future VFX Features

**Tier 1 (First Update):**
- Weather effects (ash rain, red lightning)
- Enhanced base destruction sequence
- Player skill abilities with unique VFX

**Tier 2 (Major Update):**
- New tower types with distinct spell VFX
- Boss enemy with special attack effects
- Environmental hazards (lava geysers, dark rifts)

**Tier 3 (Polish Update):**
- Dynamic hit reactions (enemies show corruption spreading)
- Victory/defeat cinematic effects
- Meta-progression visual upgrades (shinier spells with upgrades)

---

## ✅ Final Implementation Checklist

Before considering VFX implementation complete:

```
CORE EFFECTS (MUST HAVE):
□ Player spell projectile + trail
□ Player muzzle flash
□ Tower activation sequence
□ Tower idle glow (active state)
□ Tower spell projectile + trail
□ Tower muzzle flash
□ Demonic hit impact on angels
□ Angel death effect
□ Coin idle glow
□ Coin collection burst

SECONDARY EFFECTS (SHOULD HAVE):
□ Angelic hit impact on base/player
□ Angel spawn portal
□ Base shield dome
□ Ambient embers/wisps
□ Damage number popups

OPTIMIZATION (REQUIRED):
□ VFXManager pooling system implemented
□ Performance budget enforced (<200 particles)
□ Mobile build tested on target devices
□ Frame rate stable at 60 FPS
□ No memory leaks during extended play

POLISH (NICE TO HAVE):
□ Audio-VFX synchronization verified
□ LOD system for distant effects
□ Turret activation prompt glow
□ Screen effects (flashes, shakes)
□ Particle lighting (point lights on key effects)
```

---

## 🎯 Summary: Quick Reference Prompts

### For AI Asset Generation

**Creating New Effect:**
```
I need a [effect type] VFX for a low-poly mobile game with medieval hell theme.

REQUIREMENTS:
- Platform: Unity URP mobile
- Style: Low-poly, stylized
- Color: [demonic: red/purple/green] OR [angelic: gold/white/blue]
- Particle budget: [number] particles
- Duration: [seconds]
- Purpose: [describe what it communicates to player]

TECHNICAL SPECS:
[Include specific numbers: size, speed, emission rate, lifetime]

Please provide:
1. Particle system module settings
2. Material/shader recommendations
3. Performance considerations
```

### For Implementation Support

```
I'm implementing [effect name] from the Hell Gate Defender VFX guide.

CONTEXT:
- Effect: [name and description]
- Issue: [what's not working or needs adjustment]
- Current settings: [paste particle system values]

GOAL:
- [What you want the effect to look like/do]

CONSTRAINTS:
- Mobile performance (must stay under particle budget)
- Must match demonic/angelic color palette
- Needs to be clearly visible in isometric view
```

---

**Document Version:** 1.0
**Last Updated:** 2026-01-10
**Maintained By:** Game Development Team

---

*This guide is a living document. Update as you iterate on VFX during development.*
