using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider motivationBar;
    public Slider stressBar;
    public Slider performanceBar;
    public Slider turnoverBar;

    [Header("Header Value")]
    public TextMeshProUGUI motivationValueText;
    public TextMeshProUGUI stressValueText;
    public TextMeshProUGUI performanceValueText;
    public TextMeshProUGUI turnoverValueText;

    public void OnDisable()
    {
        if (StatSystem.Instance != null)
            StatSystem.Instance.OnStatsChanged -= RefreshUI;
    }

    public void Start()
    {
        StatSystem.Instance.OnStatsChanged += RefreshUI;
        RefreshUI();
    }

    public void RefreshUI()
    {
        var stats = StatSystem.Instance;

        motivationBar.value = stats.Motivation;
        stressBar.value = stats.Stress;
        performanceBar.value = stats.Performance;
        turnoverBar.value = stats.Turnover;

        motivationValueText.text = stats.Motivation.ToString();
        stressValueText.text = stats.Stress.ToString();
        performanceValueText.text = stats.Performance.ToString();
        turnoverValueText.text = stats.Turnover.ToString();
    }
}