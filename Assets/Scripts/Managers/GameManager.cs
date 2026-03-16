using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int totalWeeks = 12;

    public int CurrentWeek { get; private set; } = 1;
    public bool IsGameOver { get; private set; } = false;

    public event Action OnTurnStarted;
    public event Action<Card, bool, int, int, int, int> OnCardPlayedTriggered;
    public event Action OnNewGameTriggered;
    public event Action OnEndGameTriggered;
    public event Action OnGameAbandonedTriggered;

    public void Awake()
    {
        Debug.Log("[GameManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        CardManager.Instance.OnCardResolved += OnCardPlayed;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("MainGame");
        CurrentWeek = 1;
        IsGameOver = false;
        OnNewGameTriggered?.Invoke();
        OnTurnStarted?.Invoke();
    }

    public void OnNextTurn()
    {
        if (!IsGameOver)
        {
            CurrentWeek++;
            OnTurnStarted?.Invoke();
        }
        else
        {
            OnEndGameTriggered?.Invoke();
            SceneManager.LoadScene("GameOver");
        }
    }

    public void AbandonGame()
    {
        OnGameAbandonedTriggered?.Invoke();
        SceneManager.LoadScene("MainMenu");
    }

    private void OnCardPlayed(Card card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        OnCardPlayedTriggered?.Invoke(card, wasSuccess,
            motivDelta, stressDelta, perfDelta, turnoverDelta);

        var defeat = StatSystem.Instance.CheckDefeatConditions();
        if (defeat != DefeatReason.None)
        {
            PreloadEndGame(false, defeat);
            return;
        }

        if (CurrentWeek >= totalWeeks)
        {
            PreloadEndGame(true, DefeatReason.None);
        }
    }

    private void PreloadEndGame(bool isVictory, DefeatReason reason)
    {
        Debug.Log("Preloading end game. Victory: " + isVictory + ", Reason: " + reason);
        IsGameOver = true;
        GameOverData.IsVictory = isVictory;
        GameOverData.Reason = reason;
    }
}