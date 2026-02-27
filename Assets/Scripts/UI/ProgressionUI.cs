using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Slider xpBar;

    void Start()
    {
        PlayerProgressionSystem.Instance.OnProgressionChanged += RefreshUI;
        RefreshUI();
    }

    void OnDestroy()
    {
        if (PlayerProgressionSystem.Instance != null)
            PlayerProgressionSystem.Instance.OnProgressionChanged -= RefreshUI;
    }

    void RefreshUI()
    {
        var progression = PlayerProgressionSystem.Instance;
        levelText.text = $"{progression.LevelTitle()} — Niveau {progression.CurrentLevel}";
        xpBar.value = progression.XPProgress();
    }
}