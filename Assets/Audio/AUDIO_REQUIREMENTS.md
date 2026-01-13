# Audio Requirements - Base Defender MVP

This document outlines the audio files required for the game as specified in the GDD.

## Folder Structure

```
Audio/
├── Music/
│   └── gameplay_loop.wav (or .mp3/.ogg)
└── SFX/
    ├── player_shoot.wav
    ├── turret_shoot.wav
    ├── enemy_death.wav
    ├── coin_collect.wav
    ├── base_hit.wav
    ├── turret_activate.wav
    ├── wave_start.wav
    ├── victory.wav
    └── defeat.wav
```

## Music

### gameplay_loop
- **Location**: `Audio/Music/gameplay_loop.wav`
- **Type**: Background Music (looping)
- **Duration**: 30-60 seconds recommended
- **Format**: WAV, MP3, or OGG
- **Description**: Upbeat, action-oriented track that loops seamlessly during gameplay

## Sound Effects

### Player Sounds

#### player_shoot
- **Location**: `Audio/SFX/player_shoot.wav`
- **Type**: Sound Effect
- **Duration**: 0.1-0.3 seconds
- **Description**: Quick projectile firing sound, plays frequently (2 times per second)

### Turret Sounds

#### turret_shoot
- **Location**: `Audio/SFX/turret_shoot.wav`
- **Type**: Sound Effect
- **Duration**: 0.1-0.3 seconds
- **Description**: Turret firing sound, slightly different from player shoot

#### turret_activate
- **Location**: `Audio/SFX/turret_activate.wav`
- **Type**: Sound Effect
- **Duration**: 0.5-1.0 seconds
- **Description**: Power-up/activation sound when turret is purchased

### Enemy Sounds

#### enemy_death
- **Location**: `Audio/SFX/enemy_death.wav`
- **Type**: Sound Effect
- **Duration**: 0.3-0.5 seconds
- **Description**: Generic death sound for all enemy types

### Collectible Sounds

#### coin_collect
- **Location**: `Audio/SFX/coin_collect.wav`
- **Type**: Sound Effect
- **Duration**: 0.1-0.2 seconds
- **Description**: Pleasant coin collection sound, plays frequently

### Base Sounds

#### base_hit
- **Location**: `Audio/SFX/base_hit.wav`
- **Type**: Sound Effect
- **Duration**: 0.3-0.5 seconds
- **Description**: Impact sound when base takes damage

### Game Event Sounds

#### wave_start
- **Location**: `Audio/SFX/wave_start.wav`
- **Type**: Sound Effect
- **Duration**: 0.5-1.0 seconds
- **Description**: Alert/warning sound when new wave begins

#### victory
- **Location**: `Audio/SFX/victory.wav`
- **Type**: Sound Effect
- **Duration**: 2-4 seconds
- **Description**: Triumphant fanfare when player wins

#### defeat
- **Location**: `Audio/SFX/defeat.wav`
- **Type**: Sound Effect
- **Duration**: 2-3 seconds
- **Description**: Failure/game over sound

## Setup Instructions

1. **Import Audio Files**: Place audio files in the appropriate folders (Music or SFX)
2. **Create Sound Library Asset**:
   - Right-click in Project window
   - Select `Create > Base Defender > Sound Library`
   - Name it "GameSoundLibrary"
3. **Assign Audio Clips**:
   - Select the SoundLibrary asset
   - Drag and drop each audio file to its corresponding field in the Inspector
4. **Configure AudioManager**:
   - The AudioManager prefab/GameObject should reference the SoundLibrary asset
   - Adjust volume settings as needed (Master, Music, SFX)

## Audio Settings Recommendations

### Import Settings for Music
- Load Type: Streaming
- Compression Format: Vorbis (mobile) or MP3
- Quality: 70-100%
- Sample Rate: 44100 Hz

### Import Settings for Sound Effects
- Load Type: Decompress On Load (for short, frequently played sounds)
- Load Type: Compressed In Memory (for longer sounds like victory/defeat)
- Compression Format: PCM (for very short sounds) or ADPCM (mobile optimization)
- Force to Mono: Yes (for SFX without stereo requirements)
- Sample Rate: 22050-44100 Hz

## Testing Checklist

- [ ] Music loops seamlessly without gaps or clicks
- [ ] All sound effects play at appropriate volume levels
- [ ] No audio clipping or distortion
- [ ] Sound effects don't overlap excessively (especially rapid-fire sounds)
- [ ] Music volume is balanced with sound effects
- [ ] Victory/defeat sounds play completely before scene transition

## Notes

- All audio files are currently placeholders until actual assets are provided
- Audio system is designed to handle missing clips gracefully with warnings
- Volume controls are exposed in AudioManager for easy balancing
