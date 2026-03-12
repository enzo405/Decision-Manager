using UnityEngine;
using System;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public event Action<Card, bool, int, int, int, int> OnCardResolved;
    // (card, wasSuccess, motivDelta, stressDelta, perfDelta, turnoverDelta)

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
        bool success = UnityEngine.Random.value <= card.SuccessProbability;
        int level = PlayerProgressionSystem.Instance.LevelThisGame;
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

        StatSystem.Instance.ApplyEffects(motiv, stress, perf, turnover);

        GameManager.Instance.OnCardPlayed(card, success, motiv, stress, perf, turnover);

        // Trigger le FeedbackUI
        OnCardResolved?.Invoke(card, success, motiv, stress, perf, turnover);
    }
}