using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [Header("Leveling Curve")]
    [SerializeField] private int baseXPToLevel = 10;
    [SerializeField] private float xpCurveMultiplier = 1.3f; // each level requires 30% more XP than the last

    public int Level { get; private set; } = 1;
    public int CurrentXP { get; private set; }
    public int XPToNextLevel { get; private set; }

    // (currentXP, xpToNextLevel) — for an XP bar UI, same idea as the health bar
    public event System.Action<int, int> OnXPChanged;
    public event System.Action<int> OnLevelUp; // passes the new level

    private void Awake()
    {
        XPToNextLevel = baseXPToLevel;
    }

    public void AddXP(int amount)
    {
        // while loop (not if) in case a big XP gain crosses multiple level thresholds at once
        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            Level++;
            XPToNextLevel = Mathf.RoundToInt(XPToNextLevel * xpCurveMultiplier);
            OnLevelUp?.Invoke(Level);
        }

        OnXPChanged?.Invoke(CurrentXP, XPToNextLevel);
    }
}