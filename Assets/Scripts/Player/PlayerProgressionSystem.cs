using UnityEngine;
using System;

public class PlayerProgressionSystem : MonoBehaviour
{
    public static PlayerProgressionSystem Instance { get; private set; }

    // XP Settings
    private const int XP_PER_TURN = 50;
    private const int XP_BONUS_GOOD_DECISION = 25;

    // Level thresholds
    private static readonly int[] XP_THRESHOLDS = { 0, 3000, 7000, 12000 };

    public int CurrentXP { get; private set; }
    public int CurrentLevel { get; private set; }

    public event Action OnProgressionChanged;

    void Awake()
    {
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
        for (int i = XP_THRESHOLDS.Length - 1; i >= 0; i--)
        {
            if (CurrentXP >= XP_THRESHOLDS[i])
            {
                CurrentLevel = i + 1;
                break;
            }
        }
    }

    public int XPForNextLevel()
    {
        if (CurrentLevel >= XP_THRESHOLDS.Length) return -1; // max level
        return XP_THRESHOLDS[CurrentLevel] - CurrentXP;
    }

    public float XPProgress()
    {
        if (CurrentLevel >= XP_THRESHOLDS.Length) return 1f;
        int levelStart = XP_THRESHOLDS[CurrentLevel - 1];
        int levelEnd = XP_THRESHOLDS[CurrentLevel];
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
            2 => "Manager",
            3 => "Manager Senior",
            _ => "Directeur"
        };
    }
}