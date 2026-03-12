using UnityEngine;
using System;

public class StatSystem : MonoBehaviour
{
    public static StatSystem Instance { get; private set; }

    public int Motivation { get; private set; }
    public int Stress { get; private set; }
    public int Performance { get; private set; }
    public int Turnover { get; private set; }

    // Anyone can subscribe to know when stats change
    public event Action OnStatsChanged;

    public static int GetMaxTurnover => ConfigApiService.Instance.DefeatConditions.Turnover.Max;
    public static int GetMinTurnover => ConfigApiService.Instance.DefeatConditions.Turnover.Min;
    public static int GetMaxPerformance => ConfigApiService.Instance.DefeatConditions.Performance.Max;
    public static int GetMinPerformance => ConfigApiService.Instance.DefeatConditions.Performance.Min;
    public static int GetMaxStress => ConfigApiService.Instance.DefeatConditions.Stress.Max;
    public static int GetMinStress => ConfigApiService.Instance.DefeatConditions.Stress.Min;
    public static int GetMaxMotivation => ConfigApiService.Instance.DefeatConditions.Motivation.Max;
    public static int GetMinMotivation => ConfigApiService.Instance.DefeatConditions.Motivation.Min;


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
        NewGame();
    }

    public void NewGame()
    {
        var statsInit = ConfigApiService.Instance.StatsInit;
        Motivation = statsInit.InitialMotivation;
        Stress = statsInit.InitialStress;
        Performance = statsInit.InitialPerformance;
        Turnover = statsInit.InitialTurnover;
        OnStatsChanged?.Invoke();
    }

    public void ApplyEffects(int motivation, int stress, int performance, int turnover)
    {
        Motivation = Mathf.Clamp(Motivation + motivation, GetMinMotivation, GetMaxMotivation);
        Stress = Mathf.Clamp(Stress + stress, GetMinStress, GetMaxStress);
        Performance = Mathf.Clamp(Performance + performance, GetMinPerformance, GetMaxPerformance);
        Turnover = Mathf.Clamp(Turnover + turnover, GetMinTurnover, GetMaxTurnover);

        ApplyInterdependencies();

        OnStatsChanged?.Invoke();
    }

    public DefeatReason CheckDefeatConditions()
    {
        Debug.Log($"Checking defeat conditions: Stress={Stress}, Turnover={Turnover}, Performance={Performance}");

        if (Stress >= GetMaxStress) return DefeatReason.Burnout;
        if (Turnover >= GetMaxTurnover) return DefeatReason.MassiveDepartures;
        if (Performance <= GetMinPerformance) return DefeatReason.PoorPerformance;
        return DefeatReason.None;
    }

    private void ApplyInterdependencies()
    {
        // High stress slowly degrades motivation
        if (Stress > 70)
            Motivation = Mathf.Clamp(Motivation - 2, GetMinMotivation, GetMaxMotivation);

        // High stress increases turnover risk
        if (Stress > 80)
            Turnover = Mathf.Clamp(Turnover + 3, GetMinTurnover, GetMaxTurnover);
    }
}
