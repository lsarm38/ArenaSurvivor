using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerXP playerXP;
    [SerializeField] private AutoWeapon autoWeapon;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private UpgradeChoiceUI choiceUI;

    [Header("Upgrade Pool")]
    [SerializeField] private List<UpgradeData> allUpgrades;
    [SerializeField] private int picksPerLevelUp = 2;

    private void OnEnable()
    {
        playerXP.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        playerXP.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        Time.timeScale = 0f; // pause gameplay while the player chooses
        PromptChoice(exclude: null, picksRemaining: picksPerLevelUp);
    }

    // Recursive-ish flow: show 3 choices, apply whichever is picked, then
    // either prompt again (excluding what was just picked) or resume play
    private void PromptChoice(UpgradeData exclude, int picksRemaining)
    {
        List<UpgradeData> pool = exclude == null
            ? allUpgrades
            : allUpgrades.Where(u => u != exclude).ToList();

        List<UpgradeData> choices = GetRandomUpgrades(pool, 3);

        choiceUI.ShowChoices(choices, chosen => OnUpgradeChosen(chosen, picksRemaining));
    }

    private void OnUpgradeChosen(UpgradeData chosen, int picksRemaining)
    {
        ApplyUpgrade(chosen);

        int remaining = picksRemaining - 1;
        if (remaining > 0)
        {
            PromptChoice(chosen, remaining); // exclude this pick so round 2 doesn't repeat it
        }
        else
        {
            Time.timeScale = 1f; // all picks made — resume gameplay
        }
    }

    private List<UpgradeData> GetRandomUpgrades(List<UpgradeData> pool, int count)
    {
        // Shuffle a copy so choices don't repeat within the same round
        List<UpgradeData> shuffled = pool.OrderBy(_ => Random.value).ToList();
        return shuffled.Take(Mathf.Min(count, shuffled.Count)).ToList();
    }

    private void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.Damage:
                autoWeapon.IncreaseDamage(upgrade.amount);
                break;
            case UpgradeType.FireRate:
                autoWeapon.IncreaseFireRate(upgrade.amount);
                break;
            case UpgradeType.MoveSpeed:
                playerController.IncreaseMoveSpeed(upgrade.amount);
                break;
            case UpgradeType.MaxHealth:
                playerHealth.IncreaseMaxHealth(upgrade.amount);
                break;
            case UpgradeType.PickupRadius:
                playerXP.IncreasePickupRadius(upgrade.amount);
                break;
            case UpgradeType.InstantHeal:
                playerHealth.Heal(upgrade.amount);
                break;
            case UpgradeType.ProjectileCount:
                autoWeapon.IncreaseProjectileCount(Mathf.RoundToInt(upgrade.amount));
                break;
        }
    }
}