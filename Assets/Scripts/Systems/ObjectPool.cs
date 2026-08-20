using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic pool for any Component type (EnemyHealth, Projectile, etc).
/// Reuses inactive instances instead of calling Instantiate/Destroy repeatedly.
/// </summary>
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> inactiveObjects = new Queue<T>();

    public ObjectPool(T prefab, Transform parent, int prewarmCount = 0)
    {
        this.prefab = prefab;
        this.parent = parent;

        // Pre-create objects upfront (e.g. at level load) so the first real
        // wave of enemies doesn't cause a burst of Instantiate calls mid-gameplay
        for (int i = 0; i < prewarmCount; i++)
        {
            T obj = CreateNew();
            obj.gameObject.SetActive(false);
            inactiveObjects.Enqueue(obj);
        }
    }

    private T CreateNew()
    {
        return Object.Instantiate(prefab, parent);
    }

    /// <summary>
    /// Hands out a pooled object, reusing an inactive one if available,
    /// or creating a new one if the pool has run out.
    /// </summary>
    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = inactiveObjects.Count > 0
            ? inactiveObjects.Dequeue()
            : CreateNew();

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Returns an object to the pool instead of destroying it.
    /// </summary>
    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        inactiveObjects.Enqueue(obj);
    }
}