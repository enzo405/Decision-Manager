using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject feedbackUIPanel;

    [Header("Animation")]
    [SerializeField] private RectTransform bottomSheet;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float animDuration = 0.3f;


    [Header("Overlay Elements")]
    public TextMeshProUGUI resultText;
    public Image stripImage;
    public Image successBadge;
    public Image successBadgeIcon;
    public Button continueButton;
    public Sprite checkMarkSprite;
    public Sprite xMarkSprite;

    [Header("Card Feedback Elements")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardMessageText;
    public Image cardMessageBox;
    public TextMeshProUGUI statsPerfChangesText;
    public TextMeshProUGUI statsMotivChangesText;
    public TextMeshProUGUI statsStressChangesText;
    public TextMeshProUGUI statsTurnoverChangesText;

    [Header("Random Event Elements")]
    public GameObject eventBody;
    public TextMeshProUGUI eventName;
    public TextMeshProUGUI eventMessageText;
    public GameObject eventEffectPrefab;
    public GameObject eventEffectContainer;

    public void Start()
    {
        continueButton.onClick.AddListener(() =>
            {
                if (!GameManager.Instance.IsGameOver)
                {
                    StartCoroutine(AnimateClose());
                }
                GameManager.Instance.OnNextTurn();
            }
        );

        GameManager.Instance.OnTurnResolved += Open;
    }

    public void OnDestroy()
    {
        continueButton.onClick.RemoveAllListeners();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTurnResolved -= Open;
        }
    }

    public void Open(Card card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta, Event randomEvent, int fromTurnDecision)
    {
        feedbackUIPanel.SetActive(true);
        Populate(card, wasSuccess, motivDelta, stressDelta, perfDelta, turnoverDelta, randomEvent, fromTurnDecision);

        StartCoroutine(AnimateOpenDelayed());
    }

    private void Populate(Card card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta, Event randomEvent, int fromTurnDecision)
    {
        stripImage.color = wasSuccess ? ColorUtilities.SuccessColor : ColorUtilities.FailColor;
        resultText.text = wasSuccess ? "Succès" : "Échec";
        resultText.color = wasSuccess ? ColorUtilities.Green : ColorUtilities.Red;

        var softBg = wasSuccess ? ColorUtilities.SoftGreen : ColorUtilities.SoftRed;
        successBadge.color = softBg;
        successBadgeIcon.sprite = wasSuccess ? checkMarkSprite : xMarkSprite;
        if (GameManager.Instance.IsGameOver)
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Terminer partie";
        }

        cardNameText.text = card.DisplayName;
        cardMessageBox.color = softBg;
        cardMessageText.text = wasSuccess ? card.SuccessMessage : card.FailureMessage;
        SetStatValue(statsPerfChangesText, perfDelta, "Performance");
        SetStatValue(statsTurnoverChangesText, turnoverDelta, "Turnover");
        SetStatValue(statsMotivChangesText, motivDelta, "Motivation");
        SetStatValue(statsStressChangesText, stressDelta, "Stress");

        if (randomEvent == null)
        {
            eventBody.SetActive(false);
        }
        else
        {
            eventBody.SetActive(true);
            var originCard = GameHistoryManager.Instance.History[fromTurnDecision - 1].CardDisplayName;
            eventName.text = randomEvent.Name;
            eventMessageText.text = $"Événement déclenché par \"{originCard}\" (tour {fromTurnDecision})\n\n" +
                                    $"{randomEvent.Message}\n\n";

            // TODO: Add event effects display
            // public GameObject eventEffectPrefab;
            // public GameObject eventEffectContainer;
        }

    }

    #region Animation methods
    private IEnumerator AnimateOpenDelayed()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(bottomSheet);
        yield return null;

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

        feedbackUIPanel.SetActive(false);
    }
    #endregion

    private static string Signed(int v) => v >= 0 ? $"+{v}" : $"{v}";

    private void SetStatValue(TextMeshProUGUI label, int value, string statName)
    {
        label.text = value >= 0 ? $"+{value}" : $"{value}";
        if (statName == "Performance" || statName == "Motivation")
            label.color = value >= 0 ? ColorUtilities.Green : ColorUtilities.Red;
        else // Stress or Turnover
            label.color = value < 0 ? ColorUtilities.Green : ColorUtilities.Red;
    }
}