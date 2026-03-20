using UnityEngine;
using System;
using UnityEngine.Localization.Settings;

public class PlayerProgressionManager : MonoBehaviour
{
    public static PlayerProgressionManager Instance { get; private set; }

    // XP Settings
    private int XpPerTurn;
    private int XpBonusGoodDecision;
    private int MaxLevel;
    private int BaseXp;
    private float Exponent;

    public int CurrentXP { get; private set; }
    public int CurrentLevel { get; private set; }
    public int XPEarnedThisGame { get; private set; }
    public int LevelThisGame { get; private set; }

    public event Action OnProgressionChanged;

    public void Awake()
    {
        Debug.Log("[PlayerProgressionManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Charger les paramètres de progression depuis les Thresholds
        Thresholds thresholds = ConfigApiService.Instance.Thresholds;
        BaseXp = thresholds.BaseXp;
        Exponent = thresholds.Exponent;
        XpPerTurn = thresholds.XpPerTurn;
        XpBonusGoodDecision = thresholds.XpBonusGoodDecision;
        MaxLevel = thresholds.MaxLevel;
    }

    public void Start()
    {
        GameManager.Instance.OnEndGameTriggered += EndGame;
        GameManager.Instance.OnNewGameTriggered += InitStats;
        GameManager.Instance.OnGameAbandonedTriggered += AbandonCurrentGameProgression;

        // Initial load of player progression
        InitStats();
    }

    public void AddXP(bool wasGoodDecision)
    {
        int xpGained = XpPerTurn + (wasGoodDecision ? XpBonusGoodDecision : 0);
        CurrentXP += xpGained;
        XPEarnedThisGame += xpGained;
        CheckLevelUp();
        OnProgressionChanged?.Invoke();
    }

    private void InitStats()
    {
        CurrentXP = PlayerPrefs.GetInt("PlayerXP", 0);
        CheckLevelUp();
        XPEarnedThisGame = 0;
        LevelThisGame = CurrentLevel;
    }

    public void AbandonCurrentGameProgression()
    {
        CurrentXP -= XPEarnedThisGame;
        if (CurrentXP < 0) CurrentXP = 0;
        CheckLevelUp();
        OnProgressionChanged?.Invoke();
    }

    public float XPProgress()
    {
        if (CurrentLevel >= MaxLevel) return 1f;
        int levelStart = XPThreshold(CurrentLevel);
        int levelEnd = XPThreshold(CurrentLevel + 1);
        return (float)(CurrentXP - levelStart) / (levelEnd - levelStart);
    }

    private void EndGame()
    {
        // Backup call: Making sure we don't save a wrong Level
        CheckLevelUp();

        StartCoroutine(PlayerApiService.Instance.UpdateProgression());
        PlayerPrefs.SetInt("PlayerXP", CurrentXP);
        PlayerPrefs.Save();
    }


    private void CheckLevelUp()
    {
        for (int i = MaxLevel; i >= 1; i--)
        {
            if (CurrentXP >= XPThreshold(i))
            {
                CurrentLevel = i;
                break;
            }
        }
    }

    private int XPThreshold(int level)
    {
        if (level <= 1) return 0;
        return Mathf.RoundToInt(BaseXp * Mathf.Pow(level - 1, Exponent));
    }


    public string LevelTitle()
    {
        string key = CurrentLevel switch
        {
            1 => "progression.title.1",
            2 or 3 => "progression.title.2",
            4 or 5 => "progression.title.4",
            6 or 7 => "progression.title.6",
            8 or 9 or 10 => "progression.title.8",
            >= 11 => "progression.title.default",
            _ => "progression.title.1"
        };

        return LocalizationSettings.StringDatabase.GetLocalizedString("UI_Progression", key);
    }
}