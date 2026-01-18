using UnityEngine;
using System.Collections;

namespace BaseDefender.VFX
{
    /// <summary>
    /// Configuration helper for Base/Hell Gate Energy Field (Shield Dome) VFX.
    /// Attach this to FX_Base_Shield prefab to validate and configure settings.
    /// Creates a magical barrier effect with dome mesh, pulsing energy, and particle ring.
    /// Supports damaged and destroyed states.
    /// </summary>
    public class BaseShieldConfig : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Automatically apply configuration values on Awake")]
        [SerializeField] private bool autoConfigureOnAwake = false;

        [Header("Shield Dome Mesh")]
        [SerializeField] private MeshRenderer shieldDomeMesh;
        [Tooltip("Dark red with purple tint")]
        [SerializeField] private Color shieldColor = new Color(0.86f, 0.08f, 0.24f, 0.3f); // Crimson
        [SerializeField] private Color shieldColorDamaged = new Color(0.5f, 0.04f, 0.12f, 0.4f); // Darker red

        [Header("Particle Ring")]
        [SerializeField] private ParticleSystem particleRing;
        [Tooltip("Red and purple alternating particles")]
        [SerializeField] private Color ringColorRed = new Color(0.86f, 0.08f, 0.24f, 1f); // Crimson
        [SerializeField] private Color ringColorPurple = new Color(0.29f, 0f, 0.51f, 1f); // Dark Purple

        [Header("Shield Settings")]
        [Tooltip("Radius of the shield dome")]
        [SerializeField] private float shieldRadius = 5f;
        [Tooltip("Pulse frequency in seconds")]
        [SerializeField] private float pulseFrequency = 2f;
        [Tooltip("Minimum emission intensity")]
        [SerializeField] private float pulseMinIntensity = 1.5f;
        [Tooltip("Maximum emission intensity")]
        [SerializeField] private float pulseMaxIntensity = 2.5f;

        [Header("State")]
        [SerializeField] private ShieldState currentState = ShieldState.Normal;

        // Runtime variables
        private MaterialPropertyBlock _propertyBlock;
        private Coroutine _pulseCoroutine;
        private Coroutine _destroyCoroutine;

        public enum ShieldState
        {
            Normal,
            Damaged,
            Destroyed
        }

        #region Unity Lifecycle

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            if (autoConfigureOnAwake)
            {
                ConfigureAllSystems();
            }
        }

        private void OnEnable()
        {
            if (currentState != ShieldState.Destroyed)
            {
                StartPulsing();
            }
        }

        private void OnDisable()
        {
            StopPulsing();
        }

        private void OnValidate()
        {
            // Auto-find components
            if (shieldDomeMesh == null)
            {
                shieldDomeMesh = GetComponentInChildren<MeshRenderer>();
            }

            if (particleRing == null)
            {
                particleRing = GetComponentInChildren<ParticleSystem>();
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
            ConfigureShieldDome();
            ConfigureParticleRing();

            Debug.Log("BaseShieldConfig: All systems configured successfully!");
        }

        /// <summary>
        /// Start the shield effect
        /// </summary>
        [ContextMenu("Activate Shield")]
        public void ActivateShield()
        {
            if (shieldDomeMesh != null)
            {
                shieldDomeMesh.gameObject.SetActive(true);
            }

            if (particleRing != null)
            {
                particleRing.Play();
            }

            currentState = ShieldState.Normal;
            StartPulsing();

            Debug.Log("Base Shield Activated!");
        }

        /// <summary>
        /// Stop the shield effect
        /// </summary>
        [ContextMenu("Deactivate Shield")]
        public void DeactivateShield()
        {
            StopPulsing();

            if (shieldDomeMesh != null)
            {
                shieldDomeMesh.gameObject.SetActive(false);
            }

            if (particleRing != null)
            {
                particleRing.Stop();
            }

            Debug.Log("Base Shield Deactivated!");
        }

        /// <summary>
        /// Set shield to damaged state (when base health < 50%)
        /// </summary>
        [ContextMenu("Set Damaged State")]
        public void SetDamagedState()
        {
            if (currentState == ShieldState.Destroyed) return;

            currentState = ShieldState.Damaged;

            // Increase particle emission
            if (particleRing != null)
            {
                var emission = particleRing.emission;
                emission.rateOverTime = 30f;
            }

            // Shift color to darker red
            if (shieldDomeMesh != null && _propertyBlock != null)
            {
                shieldDomeMesh.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor("_BaseColor", shieldColorDamaged);
                _propertyBlock.SetColor("_EmissionColor", shieldColorDamaged * pulseMaxIntensity);
                shieldDomeMesh.SetPropertyBlock(_propertyBlock);
            }

            // Make pulsing erratic
            StopPulsing();
            StartCoroutine(ErraticPulseCoroutine());

            Debug.Log("Base Shield Damaged!");
        }

        /// <summary>
        /// Play shield destruction effect
        /// </summary>
        [ContextMenu("Destroy Shield")]
        public void DestroyShield()
        {
            if (currentState == ShieldState.Destroyed) return;

            currentState = ShieldState.Destroyed;
            StopPulsing();

            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
            }

            _destroyCoroutine = StartCoroutine(DestroyShieldCoroutine());

            Debug.Log("Base Shield Destroyed!");
        }

        /// <summary>
        /// Get current shield state
        /// </summary>
        public ShieldState GetState()
        {
            return currentState;
        }

        /// <summary>
        /// Set shield radius
        /// </summary>
        public void SetShieldRadius(float radius)
        {
            shieldRadius = radius;

            if (shieldDomeMesh != null)
            {
                shieldDomeMesh.transform.localScale = Vector3.one * shieldRadius;
            }

            if (particleRing != null)
            {
                var shape = particleRing.shape;
                shape.radius = shieldRadius;
            }
        }

        #endregion

        #region Shield Dome Configuration

        private void ConfigureShieldDome()
        {
            if (shieldDomeMesh == null)
            {
                Debug.LogWarning("Shield Dome Mesh not assigned!");
                return;
            }

            // Set initial scale based on radius
            shieldDomeMesh.transform.localScale = Vector3.one * shieldRadius;

            // Configure material using property block (avoids material instances)
            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            shieldDomeMesh.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", shieldColor);
            _propertyBlock.SetColor("_EmissionColor", shieldColor * pulseMinIntensity);
            shieldDomeMesh.SetPropertyBlock(_propertyBlock);

            Debug.Log("Shield Dome configured");
        }

        #endregion

        #region Particle Ring Configuration

        private void ConfigureParticleRing()
        {
            if (particleRing == null)
            {
                Debug.LogWarning("Particle Ring ParticleSystem not assigned!");
                return;
            }

            // Main Module
            var main = particleRing.main;
            main.duration = 10f;
            main.loop = true;
            main.startDelay = 0f;
            main.startLifetime = 2.0f;
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 0.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(2.0f, 3.0f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f * Mathf.Deg2Rad);
            main.gravityModifier = -0.1f; // Slight upward drift
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 25;

            // Emission - Ring of particles
            var emission = particleRing.emission;
            emission.enabled = true;
            emission.rateOverTime = 18f;

            // Shape - Circle at base of dome
            var shape = particleRing.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = shieldRadius;
            shape.radiusThickness = 0f; // Spawn on edge only
            shape.arc = 360f;
            shape.arcMode = ParticleSystemShapeMultiModeValue.Random;

            // Color Over Lifetime - Red and purple alternating
            var col = particleRing.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(ringColorRed, 0f),
                    new GradientColorKey(ringColorPurple, 0.5f),
                    new GradientColorKey(ringColorRed, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.8f, 0.2f),
                    new GradientAlphaKey(0.8f, 0.7f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            col.color = gradient;

            // Velocity over Lifetime - Rotate around base + gentle upward drift
            var vel = particleRing.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;

            // Use consistent TwoConstants mode for all linear velocities
            vel.x = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.y = new ParticleSystem.MinMaxCurve(0.1f, 0.2f); // Gentle upward drift
            vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);

            // Orbital velocities (must also use consistent mode)
            vel.orbitalX = new ParticleSystem.MinMaxCurve(0f, 0f);
            vel.orbitalY = new ParticleSystem.MinMaxCurve(0.25f, 0.35f); // Rotate around Y axis
            vel.orbitalZ = new ParticleSystem.MinMaxCurve(0f, 0f);

            // Size over Lifetime
            var size = particleRing.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.5f);
            sizeCurve.AddKey(0.3f, 1f);
            sizeCurve.AddKey(1f, 0.3f);
            size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

            // Renderer
            var renderer = particleRing.GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            Debug.Log("Particle Ring configured");
        }

        #endregion

        #region Pulse Animation

        private void StartPulsing()
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
            }

            _pulseCoroutine = StartCoroutine(PulseCoroutine());
        }

        private void StopPulsing()
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }
        }

        private IEnumerator PulseCoroutine()
        {
            float time = 0f;

            while (true)
            {
                time += Time.deltaTime;

                // Sine wave pulsing
                float t = (Mathf.Sin(time * Mathf.PI * 2f / pulseFrequency) + 1f) / 2f;
                float intensity = Mathf.Lerp(pulseMinIntensity, pulseMaxIntensity, t);

                if (shieldDomeMesh != null && _propertyBlock != null)
                {
                    Color currentColor = currentState == ShieldState.Damaged ? shieldColorDamaged : shieldColor;
                    shieldDomeMesh.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor("_EmissionColor", currentColor * intensity);
                    shieldDomeMesh.SetPropertyBlock(_propertyBlock);
                }

                yield return null;
            }
        }

        private IEnumerator ErraticPulseCoroutine()
        {
            while (currentState == ShieldState.Damaged)
            {
                // Random intensity flicker
                float intensity = Random.Range(pulseMinIntensity * 0.5f, pulseMaxIntensity * 1.5f);

                if (shieldDomeMesh != null && _propertyBlock != null)
                {
                    shieldDomeMesh.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor("_EmissionColor", shieldColorDamaged * intensity);
                    shieldDomeMesh.SetPropertyBlock(_propertyBlock);
                }

                // Random wait time for erratic effect
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            }
        }

        #endregion

        #region Destruction Sequence

        private IEnumerator DestroyShieldCoroutine()
        {
            float duration = 1.0f;
            float elapsed = 0f;

            // Burst particles on destruction
            if (particleRing != null)
            {
                var emission = particleRing.emission;
                emission.SetBursts(new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0f, 30, 40, 1)
                });
                particleRing.Emit(35);
            }

            Vector3 initialScale = shieldDomeMesh != null ? shieldDomeMesh.transform.localScale : Vector3.one * shieldRadius;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Scale down the dome
                if (shieldDomeMesh != null)
                {
                    shieldDomeMesh.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

                    // Fade out
                    if (_propertyBlock != null)
                    {
                        Color fadedColor = shieldColorDamaged;
                        fadedColor.a = Mathf.Lerp(0.4f, 0f, t);
                        shieldDomeMesh.GetPropertyBlock(_propertyBlock);
                        _propertyBlock.SetColor("_BaseColor", fadedColor);
                        shieldDomeMesh.SetPropertyBlock(_propertyBlock);
                    }
                }

                yield return null;
            }

            // Final cleanup
            if (shieldDomeMesh != null)
            {
                shieldDomeMesh.gameObject.SetActive(false);
            }

            if (particleRing != null)
            {
                particleRing.Stop();
            }

            Debug.Log("Shield Destruction Complete!");
        }

        #endregion

        #region Debug Helpers

        [ContextMenu("Print Configuration")]
        public void PrintConfiguration()
        {
            Debug.Log("=== Base Shield Configuration ===");
            Debug.Log($"Shield Dome: {(shieldDomeMesh != null ? "Assigned" : "Missing")}");
            Debug.Log($"Particle Ring: {(particleRing != null ? "Assigned" : "Missing")}");
            Debug.Log($"Shield Radius: {shieldRadius}");
            Debug.Log($"Pulse Frequency: {pulseFrequency}s");
            Debug.Log($"Current State: {currentState}");

            if (particleRing != null)
            {
                Debug.Log($"Max Particles: {particleRing.main.maxParticles}");
            }
            Debug.Log("=================================");
        }

        #endregion
    }
}
