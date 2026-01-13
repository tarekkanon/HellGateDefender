using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Demonic Spell Projectile VFX.
    /// Attach this to FX_Player_DarkSpell prefab to validate and configure settings.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class DemonicSpellProjectileConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Color Settings")]
        [Tooltip("Start color - Crimson Red")]
        [SerializeField] private Color startColor = new Color(1f, 0.27f, 0f, 1f); // RGB(255, 69, 0)

        [Tooltip("Mid color - Crimson")]
        [SerializeField] private Color midColor = new Color(0.86f, 0.08f, 0.24f, 1f); // RGB(220, 20, 60)

        [Tooltip("End color - Dark Purple")]
        [SerializeField] private Color endColor = new Color(0.29f, 0f, 0.51f, 0.78f); // RGB(75, 0, 130) Alpha 200

        [Header("Performance")]
        [Tooltip("Maximum particles for this effect")]
        [SerializeField] private int maxParticles = 20;

        [Tooltip("Emission rate per second")]
        [SerializeField] private float emissionRate = 15f;

        [Header("Size & Lifetime")]
        [Tooltip("Minimum particle start size")]
        [SerializeField] private float minStartSize = 0.2f;

        [Tooltip("Maximum particle start size")]
        [SerializeField] private float maxStartSize = 0.3f;

        [Tooltip("Particle lifetime in seconds")]
        [SerializeField] private float particleLifetime = 0.5f;

        [Header("Movement")]
        [Tooltip("Noise strength for chaotic movement")]
        [SerializeField] private float noiseStrength = 0.4f;

        [Tooltip("Noise frequency")]
        [SerializeField] private float noiseFrequency = 1.5f;

        [Header("Components")]
        [Tooltip("Trail Renderer component")]
        [SerializeField] private TrailRenderer trailRenderer;

        [Tooltip("Optional Point Light")]
        [SerializeField] private Light pointLight;

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

            if (trailRenderer == null)
            {
                trailRenderer = GetComponent<TrailRenderer>();
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
                Debug.LogError("DemonicSpellProjectileConfig: No ParticleSystem found!");
                return;
            }

            ConfigureMainModule();
            ConfigureEmissionModule();
            ConfigureShapeModule();
            ConfigureColorOverLifetime();
            ConfigureSizeOverLifetime();
            ConfigureNoiseModule();
            ConfigureVelocityOverLifetime();
            ConfigureRotationOverLifetime();
            ConfigureRenderer();

            if (trailRenderer != null)
            {
                ConfigureTrailRenderer();
            }

            if (pointLight != null)
            {
                ConfigurePointLight();
            }

            Debug.Log("DemonicSpellProjectileConfig: Configuration applied successfully!");
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

            // Check max particles
            if (main.maxParticles != maxParticles)
            {
                Debug.LogWarning($"Max Particles mismatch: Expected {maxParticles}, got {main.maxParticles}");
                isValid = false;
            }

            // Check lifetime
            if (!Mathf.Approximately(main.startLifetime.constant, particleLifetime))
            {
                Debug.LogWarning($"Lifetime mismatch: Expected {particleLifetime}, got {main.startLifetime.constant}");
                isValid = false;
            }

            // Check start speed (should be 0 for world-space trail)
            if (main.startSpeed.constant != 0)
            {
                Debug.LogWarning($"Start Speed should be 0 for proper trail effect, got {main.startSpeed.constant}");
                isValid = false;
            }

            // Check simulation space
            if (main.simulationSpace != ParticleSystemSimulationSpace.World)
            {
                Debug.LogWarning($"Simulation Space should be World, got {main.simulationSpace}");
                isValid = false;
            }

            // Check emission rate
            var emission = _particleSystem.emission;
            if (!Mathf.Approximately(emission.rateOverTime.constant, emissionRate))
            {
                Debug.LogWarning($"Emission Rate mismatch: Expected {emissionRate}, got {emission.rateOverTime.constant}");
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

            // Check trail renderer
            if (trailRenderer == null)
            {
                Debug.LogWarning("Trail Renderer component not found! Trail effect will be missing.");
            }
            else if (trailRenderer.time < 0.2f || trailRenderer.time > 0.4f)
            {
                Debug.LogWarning($"Trail time should be around 0.3s, got {trailRenderer.time}");
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
            main.startSpeed = 0f;
            main.startSize3D = true;
            main.startSizeX = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startSizeY = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startSizeZ = new ParticleSystem.MinMaxCurve(minStartSize, maxStartSize);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.startColor = startColor;
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
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.1f;
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
                    new GradientColorKey(startColor, 0f),
                    new GradientColorKey(midColor, 0.4f),
                    new GradientColorKey(endColor, 0.7f),
                    new GradientColorKey(endColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(1f, 0.4f),
                    new GradientAlphaKey(0.78f, 0.7f),
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
            curve.AddKey(0.5f, 0.8f);
            curve.AddKey(1f, 0.2f);

            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }

        private void ConfigureNoiseModule()
        {
            var noise = _particleSystem.noise;
            noise.enabled = true;
            noise.strength = new ParticleSystem.MinMaxCurve(noiseStrength);
            noise.frequency = noiseFrequency;
            noise.scrollSpeed = 0.5f;
            noise.damping = true;
            noise.octaveCount = 2;
            noise.octaveMultiplier = 0.5f;
            noise.octaveScale = 2f;
            noise.quality = ParticleSystemNoiseQuality.High;
        }

        private void ConfigureVelocityOverLifetime()
        {
            var velocity = _particleSystem.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
            velocity.y = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
            velocity.orbitalX = new ParticleSystem.MinMaxCurve(-1f, 1f);
            velocity.orbitalY = new ParticleSystem.MinMaxCurve(-1f, 1f);
            velocity.orbitalZ = new ParticleSystem.MinMaxCurve(-1f, 1f);
        }

        private void ConfigureRotationOverLifetime()
        {
            var rotation = _particleSystem.rotationOverLifetime;
            rotation.enabled = true;
            rotation.z = new ParticleSystem.MinMaxCurve(-90f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);
        }

        private void ConfigureRenderer()
        {
            var renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.sortMode = ParticleSystemSortMode.Distance;
                renderer.minParticleSize = 0f;
                renderer.maxParticleSize = 0.5f;
                renderer.alignment = ParticleSystemRenderSpace.View;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.allowRoll = true;
            }
        }

        private void ConfigureTrailRenderer()
        {
            trailRenderer.time = 0.3f;
            trailRenderer.minVertexDistance = 0.1f;
            trailRenderer.autodestruct = false;
            trailRenderer.emitting = true;
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trailRenderer.receiveShadows = false;

            // Width curve
            AnimationCurve widthCurve = new AnimationCurve();
            widthCurve.AddKey(0f, 0.15f);
            widthCurve.AddKey(1f, 0f);
            trailRenderer.widthCurve = widthCurve;

            // Color gradient
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(startColor, 0f),
                    new GradientColorKey(midColor, 0.5f),
                    new GradientColorKey(endColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.6f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;

            trailRenderer.numCornerVertices = 4;
            trailRenderer.numCapVertices = 4;
            trailRenderer.alignment = LineAlignment.View;
            trailRenderer.textureMode = LineTextureMode.Stretch;
        }

        private void ConfigurePointLight()
        {
            pointLight.type = LightType.Point;
            pointLight.color = new Color(1f, 0.27f, 0f); // Red-orange
            pointLight.intensity = 2.0f;
            pointLight.range = 3.0f;
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

            Debug.Log("=== Demonic Spell Projectile Configuration ===");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Lifetime: {main.startLifetime.constant}s");
            Debug.Log($"Start Size: {main.startSizeX.constantMin} - {main.startSizeX.constantMax}");
            Debug.Log($"Emission Rate: {emission.rateOverTime.constant}/s");
            Debug.Log($"Shape: {shape.shapeType}, Radius: {shape.radius}");
            Debug.Log($"Simulation Space: {main.simulationSpace}");
            Debug.Log($"Start Color: {main.startColor.color}");

            if (trailRenderer != null)
            {
                Debug.Log($"Trail Time: {trailRenderer.time}s");
                Debug.Log($"Trail Start Width: {trailRenderer.widthCurve.Evaluate(0)}");
            }
            else
            {
                Debug.Log("Trail Renderer: Not found");
            }

            if (pointLight != null)
            {
                Debug.Log($"Point Light: Intensity {pointLight.intensity}, Range {pointLight.range}");
            }
            else
            {
                Debug.Log("Point Light: Not found");
            }

            Debug.Log("==============================================");
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
            var emission = _particleSystem.emission;

            // Calculate theoretical max
            float lifetime = main.startLifetime.constant;
            float emissionRate = emission.rateOverTime.constant;
            int theoreticalMax = Mathf.CeilToInt(lifetime * emissionRate);

            Debug.Log("=== Performance Statistics ===");
            Debug.Log($"Current Particles: {currentParticleCount}");
            Debug.Log($"Max Particles Cap: {main.maxParticles}");
            Debug.Log($"Theoretical Max: {theoreticalMax}");
            Debug.Log($"Emission Rate: {emissionRate}/s");
            Debug.Log($"Particle Lifetime: {lifetime}s");

            // Mobile budget check
            bool withinBudget = main.maxParticles <= 20;
            Debug.Log($"Mobile Budget (20): {(withinBudget ? "✓ PASS" : "✗ FAIL")}");

            Debug.Log("==============================");
        }

        #endregion
    }
}
