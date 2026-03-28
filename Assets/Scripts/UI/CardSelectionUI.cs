using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CardSlot
{
    public GameObject cardObject;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button cardButton;
    public GameObject riskBadgePrefab;
    public Image cardStrip;
}

public class CardSelectionUI : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private RectTransform cardsContainer;

    [Header("Slots")]
    [SerializeField] private CardSlot[] slots = new CardSlot[3];


    public void Start()
    {
        DrawCards(); // Draw cards at the start of the game as well
        GameManager.Instance.OnTurnStarted += DrawCards;

        StartCoroutine(ForceCardWidths());
    }

    public void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTurnStarted -= DrawCards;
    }

    private void DrawCards()
    {
        List<Card> picked = CardManager.Instance.PickRandomCards(3);

        for (int i = 0; i < slots.Length; i++)
        {
            Card card = picked[i];
            CardSlot slot = slots[i];

            slot.titleText.text = card.DisplayName;
            slot.descriptionText.text = card.Description;

            // Modifier la couleur du badge et le text qui va avec:
            Color riskColor = ColorUtilities.GetRiskColorText(card.RiskLevel);
            slot.riskBadgePrefab.GetComponent<Image>().color = ColorUtilities.GetRiskColorBackground(card.RiskLevel);
            slot.riskBadgePrefab.GetComponentInChildren<TextMeshProUGUI>().text = ColorUtilities.GetRiskLabel(card.RiskLevel);
            slot.riskBadgePrefab.GetComponentInChildren<TextMeshProUGUI>().color = riskColor;
            slot.cardStrip.color = riskColor;

            // Capture pour le lambda
            Card capturedCard = card;
            slot.cardButton.onClick.RemoveAllListeners();
            slot.cardButton.onClick.AddListener(() => CardManager.Instance.PlayCard(capturedCard));
        }
    }

    private IEnumerator ForceCardWidths()
    {
        yield return null;

        float containerWidth = cardsContainer.rect.width;
        float padding = 40f; // left + right padding du HLG
        float spacing = 32f; // spacing * espaces entre 3 cartes
        float cardWidth = (containerWidth - padding - spacing) / 3f;

        foreach (CardSlot slot in slots)
        {
            LayoutElement le = slot.cardObject.GetComponent<LayoutElement>();
            le.preferredWidth = cardWidth;
            le.flexibleWidth = 0;
        }
    }
}