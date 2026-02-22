using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider motivationBar;
    public Slider stressBar;
    public Slider performanceBar;
    public Slider turnoverBar;

    void OnDisable()
    {
        if (StatSystem.Instance != null)
            StatSystem.Instance.OnStatsChanged -= RefreshUI;
    }


    void Start()
    {
        StatSystem.Instance.OnStatsChanged += RefreshUI;
        RefreshUI();
    }

    void RefreshUI()
    {
        motivationBar.value = StatSystem.Instance.Motivation;
        stressBar.value = StatSystem.Instance.Stress;
        performanceBar.value = StatSystem.Instance.Performance;
        turnoverBar.value = StatSystem.Instance.Turnover;
    }
}