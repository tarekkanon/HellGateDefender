using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Angelic Hit on Demonic Impact VFX.
    /// Attach this to FX_Impact_AngelicOnDark prefab to validate and configure settings.
    /// Creates holy impact effect when angelic attacks hit demonic structures.
    /// </summary>
    public class AngelicHitOnDemonicConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Radiant Burst")]
        [SerializeField] private ParticleSystem radiantBurst;
        [Tooltip("Bright white/gold burst particles")]
        [SerializeField] private Color burstColorGold = new Color(1f, 0.84f, 0f, 1f); // #FFD700
        [SerializeField] private Color burstColorWhite = new Color(1f, 1f, 1f, 1f); // Divine White

        [Header("Point Light")]
        [SerializeField] private Light pointLight;
        [Tooltip("Bright flash on impact")]
        [SerializeField] private Color lightColor = new Color(1f, 0.84f, 0f, 1f); // Gold

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
            // Auto-find particle system
            if (radiantBurst == null)
            {
                radiantBurst = GetComponentInChildren<ParticleSystem>();
            }

            // Auto-find light
            if (pointLight == null)
            {
                pointLight = GetComponentInChildren<Light>();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Apply configuration to all systems
        /// </summary>
        [ContextMenu("Apply Configuration")]
        public void ConfigureAllSystems()
        {
            ConfigureRadiantBurst();

            if (pointLight != null)
            {
                ConfigurePointLight();
            }

            Debug.Log("AngelicHitOnDemonicConfig: All systems configured successfully!");
        }

        /// <summary>
        /// Play the complete impact effect
        /// </summary>
        [ContextMenu("Play Impact Effect")]
        public void PlayImpactEffect()
        {
            if (radiantBurst != null)
            {
                radiantBurst.Play();
            }

            if (pointLight != null)
            {
                StartCoroutine(FlashLight());
            }

            Debug.Log("Angelic Impact Effect Started!");
        }

        #endregion

        #region Radiant Burst Configuration

        private void ConfigureRadiantBurst()
        {
            if (radiantBurst == null)
            {
                Debug.LogWarning("Radiant Burst ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = radiantBurst.main;
            main.duration = 0.4f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.4f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f; // No gravity - radiant energy
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 12;

            // Emission - Sphere burst
            var emission = radiantBurst.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 8, 12, 1)
            });

            // Shape - Sphere burst (radial pattern)
            var shape = radiantBurst.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 1.0f;
            shape.radiusThickness = 1f; // Spawn on surface

            // Color Over Lifetime - Bright gold → White → Transparent
            var col = radiantBurst.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(burstColorGold, 0f),
                    new GradientColorKey(burstColorWhite, 0.5f),
                    new GradientColorKey(burstColorWhite, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.9f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime - Expand then fade
            var size = radiantBurst.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.3f);
            sizeCurve.AddKey(0.3f, 0.5f); // Expand
            sizeCurve.AddKey(1f, 0f);    // Fade out
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = radiantBurst.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Radiant Burst configured");
        }

        #endregion

        #region Point Light Configuration

        private void ConfigurePointLight()
        {
            if (pointLight == null)
            {
                Debug.LogWarning("Point Light not assigned!");
                return;
            }

            pointLight.type = LightType.Point;
            pointLight.color = lightColor;
            pointLight.intensity = 0f; // Start at 0, will flash
            pointLight.range = 4.0f;
            pointLight.shadows = LightShadows.None;
            pointLight.renderMode = LightRenderMode.Auto;

            Debug.Log("Point Light configured");
        }

        #endregion

        #region Coroutines

        private System.Collections.IEnumerator FlashLight()
        {
            if (pointLight == null) yield break;

            float duration = 0.2f;
            float elapsed = 0f;
            float maxIntensity = 5.0f;

            // Fade in quickly
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                pointLight.intensity = Mathf.Lerp(0f, maxIntensity, t);
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                pointLight.intensity = Mathf.Lerp(maxIntensity, 0f, t);
                yield return null;
            }

            pointLight.intensity = 0f;
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Angelic Hit on Demonic Configuration ===");
            Debug.Log($"Radiant Burst: {(radiantBurst != null ? "Assigned" : "Missing")}");
            Debug.Log($"Point Light: {(pointLight != null ? "Assigned" : "Missing")}");
            Debug.Log($"Total Duration: 0.4 seconds");
            Debug.Log($"Total Max Particles: {(radiantBurst != null ? radiantBurst.main.maxParticles : 0)}");
            Debug.Log("============================================");
        }

        #endregion
    }
}
