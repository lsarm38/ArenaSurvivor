using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel; // the parent panel to show/hide
    [SerializeField] private Button[] buttons;          // exactly 3, assigned in Inspector
    [SerializeField] private TMP_Text[] nameTexts;       // matching order to buttons
    [SerializeField] private TMP_Text[] descriptionTexts;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public void ShowChoices(List<UpgradeData> choices, Action<UpgradeData> onChosen)
    {
        panel.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < choices.Count)
            {
                UpgradeData upgrade = choices[i]; // local copy — required so each button's
                                                  // closure captures its own upgrade, not
                                                  // whatever "i" ends up being after the loop
                buttons[i].gameObject.SetActive(true);
                nameTexts[i].text = upgrade.upgradeName;
                descriptionTexts[i].text = upgrade.description;

                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() =>
                {
                    panel.SetActive(false);
                    onChosen(upgrade);
                });
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }
}