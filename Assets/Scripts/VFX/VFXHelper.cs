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
        /// Play demonic hit on angel effect (no audio, impact only)
        /// </summary>
        public static ParticleSystem PlayDemonicHit(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffect(VFXType.DemonicHitOnAngel, position);
        }

        /// <summary>
        /// Play angelic hit on demonic effect with audio
        /// </summary>
        public static ParticleSystem PlayAngelicHit(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.AngelicHitOnDemonic,
                position,
                () => AudioManager.Instance?.PlayBaseHit()
            );
        }

        /// <summary>
        /// Play angel death effect with audio
        /// </summary>
        public static ParticleSystem PlayAngelDeath(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffectWithAudio(
                VFXType.AngelDeath,
                position,
                () => AudioManager.Instance?.PlayEnemyDeath()
            );
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
        /// Play spawn portal effect
        /// </summary>
        public static ParticleSystem PlaySpawnPortal(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffect(VFXType.SpawnPortal, position);
        }

        /// <summary>
        /// Play ambient atmosphere effect (typically one instance in scene)
        /// </summary>
        public static ParticleSystem PlayAmbientAtmosphere(Vector3 position)
        {
            return VFXManager.Instance?.PlayEffect(VFXType.AmbientAtmosphere, position);
        }

        /// <summary>
        /// Play base shield effect (attach to base)
        /// </summary>
        public static ParticleSystem PlayBaseShield(Transform baseTransform)
        {
            return VFXManager.Instance?.PlayEffectAttached(VFXType.BaseShield, baseTransform);
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
