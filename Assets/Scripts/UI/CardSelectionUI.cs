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
    private CardData[] availableCards;
    public void Start()
    {
        availableCards = Resources.LoadAll<CardData>("Cards");
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
        CardData[] picked = PickRandomCards(3);

        for (int i = 0; i < slots.Length; i++)
        {
            CardData card = picked[i];
            CardSlot slot = slots[i];

            slot.titleText.text = card.cardName;
            slot.descriptionText.text = card.description;
            slot.effectsText.text = BuildEffectsText(card);

            // Capture pour le lambda
            CardData capturedCard = card;
            slot.cardButton.onClick.RemoveAllListeners();
            slot.cardButton.onClick.AddListener(() => CardManager.Instance.PlayCard(capturedCard));
        }
    }

    public CardData[] PickRandomCards(int count)
    {
        var unlocked = Array.FindAll(availableCards,
            card => card.requiredLevel <= PlayerProgressionSystem.Instance.CurrentLevel);

        CardData[] shuffled = (CardData[])unlocked.Clone();

        // Fisher-Yates shuffle
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        CardData[] result = new CardData[count];
        for (int i = 0; i < count; i++)
            result[i] = shuffled[i];
        return result;
    }

    private string BuildEffectsText(CardData card)
    {
        return $"Motivation {Signed(card.motivationEffect)}\n" +
               $"Stress {Signed(card.stressEffect)}\n" +
               $"Performance {Signed(card.performanceEffect)}\n" +
               $"Risque : {card.riskLevel}";
    }

    private string Signed(int val) => val >= 0 ? $"+{val}" : $"{val}";
}