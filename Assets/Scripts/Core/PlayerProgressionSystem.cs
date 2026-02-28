using UnityEngine;
using System;

public class PlayerProgressionSystem : MonoBehaviour
{
    public static PlayerProgressionSystem Instance { get; private set; }

    // XP Settings
    private const int XP_PER_TURN = 50;
    private const int XP_BONUS_GOOD_DECISION = 25;
    private const int MAX_LEVEL = 20;
    private const int BASE_XP = 1000;
    private const float EXPONENT = 1.5f;
    private int XPThreshold(int level)
    {
        if (level <= 1) return 0;
        return Mathf.RoundToInt(BASE_XP * Mathf.Pow(level - 1, EXPONENT));
    }

    public int CurrentXP { get; private set; }
    public int CurrentLevel { get; private set; }

    public event Action OnProgressionChanged;

    public void Awake()
    {
        Debug.Log("PlayerProgressionSystem Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void AddXP(bool wasGoodDecision)
    {
        int xpGained = XP_PER_TURN + (wasGoodDecision ? XP_BONUS_GOOD_DECISION : 0);
        CurrentXP += xpGained;
        CheckLevelUp();
        Save();
        OnProgressionChanged?.Invoke();
    }

    private void CheckLevelUp()
    {
        for (int i = MAX_LEVEL; i >= 1; i--)
        {
            if (CurrentXP >= XPThreshold(i))
            {
                CurrentLevel = i;
                break;
            }
        }
    }

    public int XPForNextLevel()
    {
        if (CurrentLevel >= MAX_LEVEL) return -1;
        return XPThreshold(CurrentLevel + 1) - CurrentXP;
    }

    public float XPProgress()
    {
        if (CurrentLevel >= MAX_LEVEL) return 1f;
        int levelStart = XPThreshold(CurrentLevel);
        int levelEnd = XPThreshold(CurrentLevel + 1);
        return (float)(CurrentXP - levelStart) / (levelEnd - levelStart);
    }

    private void Save()
    {
        PlayerPrefs.SetInt("PlayerXP", CurrentXP);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        CurrentXP = PlayerPrefs.GetInt("PlayerXP", 0);
        CheckLevelUp();
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