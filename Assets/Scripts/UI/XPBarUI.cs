using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [SerializeField] private PlayerXP playerXP;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text levelText; // e.g. "Lv. 3"

    private void OnEnable()
    {
        if (playerXP == null) return;

        playerXP.OnXPChanged += HandleXPChanged;
        playerXP.OnLevelUp += HandleLevelUp;

        // Initialize immediately with current values, in case this UI enables
        // after the player has already gained some XP
        HandleXPChanged(playerXP.CurrentXP, playerXP.XPToNextLevel);
        HandleLevelUp(playerXP.Level);
    }

    private void OnDisable()
    {
        if (playerXP == null) return;

        playerXP.OnXPChanged -= HandleXPChanged;
        playerXP.OnLevelUp -= HandleLevelUp;
    }

    private void HandleXPChanged(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }

    private void HandleLevelUp(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv. {newLevel}";
        }
    }
}