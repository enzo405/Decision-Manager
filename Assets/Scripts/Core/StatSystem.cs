using UnityEngine;
using System;

public class StatSystem : MonoBehaviour
{
    public static StatSystem Instance { get; private set; }

    [Header("Initial Values")]
    [Range(0, 100)] public int initialMotivation = 50;
    [Range(0, 100)] public int initialStress = 30;
    [Range(0, 100)] public int initialPerformance = 40;
    [Range(0, 100)] public int initialTurnover = 20;

    public int Motivation { get; private set; }
    public int Stress { get; private set; }
    public int Performance { get; private set; }
    public int Turnover { get; private set; }

    // Anyone can subscribe to know when stats change
    public event Action OnStatsChanged;

    public int GetMaxStress() => Mathf.Max(85 - (PlayerProgressionSystem.Instance.CurrentLevel * 2), 60);
    public int GetMaxTurnover() => Mathf.Max(80 - (PlayerProgressionSystem.Instance.CurrentLevel * 2), 55);
    public int GetMinPerformance() => Mathf.Min(15 + (PlayerProgressionSystem.Instance.CurrentLevel * 2), 35);


    public void Awake()
    {
        Debug.Log("StatSystem Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        Motivation = initialMotivation;
        Stress = initialStress;
        Performance = initialPerformance;
        Turnover = initialTurnover;
        OnStatsChanged?.Invoke();
    }

    public void ApplyEffects(int motivation, int stress, int performance, int turnover)
    {
        Motivation = Mathf.Clamp(Motivation + motivation, 0, 100);
        Stress = Mathf.Clamp(Stress + stress, 0, 100);
        Performance = Mathf.Clamp(Performance + performance, 0, 100);
        Turnover = Mathf.Clamp(Turnover + turnover, 0, 100);

        // Interdependency logic (like in your GDD)
        ApplyInterdependencies();

        OnStatsChanged?.Invoke();
    }

    public DefeatReason CheckDefeatConditions()
    {
        Debug.Log($"Checking defeat conditions: Stress={Stress}, Turnover={Turnover}, Performance={Performance}");
        Debug.Log($"Thresholds: MaxStress={GetMaxStress()}, MaxTurnover={GetMaxTurnover()}, MinPerformance={GetMinPerformance()}");
        if (Stress >= GetMaxStress()) return DefeatReason.Burnout;
        if (Turnover >= GetMaxTurnover()) return DefeatReason.MassiveDepartures;
        if (Performance <= GetMinPerformance()) return DefeatReason.PoorPerformance;
        return DefeatReason.None;
    }

    private void ApplyInterdependencies()
    {
        // High stress slowly degrades motivation
        if (Stress > 70)
            Motivation = Mathf.Clamp(Motivation - 2, 0, 100);

        // High stress increases turnover risk
        if (Stress > 80)
            Turnover = Mathf.Clamp(Turnover + 3, 0, 100);
    }
}

public enum DefeatReason { None, Burnout, MassiveDepartures, PoorPerformance }