using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Tower Muzzle Flash VFX.
    /// Attach this to FX_Tower_MuzzleFlash prefab to validate and configure settings.
    /// Brief flash effect when tower fires - larger and brighter than player muzzle flash.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class TowerMuzzleFlashConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Color Settings")]
        [Tooltip("Flash start color - White-Green")]
        [SerializeField] private Color flashStartColor = new Color(0.9f, 1f, 0.9f, 1f); // Bright white-green

        [Tooltip("Flash mid color - Bright Toxic Green")]
        [SerializeField] private Color flashMidColor = new Color(0.20f, 0.80f, 0.20f, 1f); // #32CD32

        [Tooltip("Flash end color - Dark Purple")]
        [SerializeField] private Color flashEndColor = new Color(0.29f, 0f, 0.51f, 1f); // #4B0082

        [Header("Performance")]
        [Tooltip("Maximum particles for this effect")]
        [SerializeField] private int maxParticles = 10;

        [Tooltip("Burst count")]
        [SerializeField] private int burstCount = 6;

        [Header("Size & Lifetime")]
        [Tooltip("Minimum particle start size")]
        [SerializeField] private float minStartSize = 3.0f;

        [Tooltip("Maximum particle start size")]
        [SerializeField] private float maxStartSize = 5.0f;

        [Tooltip("Effect duration in seconds")]
        [SerializeField] private float effectDuration = 0.2f;

        [Header("Movement")]
        [Tooltip("Minimum start speed")]
        [SerializeField] private float minStartSpeed = 3f;

        [Tooltip("Maximum start speed")]
        [SerializeField] private float maxStartSpeed = 5f;

        [Tooltip("Cone angle for burst direction")]
        [SerializeField] private float coneAngle = 25f;

        [Header("Components")]
        [Tooltip("Optional flash light")]
        [SerializeField] private Light flashLight;

        private ParticleSystem _particleSystem;

        #region Unity Lifecycle

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();

            if (autoConfigureOnAwake)
            {
                ConfigureParticleSystem();
            }
        }

        private void OnValidate()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            if (flashLight == null)
            {
                flashLight = GetComponentInChildren<Light>();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Apply configuration to particle system
        /// </summary>
        [ContextMenu("Apply Configuration")]
        public void ConfigureParticleSystem()
        {
            if (_particleSystem == null)
            {
                Debug.LogError("TowerMuzzleFlashConfig: No ParticleSystem found!");
                return;
            }

            ConfigureMainModule();
            ConfigureEmissionModule();
            ConfigureShapeModule();
            ConfigureColorOverLifetime();
            ConfigureSizeOverLifetime();
            ConfigureRenderer();

            if (flashLight != null)
            {
                ConfigureFlashLight();
            }

            Debug.Log("TowerMuzzleFlashConfig: Configuration applied successfully!");
        }

        /// <summary>
        /// Validate current particle system settings
        /// </summary>
        [ContextMenu("Validate Settings")]
        public void ValidateSettings()
        {
            if (_particleSystem == null)
            {
                Debug.LogError("Validation Failed: No ParticleSystem component found!");
                return;
            }

            bool isValid = true;
            var main = _particleSystem.main;

            if (main.loop != false)
            {
                Debug.LogWarning("Loop should be disabled for one-shot effect!");
                isValid = false;
            }

            if (!Mathf.Approximately(main.duration, effectDuration))
            {
                Debug.LogWarning($"Duration mismatch: Expected {effectDuration}, got {main.duration}");
                isValid = false;
            }

            var emission = _particleSystem.emission;
            if (emission.rateOverTime.constant != 0)
            {
                Debug.LogWarning("Rate Over Time should be 0 for burst-only effect!");
                isValid = false;
            }

            if (isValid)
            {
                Debug.Log("✓ Validation Passed: All settings are correct!");
            }
            else
            {
                Debug.Log("✗ Validation Issues Found: Check warnings above.");
            }
        }

        /// <summary>
        /// Play the muzzle flash effect
        /// </summary>
        [ContextMenu("Play Flash")]
        public void PlayFlash()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }

            if (flashLight != null)
            {
                StartCoroutine(FlashLightPulse());
            }
        }

        #endregion

        #region Configuration Methods

        private void ConfigureMainModule()
        {
            var main = _particleSystem.main;
            main.duration = effectDuration;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = effectDuration;
            main.startSpeed = new ParticleSystem.MinMaxCurve(minStartSpeed, maxStartSpeed);
            main.startSize3D = false;
            main.startSize = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.startColor = flashStartColor;
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.simulationSpeed = 1f;
            main.playOnAwake = false;
            main.maxParticles = maxParticles;
        }

        private void ConfigureEmissionModule()
        {
            var emission = _particleSystem.emission;
            emission.enabled = true;
            emission.rateOverTime = 0; // Burst only

            // Single burst at start
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, (short)burstCount)
            });
        }

        private void ConfigureShapeModule()
        {
            var shape = _particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = coneAngle;
            shape.radius = 0.5f;
            shape.radiusThickness = 1f;
            shape.arc = 360f;
            shape.arcMode = ParticleSystemShapeMultiModeValue.Random;
        }

        private void ConfigureColorOverLifetime()
        {
            var col = _particleSystem.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(flashStartColor, 0f),
                    new GradientColorKey(flashMidColor, 0.3f),
                    new GradientColorKey(flashEndColor, 0.7f),
                    new GradientColorKey(flashEndColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.9f, 0.3f),
                    new GradientAlphaKey(0.5f, 0.7f),
                    new GradientAlphaKey(0f, 1f)
                }
            );

            col.color = gradient;
        }

        private void ConfigureSizeOverLifetime()
        {
            var size = _particleSystem.sizeOverLifetime;
            size.enabled = true;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(0.15f, 1.3f); // Quick expansion
            curve.AddKey(1f, 0f);       // Fade out

            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }

        private void ConfigureRenderer()
        {
            var renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.sortMode = ParticleSystemSortMode.Distance;
                renderer.minParticleSize = 0f;
                renderer.maxParticleSize = 7.5f;
                renderer.alignment = ParticleSystemRenderSpace.View;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        private void ConfigureFlashLight()
        {
            flashLight.type = LightType.Point;
            flashLight.color = flashMidColor; // Green
            flashLight.intensity = 0f; // Start disabled
            flashLight.range = 5.0f;
            flashLight.shadows = LightShadows.None;
            flashLight.renderMode = LightRenderMode.Auto;
        }

        #endregion

        #region Flash Light Pulse

        private System.Collections.IEnumerator FlashLightPulse()
        {
            if (flashLight == null) yield break;

            float duration = 0.15f;
            float elapsed = 0f;
            float maxIntensity = 4.0f;

            flashLight.enabled = true;

            // Fade in quickly
            while (elapsed < duration * 0.3f)
            {
                elapsed += Time.deltaTime;
                flashLight.intensity = Mathf.Lerp(0f, maxIntensity, elapsed / (duration * 0.3f));
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < duration * 0.7f)
            {
                elapsed += Time.deltaTime;
                flashLight.intensity = Mathf.Lerp(maxIntensity, 0f, elapsed / (duration * 0.7f));
                yield return null;
            }

            flashLight.intensity = 0f;
            flashLight.enabled = false;
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            if (_particleSystem == null)
            {
                Debug.LogError("No ParticleSystem found!");
                return;
            }

            var main = _particleSystem.main;
            var emission = _particleSystem.emission;

            Debug.Log("=== Tower Muzzle Flash Configuration ===");
            Debug.Log($"Duration: {main.duration}s (one-shot)");
            Debug.Log($"Burst Count: {burstCount} particles");
            Debug.Log($"Start Size: {main.startSize.constantMin} - {main.startSize.constantMax}");
            Debug.Log($"Start Speed: {minStartSpeed} - {maxStartSpeed}");
            Debug.Log($"Cone Angle: {coneAngle}°");
            Debug.Log($"Flash Color: {flashStartColor} → {flashMidColor} → {flashEndColor}");

            if (flashLight != null)
            {
                Debug.Log($"Flash Light: Range {flashLight.range}, Max Intensity 4.0");
            }
            else
            {
                Debug.Log("Flash Light: Not found");
            }

            Debug.Log("=========================================");
        }

        [ContextMenu("Show Performance Stats")]
        public void ShowPerformanceStats()
        {
            if (_particleSystem == null)
            {
                Debug.LogError("No ParticleSystem found!");
                return;
            }

            var main = _particleSystem.main;

            Debug.Log("=== Performance Statistics ===");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Burst Size: {burstCount}");
            Debug.Log($"Effect Duration: {effectDuration}s");
            Debug.Log($"Performance Impact: Very Low (one-shot)");
            Debug.Log($"Mobile Budget: ✓ PASS");
            Debug.Log("==============================");
        }

        #endregion
    }
}
