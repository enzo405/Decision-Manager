using UnityEngine;
using System;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public event Action<CardData, bool, int, int, int, int> OnCardResolved;
    // (card, wasSuccess, motivDelta, stressDelta, perfDelta, turnoverDelta)

    public CardData[] AllCards { get; private set; }

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

    public void Start()
    {
        AllCards = Resources.LoadAll<CardData>("Cards");
        Debug.Log($"Loaded {AllCards.Length} cards.");
    }

    public CardData[] GetUnlockedCards()
    {
        return AllCards
            .Where(card => card.requiredLevel <= PlayerProgressionSystem.Instance.LevelThisGame)
            .ToArray();
    }

    public void PlayCard(CardData card)
    {
        bool success = UnityEngine.Random.value <= card.successProbability;
        int level = PlayerProgressionSystem.Instance.LevelThisGame;
        float negativeMultiplier = 1f + (level * 0.05f); // +5% par niveau

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
            motiv = Mathf.RoundToInt(card.motivationEffectOnFailure * (card.motivationEffectOnFailure < 0 ? negativeMultiplier : 1f));
            stress = Mathf.RoundToInt(card.stressEffectOnFailure * (card.stressEffectOnFailure > 0 ? negativeMultiplier : 1f));
            perf = Mathf.RoundToInt(card.performanceEffectOnFailure * (card.performanceEffectOnFailure < 0 ? negativeMultiplier : 1f));
            turnover = Mathf.RoundToInt(card.turnoverEffectOnFailure * (card.turnoverEffectOnFailure > 0 ? negativeMultiplier : 1f));
        }

        StatSystem.Instance.ApplyEffects(motiv, stress, perf, turnover);

        GameManager.Instance.OnCardPlayed(card, success, motiv, stress, perf, turnover);

        // Trigger le FeedbackUI
        OnCardResolved?.Invoke(card, success, motiv, stress, perf, turnover);
    }
}