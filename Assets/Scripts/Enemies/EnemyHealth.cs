using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private GameObject xpOrbPrefab;

    [Header("Hit Flash")]
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    // Spawner/pool subscribes to this to know when to reclaim this enemy
    public event System.Action<EnemyHealth> OnDeath;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        // Reset on enable rather than Awake, since pooled objects are reused, not recreated
        CurrentHealth = maxHealth;
        spriteRenderer.color = originalColor; // in case this enemy was mid-flash when pooled
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

        // Restart the flash coroutine so rapid hits (e.g. Double Shot) keep
        // re-triggering white instead of the color settling back too early
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());

        if (IsDead)
        {
            if (xpOrbPrefab != null)
            {
                Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
            }

            OnDeath?.Invoke(this);
        }
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
}