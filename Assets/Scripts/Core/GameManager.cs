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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        if (!IsGameOver)
        {
            CurrentWeek++;
            StartTurn();
        }
    }

    public void ResetGame()
    {
        CurrentWeek = 1;
        IsGameOver = false;
        GameHistoryData.Clear();
        StatSystem.Instance.Start();
        SceneManager.LoadScene("MainGame");
    }
    public static void EndGame()
    {
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