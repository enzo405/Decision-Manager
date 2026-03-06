using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    public static FeedbackUI Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI statsChangesText;
    public TextMeshProUGUI eventMessageText;
    public Button continueButton;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        CardManager.Instance.OnCardResolved += ShowFeedback;
        RandomEventSystem.Instance.OnEventTriggered += ShowRandomEvent;
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        if (CardManager.Instance != null)
        {
            CardManager.Instance.OnCardResolved -= ShowFeedback;
            RandomEventSystem.Instance.OnEventTriggered -= ShowRandomEvent;
        }
    }

    public void ShowRandomEvent(Event randomEvent)
    {
        if (randomEvent == null)
        {
            eventMessageText.text = "";
        }
        else
        {
            eventMessageText.text = $"{randomEvent.Message}\n" +
                                $"Motivation {Signed(randomEvent.MotivationDelta)}\n" +
                                $"Stress {Signed(randomEvent.StressDelta)}\n" +
                                $"Performance {Signed(randomEvent.PerformanceDelta)}\n" +
                                $"Turnover {Signed(randomEvent.TurnoverDelta)}";
        }
    }

    public void ShowFeedback(CardData card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        // Show the popup
        gameObject.SetActive(true);

        // Success or failure title
        resultText.text = wasSuccess ? "Succès" : "Échec";

        // The explanatory message from the card
        messageText.text = wasSuccess ? card.successMessage : card.failureMessage;

        // Stats changes summary
        statsChangesText.text = $"Motivation {Signed(motivDelta)}\n" +
                                $"Stress {Signed(stressDelta)}\n" +
                                $"Performance {Signed(perfDelta)}\n" +
                                $"Turnover {Signed(turnoverDelta)}";

        // Continue button closes popup and triggers next turn
        continueButton.onClick.RemoveAllListeners();

        if (GameManager.Instance.IsGameOver)
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Terminer partie";
        }
        continueButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.IsGameOver)
            {
                GameManager.EndGame();
            }
            else
            {
                gameObject.SetActive(false);
                GameManager.Instance.OnNextTurn();
            }
        });
    }

    private static string Signed(int v) => v >= 0 ? $"+{v}" : $"{v}";
}