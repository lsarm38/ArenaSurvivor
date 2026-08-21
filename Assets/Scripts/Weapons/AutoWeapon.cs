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
        Transform target = FindNearestEnemy();
        if (target == null) return; // no enemies in range, nothing to shoot at

        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector2 direction = (target.position - transform.position).normalized;
        projectileObj.GetComponent<Projectile>().Launch(direction, damage);
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (hits.Length == 0) return null;

        Transform nearest = null;
        float nearestDistSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            float distSqr = (hit.transform.position - transform.position).sqrMagnitude;
            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    // Visualize weapon range in the Editor without needing Play mode
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}