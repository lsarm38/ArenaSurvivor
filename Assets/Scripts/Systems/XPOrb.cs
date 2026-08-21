using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int xpValue = 1;

    [Header("Magnet Behavior")]
    [SerializeField] private float attractRadius = 3f;
    [SerializeField] private float attractSpeed = 10f;

    private Transform player;

    private void OnEnable()
    {
        // Pooled objects get re-enabled rather than re-instantiated, so we
        // re-find the player reference each time this orb becomes active
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Only move toward the player once within attractRadius — otherwise
        // orbs stay put until the player gets close, like the genre standard
        if (distance <= attractRadius)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                attractSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerXP>(out var playerXP)) return;

        playerXP.AddXP(xpValue);
        Destroy(gameObject);
    }
}