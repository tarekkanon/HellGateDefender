using UnityEngine;
using System.Collections;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Spawn Portal VFX.
    /// Attach this to FX_Spawn_Portal prefab to validate and configure settings.
    /// Creates a holy portal spawn effect for angel enemies entering the battle.
    /// 3-phase effect: Portal Opening → Portal Active → Portal Closing
    /// </summary>
    public class SpawnPortalConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Phase 1: Opening Ring (0.0-0.5s)")]
        [SerializeField] private ParticleSystem openingRing;
        [Tooltip("Bright gold and white particles forming a circle")]
        [SerializeField] private Color openingColorGold = new Color(1f, 0.84f, 0f, 1f); // #FFD700
        [SerializeField] private Color openingColorWhite = new Color(1f, 1f, 1f, 1f); // Divine White

        [Header("Phase 2: Active Portal (0.5-1.0s)")]
        [SerializeField] private ParticleSystem activePortalParticles;
        [Tooltip("Continuous particles spiraling inward")]
        [SerializeField] private Color activeColorWhite = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color activeColorGold = new Color(1f, 0.84f, 0f, 0.8f);

        [Header("Portal Mesh (Optional)")]
        [SerializeField] private MeshRenderer portalMesh;
        [Tooltip("Vertical disc of light")]
        [SerializeField] private Color portalMeshColor = new Color(1f, 1f, 1f, 0.8f);

        [Header("Phase 3: Closing (1.0-1.5s)")]
        [SerializeField] private ParticleSystem closingBurst;
        [Tooltip("Particles collapse to center")]
        [SerializeField] private Color closingColorWhite = new Color(1f, 1f, 1f, 1f);

        [Header("Point Light")]
        [SerializeField] private Light portalLight;
        [Tooltip("Radiant light during portal active phase")]
        [SerializeField] private Color lightColor = new Color(1f, 0.84f, 0f, 1f); // Gold

        [Header("Portal Settings")]
        [Tooltip("Total duration of portal sequence")]
        [SerializeField] private float totalDuration = 1.5f;
        [Tooltip("Portal disc diameter")]
        [SerializeField] private float portalDiameter = 2.5f;
        [Tooltip("Time at which enemy spawns (during active phase)")]
        [SerializeField] private float enemySpawnTime = 0.7f;

        // Runtime
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _portalSequenceCoroutine;

        // Events
        public event System.Action OnEnemySpawnTime;
        public event System.Action OnPortalComplete;

        #region Unity Lifecycle

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            if (autoConfigureOnAwake)
            {
                ConfigureAllPhases();
            }
        }

        private void OnValidate()
        {
            // Auto-find particle systems
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            if (particles.Length >= 3)
            {
                if (openingRing == null) openingRing = particles[0];
                if (activePortalParticles == null) activePortalParticles = particles[1];
                if (closingBurst == null) closingBurst = particles[2];
            }

            // Auto-find light
            if (portalLight == null)
            {
                portalLight = GetComponentInChildren<Light>();
            }

            // Auto-find mesh
            if (portalMesh == null)
            {
                portalMesh = GetComponentInChildren<MeshRenderer>();
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
            ConfigurePhase1_OpeningRing();
            ConfigurePhase2_ActivePortal();
            ConfigurePhase3_ClosingBurst();
            ConfigurePortalMesh();
            ConfigurePortalLight();

            Debug.Log("SpawnPortalConfig: All phases configured successfully!");
        }

        /// <summary>
        /// Play the full portal spawn sequence
        /// </summary>
        [ContextMenu("Play Portal Sequence")]
        public void PlayPortalSequence()
        {
            if (_portalSequenceCoroutine != null)
            {
                StopCoroutine(_portalSequenceCoroutine);
            }

            _portalSequenceCoroutine = StartCoroutine(PortalSequenceCoroutine());
        }

        /// <summary>
        /// Stop the portal sequence immediately
        /// </summary>
        [ContextMenu("Stop Portal")]
        public void StopPortal()
        {
            if (_portalSequenceCoroutine != null)
            {
                StopCoroutine(_portalSequenceCoroutine);
                _portalSequenceCoroutine = null;
            }

            StopAllParticles();
            HidePortalMesh();
            DisableLight();
        }

        /// <summary>
        /// Get the enemy spawn time for synchronization
        /// </summary>
        public float GetEnemySpawnTime()
        {
            return enemySpawnTime;
        }

        /// <summary>
        /// Get total portal duration
        /// </summary>
        public float GetTotalDuration()
        {
            return totalDuration;
        }

        #endregion

        #region Portal Sequence Coroutine

        private IEnumerator PortalSequenceCoroutine()
        {
            // Phase 1: Opening (0.0 - 0.5s)
            if (openingRing != null)
            {
                openingRing.Play();
            }

            // Start light fade in
            if (portalLight != null)
            {
                StartCoroutine(FadeLightIn(0.5f));
            }

            yield return new WaitForSeconds(0.5f);

            // Phase 2: Active Portal (0.5 - 1.0s)
            if (activePortalParticles != null)
            {
                activePortalParticles.Play();
            }

            // Show portal mesh
            ShowPortalMesh();

            // Wait for enemy spawn time
            yield return new WaitForSeconds(enemySpawnTime - 0.5f);

            // Trigger enemy spawn event
            OnEnemySpawnTime?.Invoke();

            // Wait for remaining active phase
            yield return new WaitForSeconds(1.0f - enemySpawnTime);

            // Phase 3: Closing (1.0 - 1.5s)
            if (activePortalParticles != null)
            {
                activePortalParticles.Stop();
            }

            if (closingBurst != null)
            {
                closingBurst.Play();
            }

            // Start closing portal mesh and light
            StartCoroutine(ClosePortalMesh(0.5f));
            StartCoroutine(FadeLightOut(0.5f));

            yield return new WaitForSeconds(0.5f);

            // Complete
            StopAllParticles();
            HidePortalMesh();
            DisableLight();

            OnPortalComplete?.Invoke();
            Debug.Log("Spawn Portal Sequence Complete!");
        }

        #endregion

        #region Phase 1: Opening Ring Configuration

        private void ConfigurePhase1_OpeningRing()
        {
            if (openingRing == null)
            {
                Debug.LogWarning("Opening Ring ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = openingRing.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.5f;
            main.startSpeed = 0f; // Static particles form a ring
            main.startSize = 3.0f;
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = 16;

            // Emission - Burst to form ring
            var emission = openingRing.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 12, 16, 1)
            });

            // Shape - Circle to form ring
            var shape = openingRing.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = portalDiameter / 2f;
            shape.radiusThickness = 0f; // On edge only
            shape.arc = 360f;
            shape.rotation = new Vector3(90f, 0f, 0f); // Vertical ring

            // Color Over Lifetime - Gold to white glow
            var col = openingRing.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(openingColorGold, 0f),
                    new GradientColorKey(openingColorWhite, 0.5f),
                    new GradientColorKey(openingColorWhite, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 0.3f),
                    new GradientAlphaKey(1f, 0.7f),
                    new GradientAlphaKey(0.8f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime - Scale up from 0
            var size = openingRing.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0f);
            sizeCurve.AddKey(0.5f, 1f);
            sizeCurve.AddKey(1f, 1f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Rotation over Lifetime - Slow spin
            var rot = openingRing.rotationOverLifetime;
            rot.enabled = true;
            rot.z = 30f * Mathf.Deg2Rad; // 30 degrees per second

            // Renderer
            var renderer = openingRing.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 1: Opening Ring configured");
        }

        #endregion

        #region Phase 2: Active Portal Configuration

        private void ConfigurePhase2_ActivePortal()
        {
            if (activePortalParticles == null)
            {
                Debug.LogWarning("Active Portal ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = activePortalParticles.main;
            main.duration = 0.5f;
            main.loop = true;
            main.startDelay = 0f;
            main.startLifetime = 0.8f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
            main.startSize = new ParticleSystem.MinMaxCurve(1.5f, 2.5f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = 30;

            // Emission - Continuous from edges
            var emission = activePortalParticles.emission;
            emission.enabled = true;
            emission.rateOverTime = 20f;

            // Shape - Circle edge spawning inward
            var shape = activePortalParticles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = portalDiameter / 2f;
            shape.radiusThickness = 0f; // On edge only
            shape.arc = 360f;
            shape.rotation = new Vector3(90f, 0f, 0f); // Vertical ring

            // Color Over Lifetime - White with gold tint
            var col = activePortalParticles.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(activeColorWhite, 0f),
                    new GradientColorKey(activeColorGold, 0.5f),
                    new GradientColorKey(activeColorGold, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Velocity over Lifetime - Inward spiral toward center
            var vel = activePortalParticles.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;

            // Use consistent TwoConstants mode for all velocity components
            vel.x = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.y = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.radial = new ParticleSystem.MinMaxCurve(-1.5f, -1.5f); // Move toward center
            vel.orbitalX = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.orbitalY = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.orbitalZ = new ParticleSystem.MinMaxCurve(0.8f, 1.2f); // Spiral around Z axis

            // Size Over Lifetime
            var size = activePortalParticles.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 0.8f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = activePortalParticles.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 2: Active Portal configured");
        }

        #endregion

        #region Phase 3: Closing Burst Configuration

        private void ConfigurePhase3_ClosingBurst()
        {
            if (closingBurst == null)
            {
                Debug.LogWarning("Closing Burst ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = closingBurst.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startDelay = 0f;
            main.startLifetime = 0.4f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(-2f, -4f); // Move inward (negative speed)
            main.startSize = new ParticleSystem.MinMaxCurve(2.0f, 3.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = 0f;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            main.maxParticles = 16;

            // Emission - Burst on close
            var emission = closingBurst.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 12, 16, 1)
            });

            // Shape - Circle collapsing inward
            var shape = closingBurst.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = portalDiameter / 2f;
            shape.radiusThickness = 0f;
            shape.arc = 360f;
            shape.rotation = new Vector3(90f, 0f, 0f);

            // Color Over Lifetime - White flash then fade
            var col = closingBurst.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(closingColorWhite, 0f),
                    new GradientColorKey(closingColorWhite, 0.3f),
                    new GradientColorKey(openingColorGold, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.3f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Size Over Lifetime - Shrink as they collapse
            var size = closingBurst.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 1f);
            sizeCurve.AddKey(0.5f, 0.7f);
            sizeCurve.AddKey(1f, 0f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = closingBurst.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Phase 3: Closing Burst configured");
        }

        #endregion

        #region Portal Mesh Configuration

        private void ConfigurePortalMesh()
        {
            if (portalMesh == null)
            {
                Debug.LogWarning("Portal Mesh not assigned (optional)");
                return;
            }

            // Set initial scale
            portalMesh.transform.localScale = Vector3.zero;

            // Configure material
            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            portalMesh.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", portalMeshColor);
            _propertyBlock.SetColor("_EmissionColor", portalMeshColor * 2f);
            portalMesh.SetPropertyBlock(_propertyBlock);

            Debug.Log("Portal Mesh configured");
        }

        private void ShowPortalMesh()
        {
            if (portalMesh == null) return;

            portalMesh.gameObject.SetActive(true);
            StartCoroutine(ScalePortalMesh(Vector3.zero, Vector3.one * portalDiameter, 0.3f));
        }

        private void HidePortalMesh()
        {
            if (portalMesh == null) return;

            portalMesh.transform.localScale = Vector3.zero;
            portalMesh.gameObject.SetActive(false);
        }

        private IEnumerator ScalePortalMesh(Vector3 from, Vector3 to, float duration)
        {
            if (portalMesh == null) yield break;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t); // Smooth easing

                portalMesh.transform.localScale = Vector3.Lerp(from, to, t);

                yield return null;
            }

            portalMesh.transform.localScale = to;
        }

        private IEnumerator ClosePortalMesh(float duration)
        {
            if (portalMesh == null) yield break;

            Vector3 currentScale = portalMesh.transform.localScale;
            yield return ScalePortalMesh(currentScale, Vector3.zero, duration);
        }

        #endregion

        #region Portal Light Configuration

        private void ConfigurePortalLight()
        {
            if (portalLight == null)
            {
                Debug.LogWarning("Portal Light not assigned (optional)");
                return;
            }

            portalLight.type = LightType.Point;
            portalLight.color = lightColor;
            portalLight.intensity = 0f;
            portalLight.range = 6f;
            portalLight.shadows = LightShadows.None;

            Debug.Log("Portal Light configured");
        }

        private IEnumerator FadeLightIn(float duration)
        {
            if (portalLight == null) yield break;

            float elapsed = 0f;
            float targetIntensity = 5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                portalLight.intensity = Mathf.Lerp(0f, targetIntensity, t);
                yield return null;
            }

            portalLight.intensity = targetIntensity;
        }

        private IEnumerator FadeLightOut(float duration)
        {
            if (portalLight == null) yield break;

            float elapsed = 0f;
            float startIntensity = portalLight.intensity;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                portalLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
                yield return null;
            }

            portalLight.intensity = 0f;
        }

        private void DisableLight()
        {
            if (portalLight != null)
            {
                portalLight.intensity = 0f;
            }
        }

        #endregion

        #region Utility Methods

        private void StopAllParticles()
        {
            if (openingRing != null)
            {
                openingRing.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (activePortalParticles != null)
            {
                activePortalParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            if (closingBurst != null)
            {
                closingBurst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Spawn Portal Configuration ===");
            Debug.Log($"Opening Ring: {(openingRing != null ? "Assigned" : "Missing")}");
            Debug.Log($"Active Portal Particles: {(activePortalParticles != null ? "Assigned" : "Missing")}");
            Debug.Log($"Closing Burst: {(closingBurst != null ? "Assigned" : "Missing")}");
            Debug.Log($"Portal Mesh: {(portalMesh != null ? "Assigned" : "Missing (Optional)")}");
            Debug.Log($"Portal Light: {(portalLight != null ? "Assigned" : "Missing (Optional)")}");
            Debug.Log($"Total Duration: {totalDuration}s");
            Debug.Log($"Portal Diameter: {portalDiameter}");
            Debug.Log($"Enemy Spawn Time: {enemySpawnTime}s");

            int totalParticles = 0;
            if (openingRing != null) totalParticles += openingRing.main.maxParticles;
            if (activePortalParticles != null) totalParticles += activePortalParticles.main.maxParticles;
            if (closingBurst != null) totalParticles += closingBurst.main.maxParticles;

            Debug.Log($"Total Max Particles: {totalParticles}");
            Debug.Log("===================================");
        }

        #endregion
    }
}
