using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool for efficient GameObject reuse.
/// Reduces instantiation overhead and garbage collection by recycling objects.
/// </summary>
/// <typeparam name="T">Component type to pool (must be a Component)</typeparam>
public class ObjectPool<T> where T : Component
{
    #region Private Fields

    private readonly GameObject _prefab;
    private readonly Transform _parentTransform;
    private readonly Queue<T> _availableObjects;
    private readonly HashSet<T> _activeObjects;
    private readonly int _initialSize;
    private readonly int _maxSize;
    private readonly bool _expandable;

    #endregion

    #region Properties

    /// <summary>
    /// Number of available objects in the pool
    /// </summary>
    public int AvailableCount => _availableObjects.Count;

    /// <summary>
    /// Number of active objects from the pool
    /// </summary>
    public int ActiveCount => _activeObjects.Count;

    /// <summary>
    /// Total number of objects created by this pool
    /// </summary>
    public int TotalCount => AvailableCount + ActiveCount;

    #endregion

    #region Constructor

    /// <summary>
    /// Create a new object pool
    /// </summary>
    /// <param name="prefab">Prefab to instantiate</param>
    /// <param name="initialSize">Initial pool size</param>
    /// <param name="maxSize">Maximum pool size (0 for unlimited)</param>
    /// <param name="expandable">Can pool grow beyond initial size?</param>
    /// <param name="parent">Optional parent transform for pooled objects</param>
    public ObjectPool(GameObject prefab, int initialSize = 10, int maxSize = 0, bool expandable = true, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPool: Cannot create pool with null prefab!");
            return;
        }

        if (prefab.GetComponent<T>() == null)
        {
            Debug.LogError($"ObjectPool: Prefab '{prefab.name}' doesn't have component of type {typeof(T).Name}!");
            return;
        }

        _prefab = prefab;
        _initialSize = Mathf.Max(0, initialSize);
        _maxSize = Mathf.Max(0, maxSize);
        _expandable = expandable;
        _parentTransform = parent;

        _availableObjects = new Queue<T>(_initialSize);
        _activeObjects = new HashSet<T>();

        // Pre-populate the pool
        Prewarm();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Get an object from the pool
    /// </summary>
    /// <param name="position">Position to place the object</param>
    /// <param name="rotation">Rotation to apply to the object</param>
    /// <returns>Pooled object, or null if pool is at max capacity</returns>
    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = null;

        // Try to get an available object
        if (_availableObjects.Count > 0)
        {
            obj = _availableObjects.Dequeue();
        }
        // Try to create a new object if pool is expandable
        else if (_expandable && (_maxSize == 0 || TotalCount < _maxSize))
        {
            obj = CreateNewObject();
        }
        else
        {
            Debug.LogWarning($"ObjectPool: Pool for {typeof(T).Name} is at max capacity or not expandable!");
            return null;
        }

        if (obj != null)
        {
            // Activate and position the object
            obj.gameObject.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            // Track as active
            _activeObjects.Add(obj);
        }

        return obj;
    }

    /// <summary>
    /// Return an object to the pool
    /// </summary>
    /// <param name="obj">Object to return</param>
    public void Return(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("ObjectPool: Attempted to return null object!");
            return;
        }

        // Remove from active tracking
        if (!_activeObjects.Remove(obj))
        {
            Debug.LogWarning($"ObjectPool: Object '{obj.name}' was not tracked as active!");
        }

        // Deactivate the object
        obj.gameObject.SetActive(false);

        // Reset position and parent
        if (_parentTransform != null)
        {
            obj.transform.SetParent(_parentTransform);
        }
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        // Return to available queue
        _availableObjects.Enqueue(obj);
    }

    /// <summary>
    /// Return all active objects to the pool
    /// </summary>
    public void ReturnAll()
    {
        // Create a temporary list to avoid modifying collection during iteration
        var activeList = new List<T>(_activeObjects);

        foreach (var obj in activeList)
        {
            if (obj != null)
            {
                Return(obj);
            }
        }

        _activeObjects.Clear();
    }

    /// <summary>
    /// Clear and destroy all objects in the pool
    /// </summary>
    public void Clear()
    {
        // Destroy all available objects
        while (_availableObjects.Count > 0)
        {
            T obj = _availableObjects.Dequeue();
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }

        // Destroy all active objects
        foreach (var obj in _activeObjects)
        {
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }

        _activeObjects.Clear();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Pre-populate the pool with initial objects
    /// </summary>
    private void Prewarm()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            T obj = CreateNewObject();
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
                _availableObjects.Enqueue(obj);
            }
        }
    }

    /// <summary>
    /// Create a new pooled object
    /// </summary>
    /// <returns>Newly created object</returns>
    private T CreateNewObject()
    {
        GameObject newObj = Object.Instantiate(_prefab, _parentTransform);
        newObj.name = $"{_prefab.name} (Pooled)";

        T component = newObj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"ObjectPool: Created object doesn't have component {typeof(T).Name}!");
            Object.Destroy(newObj);
            return null;
        }

        return component;
    }

    #endregion

    #region Debug

    /// <summary>
    /// Get debug information about the pool
    /// </summary>
    public string GetDebugInfo()
    {
        return $"Pool<{typeof(T).Name}>: Active={ActiveCount}, Available={AvailableCount}, Total={TotalCount}";
    }

    #endregion
}
