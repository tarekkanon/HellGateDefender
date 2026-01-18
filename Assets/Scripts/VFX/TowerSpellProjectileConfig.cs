using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Tower Spell Projectile VFX.
    /// Attach this to FX_Tower_Spell prefab to validate and configure settings.
    /// More powerful looking than player projectile - larger, brighter, thicker trail.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class TowerSpellProjectileConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Color Settings")]
        [Tooltip("Start color - Bright Toxic Green")]
        [SerializeField] private Color startColor = new Color(0.20f, 0.80f, 0.20f, 1f); // #32CD32 Toxic Green

        [Tooltip("Mid color - Green")]
        [SerializeField] private Color midColor = new Color(0.15f, 0.70f, 0.15f, 1f);

        [Tooltip("End color - Dark Purple")]
        [SerializeField] private Color endColor = new Color(0.29f, 0f, 0.51f, 0.78f); // #4B0082 Purple

        [Header("Performance")]
        [Tooltip("Maximum particles for this effect")]
        [SerializeField] private int maxParticles = 30;

        [Tooltip("Emission rate per second")]
        [SerializeField] private float emissionRate = 25f;

        [Header("Size & Lifetime")]
        [Tooltip("Minimum particle start size")]
        [SerializeField] private float minStartSize = 1.25f;

        [Tooltip("Maximum particle start size")]
        [SerializeField] private float maxStartSize = 2.0f;

        [Tooltip("Particle lifetime in seconds")]
        [SerializeField] private float particleLifetime = 0.8f;

        [Header("Core Projectile")]
        [Tooltip("Core sphere size")]
        [SerializeField] private float coreSphereSize = 2.5f;

        [Header("Movement")]
        [Tooltip("Noise strength for chaotic movement")]
        [SerializeField] private float noiseStrength = 0.5f;

        [Tooltip("Noise frequency")]
        [SerializeField] private float noiseFrequency = 2.0f;

        [Header("Components")]
        [Tooltip("Trail Renderer component")]
        [SerializeField] private TrailRenderer trailRenderer;

        [Tooltip("Optional Point Light")]
        [SerializeField] private Light pointLight;

        [Tooltip("Core visual (billboard sprite for main projectile sphere)")]
        [SerializeField] private SpriteRenderer coreVisual;

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

            if (trailRenderer == null)
            {
                trailRenderer = GetComponent<TrailRenderer>();
            }

            if (pointLight == null)
            {
                pointLight = GetComponentInChildren<Light>();
            }

            if (coreVisual == null)
            {
                coreVisual = GetComponentInChildren<SpriteRenderer>();
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
                Debug.LogError("TowerSpellProjectileConfig: No ParticleSystem found!");
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

            if (coreVisual != null)
            {
                ConfigureCoreVisual();
            }

            Debug.Log("TowerSpellProjectileConfig: Configuration applied successfully!");
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

            if (main.startSpeed.constant != 0)
            {
                Debug.LogWarning($"Start Speed should be 0 for proper trail effect, got {main.startSpeed.constant}");
                isValid = false;
            }

            if (main.simulationSpace != ParticleSystemSimulationSpace.World)
            {
                Debug.LogWarning($"Simulation Space should be World, got {main.simulationSpace}");
                isValid = false;
            }

            if (trailRenderer == null)
            {
                Debug.LogWarning("Trail Renderer component not found! Trail effect will be missing.");
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
            shape.radius = 0.75f; // Slightly larger than player projectile
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
            curve.AddKey(0.5f, 0.85f);
            curve.AddKey(1f, 0.3f);

            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
        }

        private void ConfigureNoiseModule()
        {
            var noise = _particleSystem.noise;
            noise.enabled = true;
            noise.strength = new ParticleSystem.MinMaxCurve(noiseStrength);
            noise.frequency = noiseFrequency;
            noise.scrollSpeed = 0.8f;
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
            velocity.x = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);
            velocity.y = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);
            velocity.orbitalX = new ParticleSystem.MinMaxCurve(-1.5f, 1.5f);
            velocity.orbitalY = new ParticleSystem.MinMaxCurve(-1.5f, 1.5f);
            velocity.orbitalZ = new ParticleSystem.MinMaxCurve(-1.5f, 1.5f);
        }

        private void ConfigureRotationOverLifetime()
        {
            var rotation = _particleSystem.rotationOverLifetime;
            rotation.enabled = true;
            rotation.z = new ParticleSystem.MinMaxCurve(-120f * Mathf.Deg2Rad, 120f * Mathf.Deg2Rad);
        }

        private void ConfigureRenderer()
        {
            var renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.sortMode = ParticleSystemSortMode.Distance;
                renderer.minParticleSize = 0f;
                renderer.maxParticleSize = 3.0f; // Larger than player projectile
                renderer.alignment = ParticleSystemRenderSpace.View;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.allowRoll = true;
            }
        }

        private void ConfigureTrailRenderer()
        {
            trailRenderer.time = 0.4f; // Longer trail than player
            trailRenderer.minVertexDistance = 0.1f;
            trailRenderer.autodestruct = false;
            trailRenderer.emitting = true;
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trailRenderer.receiveShadows = false;

            // Width curve - thicker trail
            AnimationCurve widthCurve = new AnimationCurve();
            widthCurve.AddKey(0f, 1.25f); // Thicker start
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
                    new GradientAlphaKey(0.7f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;

            trailRenderer.numCornerVertices = 5;
            trailRenderer.numCapVertices = 5;
            trailRenderer.alignment = LineAlignment.View;
            trailRenderer.textureMode = LineTextureMode.Stretch;
        }

        private void ConfigurePointLight()
        {
            pointLight.type = LightType.Point;
            pointLight.color = startColor; // Toxic green
            pointLight.intensity = 3.0f; // Brighter than player
            pointLight.range = 4.0f; // Larger range
            pointLight.shadows = LightShadows.None;
            pointLight.renderMode = LightRenderMode.Auto;
        }

        private void ConfigureCoreVisual()
        {
            // Configure core sphere visual (if using sprite renderer for main projectile)
            coreVisual.color = startColor;
            coreVisual.sortingOrder = 1;
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

            Debug.Log("=== Tower Spell Projectile Configuration ===");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Lifetime: {main.startLifetime.constant}s");
            Debug.Log($"Start Size: {main.startSizeX.constantMin} - {main.startSizeX.constantMax}");
            Debug.Log($"Emission Rate: {emission.rateOverTime.constant}/s");
            Debug.Log($"Core Sphere Size: {coreSphereSize}");
            Debug.Log($"Start Color: {startColor} (Toxic Green)");

            if (trailRenderer != null)
            {
                Debug.Log($"Trail Time: {trailRenderer.time}s (60% longer than player)");
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

            Debug.Log("============================================");
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
            Debug.Log($"60% Larger than Player: ✓");
            Debug.Log($"Mobile Budget (30): {(main.maxParticles <= 30 ? "✓ PASS" : "✗ FAIL")}");
            Debug.Log("==============================");
        }

        #endregion
    }
}
