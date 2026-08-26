using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] private float fireRate = 1f; // shots per second
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 6f;

    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private LayerMask enemyLayer; // set this to whatever layer you put enemies on

    private float fireCooldown;
    private int extraProjectiles; // 0 = single shot, 1 = double shot, etc.

    public void IncreaseDamage(float amount)
    {
        damage += amount;
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate += amount;
    }

    public void IncreaseProjectileCount(int amount)
    {
        extraProjectiles += amount;
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            TryFire();
            fireCooldown = 1f / fireRate;
        }
    }

    private void TryFire()
    {
        // Fire at (1 + extraProjectiles) separate nearest enemies each shot,
        // so "double shot" hits two different targets instead of stacking on one
        List<Transform> targets = FindNearestEnemies(1 + extraProjectiles);
        if (targets.Count == 0) return; // no enemies in range, nothing to shoot at

        foreach (Transform target in targets)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Vector2 direction = (target.position - transform.position).normalized;
            projectileObj.GetComponent<Projectile>().Launch(direction, damage);
        }
    }

    private List<Transform> FindNearestEnemies(int count)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        return hits
            .OrderBy(hit => (hit.transform.position - transform.position).sqrMagnitude)
            .Take(count)
            .Select(hit => hit.transform)
            .ToList();
    }

    // Visualize weapon range in the Editor without needing Play mode
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}