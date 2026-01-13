using UnityEngine;

namespace BaseDefender.Core
{
    /// <summary>
    /// ScriptableObject that holds references to all audio clips used in the game.
    /// Create via: Assets > Create > Base Defender > Sound Library
    /// </summary>
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Base Defender/Sound Library", order = 1)]
    public class SoundLibrary : ScriptableObject
    {
        [Header("Music")]
        [Tooltip("Looping background music for gameplay")]
        public AudioClip gameplayMusic;

        [Header("Player Sounds")]
        [Tooltip("Sound when player shoots")]
        public AudioClip playerShoot;

        [Header("Turret Sounds")]
        [Tooltip("Sound when turret shoots")]
        public AudioClip turretShoot;
        [Tooltip("Sound when turret is activated")]
        public AudioClip turretActivate;

        [Header("Enemy Sounds")]
        [Tooltip("Sound when enemy dies")]
        public AudioClip enemyDeath;

        [Header("Collectible Sounds")]
        [Tooltip("Sound when player collects a coin")]
        public AudioClip coinCollect;

        [Header("Base Sounds")]
        [Tooltip("Sound when base takes damage")]
        public AudioClip baseHit;

        [Header("Game Events")]
        [Tooltip("Sound when a new wave starts")]
        public AudioClip waveStart;
        [Tooltip("Sound when player wins")]
        public AudioClip victory;
        [Tooltip("Sound when player loses")]
        public AudioClip defeat;

        #region Validation

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateAudioClips();
        }

        private void ValidateAudioClips()
        {
            int missingCount = 0;

            if (gameplayMusic == null) missingCount++;
            if (playerShoot == null) missingCount++;
            if (turretShoot == null) missingCount++;
            if (turretActivate == null) missingCount++;
            if (enemyDeath == null) missingCount++;
            if (coinCollect == null) missingCount++;
            if (baseHit == null) missingCount++;
            if (waveStart == null) missingCount++;
            if (victory == null) missingCount++;
            if (defeat == null) missingCount++;

            if (missingCount > 0)
            {
                Debug.LogWarning($"SoundLibrary '{name}': {missingCount} audio clips are not assigned. Please assign all clips for complete audio experience.");
            }
        }
#endif

        #endregion
    }
}
