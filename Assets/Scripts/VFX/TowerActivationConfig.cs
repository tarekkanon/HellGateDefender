using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Tower Activation Sequence VFX.
    /// Attach this to FX_Tower_Activation prefab to validate and configure settings.
    /// This is a complex 3-phase effect (Ground Eruption → Energy Spiral → Power Surge).
    /// </summary>
    public class TowerActivationConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Phase 1: Ground Eruption (0.0-0.5s)")]
        [SerializeField] private ParticleSystem phase1_GroundEruption;
        [Tooltip("Dark red and purple mix")]
        [SerializeField] private Color phase1_ColorRed = new Color(0.86f, 0.08f, 0.24f, 1f); // Crimson
        [SerializeField] private Color phase1_ColorPurple = new Color(0.29f, 0f, 0.51f, 1f); // Dark Purple

        [Header("Phase 2: Energy Spiral (0.5-1.5s)")]
        [SerializeField] private ParticleSystem phase2_EnergySpiral;
        [Tooltip("Red → Purple → Green gradient")]
        [SerializeField] private Color phase2_ColorRed = new Color(0.86f, 0.08f, 0.24f, 1f);
        [SerializeField] private Color phase2_ColorPurple = new Color(0.29f, 0f, 0.51f, 1f);
        [SerializeField] private Color phase2_ColorGreen = new Color(0.20f, 0.80f, 0.20f, 1f); // Toxic Green

        [Header("Phase 3: Power Surge (1.5-2.0s)")]
        [SerializeField] private ParticleSystem phase3_PowerSurge;
        [Tooltip("Bright green-white flash")]
        [SerializeField] private Color phase3_FlashColor = new Color(0.80f, 1f, 0.80f, 1f); // Bright green-white

        [Header("Additional Components")]
        [SerializeField] private Light pointLight;
        [Tooltip("Tower mesh renderer for emissive glow")]
        [SerializeField] private Renderer towerRenderer;

        private Material _towerMaterial;

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
            if (phase1_GroundEruption == null || phase2_EnergySpiral == null || phase3_PowerSurge == null)
            {
                ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
                if (particles.Length >= 3)
                {
                    phase1_GroundEruption = particles[0];
                    phase2_EnergySpiral = particles[1];
                    phase3_PowerSurge = particles[2];
                }
            }

            if (pointLight == null)
            {
                pointLight = GetComponentInChildren<Light>();
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
            ConfigurePhase1_GroundEruption();
            ConfigurePhase2_EnergySpiral();
            ConfigurePhase3_PowerSurge();

            if (pointLight != null)
            {
                ConfigurePointLight();
            }

            Debug.Log("TowerActivationConfig: All phases configured successfully!");
        }

        /// <summary>
        /// Play the full activation sequence
        /// </summary>
        [ContextMenu("Play Activation Sequence")]
        public void PlayActivationSequence()
        {
            if (phase1_GroundEruption != null)
            {
                phase1_GroundEruption.Play();
            }

            if (phase2_EnergySpiral != null)
            {
                StartCoroutine(PlayPhase2AfterDelay(0.5f));
            }

            if (phase3_PowerSurge != null)
            {
                StartCoroutine(PlayPhase3AfterDelay(1.5f));
            }

            if (pointLight != null)
            {
                StartCoroutine(FadeLightIn());
            }

            Debug.Log("Tower Activation Sequence Started!");
        }

        #endregion

        #region Phase 1: Ground Eruption

        private void ConfigurePhase1_GroundEruption()
        {
            if (phase1_GroundEruption == null)
            {
                Debug.LogWarning("Phase 1 ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = phase1_GroundEruption.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.6f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 6f);
            main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0.5f; // Slight fall
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 20;

            // Emission - Burst
            var emission = phase1_GroundEruption.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 15, 20, 1)
            });

            // Shape - Cone upward
            var shape = phase1_GroundEruption.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 15f;
            shape.radius = 1.5f;
            shape.rotation = new Vector3(-90f, 0f, 0f); // Point upward

            // Color Over Lifetime
            var col = phase1_GroundEruption.colorOverLifetime;
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
            var size = phase1_GroundEruption.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.3f, 1.2f);
            sizeCurve.AddKey(1f, 0.5f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = phase1_GroundEruption.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 1: Ground Eruption configured");
        }

        #endregion

        #region Phase 2: Energy Spiral

        private void ConfigurePhase2_EnergySpiral()
        {
            if (phase2_EnergySpiral == null)
            {
                Debug.LogWarning("Phase 2 ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = phase2_EnergySpiral.main;
            main.duration = 1.0f;
            main.loop = false;
            main.startDelay = 0.5f; // Starts after Phase 1
            main.startLifetime = 1.2f;
            main.startSpeed = 2f;
            main.startSize = new ParticleSystem.MinMaxCurve(1.0f, 2.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = -0.5f; // Upward tendency
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 40;

            // Emission - Continuous
            var emission = phase2_EnergySpiral.emission;
            emission.enabled = true;
            emission.rateOverTime = 30f;

            // Shape - Sphere base
            var shape = phase2_EnergySpiral.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 2.0f;

            // Velocity Over Lifetime - Spiral effect
            var velocity = phase2_EnergySpiral.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;
            velocity.x = new ParticleSystem.MinMaxCurve(-2f, 2f);
            velocity.y = new ParticleSystem.MinMaxCurve(3f, 3f); // Strong upward
            velocity.z = new ParticleSystem.MinMaxCurve(-2f, 2f);
            velocity.orbitalX = new ParticleSystem.MinMaxCurve(0.5f, 0.5f);
            velocity.orbitalY = new ParticleSystem.MinMaxCurve(2f, 2f); // Creates spiral
            velocity.orbitalZ = new ParticleSystem.MinMaxCurve(0.5f, 0.5f);

            // Color Over Lifetime - Red → Purple → Green
            var col = phase2_EnergySpiral.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(phase2_ColorRed, 0f),
                    new GradientColorKey(phase2_ColorPurple, 0.4f),
                    new GradientColorKey(phase2_ColorGreen, 0.8f),
                    new GradientColorKey(phase2_ColorGreen, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.9f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime
            var size = phase2_EnergySpiral.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.5f);
            sizeCurve.AddKey(0.5f, 1f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = phase2_EnergySpiral.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 2: Energy Spiral configured");
        }

        #endregion

        #region Phase 3: Power Surge

        private void ConfigurePhase3_PowerSurge()
        {
            if (phase3_PowerSurge == null)
            {
                Debug.LogWarning("Phase 3 ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = phase3_PowerSurge.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startDelay = 1.5f; // Starts at end of Phase 2
            main.startLifetime = 0.5f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startSize = new ParticleSystem.MinMaxCurve(2.5f, 3.5f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 15;

            // Emission - Ring burst
            var emission = phase3_PowerSurge.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 8, 10, 1)
            });

            // Shape - Sphere burst
            var shape = phase3_PowerSurge.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 2.5f;

            // Color Over Lifetime - Bright flash
            var col = phase3_PowerSurge.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(phase3_FlashColor, 0.3f),
                    new GradientColorKey(phase2_ColorGreen, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.3f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime - Rapid expansion
            var size = phase3_PowerSurge.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.2f, 2.5f); // Rapid expansion
            sizeCurve.AddKey(1f, 0.2f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = phase3_PowerSurge.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 3: Power Surge configured");
        }

        #endregion

        #region Point Light

        private void ConfigurePointLight()
        {
            pointLight.type = LightType.Point;
            pointLight.color = new Color(0.20f, 0.80f, 0.20f, 1f); // Toxic green
            pointLight.intensity = 0f; // Start at 0, fade in
            pointLight.range = 6.0f;
            pointLight.shadows = LightShadows.None;
            pointLight.renderMode = LightRenderMode.Auto;

            Debug.Log("Point Light configured");
        }

        #endregion

        #region Coroutines

        private System.Collections.IEnumerator PlayPhase2AfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (phase2_EnergySpiral != null)
            {
                phase2_EnergySpiral.Play();
            }
        }

        private System.Collections.IEnumerator PlayPhase3AfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (phase3_PowerSurge != null)
            {
                phase3_PowerSurge.Play();
            }
        }

        private System.Collections.IEnumerator FadeLightIn()
        {
            if (pointLight == null) yield break;

            float duration = 2.0f;
            float elapsed = 0f;
            float targetIntensity = 5.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                pointLight.intensity = Mathf.Lerp(0f, targetIntensity, elapsed / duration);
                yield return null;
            }

            pointLight.intensity = targetIntensity;
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Tower Activation Sequence Configuration ===");
            Debug.Log($"Phase 1 (Ground Eruption): {(phase1_GroundEruption != null ? "Assigned" : "Missing")}");
            Debug.Log($"Phase 2 (Energy Spiral): {(phase2_EnergySpiral != null ? "Assigned" : "Missing")}");
            Debug.Log($"Phase 3 (Power Surge): {(phase3_PowerSurge != null ? "Assigned" : "Missing")}");
            Debug.Log($"Point Light: {(pointLight != null ? "Assigned" : "Missing")}");
            Debug.Log($"Total Duration: 2.0 seconds");
            Debug.Log("===============================================");
        }

        #endregion
    }
}
