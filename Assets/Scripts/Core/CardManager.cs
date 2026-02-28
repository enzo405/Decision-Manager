using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public event Action<CardData, bool, int, int, int, int> OnCardResolved;
    // (card, wasSuccess, motivDelta, stressDelta, perfDelta, turnoverDelta)

    public void Awake()
    {
        Debug.Log("CardManager Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        GameHistoryData.Record(
            card.cardName, success,
            motiv, stress, perf, turnover,
            StatSystem.Instance.Motivation,
            StatSystem.Instance.Stress,
            StatSystem.Instance.Performance,
            StatSystem.Instance.Turnover
        );

        GameManager.Instance.OnCardPlayed();
        OnCardResolved?.Invoke(card, success, motiv, stress, perf, turnover);

        bool wasGood = GameHistoryData.History.Count > 0 &&
                       GameHistoryData.History[^1].wasGoodDecision;
        PlayerProgressionSystem.Instance.AddXP(wasGood);
    }
}