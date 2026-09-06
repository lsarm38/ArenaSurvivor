using System.Collections.Generic;
using UnityEngine;

public class OrbitDamager : MonoBehaviour
{
    [SerializeField] private float damageInterval = 0.5f; // seconds between hits to the SAME enemy while still touching

    private float damage = 8f;

    // Tracks the last time each currently-overlapping enemy was hit, so a
    // continuously-touching enemy takes damage every damageInterval seconds
    // instead of every physics frame (~50x/sec)
    private readonly Dictionary<EnemyHealth, float> lastHitTime = new Dictionary<EnemyHealth, float>();

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyHealth>(out var enemyHealth)) return;

        if (lastHitTime.TryGetValue(enemyHealth, out float last) && Time.time - last < damageInterval)
        {
            return;
        }

        enemyHealth.TakeDamage(damage);
        lastHitTime[enemyHealth] = Time.time;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset timing on exit, so briefly leaving and re-entering doesn't
        // inherit stale cooldown state from the earlier contact
        if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            lastHitTime.Remove(enemyHealth);
        }
    }
}