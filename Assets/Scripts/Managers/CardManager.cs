using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public event Action<Card, bool, int, int, int, int> OnCardResolved;

    public void Awake()
    {
        Debug.Log("[CardManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayCard(Card card)
    {
        float probability = CalculateSuccessProbability(card);
        bool success = UnityEngine.Random.value <= probability;
        int level = PlayerProgressionManager.Instance.LevelThisGame;
        float negativeMultiplier = 1f + (level * 0.05f); // +5% par niveau

        int motiv, stress, perf, turnover;

        if (success)
        {
            motiv = card.MotivationEffect;
            stress = card.StressEffect;
            perf = card.PerformanceEffect;
            turnover = card.TurnoverEffect;
        }
        else
        {
            motiv = Mathf.RoundToInt(card.MotivationEffectOnFailure * (card.MotivationEffectOnFailure < 0 ? negativeMultiplier : 1f));
            stress = Mathf.RoundToInt(card.StressEffectOnFailure * (card.StressEffectOnFailure > 0 ? negativeMultiplier : 1f));
            perf = Mathf.RoundToInt(card.PerformanceEffectOnFailure * (card.PerformanceEffectOnFailure < 0 ? negativeMultiplier : 1f));
            turnover = Mathf.RoundToInt(card.TurnoverEffectOnFailure * (card.TurnoverEffectOnFailure > 0 ? negativeMultiplier : 1f));
        }

        OnCardResolved?.Invoke(card, success, motiv, stress, perf, turnover);
    }

    private float CalculateSuccessProbability(Card card)
    {
        float prob = card.SuccessProbability;

        foreach (CardStatThreshold threshold in card.StatThresholds)
        {
            int statValue = StatManager.Instance.GetStatValue(threshold.StatName);
            bool isThresholdExceeded = threshold.Condition == ConditionTreshold.Above
                ? statValue >= threshold.Threshold
                : statValue < threshold.Threshold;

            if (isThresholdExceeded)
            {
                prob -= (float)threshold.PenaltyAmount;
            }
        }

        return prob;
    }
}