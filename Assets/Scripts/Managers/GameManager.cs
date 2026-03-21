using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int totalWeeks = 12;

    public int CurrentWeek { get; private set; } = 1;
    public bool IsGameOver { get; private set; } = false;

    public event Action OnTurnStarted;
    public event Action<Card, bool, int, int, int, int, TurnEventRecord, int> OnTurnResolved;
    public event Action<Card, bool, int, int, int, int> OnCardPlayedTriggered;
    public event Action OnNewGameTriggered;
    public event Action OnEndGameTriggered;
    public event Action OnGameAbandonedTriggered;
    public event Action<string> OnChangeLanguageTriggered;

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

    public void ToggleLanguage()
    {
        string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        string newCode = currentCode == "fr" ? "en" : "fr";

        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(newCode);

        PlayerPrefs.SetString("selected_language", newCode);
        PlayerPrefs.Save();

        OnChangeLanguageTriggered?.Invoke(newCode);
    }


    private void OnCardPlayed(Card card, bool wasSuccess, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        OnCardPlayedTriggered?.Invoke(card, wasSuccess,
            motivDelta, stressDelta, perfDelta, turnoverDelta);

        var (randomEvent, turn) = EventManager.Instance.RollEvent();

        var defeat = StatManager.Instance.CheckDefeatConditions();
        if (defeat != DefeatReason.None)
        {
            PreloadEndGame(false, defeat);
        }
        else if (CurrentWeek >= totalWeeks)
        {
            PreloadEndGame(true, DefeatReason.None);
        }

        OnTurnResolved?.Invoke(
            card,
            wasSuccess,
            motivDelta,
            stressDelta,
            perfDelta,
            turnoverDelta,
            randomEvent,
            turn
        );
    }

    private void PreloadEndGame(bool isVictory, DefeatReason reason)
    {
        Debug.Log("Preloading end game. Victory: " + isVictory + ", Reason: " + reason);
        IsGameOver = true;
        GameOverData.IsVictory = isVictory;
        GameOverData.Reason = reason;
    }
}