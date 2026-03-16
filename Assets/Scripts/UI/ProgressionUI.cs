using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionUI : MonoBehaviour
{
    [Header("Progression Display")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelTitle;
    public Slider xpBar;

    public void Start()
    {
        PlayerProgressionSystem.Instance.OnProgressionChanged += RefreshUI;
        RefreshUI();
    }

    public void OnDestroy()
    {
        if (PlayerProgressionSystem.Instance != null)
            PlayerProgressionSystem.Instance.OnProgressionChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        var progression = PlayerProgressionSystem.Instance;
        levelText.text = $"Niv. {progression.CurrentLevel}";
        levelTitle.text = progression.LevelTitle();
        xpBar.value = progression.XPProgress();
    }
}