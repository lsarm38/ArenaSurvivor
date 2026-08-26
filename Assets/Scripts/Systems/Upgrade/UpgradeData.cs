using UnityEngine;

public enum UpgradeType
{
    Damage,
    FireRate,
    MoveSpeed,
    MaxHealth,
    PickupRadius,   // increases how close an XP orb needs to be before it magnets toward the player
    InstantHeal,    // one-time heal, doesn't raise max HP
    ProjectileCount // fires additional simultaneous projectiles at extra nearby enemies ("double/triple shot")
}

// CreateAssetMenu adds a right-click > Create option in the Project window,
// so each upgrade becomes its own .asset file instead of a class you'd
// otherwise have to subclass or hardcode
[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ArenaSurvivor/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;

    [TextArea]
    public string description;

    public UpgradeType type;
    public float amount; // meaning depends on type: +damage, +fire rate, +move speed, +max health
}