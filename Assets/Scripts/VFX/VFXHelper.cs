using UnityEngine;
using BaseDefender.Core;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Helper class providing convenient methods to play VFX with audio synchronization.
    /// Simplifies integration with AudioManager and provides unified VFX+Audio playback.
    /// </summary>
    public static class VFXHelper
    {
        #region Player Effects

        /// <summary>
        /// Play player spell projectile effect with audio
        /// </summary>
        public static ParticleSystem PlayPlayerSpell(Vector3 position, Quaternion rotation = default)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.PlayerSpellProjectile,
                position,
                () => AudioManager.Instance?.PlayPlayerShoot()
            );
        }

        /// <summary>
        /// Play player muzzle flash effect with audio
        /// </summary>
        public static ParticleSystem PlayPlayerMuzzleFlash(Vector3 position, Quaternion rotation = default)
        {
            var ps = VFXManager.Instance?.PlayEffect(VFXType.PlayerMuzzleFlash, position, rotation);
            AudioManager.Instance?.PlayPlayerShoot();
            return ps;
        }

        #endregion

        #region Tower Effects

        /// <summary>
        /// Play tower activation sequence with audio
        /// </summary>
        public static ParticleSystem PlayTowerActivation(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.TowerActivation,
                position,
                () => AudioManager.Instance?.PlayTurretActivate()
            );
        }

        /// <summary>
        /// Play tower spell projectile effect with audio
        /// </summary>
        public static ParticleSystem PlayTowerSpell(Vector3 position, Quaternion rotation = default)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.TowerSpellProjectile,
                position,
                () => AudioManager.Instance?.PlayTurretShoot()
            );
        }

        /// <summary>
        /// Play tower muzzle flash effect with audio
        /// </summary>
        public static ParticleSystem PlayTowerMuzzleFlash(Vector3 position, Quaternion rotation = default)
        {
            var ps = VFXManager.Instance?.PlayEffect(VFXType.TowerMuzzleFlash, position, rotation);
            AudioManager.Instance?.PlayTurretShoot();
            return ps;
        }

        /// <summary>
        /// Play tower idle glow effect (looping, attach to tower)
        /// </summary>
        public static ParticleSystem PlayTowerIdleGlow(Transform towerTransform)
        {
            return VFXManager.Instance?.PlayEffectAttached(VFXType.TowerIdleGlow, towerTransform);
        }

        #endregion

        #region Combat Effects

        /// <summary>
        /// Play demonic hit on angel effect (impact burst + corruption sparks + markers)
        /// </summary>
        /// <param name="position">World position of the impact</param>
        public static ParticleSystem PlayDemonicHit(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffect(VFXType.DemonicHitOnAngel, position);
        }

        /// <summary>
        /// Play demonic hit on angel effect facing a specific direction
        /// </summary>
        /// <param name="position">World position of the impact</param>
        /// <param name="impactNormal">Normal direction of the impact surface</param>
        public static ParticleSystem PlayDemonicHit(Vector3 position, Vector3 impactNormal)
        {
            Quaternion rotation = Quaternion.LookRotation(impactNormal);
            return VFXManager.Instance?.PlayEffect(VFXType.DemonicHitOnAngel, position, rotation);
        }

        /// <summary>
        /// Play angelic hit on demonic effect with audio (radiant burst + light flash)
        /// </summary>
        /// <param name="position">World position of the impact</param>
        public static ParticleSystem PlayAngelicHit(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.AngelicHitOnDemonic,
                position,
                () => AudioManager.Instance?.PlayBaseHit()
            );
        }

        /// <summary>
        /// Play angelic hit on demonic effect facing a specific direction
        /// </summary>
        /// <param name="position">World position of the impact</param>
        /// <param name="impactNormal">Normal direction of the impact surface</param>
        public static ParticleSystem PlayAngelicHit(Vector3 position, Vector3 impactNormal)
        {
            Quaternion rotation = Quaternion.LookRotation(impactNormal);
            var ps = VFXManager.Instance?.PlayEffect(VFXType.AngelicHitOnDemonic, position, rotation);
            AudioManager.Instance?.PlayBaseHit();
            return ps;
        }

        /// <summary>
        /// Play angel death effect with audio (3-phase: corruption spread → dissolution → soul release)
        /// </summary>
        /// <param name="position">World position of the dying angel</param>
        public static ParticleSystem PlayAngelDeath(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.AngelDeath,
                position,
                () => AudioManager.Instance?.PlayEnemyDeath()
            );
        }

        /// <summary>
        /// Play the full angel death sequence with config control
        /// Allows manual triggering of the 3-phase death effect
        /// </summary>
        /// <param name="position">World position of the dying angel</param>
        /// <returns>The particle system with AngelDeathConfig for additional control</returns>
        public static ParticleSystem PlayAngelDeathSequence(Vector3 position)
        {
            ParticleSystem ps = VFXManager.Instance?.PlayEffect(VFXType.AngelDeath, position);

            if (ps != null)
            {
                AngelDeathConfig config = ps.GetComponent<AngelDeathConfig>();
                if (config != null)
                {
                    config.PlayDeathSequence();
                }
                AudioManager.Instance?.PlayEnemyDeath();
            }

            return ps;
        }

        #endregion

        #region Collection Effects

        /// <summary>
        /// Play coin collection effect with audio
        /// </summary>
        public static ParticleSystem PlayCoinCollect(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.CoinCollect,
                position,
                () => AudioManager.Instance?.PlayCoinCollect()
            );
        }

        /// <summary>
        /// Play coin idle glow effect (attach to coin)
        /// </summary>
        public static ParticleSystem PlayCoinIdle(Transform coinTransform)
        {
            return VFXManager.Instance?.PlayEffectAttached(VFXType.CoinIdle, coinTransform);
        }

        /// <summary>
        /// Play coin magnet trail effect (attach to moving coin)
        /// </summary>
        public static ParticleSystem PlayCoinMagnetTrail(Transform coinTransform)
        {
            return VFXManager.Instance?.PlayEffectAttached(VFXType.CoinMagnetTrail, coinTransform);
        }

        #endregion

        #region Environment Effects

        /// <summary>
        /// Play spawn portal effect with enemy spawn callback
        /// </summary>
        /// <param name="position">World position for the portal</param>
        /// <param name="onEnemySpawn">Callback invoked when enemy should spawn (at 0.7s)</param>
        /// <param name="onComplete">Callback invoked when portal sequence completes</param>
        public static ParticleSystem PlaySpawnPortal(Vector3 position, System.Action onEnemySpawn = null, System.Action onComplete = null)
        {
            ParticleSystem ps = VFXManager.Instance?.PlayEffect(VFXType.SpawnPortal, position);

            if (ps != null && (onEnemySpawn != null || onComplete != null))
            {
                SpawnPortalConfig config = ps.GetComponent<SpawnPortalConfig>();
                if (config != null)
                {
                    if (onEnemySpawn != null)
                    {
                        config.OnEnemySpawnTime += onEnemySpawn;
                    }
                    if (onComplete != null)
                    {
                        config.OnPortalComplete += onComplete;
                    }
                    config.PlayPortalSequence();
                }
            }

            return ps;
        }

        /// <summary>
        /// Play spawn portal effect at position (simple version without callbacks)
        /// </summary>
        public static ParticleSystem PlaySpawnPortalSimple(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffect(VFXType.SpawnPortal, position);
        }

        /// <summary>
        /// Play ambient atmosphere effect (typically one instance in scene)
        /// Includes floating embers, dark wisps, and energy motes
        /// </summary>
        public static ParticleSystem PlayAmbientAtmosphere(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffect(VFXType.AmbientAtmosphere, position);
        }

        /// <summary>
        /// Play ambient atmosphere effect with custom spawn volume
        /// </summary>
        /// <param name="position">Center position for the effect</param>
        /// <param name="spawnVolumeSize">Size of the spawn area (width, height, depth)</param>
        public static ParticleSystem PlayAmbientAtmosphere(Vector3 position, Vector3 spawnVolumeSize)
        {
            ParticleSystem ps = VFXManager.Instance?.PlayEffect(VFXType.AmbientAtmosphere, position);

            if (ps != null)
            {
                AmbientAtmosphereConfig config = ps.GetComponent<AmbientAtmosphereConfig>();
                if (config != null)
                {
                    config.SetSpawnVolume(spawnVolumeSize);
                }
            }

            return ps;
        }

        /// <summary>
        /// Play base shield effect (attach to base)
        /// </summary>
        public static ParticleSystem PlayBaseShield(Transform baseTransform)
        {
            return VFXManager.Instance?.PlayEffectAttached(VFXType.BaseShield, baseTransform);
        }

        /// <summary>
        /// Play base shield effect with custom radius
        /// </summary>
        /// <param name="baseTransform">Transform to attach shield to</param>
        /// <param name="radius">Radius of the shield dome</param>
        public static ParticleSystem PlayBaseShield(Transform baseTransform, float radius)
        {
            ParticleSystem ps = VFXManager.Instance?.PlayEffectAttached(VFXType.BaseShield, baseTransform);

            if (ps != null)
            {
                BaseShieldConfig config = ps.GetComponent<BaseShieldConfig>();
                if (config != null)
                {
                    config.SetShieldRadius(radius);
                }
            }

            return ps;
        }

        /// <summary>
        /// Set base shield to damaged state (flickering, increased particles)
        /// Call this when base health falls below 50%
        /// </summary>
        public static void SetBaseShieldDamaged(ParticleSystem shieldEffect)
        {
            if (shieldEffect == null) return;

            BaseShieldConfig config = shieldEffect.GetComponent<BaseShieldConfig>();
            if (config != null)
            {
                config.SetDamagedState();
            }
        }

        /// <summary>
        /// Play base shield destruction effect
        /// Call this when base is destroyed
        /// </summary>
        public static void DestroyBaseShield(ParticleSystem shieldEffect)
        {
            if (shieldEffect == null) return;

            BaseShieldConfig config = shieldEffect.GetComponent<BaseShieldConfig>();
            if (config != null)
            {
                config.DestroyShield();
            }
        }

        /// <summary>
        /// Get the enemy spawn time for a spawn portal (for synchronization)
        /// </summary>
        public static float GetSpawnPortalEnemySpawnTime(ParticleSystem portalEffect)
        {
            if (portalEffect == null) return 0.7f; // Default

            SpawnPortalConfig config = portalEffect.GetComponent<SpawnPortalConfig>();
            return config != null ? config.GetEnemySpawnTime() : 0.7f;
        }

        #endregion

        #region Generic Helpers

        /// <summary>
        /// Play any VFX effect at a position
        /// </summary>
        public static ParticleSystem PlayEffect(VFXType vfxType, Vector3 position, Quaternion rotation = default)
        {
            return VFXManager.Instance?.PlayEffect(vfxType, position, rotation);
        }

        /// <summary>
        /// Play any VFX effect attached to a transform
        /// </summary>
        public static ParticleSystem PlayEffectAttached(VFXType vfxType, Transform parent, Vector3 localPosition = default)
        {
            return VFXManager.Instance?.PlayEffectAttached(vfxType, parent, localPosition);
        }

        /// <summary>
        /// Stop a specific effect
        /// </summary>
        public static void StopEffect(ParticleSystem effect)
        {
            VFXManager.Instance?.StopEffect(effect);
        }

        /// <summary>
        /// Clear all active effects
        /// </summary>
        public static void ClearAllEffects()
        {
            VFXManager.Instance?.ClearAllEffects();
        }

        #endregion

        #region Debug Helpers

        /// <summary>
        /// Get current active particle count
        /// </summary>
        public static int GetActiveParticleCount()
        {
            return VFXManager.Instance?.GetActiveParticleCount() ?? 0;
        }

        /// <summary>
        /// Get current active effect count
        /// </summary>
        public static int GetActiveEffectCount()
        {
            return VFXManager.Instance?.GetActiveEffectCount() ?? 0;
        }

        /// <summary>
        /// Check if VFX system is ready
        /// </summary>
        public static bool IsVFXSystemReady()
        {
            return VFXManager.Instance != null && VFXManager.Instance.IsInitialized();
        }

        #endregion
    }
}
