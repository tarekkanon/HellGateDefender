using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Active Tower - Idle Glow VFX.
    /// Attach this to FX_Tower_IdleGlow prefab to validate and configure settings.
    /// Combines particle system with material emissive glow animation.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class TowerIdleGlowConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Color Settings")]
        [Tooltip("Toxic Green - Primary color")]
        [SerializeField] private Color greenColor = new Color(0.20f, 0.80f, 0.20f, 1f); // #32CD32

        [Tooltip("Dark Purple - Secondary color")]
        [SerializeField] private Color purpleColor = new Color(0.29f, 0f, 0.51f, 1f); // #4B0082

        [Header("Performance")]
        [Tooltip("Maximum particles for this effect")]
        [SerializeField] private int maxParticles = 15;

        [Tooltip("Emission rate per second")]
        [SerializeField] private float emissionRate = 4f;

        [Header("Size & Lifetime")]
        [Tooltip("Minimum particle start size")]
        [SerializeField] private float minStartSize = 0.5f;

        [Tooltip("Maximum particle start size")]
        [SerializeField] private float maxStartSize = 1.0f;

        [Tooltip("Particle lifetime in seconds")]
        [SerializeField] private float particleLifetime = 2.0f;

        [Header("Orbit Settings")]
        [Tooltip("Orbit radius around tower")]
        [SerializeField] private float orbitRadius = 4.0f;

        [Tooltip("Orbit speed")]
        [SerializeField] private float orbitSpeed = 0.5f;

        [Header("Material Pulse Animation")]
        [Tooltip("Tower mesh renderer for emissive glow")]
        [SerializeField] private Renderer towerRenderer;

        [Tooltip("Enable material pulse animation")]
        [SerializeField] private bool enableMaterialPulse = true;

        [Tooltip("Minimum emission intensity")]
        [SerializeField] private float minEmissionIntensity = 1.5f;

        [Tooltip("Maximum emission intensity")]
        [SerializeField] private float maxEmissionIntensity = 2.5f;

        [Tooltip("Pulse frequency (seconds per cycle)")]
        [SerializeField] private float pulseFrequency = 2f;

        private ParticleSystem _particleSystem;
        private MaterialPropertyBlock _propertyBlock;
        private float _pulseTimer;

        #region Unity Lifecycle

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _propertyBlock = new MaterialPropertyBlock();

            if (autoConfigureOnAwake)
            {
                ConfigureParticleSystem();
            }
        }

        private void Update()
        {
            if (enableMaterialPulse && towerRenderer != null)
            {
                UpdateMaterialPulse();
            }
        }

        private void OnValidate()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
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
                Debug.LogError("TowerIdleGlowConfig: No ParticleSystem found!");
                return;
            }

            ConfigureMainModule();
            ConfigureEmissionModule();
            ConfigureShapeModule();
            ConfigureColorOverLifetime();
            ConfigureSizeOverLifetime();
            ConfigureVelocityOverLifetime();
            ConfigureRotationOverLifetime();
            ConfigureRenderer();

            Debug.Log("TowerIdleGlowConfig: Configuration applied successfully!");
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

            if (main.maxParticles != maxParticles)
            {
                Debug.LogWarning($"Max Particles mismatch: Expected {maxParticles}, got {main.maxParticles}");
                isValid = false;
            }

            if (!Mathf.Approximately(main.startLifetime.constant, particleLifetime))
            {
                Debug.LogWarning($"Lifetime mismatch: Expected {particleLifetime}, got {main.startLifetime.constant}");
                isValid = false;
            }

            if (main.simulationSpace != ParticleSystemSimulationSpace.World)
            {
                Debug.LogWarning($"Simulation Space should be World, got {main.simulationSpace}");
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
            main.duration = 5.0f;
            main.loop = true;
            main.startDelay = 0f;
            main.startLifetime = particleLifetime;
            main.startSpeed = 0.5f; // Slow initial movement
            main.startSize3D = false;
            main.startSize = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.startColor = greenColor;
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.simulationSpeed = 1f;
            main.playOnAwake = true;
            main.maxParticles = maxParticles;
        }

        private void ConfigureEmissionModule()
        {
            var emission = _particleSystem.emission;
            emission.enabled = true;
            emission.rateOverTime = emissionRate;
        }

        private void ConfigureShapeModule()
        {
            var shape = _particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = orbitRadius;
            shape.radiusThickness = 0.1f; // Thin ring
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
                    new GradientColorKey(greenColor, 0f),
                    new GradientColorKey(greenColor, 0.3f),
                    new GradientColorKey(purpleColor, 0.6f),
                    new GradientColorKey(purpleColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),      // Fade in
                    new GradientAlphaKey(0.6f, 0.2f),  // Peak
                    new GradientAlphaKey(0.6f, 0.7f),  // Hold
                    new GradientAlphaKey(0f, 1f)       // Fade out
                }
            );

            col.color = gradient;
        }

        private void ConfigureSizeOverLifetime()
        {
            var size = _particleSystem.sizeOverLifetime;
            size.enabled = true;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0.5f);
            curve.AddKey(0.5f, 1.0f);
            curve.AddKey(1f, 0.6f);

            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }

        private void ConfigureVelocityOverLifetime()
        {
            var velocity = _particleSystem.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.Local;

            // Linear velocity (all must use same curve mode)
            velocity.x = new ParticleSystem.MinMaxCurve(0f, 0f);
            velocity.y = new ParticleSystem.MinMaxCurve(0.3f, 0.3f); // Slight upward drift
            velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

            // Orbital motion around tower
            velocity.orbitalX = new ParticleSystem.MinMaxCurve(0.2f, 0.2f);
            velocity.orbitalY = new ParticleSystem.MinMaxCurve(orbitSpeed, orbitSpeed);
            velocity.orbitalZ = new ParticleSystem.MinMaxCurve(0.2f, 0.2f);
        }

        private void ConfigureRotationOverLifetime()
        {
            var rotation = _particleSystem.rotationOverLifetime;
            rotation.enabled = true;
            rotation.z = new ParticleSystem.MinMaxCurve(-45f * Mathf.Deg2Rad, 45f * Mathf.Deg2Rad);
        }

        private void ConfigureRenderer()
        {
            var renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.sortMode = ParticleSystemSortMode.Distance;
                renderer.minParticleSize = 0f;
                renderer.maxParticleSize = 1.5f;
                renderer.alignment = ParticleSystemRenderSpace.View;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        #endregion

        #region Material Pulse Animation

        private void UpdateMaterialPulse()
        {
            if (towerRenderer == null) return;

            // Update pulse timer
            _pulseTimer += Time.deltaTime;

            // Calculate sine wave for pulsing
            float t = Mathf.Sin(_pulseTimer * (Mathf.PI * 2f / pulseFrequency));
            float intensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, (t + 1f) * 0.5f);

            // Apply emission to material using property block
            Color emissionColor = greenColor * intensity;

            towerRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_EmissionColor", emissionColor);
            towerRenderer.SetPropertyBlock(_propertyBlock);
        }

        /// <summary>
        /// Enable material pulse effect
        /// </summary>
        public void EnableMaterialPulse(bool enable)
        {
            enableMaterialPulse = enable;
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

            Debug.Log("=== Tower Idle Glow Configuration ===");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Lifetime: {main.startLifetime.constant}s");
            Debug.Log($"Start Size: {main.startSize.constantMin} - {main.startSize.constantMax}");
            Debug.Log($"Emission Rate: {emission.rateOverTime.constant}/s");
            Debug.Log($"Orbit Radius: {orbitRadius}");
            Debug.Log($"Orbit Speed: {orbitSpeed}");
            Debug.Log($"Material Pulse: {(enableMaterialPulse ? "Enabled" : "Disabled")}");

            if (towerRenderer != null)
            {
                Debug.Log($"Tower Renderer: Assigned ({towerRenderer.name})");
            }
            else
            {
                Debug.Log("Tower Renderer: Not assigned");
            }

            Debug.Log("=====================================");
        }

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
            var emission = _particleSystem.emission;

            float lifetime = main.startLifetime.constant;
            float emissionRateValue = emission.rateOverTime.constant;
            int theoreticalMax = Mathf.CeilToInt(lifetime * emissionRateValue);

            Debug.Log("=== Performance Statistics ===");
            Debug.Log($"Current Particles: {currentParticleCount}");
            Debug.Log($"Max Particles Cap: {main.maxParticles}");
            Debug.Log($"Theoretical Max: {theoreticalMax}");
            Debug.Log($"Emission Rate: {emissionRateValue}/s");
            Debug.Log($"Particle Lifetime: {lifetime}s");
            Debug.Log($"Mobile Budget Check: {(main.maxParticles <= 15 ? "✓ PASS" : "✗ FAIL")}");
            Debug.Log("==============================");
        }

        #endregion
    }
}
