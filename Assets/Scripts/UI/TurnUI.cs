using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    public void Start()
    {
        GameManager.Instance.OnTurnStarted += RefreshUI;
        RefreshUI();
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTurnStarted -= RefreshUI;
    }

    public void RefreshUI()
    {
        turnText.text = LocalizationSettings.StringDatabase.GetLocalizedString(
            "UI_Progression", "progression.week",
            new object[] { GameManager.Instance.CurrentWeek, GameManager.Instance.totalWeeks }
        );
    }
}