using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI effectsText;
    public Button playButton;

    private CardData currentCard;

    public void DisplayCard(CardData card)
    {
        currentCard = card;
        cardNameText.text = card.cardName;
        descriptionText.text = card.description;
        effectsText.text = BuildEffectsText(card);
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => CardManager.Instance.PlayCard(currentCard));
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