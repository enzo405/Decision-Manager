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

    public CardCombo CheckForCombos()
    {
        List<string> cardsHistorySlug = GameHistoryManager.Instance.History
            .Where(card => card.WasSuccess)
            .Select(c => c.CardSlug)
            .ToList();

        var allCardCombos = CardComboApiService.Instance.AllCombos;

        return allCardCombos
            .FirstOrDefault(c => c.TriggerSlugs.All(t => cardsHistorySlug.Contains(t)));
    }
}