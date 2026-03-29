using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardComboManager : MonoBehaviour
{
    public static CardComboManager Instance { get; private set; }

    public void Awake()
    {
        Debug.Log("[CardComboManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public CardCombo CheckForCombo(Card playedCard)
    {
        List<string> cardsHistorySlug = GameHistoryManager.Instance.History
            .Where(card => card.WasSuccess)
            .Select(c => c.CardSlug)
            .ToList();

        var historyCombo = GameHistoryManager.Instance.HistoryCombo.Values;

        return CardComboApiService.Instance.AllCombos
            .Where(c => c.TriggerSlugs.Contains(playedCard.Slug))
            .Where(c => !historyCombo.Contains(c)) // Exclure les combos déjà passé
            .FirstOrDefault(c => c.TriggerSlugs.All(t => cardsHistorySlug.Contains(t)));
    }
}