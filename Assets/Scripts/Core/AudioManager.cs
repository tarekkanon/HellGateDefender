using UnityEngine;

namespace BaseDefender.Core
{
    /// <summary>
    /// Manages all audio playback in the game including sound effects and music.
    /// Singleton pattern ensures single instance throughout the game.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Sound Library")]
        [SerializeField] private SoundLibrary soundLibrary;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize audio sources if not assigned
            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = masterVolume * musicVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = masterVolume * sfxVolume;
            }
        }

        #region Music Control

        /// <summary>
        /// Plays the gameplay background music loop
        /// </summary>
        public void PlayGameplayMusic()
        {
            if (soundLibrary == null || soundLibrary.gameplayMusic == null)
            {
                Debug.LogWarning("AudioManager: Gameplay music clip not assigned!");
                return;
            }

            musicSource.clip = soundLibrary.gameplayMusic;
            musicSource.Play();
        }

        /// <summary>
        /// Stops the currently playing music
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Pauses the currently playing music
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }

        /// <summary>
        /// Resumes paused music
        /// </summary>
        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }

        #endregion

        #region Sound Effects

        /// <summary>
        /// Plays the player shoot sound effect
        /// </summary>
        public void PlayPlayerShoot()
        {
            PlaySoundEffect(soundLibrary?.playerShoot);
        }

        /// <summary>
        /// Plays the turret shoot sound effect
        /// </summary>
        public void PlayTurretShoot()
        {
            PlaySoundEffect(soundLibrary?.turretShoot);
        }

        /// <summary>
        /// Plays the enemy death sound effect
        /// </summary>
        public void PlayEnemyDeath()
        {
            PlaySoundEffect(soundLibrary?.enemyDeath);
        }

        /// <summary>
        /// Plays the coin collect sound effect
        /// </summary>
        public void PlayCoinCollect()
        {
            PlaySoundEffect(soundLibrary?.coinCollect);
        }

        /// <summary>
        /// Plays the base hit sound effect
        /// </summary>
        public void PlayBaseHit()
        {
            PlaySoundEffect(soundLibrary?.baseHit);
        }

        /// <summary>
        /// Plays the turret activation sound effect
        /// </summary>
        public void PlayTurretActivate()
        {
            PlaySoundEffect(soundLibrary?.turretActivate);
        }

        /// <summary>
        /// Plays the wave start sound effect
        /// </summary>
        public void PlayWaveStart()
        {
            PlaySoundEffect(soundLibrary?.waveStart);
        }

        /// <summary>
        /// Plays the victory sound effect
        /// </summary>
        public void PlayVictory()
        {
            PlaySoundEffect(soundLibrary?.victory);
        }

        /// <summary>
        /// Plays the defeat sound effect
        /// </summary>
        public void PlayDefeat()
        {
            PlaySoundEffect(soundLibrary?.defeat);
        }

        /// <summary>
        /// Plays a one-shot sound effect
        /// </summary>
        private void PlaySoundEffect(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("AudioManager: Attempted to play null audio clip!");
                return;
            }

            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Sets the master volume (affects both music and SFX)
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets the music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        /// <summary>
        /// Sets the sound effects volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Update volumes when values change in inspector
            UpdateVolumes();
        }
#endif

        #endregion
    }
}
