using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float moveSpeed = 2.5f;
	[SerializeField] private Transform target;

	[Header("Combat")]
	[SerializeField] private float contactDamage = 10f;
	[SerializeField] private float damageCooldown = 1f; // seconds between hits while touching

	private Rigidbody2D rb;
	private float lastDamageTime = -999f;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (target == null) return;

		Vector2 direction = (target.position - transform.position).normalized;
		Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
		rb.MovePosition(newPosition);
	}

	// Called continuously while colliders overlap
	private void OnCollisionStay2D(Collision2D collision)
	{
		if (!collision.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth)) return;

		// Cooldown prevents dealing damage every single physics frame (which would be ~50/sec)
		if (Time.time - lastDamageTime < damageCooldown) return;

		playerHealth.TakeDamage(contactDamage);
		lastDamageTime = Time.time;
	}
}