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
        StartTurn();
    }

    public void StartTurn()
    {
        if (IsGameOver) return;
        OnTurnStarted?.Invoke();
    }

    public void OnCardPlayed(Card card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        GameHistoryManager.Instance.RecordTurn(
            card, wasSuccess,
            motivDelta, stressDelta, perfDelta, turnoverDelta,
            StatSystem.Instance.Motivation,
            StatSystem.Instance.Stress,
            StatSystem.Instance.Performance,
            StatSystem.Instance.Turnover
        );

        EventSystem.Instance.RollEvent();

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

    public void OnNextTurn()
    {
        if (!IsGameOver)
        {
            CurrentWeek++;
            StartTurn();
        }
    }

    public void NewGame()
    {
        ResetGameStats();
        SceneManager.LoadScene("MainGame");
    }

    public void ResetGameStats()
    {
        CurrentWeek = 1;
        IsGameOver = false;
        GameHistoryManager.Instance.Reset();
        EventSystem.Instance.Reset();
        StatSystem.Instance.NewGame();
        PlayerProgressionSystem.Instance.NewGame();
    }

    public static void EndGame()
    {
        PlayerProgressionSystem.Instance.EndGame();
        SceneManager.LoadScene("GameOver");
    }

    private void PreloadEndGame(bool isVictory, DefeatReason reason)
    {
        Debug.Log("Preloading end game. Victory: " + isVictory + ", Reason: " + reason);
        IsGameOver = true;
        GameOverData.IsVictory = isVictory;
        GameOverData.Reason = reason;
    }
}