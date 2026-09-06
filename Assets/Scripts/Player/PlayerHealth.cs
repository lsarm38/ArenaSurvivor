using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public bool IsDead => CurrentHealth <= 0f;

    // UI (or anything else) subscribes to this instead of checking CurrentHealth every frame
    public event System.Action<float, float> OnHealthChanged; // (current, max)

    // Separate from OnHealthChanged since screen shake (and similar "ouch" feedback)
    // should only fire on actual damage, not on healing or max-health increases
    public event System.Action OnDamaged;

    // Fired exactly once, the moment health first hits 0
    public event System.Action OnDeath;

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
        OnDamaged?.Invoke();

        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        CurrentHealth += amount; // also heal by the increase, so it feels like a real upgrade, not just a higher ceiling
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}