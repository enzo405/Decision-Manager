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
        Debug.Log("[CardApiService] Awake");
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
            (err) => onError?.Invoke(err)
        ));
    }

    public Card[] GetUnlockedCards()
    {
        int level = PlayerProgressionSystem.Instance.LevelThisGame;
        return AllCards
            .Where(card => card.RequiredLevel <= level)
            .ToArray();
    }
}