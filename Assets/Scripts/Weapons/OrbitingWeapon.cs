using System.Collections.Generic;
using UnityEngine;

public class OrbitingWeapon : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private GameObject orbitObjectPrefab;
    [SerializeField] private float orbitRadius = 2f;
    [SerializeField] private float orbitSpeed = 90f; // degrees per second
    [SerializeField] private float damage = 8f;
    [SerializeField] private int orbitCount = 1;

    private readonly List<Transform> orbitInstances = new List<Transform>();
    private float currentAngle;

    private void Start()
    {
        RebuildOrbitInstances();
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;

        foreach (Transform instance in orbitInstances)
        {
            if (instance != null && instance.TryGetComponent<OrbitDamager>(out var damager))
            {
                damager.SetDamage(damage);
            }
        }
    }

    public void IncreaseOrbitCount(int amount)
    {
        orbitCount += amount;
        RebuildOrbitInstances(); // re-spaced evenly for the new count
    }

    // Destroys and recreates every orbiting instance, evenly spaced around
    // the circle. Simpler than trying to insert new ones into existing
    // spacing, and orbit count changes rarely enough that this cost is fine.
    private void RebuildOrbitInstances()
    {
        foreach (Transform instance in orbitInstances)
        {
            if (instance != null) Destroy(instance.gameObject);
        }
        orbitInstances.Clear();

        for (int i = 0; i < orbitCount; i++)
        {
            GameObject obj = Instantiate(orbitObjectPrefab, transform.position, Quaternion.identity);

            if (obj.TryGetComponent<OrbitDamager>(out var damager))
            {
                damager.SetDamage(damage);
            }

            orbitInstances.Add(obj.transform);
        }
    }

    private void Update()
    {
        currentAngle += orbitSpeed * Time.deltaTime;

        for (int i = 0; i < orbitInstances.Count; i++)
        {
            if (orbitInstances[i] == null) continue;

            // Evenly space instances around the circle (e.g. 2 instances = 180° apart)
            float angleOffset = (360f / orbitInstances.Count) * i;
            float angleRad = (currentAngle + angleOffset) * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * orbitRadius;
            orbitInstances[i].position = transform.position + offset;
        }
    }
}