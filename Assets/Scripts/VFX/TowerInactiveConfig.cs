using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Inactive Tower - Dormant State VFX.
    /// Attach this to FX_Tower_Inactive prefab to validate and configure settings.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class TowerInactiveConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Color Settings")]
        [Tooltip("Faint Purple - Residual dark magic")]
        [SerializeField] private Color particleColor = new Color(0.29f, 0f, 0.51f, 0.3f); // RGB(75, 0, 130) Alpha 0.3

        [Header("Performance")]
        [Tooltip("Maximum particles for this effect")]
        [SerializeField] private int maxParticles = 10;

        [Tooltip("Emission rate per second (very subtle)")]
        [SerializeField] private float emissionRate = 2.5f;

        [Header("Size & Lifetime")]
        [Tooltip("Minimum particle start size")]
        [SerializeField] private float minStartSize = 0.75f;

        [Tooltip("Maximum particle start size")]
        [SerializeField] private float maxStartSize = 1.25f;

        [Tooltip("Particle lifetime in seconds")]
        [SerializeField] private float particleLifetime = 2.5f;

        [Header("Movement")]
        [Tooltip("Upward drift speed")]
        [SerializeField] private float upwardSpeed = 0.5f;

        [Tooltip("Horizontal wander strength")]
        [SerializeField] private float wanderStrength = 0.3f;

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
                Debug.LogError("TowerInactiveConfig: No ParticleSystem found!");
                return;
            }

            ConfigureMainModule();
            ConfigureEmissionModule();
            ConfigureShapeModule();
            ConfigureColorOverLifetime();
            ConfigureSizeOverLifetime();
            ConfigureVelocityOverLifetime();
            ConfigureNoiseModule();
            ConfigureRenderer();

            Debug.Log("TowerInactiveConfig: Configuration applied successfully!");
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

            if (!_particleSystem.colorOverLifetime.enabled)
            {
                Debug.LogWarning("Color Over Lifetime module should be enabled!");
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
            main.startSpeed = upwardSpeed;
            main.startSize3D = false;
            main.startSize = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.startColor = particleColor;
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
            shape.radius = 2.5f;
            shape.radiusThickness = 0.2f; // Spawn near edge
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
                    new GradientColorKey(particleColor, 0f),
                    new GradientColorKey(particleColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),      // Fade in
                    new GradientAlphaKey(0.4f, 0.2f),  // Peak at 20%
                    new GradientAlphaKey(0.4f, 0.7f),  // Hold
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
            curve.AddKey(0f, 0.8f);
            curve.AddKey(0.5f, 1.0f);
            curve.AddKey(1f, 0.7f);

            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }

        private void ConfigureVelocityOverLifetime()
        {
            var velocity = _particleSystem.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = new ParticleSystem.MinMaxCurve(-wanderStrength, wanderStrength);
            velocity.y = new ParticleSystem.MinMaxCurve(0f, 0f); // No vertical velocity (upward motion in start speed)
            velocity.z = new ParticleSystem.MinMaxCurve(-wanderStrength, wanderStrength);
        }

        private void ConfigureNoiseModule()
        {
            var noise = _particleSystem.noise;
            noise.enabled = true;
            noise.strength = new ParticleSystem.MinMaxCurve(0.2f);
            noise.frequency = 0.5f; // Gentle
            noise.scrollSpeed = 0.2f;
            noise.damping = true;
            noise.octaveCount = 1;
            noise.quality = ParticleSystemNoiseQuality.Medium;
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

            Debug.Log("=== Inactive Tower Configuration ===");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Lifetime: {main.startLifetime.constant}s");
            Debug.Log($"Start Size: {main.startSize.constantMin} - {main.startSize.constantMax}");
            Debug.Log($"Emission Rate: {emission.rateOverTime.constant}/s");
            Debug.Log($"Upward Speed: {upwardSpeed}");
            Debug.Log($"Particle Color: {particleColor}");
            Debug.Log("=====================================");
        }

        #endregion
    }
}
