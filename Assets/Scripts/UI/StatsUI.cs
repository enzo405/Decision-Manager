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
        stressBar.value = stats.Stress;
        performanceBar.value = stats.Performance;
        turnoverBar.value = stats.Turnover;
    }
}