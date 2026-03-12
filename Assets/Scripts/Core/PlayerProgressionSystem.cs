using UnityEngine;
using System;

public class PlayerProgressionSystem : MonoBehaviour
{
    public static PlayerProgressionSystem Instance { get; private set; }

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
        Debug.Log("[PlayerProgressionSystem] Awake");
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
        // Charger les paramètres de progression depuis les Thresholds
        Thresholds thresholds = ConfigApiService.Instance.Thresholds;
        BaseXp = thresholds.BaseXp;
        Exponent = thresholds.Exponent;
        XpPerTurn = thresholds.XpPerTurn;
        XpBonusGoodDecision = thresholds.XpBonusGoodDecision;
        MaxLevel = thresholds.MaxLevel;

        // Valeur initiale
        NewGame();
    }

    public void AddXP(bool wasGoodDecision)
    {
        int xpGained = XpPerTurn + (wasGoodDecision ? XpBonusGoodDecision : 0);
        CurrentXP += xpGained;
        XPEarnedThisGame += xpGained;
        CheckLevelUp();
        OnProgressionChanged?.Invoke();
    }

    public void NewGame()
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

    public void EndGame()
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
        return CurrentLevel switch
        {
            1 => "Manager Junior",
            2 or 3 => "Manager",
            4 or 5 => "Manager Confirmé",
            6 or 7 => "Manager Senior",
            8 or 9 or 10 => "Directeur",
            >= 11 => "Directeur Exécutif",
            _ => "Manager Junior"
        };
    }
}