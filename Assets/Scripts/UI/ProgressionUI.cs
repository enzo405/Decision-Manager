using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
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
        levelText.text = $"{progression.LevelTitle()} — Niveau {progression.CurrentLevel}";
        xpBar.value = progression.XPProgress();
    }
}