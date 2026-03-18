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
        if (StatManager.Instance != null)
            StatManager.Instance.OnStatsChanged -= RefreshUI;
    }

    public void Start()
    {
        StatManager.Instance.OnStatsChanged += RefreshUI;
        RefreshUI();
    }

    public void RefreshUI()
    {
        var stats = StatManager.Instance;

        motivationBar.value = Normalize(stats.Motivation, StatManager.GetMinMotivation, StatManager.GetMaxMotivation);
        stressBar.value = Normalize(stats.Stress, StatManager.GetMinStress, StatManager.GetMaxStress);
        performanceBar.value = Normalize(stats.Performance, StatManager.GetMinPerformance, StatManager.GetMaxPerformance);
        turnoverBar.value = Normalize(stats.Turnover, StatManager.GetMinTurnover, StatManager.GetMaxTurnover);

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