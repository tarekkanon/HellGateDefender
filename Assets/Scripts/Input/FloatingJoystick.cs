using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Touch-based floating joystick for mobile input.
/// Appears at touch position and provides directional input.
/// </summary>
public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Inspector Fields

    [Header("Joystick Components")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 50f;
    [SerializeField] private bool hideOnRelease = true;

    [Header("Camera Settings")]
    [SerializeField] private Camera gameCamera;

    #endregion

    #region Private Fields

    private Vector2 _input = Vector2.zero;
    private Canvas _canvas;
    private Camera _camera;

    #endregion

    #region Properties

    /// <summary>
    /// Horizontal input (-1 to 1)
    /// </summary>
    public float Horizontal => _input.x;

    /// <summary>
    /// Vertical input (-1 to 1)
    /// </summary>
    public float Vertical => _input.y;

    /// <summary>
    /// Combined direction vector (screen space)
    /// </summary>
    public Vector2 Direction => _input;

    /// <summary>
    /// Camera-relative 3D direction vector for character movement
    /// </summary>
    public Vector3 Direction3D
    {
        get
        {
            if (gameCamera == null)
            {
                return new Vector3(_input.x, 0f, _input.y);
            }

            // Get camera's forward and right vectors projected onto horizontal plane
            Vector3 cameraForward = gameCamera.transform.forward;
            Vector3 cameraRight = gameCamera.transform.right;

            // Project onto horizontal plane (remove Y component)
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            // Normalize to maintain consistent speed
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate movement direction relative to camera
            Vector3 direction = cameraRight * _input.x + cameraForward * _input.y;

            return direction;
        }
    }

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        Initialize();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize the joystick
    /// </summary>
    private void Initialize()
    {
        // Get parent canvas
        _canvas = GetComponentInParent<Canvas>();

        // Get camera for screen space calculations
        if (_canvas != null)
        {
            if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                _camera = _canvas.worldCamera;
            }
            else if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                _camera = null;
            }
        }

        // Get game camera if not assigned
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }

        // Validate components
        if (background == null)
        {
            Debug.LogError("FloatingJoystick: Background RectTransform not assigned!");
        }

        if (handle == null)
        {
            Debug.LogError("FloatingJoystick: Handle RectTransform not assigned!");
        }

        // Hide joystick initially
        if (hideOnRelease)
        {
            SetJoystickVisibility(false);
        }
    }

    #endregion

    #region Input Handlers

    /// <summary>
    /// Called when touch/click begins
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (background == null)
        {
            return;
        }

        // Show joystick
        SetJoystickVisibility(true);

        // Position background at touch point using world position
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _camera,
            out worldPosition
        );
        worldPosition.Set(worldPosition.x -75, worldPosition.y -75, worldPosition.z);
        background.position = worldPosition;

        // Reset handle to center
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }

        // Process initial input
        ProcessInput(eventData);
    }

    /// <summary>
    /// Called while dragging
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        ProcessInput(eventData);
    }

    /// <summary>
    /// Called when touch/click ends
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset input
        _input = Vector2.zero;

        // Reset handle to center
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }

        // Hide joystick
        if (hideOnRelease)
        {
            SetJoystickVisibility(false);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Process touch/drag input
    /// </summary>
    private void ProcessInput(PointerEventData eventData)
    {
        if (background == null || handle == null)
        {
            return;
        }

        // Get touch position in world space
        Vector3 touchWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _camera,
            out touchWorldPosition
        );

        // Calculate direction from background center to touch point
        Vector2 direction = new Vector2(
            touchWorldPosition.x - background.position.x,
            touchWorldPosition.y - background.position.y
        );

        // Clamp to handle range
        if (direction.magnitude > handleRange)
        {
            direction = direction.normalized * handleRange;
        }

        // Update handle position
        handle.anchoredPosition = direction;

        // Calculate normalized input (-1 to 1)
        _input = direction / handleRange;
    }

    /// <summary>
    /// Show or hide the joystick
    /// </summary>
    private void SetJoystickVisibility(bool visible)
    {
        if (background != null)
        {
            background.gameObject.SetActive(visible);
        }
    }

    #endregion

    #region Debug

    /// <summary>
    /// Get current input as string for debugging
    /// </summary>
    public string GetDebugInfo()
    {
        return $"Input: ({Horizontal:F2}, {Vertical:F2})";
    }

    #endregion
}
