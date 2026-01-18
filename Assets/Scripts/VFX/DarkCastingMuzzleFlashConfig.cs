using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Dark Casting Muzzle Flash VFX (P0).
    /// Attach this to FX_Player_MuzzleFlash prefab to validate and configure settings.
    ///
    /// Specifications from VFX_Implementation_Guide.md:
    /// - One-shot particle burst
    /// - Duration: 0.15 seconds
    /// - 3-5 particles radiating outward in cone shape
    /// - Color: Bright orange-red core with purple outer particles
    /// - Size: 0.4-0.8 units
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class DarkCastingMuzzleFlashConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Color Settings")]
        [Tooltip("Start color - Bright Orange")]
        [SerializeField] private Color startColor = new Color(1f, 0.5f, 0f, 1f); // Bright orange

        [Tooltip("Mid color - Orange-Red")]
        [SerializeField] private Color midColor = new Color(1f, 0.27f, 0f, 1f); // RGB(255, 69, 0)

        [Tooltip("End color - Dark Red")]
        [SerializeField] private Color endColor = new Color(0.55f, 0f, 0f, 1f); // Dark red

        [Header("Performance")]
        [Tooltip("Maximum particles for this effect")]
        [SerializeField] private int maxParticles = 5;

        [Header("Burst Settings")]
        [Tooltip("Minimum particles in burst")]
        [SerializeField] private int minBurstCount = 3;

        [Tooltip("Maximum particles in burst")]
        [SerializeField] private int maxBurstCount = 5;

        [Header("Duration & Lifetime")]
        [Tooltip("Effect duration in seconds")]
        [SerializeField] private float duration = 0.15f;

        [Tooltip("Particle lifetime in seconds")]
        [SerializeField] private float particleLifetime = 0.15f;

        [Header("Size Settings")]
        [Tooltip("Minimum particle start size")]
        [SerializeField] private float minStartSize = 2.0f;

        [Tooltip("Maximum particle start size")]
        [SerializeField] private float maxStartSize = 4.0f;

        [Header("Speed Settings")]
        [Tooltip("Minimum start speed")]
        [SerializeField] private float minStartSpeed = 2f;

        [Tooltip("Maximum start speed")]
        [SerializeField] private float maxStartSpeed = 4f;

        [Header("Shape Settings")]
        [Tooltip("Cone angle in degrees")]
        [SerializeField] private float coneAngle = 30f;

        [Tooltip("Cone radius")]
        [SerializeField] private float coneRadius = 1.0f;

        [Header("Optional Enhancements")]
        [Tooltip("Optional Point Light for flash effect")]
        [SerializeField] private Light pointLight;

        [Tooltip("Light flash duration")]
        [SerializeField] private float lightDuration = 0.15f;

        [Tooltip("Light intensity")]
        [SerializeField] private float lightIntensity = 4.0f;

        [Tooltip("Light range")]
        [SerializeField] private float lightRange = 5.0f;

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
            // Auto-find components
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }

            if (pointLight == null)
            {
                pointLight = GetComponentInChildren<Light>();
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
                Debug.LogError("DarkCastingMuzzleFlashConfig: No ParticleSystem found!");
                return;
            }

            ConfigureMainModule();
            ConfigureEmissionModule();
            ConfigureShapeModule();
            ConfigureColorOverLifetime();
            ConfigureSizeOverLifetime();
            ConfigureRenderer();

            if (pointLight != null)
            {
                ConfigurePointLight();
            }

            Debug.Log("DarkCastingMuzzleFlashConfig: Configuration applied successfully!");
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
            var emission = _particleSystem.emission;

            // Check duration
            if (!Mathf.Approximately(main.duration, duration))
            {
                Debug.LogWarning($"Duration mismatch: Expected {duration}, got {main.duration}");
                isValid = false;
            }

            // Check lifetime
            if (!Mathf.Approximately(main.startLifetime.constant, particleLifetime))
            {
                Debug.LogWarning($"Lifetime mismatch: Expected {particleLifetime}, got {main.startLifetime.constant}");
                isValid = false;
            }

            // Check max particles
            if (main.maxParticles != maxParticles)
            {
                Debug.LogWarning($"Max Particles mismatch: Expected {maxParticles}, got {main.maxParticles}");
                isValid = false;
            }

            // Check looping (should be false for one-shot)
            if (main.loop)
            {
                Debug.LogWarning("Loop should be false for one-shot muzzle flash effect!");
                isValid = false;
            }

            // Check play on awake (should be false - triggered manually)
            if (main.playOnAwake)
            {
                Debug.LogWarning("Play On Awake should be false - this is a triggered effect!");
                isValid = false;
            }

            // Check emission bursts
            if (emission.burstCount == 0)
            {
                Debug.LogWarning("No burst configured! This effect requires a burst emission.");
                isValid = false;
            }

            // Check color over lifetime is enabled
            if (!_particleSystem.colorOverLifetime.enabled)
            {
                Debug.LogWarning("Color Over Lifetime module should be enabled!");
                isValid = false;
            }

            // Check size over lifetime is enabled
            if (!_particleSystem.sizeOverLifetime.enabled)
            {
                Debug.LogWarning("Size Over Lifetime module should be enabled!");
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

        #endregion

        #region Configuration Methods

        private void ConfigureMainModule()
        {
            var main = _particleSystem.main;
            main.duration = duration;
            main.loop = false; // One-shot effect
            main.startDelay = 0f;
            main.startLifetime = particleLifetime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(minStartSpeed, maxStartSpeed);
            main.startSize = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.startColor = new ParticleSystem.MinMaxGradient(startColor, midColor);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.simulationSpeed = 1f;
            main.playOnAwake = false; // Triggered on spell cast
            main.maxParticles = maxParticles;
            main.stopAction = ParticleSystemStopAction.Destroy; // Auto cleanup for pooling
        }

        private void ConfigureEmissionModule()
        {
            var emission = _particleSystem.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f; // No continuous emission

            // Configure burst
            ParticleSystem.Burst burst = new ParticleSystem.Burst(
                _time: 0f,
                _count: new ParticleSystem.MinMaxCurve(minBurstCount, maxBurstCount),
                _cycleCount: 1,
                _repeatInterval: 0f
            );

            emission.SetBurst(0, burst);
            emission.burstCount = 1;
        }

        private void ConfigureShapeModule()
        {
            var shape = _particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = coneAngle;
            shape.radius = coneRadius;
            shape.radiusThickness = 1f;
            shape.arc = 360f;
            shape.arcMode = ParticleSystemShapeMultiModeValue.Random;
            shape.length = 0f;
            shape.arcSpread = 0f;
        }

        private void ConfigureColorOverLifetime()
        {
            var col = _particleSystem.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(startColor, 0f),      // Bright orange
                    new GradientColorKey(midColor, 0.3f),      // Orange-red
                    new GradientColorKey(endColor, 0.6f),      // Dark red
                    new GradientColorKey(endColor, 1f)         // Dark red
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),              // Full opacity
                    new GradientAlphaKey(1f, 0.5f),            // Full opacity
                    new GradientAlphaKey(0.5f, 0.8f),          // Fade out
                    new GradientAlphaKey(0f, 1f)               // Transparent
                }
            );

            col.color = gradient;
        }

        private void ConfigureSizeOverLifetime()
        {
            var size = _particleSystem.sizeOverLifetime;
            size.enabled = true;

            // Quick expansion then fade: 1.0 → 1.5 → 0
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 1f);      // Start at normal size
            curve.AddKey(0.2f, 1.5f);  // Quick expansion
            curve.AddKey(1f, 0f);      // Shrink to nothing

            // Set curve tangents for smooth interpolation
            for (int i = 0; i < curve.keys.Length; i++)
            {
                curve.SmoothTangents(i, 0f);
            }

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
                renderer.maxParticleSize = 5.0f;
                renderer.alignment = ParticleSystemRenderSpace.View;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.allowRoll = false;
            }
        }

        private void ConfigurePointLight()
        {
            pointLight.type = LightType.Point;
            pointLight.color = new Color(1f, 0.5f, 0f); // Bright orange
            pointLight.intensity = lightIntensity;
            pointLight.range = lightRange;
            pointLight.shadows = LightShadows.None; // Performance
            pointLight.renderMode = LightRenderMode.Auto;
        }

        #endregion

        #region Debug Helpers

        /// <summary>
        /// Print current configuration to console
        /// </summary>
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
            var shape = _particleSystem.shape;

            Debug.Log("=== Dark Casting Muzzle Flash Configuration ===");
            Debug.Log($"Duration: {main.duration}s (One-shot: {!main.loop})");
            Debug.Log($"Lifetime: {main.startLifetime.constant}s");
            Debug.Log($"Start Size: {main.startSize.constantMin} - {main.startSize.constantMax}");
            Debug.Log($"Start Speed: {main.startSpeed.constantMin} - {main.startSpeed.constantMax}");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Burst Count: {minBurstCount}-{maxBurstCount} particles");
            Debug.Log($"Shape: {shape.shapeType}, Angle: {shape.angle}°, Radius: {shape.radius}");
            Debug.Log($"Simulation Space: {main.simulationSpace}");
            Debug.Log($"Play On Awake: {main.playOnAwake} (Should be false - triggered effect)");
            Debug.Log($"Start Color Range: {main.startColor.colorMin} to {main.startColor.colorMax}");

            if (pointLight != null)
            {
                Debug.Log($"Point Light: Intensity {pointLight.intensity}, Range {pointLight.range}");
            }
            else
            {
                Debug.Log("Point Light: Not configured");
            }

            Debug.Log("===============================================");
        }

        /// <summary>
        /// Get performance statistics
        /// </summary>
        [ContextMenu("Show Performance Stats")]
        public void ShowPerformanceStats()
        {
            if (_particleSystem == null)
            {
                Debug.LogError("No ParticleSystem found!");
                return;
            }

            int currentParticleCount = _particleSystem.particleCount;
            var main = _particleSystem.main;

            Debug.Log("=== Performance Statistics ===");
            Debug.Log($"Current Particles: {currentParticleCount}");
            Debug.Log($"Max Particles Cap: {main.maxParticles}");
            Debug.Log($"Burst Size: {minBurstCount}-{maxBurstCount} particles");
            Debug.Log($"Effect Duration: {duration}s");
            Debug.Log($"Particle Lifetime: {particleLifetime}s");

            // Mobile budget check
            bool withinBudget = main.maxParticles <= 5;
            Debug.Log($"Mobile Budget (≤5 for muzzle flash): {(withinBudget ? "✓ PASS" : "✗ FAIL")}");

            // Priority check
            Debug.Log("Priority: P0 (Critical - Must have for MVP)");
            Debug.Log("Type: One-shot burst effect (triggered on spell cast)");

            Debug.Log("==============================");
        }

        /// <summary>
        /// Simulate the effect (for testing in editor)
        /// </summary>
        [ContextMenu("Test Effect")]
        public void TestEffect()
        {
            if (_particleSystem == null)
            {
                Debug.LogError("No ParticleSystem found!");
                return;
            }

            _particleSystem.Clear();
            _particleSystem.Play();

            if (pointLight != null)
            {
                pointLight.enabled = true;
                // In production, use a coroutine or script to fade out after lightDuration
            }

            Debug.Log("Muzzle Flash effect triggered for testing!");
        }

        #endregion

        #region Public API for Runtime Usage

        /// <summary>
        /// Play the muzzle flash effect (call this when player casts spell)
        /// </summary>
        public void PlayEffect()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Clear();
                _particleSystem.Play();
            }

            if (pointLight != null)
            {
                pointLight.enabled = true;
                StartCoroutine(FadeLightCoroutine());
            }
        }

        /// <summary>
        /// Stop the effect immediately
        /// </summary>
        public void StopEffect()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (pointLight != null)
            {
                pointLight.enabled = false;
            }
        }

        private System.Collections.IEnumerator FadeLightCoroutine()
        {
            if (pointLight == null) yield break;

            float elapsed = 0f;
            float startIntensity = lightIntensity;

            while (elapsed < lightDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lightDuration;
                pointLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
                yield return null;
            }

            pointLight.enabled = false;
            pointLight.intensity = startIntensity; // Reset for next use
        }

        #endregion
    }
}
