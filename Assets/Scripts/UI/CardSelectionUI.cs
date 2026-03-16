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
    public Button cardButton;
    public GameObject riskBadgePrefab;
}

public class CardSelectionUI : MonoBehaviour
{
    public CardSlot[] slots = new CardSlot[3];
    private Card[] unlockedCards;

    public void Start()
    {
        unlockedCards = CardApiService.Instance.GetUnlockedCards();

        DrawCards(); // Draw cards at the start of the game as well
        GameManager.Instance.OnTurnStarted += DrawCards;
    }

    public void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTurnStarted -= DrawCards;
    }

    private void DrawCards()
    {
        // Mélange et pioche 3 cartes aléatoires
        Card[] picked = PickRandomCards(3);

        for (int i = 0; i < slots.Length; i++)
        {
            Card card = picked[i];
            CardSlot slot = slots[i];

            slot.titleText.text = card.DisplayName;
            slot.descriptionText.text = card.Description;

            // Modifier la couleur du badge et le text qui va avec:
            Color riskColor = RiskUtilities.GetRiskColor(card.RiskLevel);
            slot.riskBadgePrefab.GetComponent<Image>().color = new Color(riskColor.r, riskColor.g, riskColor.b, 0.06f);
            slot.riskBadgePrefab.GetComponentInChildren<TextMeshProUGUI>().text = RiskUtilities.GetRiskLabel(card.RiskLevel);
            slot.riskBadgePrefab.GetComponentInChildren<TextMeshProUGUI>().color = riskColor;

            // Capture pour le lambda
            Card capturedCard = card;
            slot.cardButton.onClick.RemoveAllListeners();
            slot.cardButton.onClick.AddListener(() => CardManager.Instance.PlayCard(capturedCard));
        }
    }

    private Card[] PickRandomCards(int count)
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
}