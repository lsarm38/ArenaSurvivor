using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30f;

    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    // Spawner/pool subscribes to this to know when to reclaim this enemy
    public event System.Action<EnemyHealth> OnDeath;

    private void OnEnable()
    {
        // Reset on enable rather than Awake, since pooled objects are reused, not recreated
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

        if (IsDead)
        {
            OnDeath?.Invoke(this);
        }
    }
}