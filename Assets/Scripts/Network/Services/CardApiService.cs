using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardApiService : MonoBehaviour
{
    public static CardApiService Instance { get; private set; }

    public List<Card> AllCards { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator FetchAllCards(Action<List<Card>> onSuccess = null, Action<string> onError = null)
    {
        yield return StartCoroutine(ApiClient.Get<List<Card>>(
            "/api/cards",
            cards =>
            {
                AllCards = cards;
                onSuccess?.Invoke(cards);
            },
            onError
        ));
    }

    public Card[] GetUnlockedCards(int level)
    {
        return AllCards
            .Where(card => card.RequiredLevel <= level)
            .ToArray();
    }
}