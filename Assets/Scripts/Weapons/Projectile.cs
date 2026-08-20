using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 3f; // auto-destroy if it never hits anything

    private Rigidbody2D rb;
    private float damage;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called by AutoWeapon right after instantiating, since a fresh Instantiate
    // has no direction or damage set yet
    public void Launch(Vector2 direction, float damageAmount)
    {
        rb.linearVelocity = direction.normalized * speed;
        damage = damageAmount;
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyHealth>(out var enemyHealth)) return;

        enemyHealth.TakeDamage(damage);
        Destroy(gameObject);
    }
}