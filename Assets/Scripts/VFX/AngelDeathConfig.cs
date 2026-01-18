using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Angel Death VFX.
    /// Attach this to FX_Angel_Death prefab to validate and configure settings.
    /// This is a complex 3-phase effect (Corruption Spread → Dissolution → Soul Release).
    /// </summary>
    public class AngelDeathConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Phase 1: Corruption Spread (0.0-0.3s)")]
        [SerializeField] private ParticleSystem phase1_CorruptionSpread;
        [Tooltip("Red and purple dark energy")]
        [SerializeField] private Color phase1_ColorRed = new Color(0.86f, 0.08f, 0.24f, 1f); // Crimson
        [SerializeField] private Color phase1_ColorPurple = new Color(0.29f, 0f, 0.51f, 1f); // Dark Purple

        [Header("Phase 2: Dissolution Stream (0.3-0.7s)")]
        [SerializeField] private ParticleSystem phase2_DissolutionStream;
        [Tooltip("White/gold → Purple gradient")]
        [SerializeField] private Color phase2_ColorWhite = new Color(1f, 1f, 1f, 1f); // Angel essence
        [SerializeField] private Color phase2_ColorGold = new Color(1f, 0.84f, 0f, 1f); // Gold
        [SerializeField] private Color phase2_ColorPurple = new Color(0.29f, 0f, 0.51f, 1f); // Dark magic

        [Header("Phase 3: Soul Release (0.7-1.0s)")]
        [SerializeField] private ParticleSystem phase3_SoulRelease;
        [Tooltip("Fading white with purple edges")]
        [SerializeField] private Color phase3_ColorWhite = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color phase3_ColorPurple = new Color(0.29f, 0f, 0.51f, 1f);

        #region Unity Lifecycle

        private void Awake()
        {
            if (autoConfigureOnAwake)
            {
                ConfigureAllPhases();
            }
        }

        private void OnValidate()
        {
            // Auto-find particle systems
            if (phase1_CorruptionSpread == null || phase2_DissolutionStream == null || phase3_SoulRelease == null)
            {
                ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
                if (particles.Length >= 3)
                {
                    phase1_CorruptionSpread = particles[0];
                    phase2_DissolutionStream = particles[1];
                    phase3_SoulRelease = particles[2];
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Apply configuration to all phases
        /// </summary>
        [ContextMenu("Apply Configuration")]
        public void ConfigureAllPhases()
        {
            ConfigurePhase1_CorruptionSpread();
            ConfigurePhase2_DissolutionStream();
            ConfigurePhase3_SoulRelease();

            Debug.Log("AngelDeathConfig: All phases configured successfully!");
        }

        /// <summary>
        /// Play the full death sequence
        /// </summary>
        [ContextMenu("Play Death Sequence")]
        public void PlayDeathSequence()
        {
            if (phase1_CorruptionSpread != null)
            {
                phase1_CorruptionSpread.Play();
            }

            if (phase2_DissolutionStream != null)
            {
                StartCoroutine(PlayPhase2AfterDelay(0.3f));
            }

            if (phase3_SoulRelease != null)
            {
                StartCoroutine(PlayPhase3AfterDelay(0.7f));
            }

            Debug.Log("Angel Death Sequence Started!");
        }

        #endregion

        #region Phase 1: Corruption Spread

        private void ConfigurePhase1_CorruptionSpread()
        {
            if (phase1_CorruptionSpread == null)
            {
                Debug.LogWarning("Phase 1 ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = phase1_CorruptionSpread.main;
            main.duration = 0.3f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.3f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
            main.startSize = new ParticleSystem.MinMaxCurve(3.0f, 3.5f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 20;

            // Emission - Burst
            var emission = phase1_CorruptionSpread.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 15, 20, 1)
            });

            // Shape - Sphere burst
            var shape = phase1_CorruptionSpread.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 2.5f;
            shape.radiusThickness = 1f; // Spawn on surface

            // Color Over Lifetime - Red/Purple mix
            var col = phase1_CorruptionSpread.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(phase1_ColorRed, 0f),
                    new GradientColorKey(phase1_ColorPurple, 0.5f),
                    new GradientColorKey(phase1_ColorPurple, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime
            var size = phase1_CorruptionSpread.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 1.2f);
            sizeCurve.AddKey(1f, 0.5f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = phase1_CorruptionSpread.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 1: Corruption Spread configured");
        }

        #endregion

        #region Phase 2: Dissolution Stream

        private void ConfigurePhase2_DissolutionStream()
        {
            if (phase2_DissolutionStream == null)
            {
                Debug.LogWarning("Phase 2 ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = phase2_DissolutionStream.main;
            main.duration = 0.4f;
            main.loop = false;
            main.startDelay = 0.3f; // Starts after Phase 1
            main.startLifetime = 0.6f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startSize = new ParticleSystem.MinMaxCurve(4.0f, 4.5f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = -0.5f; // Slight upward drift
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 15;

            // Emission - Continuous stream
            var emission = phase2_DissolutionStream.emission;
            emission.enabled = true;
            emission.rateOverTime = 25f;

            // Shape - Narrow cone upward
            var shape = phase2_DissolutionStream.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 1.0f;
            shape.rotation = new Vector3(-90f, 0f, 0f); // Point upward

            // Color Over Lifetime - White → Gold → Purple
            var col = phase2_DissolutionStream.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(phase2_ColorWhite, 0f),
                    new GradientColorKey(phase2_ColorGold, 0.3f),
                    new GradientColorKey(phase2_ColorPurple, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime
            var size = phase2_DissolutionStream.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.8f);
            sizeCurve.AddKey(0.5f, 1f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = phase2_DissolutionStream.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 2: Dissolution Stream configured");
        }

        #endregion

        #region Phase 3: Soul Release

        private void ConfigurePhase3_SoulRelease()
        {
            if (phase3_SoulRelease == null)
            {
                Debug.LogWarning("Phase 3 ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = phase3_SoulRelease.main;
            main.duration = 0.3f;
            main.loop = false;
            main.startDelay = 0.7f; // Starts at end of Phase 2
            main.startLifetime = 0.3f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 8f);
            main.startSize = new ParticleSystem.MinMaxCurve(4.5f, 6.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = -1.0f; // Strong upward
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 8;

            // Emission - Final burst
            var emission = phase3_SoulRelease.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 5, 8, 1)
            });

            // Shape - Cone upward
            var shape = phase3_SoulRelease.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 20f;
            shape.radius = 1.5f;
            shape.rotation = new Vector3(-90f, 0f, 0f); // Point upward

            // Color Over Lifetime - White → Purple → Transparent
            var col = phase3_SoulRelease.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(phase3_ColorWhite, 0f),
                    new GradientColorKey(phase3_ColorPurple, 0.5f),
                    new GradientColorKey(phase3_ColorPurple, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.6f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime
            var size = phase3_SoulRelease.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 1.2f);
            sizeCurve.AddKey(1f, 0.5f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = phase3_SoulRelease.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 3: Soul Release configured");
        }

        #endregion

        #region Coroutines

        private System.Collections.IEnumerator PlayPhase2AfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (phase2_DissolutionStream != null)
            {
                phase2_DissolutionStream.Play();
            }
        }

        private System.Collections.IEnumerator PlayPhase3AfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (phase3_SoulRelease != null)
            {
                phase3_SoulRelease.Play();
            }
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Angel Death Sequence Configuration ===");
            Debug.Log($"Phase 1 (Corruption Spread): {(phase1_CorruptionSpread != null ? "Assigned" : "Missing")}");
            Debug.Log($"Phase 2 (Dissolution Stream): {(phase2_DissolutionStream != null ? "Assigned" : "Missing")}");
            Debug.Log($"Phase 3 (Soul Release): {(phase3_SoulRelease != null ? "Assigned" : "Missing")}");
            Debug.Log($"Total Duration: 1.0 second");

            int totalParticles = 0;
            if (phase1_CorruptionSpread != null) totalParticles += phase1_CorruptionSpread.main.maxParticles;
            if (phase2_DissolutionStream != null) totalParticles += phase2_DissolutionStream.main.maxParticles;
            if (phase3_SoulRelease != null) totalParticles += phase3_SoulRelease.main.maxParticles;

            Debug.Log($"Total Max Particles: {totalParticles}");
            Debug.Log("===========================================");
        }

        #endregion
    }
}
