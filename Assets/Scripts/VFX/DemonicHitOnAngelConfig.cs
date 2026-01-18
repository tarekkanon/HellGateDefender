using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Demonic Hit on Angel Impact VFX.
    /// Attach this to FX_Impact_DemonicOnAngel prefab to validate and configure settings.
    /// Creates impact effect when demonic spells hit angelic enemies.
    /// </summary>
    public class DemonicHitOnAngelConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Main Impact Burst")]
        [SerializeField] private ParticleSystem impactBurst;
        [Tooltip("Red, orange, and black particles")]
        [SerializeField] private Color burstColorOrange = new Color(1f, 0.27f, 0f, 1f); // #FF4500
        [SerializeField] private Color burstColorRed = new Color(0.86f, 0.08f, 0.24f, 1f); // Crimson
        [SerializeField] private Color burstColorBlack = new Color(0.11f, 0.11f, 0.11f, 1f); // Shadow Black

        [Header("Corruption Sparks")]
        [SerializeField] private ParticleSystem corruptionSparks;
        [Tooltip("Toxic green and purple")]
        [SerializeField] private Color corruptionGreen = new Color(0.20f, 0.80f, 0.20f, 1f); // Toxic Green
        [SerializeField] private Color corruptionPurple = new Color(0.29f, 0f, 0.51f, 1f); // Dark Purple

        [Header("Visual Flavor")]
        [SerializeField] private ParticleSystem impactMarkers;
        [Tooltip("Comic-book style impact markers")]
        [SerializeField] private Color markerColor = new Color(0.86f, 0.08f, 0.24f, 1f); // Bright Red

        #region Unity Lifecycle

        private void Awake()
        {
            if (autoConfigureOnAwake)
            {
                ConfigureAllSystems();
            }
        }

        private void OnValidate()
        {
            // Auto-find particle systems
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            if (particles.Length >= 3)
            {
                if (impactBurst == null) impactBurst = particles[0];
                if (corruptionSparks == null) corruptionSparks = particles[1];
                if (impactMarkers == null && particles.Length > 2) impactMarkers = particles[2];
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Apply configuration to all particle systems
        /// </summary>
        [ContextMenu("Apply Configuration")]
        public void ConfigureAllSystems()
        {
            ConfigureImpactBurst();
            ConfigureCorruptionSparks();

            if (impactMarkers != null)
            {
                ConfigureImpactMarkers();
            }

            Debug.Log("DemonicHitOnAngelConfig: All systems configured successfully!");
        }

        /// <summary>
        /// Play the complete impact effect
        /// </summary>
        [ContextMenu("Play Impact Effect")]
        public void PlayImpactEffect()
        {
            if (impactBurst != null)
            {
                impactBurst.Play();
            }

            if (corruptionSparks != null)
            {
                corruptionSparks.Play();
            }

            if (impactMarkers != null)
            {
                impactMarkers.Play();
            }

            Debug.Log("Demonic Impact Effect Started!");
        }

        #endregion

        #region Impact Burst Configuration

        private void ConfigureImpactBurst()
        {
            if (impactBurst == null)
            {
                Debug.LogWarning("Impact Burst ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = impactBurst.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.4f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
            main.startSize = new ParticleSystem.MinMaxCurve(1.0f, 2.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0.5f; // Slight fall
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 15;

            // Emission - Burst
            var emission = impactBurst.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 10, 15, 1)
            });

            // Shape - Hemisphere facing outward
            var shape = impactBurst.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Hemisphere;
            shape.radius = 1.0f;
            shape.radiusThickness = 1f; // Spawn on surface

            // Color Over Lifetime - Orange → Red → Black transparent
            var col = impactBurst.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(burstColorOrange, 0f),
                    new GradientColorKey(burstColorRed, 0.5f),
                    new GradientColorKey(burstColorBlack, 1f)
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
            var size = impactBurst.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 1.2f);
            sizeCurve.AddKey(1f, 0.5f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = impactBurst.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Impact Burst configured");
        }

        #endregion

        #region Corruption Sparks Configuration

        private void ConfigureCorruptionSparks()
        {
            if (corruptionSparks == null)
            {
                Debug.LogWarning("Corruption Sparks ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = corruptionSparks.main;
            main.duration = 0.6f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.6f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 2.0f; // Falls quickly
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 10;

            // Emission - Burst
            var emission = corruptionSparks.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 5, 8, 1)
            });

            // Shape - Cone upward
            var shape = corruptionSparks.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 45f;
            shape.radius = 0.75f;
            shape.rotation = new Vector3(-90f, 0f, 0f); // Point upward

            // Color Over Lifetime - Alternating green/purple
            var col = corruptionSparks.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(corruptionGreen, 0f),
                    new GradientColorKey(corruptionPurple, 0.5f),
                    new GradientColorKey(corruptionPurple, 1f)
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
            var size = corruptionSparks.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 1.1f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = corruptionSparks.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Corruption Sparks configured");
        }

        #endregion

        #region Impact Markers Configuration

        private void ConfigureImpactMarkers()
        {
            if (impactMarkers == null)
            {
                Debug.LogWarning("Impact Markers ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = impactMarkers.main;
            main.duration = 0.3f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.3f;
            main.startSpeed = 0f; // Static particles
            main.startSize = 1.5f;
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 3;

            // Emission - Small burst
            var emission = impactMarkers.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 2, 3, 1)
            });

            // Shape - Point
            var shape = impactMarkers.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.5f;

            // Color Over Lifetime - Bright red fade
            var col = impactMarkers.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(markerColor, 0f),
                    new GradientColorKey(markerColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime
            var size = impactMarkers.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 1.3f);
            sizeCurve.AddKey(1f, 0.8f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = impactMarkers.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Impact Markers configured");
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Demonic Hit on Angel Configuration ===");
            Debug.Log($"Impact Burst: {(impactBurst != null ? "Assigned" : "Missing")}");
            Debug.Log($"Corruption Sparks: {(corruptionSparks != null ? "Assigned" : "Missing")}");
            Debug.Log($"Impact Markers: {(impactMarkers != null ? "Assigned" : "Missing")}");
            Debug.Log($"Total Duration: 0.6 seconds");
            Debug.Log($"Total Max Particles: {(impactBurst != null ? impactBurst.main.maxParticles : 0) + (corruptionSparks != null ? corruptionSparks.main.maxParticles : 0) + (impactMarkers != null ? impactMarkers.main.maxParticles : 0)}");
            Debug.Log("==========================================");
        }

        #endregion
    }
}
