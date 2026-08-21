using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0f;

    // UI (or anything else) subscribes to this instead of checking CurrentHealth every frame
    public event System.Action<float, float> OnHealthChanged; // (current, max)

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Start()
    {
        // Fire once at startup so subscribers (like the health bar) initialize at full HP
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (IsDead)
        {
            Debug.Log("Player died.");
            // We'll hook this into a proper Game Over screen in Week 5
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}