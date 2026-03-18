using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDetailsOverlayUI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private RectTransform bottomSheet;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float animDuration = 0.3f;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI riskBadgeText;
    [SerializeField] private Image riskBadgeBg;

    [Header("Description")]
    [SerializeField] private TextMeshProUGUI description;

    [Header("Probability")]
    [SerializeField] private RectTransform probFill;
    [SerializeField] private RectTransform probTrack;
    [SerializeField] private TextMeshProUGUI probValue;

    [Header("Effects")]
    [SerializeField] private TextMeshProUGUI statValuePerformance;
    [SerializeField] private TextMeshProUGUI statValueTurnover;
    [SerializeField] private TextMeshProUGUI statValueMotivation;
    [SerializeField] private TextMeshProUGUI statValueStress;

    [Header("Messages")]
    [SerializeField] private TextMeshProUGUI msgSuccessText;
    [SerializeField] private TextMeshProUGUI msgFailureText;

    [Header("Border")]
    [SerializeField] private Image borderImage;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button handleButton;

    private Card _currentCard;


    public void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void Open(Card card)
    {
        gameObject.SetActive(true);
        _currentCard = card;
        Populate(card);
        StartCoroutine(AnimateOpenDelayed());
    }

    public void Close()
    {
        _currentCard = null;
        StartCoroutine(AnimateClose());
    }

    private void Populate(Card card)
    {
        // Header
        cardName.text = card.DisplayName;
        description.text = card.Description;

        // Risk badge + strip + border color
        riskBadgeText.text = ColorUtilities.GetRiskLabel(card.RiskLevel);
        riskBadgeText.color = ColorUtilities.GetRiskColorText(card.RiskLevel);
        riskBadgeBg.color = ColorUtilities.GetRiskColorBackground(card.RiskLevel);
        borderImage.color = ColorUtilities.GetRiskColorText(card.RiskLevel);

        // Probability
        probValue.text = $"{card.SuccessProbability * 100f}%";

        // Effects — values with sign and color
        SetStatValue(statValuePerformance, card.PerformanceEffect, "Performance");
        SetStatValue(statValueTurnover, card.TurnoverEffect, "Turnover");
        SetStatValue(statValueMotivation, card.MotivationEffect, "Motivation");
        SetStatValue(statValueStress, card.StressEffect, "Stress");

        // Messages
        msgSuccessText.text = card.SuccessMessage;
        msgFailureText.text = card.FailureMessage;
    }

    private void SetStatValue(TextMeshProUGUI label, int value, string statName)
    {
        label.text = value >= 0 ? $"+{value}" : $"{value}";
        if (statName == "Performance" || statName == "Motivation")
            label.color = value >= 0 ? ColorUtilities.Green : ColorUtilities.Red;
        else
            label.color = value < 0 ? ColorUtilities.Green : ColorUtilities.Red;
    }

    #region Animation Methods
    private IEnumerator AnimateOpenDelayed()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(bottomSheet);
        yield return null;

        float trackWidth = probTrack.rect.width;
        float fillWidth = trackWidth * _currentCard.SuccessProbability;
        probFill.sizeDelta = new Vector2(fillWidth, probFill.sizeDelta.y);
        probFill.anchoredPosition = new Vector2(fillWidth / 2f, 0f);
        StartCoroutine(AnimateOpen());
    }

    private IEnumerator AnimateOpen()
    {
        float sheetHeight = bottomSheet.rect.height;
        float elapsed = 0f;
        canvasGroup.interactable = true;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            bottomSheet.anchoredPosition = Vector2.Lerp(new Vector2(0, -sheetHeight), Vector2.zero, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        bottomSheet.anchoredPosition = Vector2.zero;
        canvasGroup.alpha = 1f;
    }

    private IEnumerator AnimateClose()
    {
        float sheetHeight = bottomSheet.rect.height;
        float elapsed = 0f;
        canvasGroup.interactable = false;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            bottomSheet.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, -sheetHeight), t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
    #endregion
}