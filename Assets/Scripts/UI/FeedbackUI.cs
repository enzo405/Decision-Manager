using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{

    [Header("UI References")]
    public GameObject feedbackUIPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI statsChangesText;
    public TextMeshProUGUI eventMessageText;
    public Button continueButton;

    public void Start()
    {
        continueButton.onClick.AddListener(() =>
            {
                feedbackUIPanel.SetActive(false);
                GameManager.Instance.OnNextTurn();
            }
        );

        CardManager.Instance.OnCardResolved += ShowFeedback;
        EventSystem.Instance.OnEventTriggered += ShowRandomEvent;
    }

    public void OnDestroy()
    {
        continueButton.onClick.RemoveAllListeners();
        if (CardManager.Instance != null)
        {
            CardManager.Instance.OnCardResolved -= ShowFeedback;
            EventSystem.Instance.OnEventTriggered -= ShowRandomEvent;
        }
    }

    public void ShowRandomEvent(Event randomEvent, int fromTurnDecision)
    {
        if (randomEvent == null)
        {
            eventMessageText.text = "";
        }
        else
        {
            var originCard = GameHistoryManager.Instance.History[fromTurnDecision - 1].CardDisplayName;

            eventMessageText.text = $"Événement déclenché par \"{originCard}\" (tour {fromTurnDecision})\n\n" +
                            $"{randomEvent.Message}\n\n" +
                            $"Motivation {Signed(randomEvent.MotivationDelta)}\n" +
                            $"Stress {Signed(randomEvent.StressDelta)}\n" +
                            $"Performance {Signed(randomEvent.PerformanceDelta)}\n" +
                            $"Turnover {Signed(randomEvent.TurnoverDelta)}";
        }
    }

    public void ShowFeedback(Card card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        feedbackUIPanel.SetActive(true);
        // Success or failure title
        resultText.text = wasSuccess ? "Succès" : "Échec";

        // The explanatory message from the card
        messageText.text = wasSuccess ? card.SuccessMessage : card.FailureMessage;

        // Stats changes summary
        statsChangesText.text = $"Motivation {Signed(motivDelta)}\n" +
                                $"Stress {Signed(stressDelta)}\n" +
                                $"Performance {Signed(perfDelta)}\n" +
                                $"Turnover {Signed(turnoverDelta)}";

        if (GameManager.Instance.IsGameOver)
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Terminer partie";
        }
    }

    private static string Signed(int v) => v >= 0 ? $"+{v}" : $"{v}";
}