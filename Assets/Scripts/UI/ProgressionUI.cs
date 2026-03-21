using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class ProgressionUI : MonoBehaviour
{
    [Header("Progression Display")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelTitle;
    public Slider xpBar;

    public void Start()
    {
        PlayerProgressionManager.Instance.OnProgressionChanged += RefreshUI;
        RefreshUI();
    }

    public void OnDestroy()
    {
        if (PlayerProgressionManager.Instance != null)
            PlayerProgressionManager.Instance.OnProgressionChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        var progression = PlayerProgressionManager.Instance;

        levelText.text = LocalizationSettings.StringDatabase.GetLocalizedString(
            "UI_Progression", "progression.level",
            new object[] { progression.CurrentLevel }
        );

        levelTitle.text = progression.LevelTitle();
        xpBar.value = progression.XPProgress();
    }
}