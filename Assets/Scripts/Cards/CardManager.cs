using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public event Action<CardData, bool, int, int, int, int> OnCardResolved;
    // (card, wasSuccess, motivDelta, stressDelta, perfDelta, turnoverDelta)

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PlayCard(CardData card)
    {
        bool success = UnityEngine.Random.value <= card.successProbability;

        int motiv, stress, perf, turnover;

        if (success)
        {
            motiv = card.motivationEffect;
            stress = card.stressEffect;
            perf = card.performanceEffect;
            turnover = card.turnoverEffect;
        }
        else
        {
            motiv = card.motivationEffectOnFailure;
            stress = card.stressEffectOnFailure;
            perf = card.performanceEffectOnFailure;
            turnover = card.turnoverEffectOnFailure;
        }

        StatSystem.Instance.ApplyEffects(motiv, stress, perf, turnover);
        OnCardResolved?.Invoke(card, success, motiv, stress, perf, turnover);
        GameManager.Instance.OnCardPlayed();
    }
}