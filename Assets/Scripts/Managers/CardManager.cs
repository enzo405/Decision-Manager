using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

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

    public List<Card> PickRandomCards(int count)
    {
        var allPlayedSlugs = GameHistoryManager.Instance.History
            .Select(t => t.CardSlug)
            .ToHashSet();

        var recentSlugs = allPlayedSlugs
            .TakeLast(3)  // ← Last 3 turns to avoid repetition
            .ToHashSet();

        var availableCards = CardApiService.Instance
            .GetUnlockedCards()
            .Where(c => !allPlayedSlugs.Contains(c.Slug))
            .ToList();

        var smartPool = availableCards
            .Where(card => IsCardUnlocked(card, allPlayedSlugs, recentSlugs))
            .Distinct()
            .ToList();

        Debug.Log($"[CardManager] Available: {availableCards.Count}, SmartPool: {smartPool.Count}");

        if (smartPool.Count == count)
            return smartPool.Shuffle().ToList();
        else if (smartPool.Count >= count)
            return smartPool.Shuffle().Take(count).ToList();

        var remaining = count - smartPool.Count;

        var fallbackCards = availableCards
            .Except(smartPool)
            .Shuffle()
            .Take(remaining);

        return smartPool.Concat(fallbackCards).Shuffle().ToList();
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

    private bool IsCardUnlocked(Card card, HashSet<string> allPlayedSlugs, HashSet<string> recentSlugs)
    {
        switch (card.Type)
        {
            case CardType.Universal:
                // Always available (no requirements check needed)
                return true;

            case CardType.Reactive:
                // Unlocks if ANY requirement met in last 3 turns
                if (card.RequiredCardSlugs.Count == 0)
                    return true;
                return card.RequiredCardSlugs.Any(req => recentSlugs.Contains(req));

            case CardType.Foundation:
                // Unlocks if ALL requirements met in full history
                if (card.RequiredCardSlugs.Count == 0)
                    return true;
                return card.RequiredCardSlugs.All(req => allPlayedSlugs.Contains(req));

            case CardType.Emergency:
                if (card.RequiredCardSlugs.Count == 0)
                    return true;
                return card.RequiredCardSlugs.Any(req => recentSlugs.Contains(req));

            default:
                return false;
        }
    }
}