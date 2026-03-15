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

    // Stat colors
    private static readonly Color ColorRed = new(0.91f, 0.30f, 0.24f); // #E84C3D
    private static readonly Color ColorGreen = new(0.18f, 0.80f, 0.44f); // #2ECC70
    private static readonly Color ColorOrange = new(0.90f, 0.49f, 0.13f); // #E67E21

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
        Color riskColor = GetRiskColor(card.RiskLevel);
        riskBadgeText.text = GetRiskLabel(card.RiskLevel);
        riskBadgeText.color = riskColor;
        riskBadgeBg.color = new Color(riskColor.r, riskColor.g, riskColor.b, 0.04f);
        borderImage.color = new Color(riskColor.r, riskColor.g, riskColor.b, 0.55f);

        // Probability
        probValue.text = $"{card.SuccessProbability * 100f}%";

        // Effects — values with sign and color
        SetStatValue(statValuePerformance, card.PerformanceEffect);
        SetStatValue(statValueTurnover, card.TurnoverEffect);
        SetStatValue(statValueMotivation, card.MotivationEffect);
        SetStatValue(statValueStress, card.StressEffect);

        // Messages
        msgSuccessText.text = card.SuccessMessage;
        msgFailureText.text = card.FailureMessage;
    }

    private void SetStatValue(TextMeshProUGUI label, int value)
    {
        label.text = value >= 0 ? $"+{value}" : $"{value}";
        label.color = value >= 0 ? ColorGreen : ColorRed;
    }

    private Color GetRiskColor(RiskLevel risk) => risk switch
    {
        RiskLevel.Low => ColorGreen,
        RiskLevel.Medium => ColorOrange,
        RiskLevel.High => ColorRed,
        _ => Color.white
    };

    private string GetRiskLabel(RiskLevel risk) => risk switch
    {
        RiskLevel.Low => "FAIBLE",
        RiskLevel.Medium => "MOYEN",
        RiskLevel.High => "ÉLEVÉ",
        _ => ""
    };


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
}