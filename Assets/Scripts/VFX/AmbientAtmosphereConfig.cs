using UnityEngine;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Hell Gate Ambient Atmosphere VFX.
    /// Attach this to FX_Ambient_Atmosphere prefab to validate and configure settings.
    /// Creates atmospheric particle system with floating embers, dark wisps, and energy motes.
    /// </summary>
    public class AmbientAtmosphereConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("System 1: Floating Embers")]
        [SerializeField] private ParticleSystem floatingEmbers;
        [Tooltip("Orange-red ember color with low alpha")]
        [SerializeField] private Color emberColor = new Color(1f, 0.27f, 0f, 0.4f); // #FF4500 with alpha

        [Header("System 2: Dark Wisps")]
        [SerializeField] private ParticleSystem darkWisps;
        [Tooltip("Dark purple wisps - very faint")]
        [SerializeField] private Color wispColor = new Color(0.29f, 0f, 0.51f, 0.2f); // #4B0082 with low alpha

        [Header("System 3: Energy Motes (Near Hell Gate)")]
        [SerializeField] private ParticleSystem energyMotes;
        [Tooltip("Toxic green energy particles")]
        [SerializeField] private Color moteColor = new Color(0.20f, 0.80f, 0.20f, 0.4f); // #32CD32 with alpha

        [Header("Spawn Volume Settings")]
        [Tooltip("Size of the spawn area for embers and wisps")]
        [SerializeField] private Vector3 spawnVolumeSize = new Vector3(20f, 10f, 20f);
        [Tooltip("Radius for energy motes spiral around center")]
        [SerializeField] private float moteOrbitRadius = 5f;

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
                if (floatingEmbers == null) floatingEmbers = particles[0];
                if (darkWisps == null) darkWisps = particles[1];
                if (energyMotes == null) energyMotes = particles[2];
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
            ConfigureFloatingEmbers();
            ConfigureDarkWisps();
            ConfigureEnergyMotes();

            Debug.Log("AmbientAtmosphereConfig: All systems configured successfully!");
        }

        /// <summary>
        /// Start playing the ambient atmosphere effect
        /// </summary>
        [ContextMenu("Play Ambient Atmosphere")]
        public void PlayAmbientAtmosphere()
        {
            if (floatingEmbers != null)
            {
                floatingEmbers.Play();
            }

            if (darkWisps != null)
            {
                darkWisps.Play();
            }

            if (energyMotes != null)
            {
                energyMotes.Play();
            }

            Debug.Log("Ambient Atmosphere Started!");
        }

        /// <summary>
        /// Stop the ambient atmosphere effect
        /// </summary>
        [ContextMenu("Stop Ambient Atmosphere")]
        public void StopAmbientAtmosphere()
        {
            if (floatingEmbers != null)
            {
                floatingEmbers.Stop();
            }

            if (darkWisps != null)
            {
                darkWisps.Stop();
            }

            if (energyMotes != null)
            {
                energyMotes.Stop();
            }

            Debug.Log("Ambient Atmosphere Stopped!");
        }

        #endregion

        #region System 1: Floating Embers Configuration

        private void ConfigureFloatingEmbers()
        {
            if (floatingEmbers == null)
            {
                Debug.LogWarning("Floating Embers ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = floatingEmbers.main;
            main.duration = 10f;
            main.loop = true;
            main.startDelay = 0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(8f, 12f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = -0.02f; // Slow upward drift
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 30;
            main.startColor = emberColor;

            // Emission - Low rate for subtle effect
            var emission = floatingEmbers.emission;
            emission.enabled = true;
            emission.rateOverTime = 5f;

            // Shape - Box volume covering the map
            var shape = floatingEmbers.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = spawnVolumeSize;

            // Color Over Lifetime - Fade in and out
            var col = floatingEmbers.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(emberColor, 0f),
                    new GradientColorKey(emberColor, 0.5f),
                    new GradientColorKey(emberColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.4f, 0.2f), // Fade in first 20%
                    new GradientAlphaKey(0.4f, 0.7f),
                    new GradientAlphaKey(0f, 1f)      // Fade out last 30%
                }
            );
            col.color = gradient;

            // Velocity over Lifetime - Gentle horizontal wander
            var vel = floatingEmbers.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.World;
            vel.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
            vel.y = new ParticleSystem.MinMaxCurve(0.1f, 0.3f); // Upward drift
            vel.z = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);

            // Rotation over Lifetime - Slow random rotation
            var rot = floatingEmbers.rotationOverLifetime;
            rot.enabled = true;
            rot.z = new ParticleSystem.MinMaxCurve(-30f * Mathf.Deg2Rad, 30f * Mathf.Deg2Rad);

            // Renderer
            var renderer = floatingEmbers.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.sortingFudge = 1f; // Render in background
            }

            Debug.Log("Floating Embers configured");
        }

        #endregion

        #region System 2: Dark Wisps Configuration

        private void ConfigureDarkWisps()
        {
            if (darkWisps == null)
            {
                Debug.LogWarning("Dark Wisps ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = darkWisps.main;
            main.duration = 10f;
            main.loop = true;
            main.startDelay = 0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 8f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
            main.startSize = new ParticleSystem.MinMaxCurve(3.0f, 5.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 15;
            main.startColor = wispColor;

            // Emission - Very low rate for subtle wisps
            var emission = darkWisps.emission;
            emission.enabled = true;
            emission.rateOverTime = 2.5f;

            // Shape - Same volume as embers
            var shape = darkWisps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = spawnVolumeSize;

            // Color Over Lifetime - Very faint, fade in/out
            var col = darkWisps.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(wispColor, 0f),
                    new GradientColorKey(wispColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.2f, 0.2f), // Fade in
                    new GradientAlphaKey(0.25f, 0.5f),
                    new GradientAlphaKey(0f, 1f)      // Fade out
                }
            );
            col.color = gradient;

            // Noise Module - Meandering path using curl noise
            var noise = darkWisps.noise;
            noise.enabled = true;
            noise.strength = 0.5f;
            noise.frequency = 0.3f;
            noise.scrollSpeed = 0.1f;
            noise.damping = true;
            noise.octaveCount = 2;

            // Size over Lifetime - Grow then shrink
            var size = darkWisps.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.5f);
            sizeCurve.AddKey(0.5f, 1f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = darkWisps.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.sortingFudge = 2f; // Render in background
            }

            Debug.Log("Dark Wisps configured");
        }

        #endregion

        #region System 3: Energy Motes Configuration

        private void ConfigureEnergyMotes()
        {
            if (energyMotes == null)
            {
                Debug.LogWarning("Energy Motes ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = energyMotes.main;
            main.duration = 10f;
            main.loop = true;
            main.startDelay = 0f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(3f, 5f);
            main.startSpeed = 0.5f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 40;
            main.startColor = moteColor;

            // Emission - Higher rate near the gate
            var emission = energyMotes.emission;
            emission.enabled = true;
            emission.rateOverTime = 9f;

            // Shape - Cylinder around base/gate structure
            var shape = energyMotes.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 0f;
            shape.radius = moteOrbitRadius;
            shape.radiusThickness = 1f;
            shape.rotation = new Vector3(-90f, 0f, 0f); // Point upward

            // Color Over Lifetime - Fade in and out
            var col = energyMotes.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(moteColor, 0f),
                    new GradientColorKey(moteColor, 0.5f),
                    new GradientColorKey(moteColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.4f, 0.2f),
                    new GradientAlphaKey(0.4f, 0.8f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Velocity over Lifetime - Spiral upward using orbit velocity
            var vel = energyMotes.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;

            // Use consistent TwoConstants mode for all linear velocities
            vel.x = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.y = new ParticleSystem.MinMaxCurve(0.3f, 0.5f); // Upward drift
            vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);

            // Orbital velocities (separate from linear, must also be consistent)
            vel.orbitalX = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.orbitalY = new ParticleSystem.MinMaxCurve(0.4f, 0.6f); // Orbit around Y axis
            vel.orbitalZ = new ParticleSystem.MinMaxCurve(0f, 0f);

            // Size over Lifetime
            var size = energyMotes.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.5f);
            sizeCurve.AddKey(0.5f, 1f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = energyMotes.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Energy Motes configured");
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Ambient Atmosphere Configuration ===");
            Debug.Log($"Floating Embers: {(floatingEmbers != null ? "Assigned" : "Missing")}");
            Debug.Log($"Dark Wisps: {(darkWisps != null ? "Assigned" : "Missing")}");
            Debug.Log($"Energy Motes: {(energyMotes != null ? "Assigned" : "Missing")}");
            Debug.Log($"Spawn Volume: {spawnVolumeSize}");
            Debug.Log($"Mote Orbit Radius: {moteOrbitRadius}");

            int totalParticles = 0;
            if (floatingEmbers != null) totalParticles += floatingEmbers.main.maxParticles;
            if (darkWisps != null) totalParticles += darkWisps.main.maxParticles;
            if (energyMotes != null) totalParticles += energyMotes.main.maxParticles;

            Debug.Log($"Total Max Particles: {totalParticles}");
            Debug.Log("=========================================");
        }

        /// <summary>
        /// Update spawn volume size at runtime
        /// </summary>
        public void SetSpawnVolume(Vector3 newSize)
        {
            spawnVolumeSize = newSize;

            if (floatingEmbers != null)
            {
                var shape = floatingEmbers.shape;
                shape.scale = spawnVolumeSize;
            }

            if (darkWisps != null)
            {
                var shape = darkWisps.shape;
                shape.scale = spawnVolumeSize;
            }
        }

        /// <summary>
        /// Update mote orbit radius at runtime
        /// </summary>
        public void SetMoteOrbitRadius(float radius)
        {
            moteOrbitRadius = radius;

            if (energyMotes != null)
            {
                var shape = energyMotes.shape;
                shape.radius = moteOrbitRadius;
            }
        }

        #endregion
    }
}
