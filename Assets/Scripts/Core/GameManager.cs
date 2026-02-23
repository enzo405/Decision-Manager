using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int totalWeeks = 12;

    public int CurrentWeek { get; private set; } = 1;
    public bool IsGameOver { get; private set; } = false;

    public event Action OnTurnStarted;
    public event Action<bool, DefeatReason> OnGameEnded; // bool = isVictory

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        if (IsGameOver) return;
        OnTurnStarted?.Invoke();
    }

    public void OnCardPlayed()
    {
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
        if (IsGameOver)
        {
            EndGame();
            return;
        }

        CurrentWeek++;
        StartTurn();
    }

    private void PreloadEndGame(bool isVictory, DefeatReason reason)
    {
        IsGameOver = true;
        GameOverData.IsVictory = isVictory;
        GameOverData.Reason = reason;
    }

    private void EndGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }
}