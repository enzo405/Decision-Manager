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

        motivationBar.value = Normalize(stats.Motivation, StatSystem.GetMinMotivation, StatSystem.GetMaxMotivation);
        stressBar.value = Normalize(stats.Stress, StatSystem.GetMinStress, StatSystem.GetMaxStress);
        performanceBar.value = Normalize(stats.Performance, StatSystem.GetMinPerformance, StatSystem.GetMaxPerformance);
        turnoverBar.value = Normalize(stats.Turnover, StatSystem.GetMinTurnover, StatSystem.GetMaxTurnover);

        motivationValueText.text = stats.Motivation.ToString();
        stressValueText.text = stats.Stress.ToString();
        performanceValueText.text = stats.Performance.ToString();
        turnoverValueText.text = stats.Turnover.ToString();
    }

    private float Normalize(int value, int min, int max)
    {
        Debug.Log($"Result: {(float)(value - min) / (max - min)}");
        Debug.Log($"Value: {value}, Min: {min}, Max: {max}");
        return (float)(value - min) / (max - min);
    }
}