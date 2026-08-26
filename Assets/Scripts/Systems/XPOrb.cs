using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int xpValue = 1;

    [Header("Magnet Behavior")]
    [SerializeField] private float attractSpeed = 10f;

    private Transform player;
    private PlayerXP playerXP;

    private void OnEnable()
    {
        // Pooled objects get re-enabled rather than re-instantiated, so we
        // re-find the player reference each time this orb becomes active
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj?.transform;
        playerXP = playerObj?.GetComponent<PlayerXP>();
    }

    private void Update()
    {
        if (player == null || playerXP == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Only move toward the player once within PickupRadius — otherwise
        // orbs stay put until the player gets close, like the genre standard.
        // Reading this from PlayerXP (instead of a local field) is what lets
        // the PickupRadius upgrade affect every orb in the scene at once.
        if (distance <= playerXP.PickupRadius)
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
        if (!other.TryGetComponent<PlayerXP>(out var hitPlayerXP)) return;

        hitPlayerXP.AddXP(xpValue);
        Destroy(gameObject);
    }
}