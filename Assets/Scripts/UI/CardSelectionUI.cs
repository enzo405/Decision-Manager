using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class CardSlot
{
    public GameObject cardObject;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI effectsText;
    public Button cardButton;
}

public class CardSelectionUI : MonoBehaviour
{
    public CardSlot[] slots = new CardSlot[3];
    private Card[] unlockedCards;
    public void Start()
    {
        unlockedCards = CardApiService.Instance.GetUnlockedCards();

        GameManager.Instance.OnTurnStarted += DrawCards;
        DrawCards();
    }

    public void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTurnStarted -= DrawCards;
    }

    public void DrawCards()
    {
        // Mélange et pioche 3 cartes aléatoires
        Card[] picked = PickRandomCards(3);

        for (int i = 0; i < slots.Length; i++)
        {
            Card card = picked[i];
            CardSlot slot = slots[i];

            slot.titleText.text = card.DisplayName;
            slot.descriptionText.text = card.Description;
            slot.effectsText.text = BuildEffectsText(card);

            // Capture pour le lambda
            Card capturedCard = card;
            slot.cardButton.onClick.RemoveAllListeners();
            slot.cardButton.onClick.AddListener(() => CardManager.Instance.PlayCard(capturedCard));
        }
    }

    public Card[] PickRandomCards(int count)
    {
        Card[] shuffled = (Card[])unlockedCards.Clone();

        // Fisher-Yates shuffle
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        Card[] result = new Card[count];
        for (int i = 0; i < count; i++)
            result[i] = shuffled[i];
        return result;
    }

    private string BuildEffectsText(Card card)
    {
        return $"Motivation {Signed(card.MotivationEffect)}\n" +
               $"Stress {Signed(card.StressEffect)}\n" +
               $"Performance {Signed(card.PerformanceEffect)}\n" +
               $"Turnover {Signed(card.TurnoverEffect)}\n" +
               $"Risque : {card.RiskLevel}";
    }

    private string Signed(int v) => v >= 0 ? $"+{v}" : $"{v}";
}