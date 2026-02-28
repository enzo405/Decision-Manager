using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider motivationBar;
    public Slider stressBar;
    public Slider performanceBar;
    public Slider turnoverBar;

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
        stressBar.value = (float)stats.Stress / StatSystem.GetMaxStress() * 100f;
        performanceBar.value = (float)(stats.Performance - StatSystem.GetMinPerformance()) / (100f - StatSystem.GetMinPerformance()) * 100f;
        turnoverBar.value = (float)stats.Turnover / StatSystem.GetMaxTurnover() * 100f;
    }
}