using UnityEngine;
using System;

public class RandomEventSystem : MonoBehaviour
{
    public static RandomEventSystem Instance { get; private set; }

    public event Action<RandomEvent> OnEventTriggered;

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

    public void RollForEvent()
    {
        int level = PlayerProgressionSystem.Instance.CurrentLevel;
        float eventChance = Mathf.Min(0.05f + (level * 0.02f), 0.30f); // 5% + 2% par niveau, max 30%

        RandomEvent randomEvent = null;
        if (UnityEngine.Random.value <= eventChance)
        {
            randomEvent = TriggerRandomEvent(level);
        }

        OnEventTriggered?.Invoke(randomEvent);
    }

    private RandomEvent TriggerRandomEvent(int level)
    {
        var events = GetEventsForLevel(level);
        var randomEvent = events[UnityEngine.Random.Range(0, events.Length)];

        StatSystem.Instance.ApplyEffects(
            randomEvent.MotivationDelta,
            randomEvent.StressDelta,
            randomEvent.PerformanceDelta,
            randomEvent.TurnoverDelta
        );

        return randomEvent;
    }

    private RandomEvent[] GetEventsForLevel(int level)
    {
        if (level <= 3) return lowLevelEvents;
        if (level <= 6) return midLevelEvents;
        return highLevelEvents;
    }

    private static readonly RandomEvent[] lowLevelEvents = {
        new("Un membre clé est absent cette semaine.", 0, 5, -5, 2),
        new("Conflit mineur entre deux collègues.", -3, 6, 0, 1),
        new("Retard inattendu sur un livrable.", 0, 4, -6, 0),
    };

    private static readonly RandomEvent[] midLevelEvents = {
        new("Départ surprise d'un talent clé.", -8, 8, -5, 8),
        new("Crise client impactant toute l'équipe.", -4, 10, -8, 3),
        new("Vague de maladies dans l'équipe.", -3, 5, -10, 2),
    };

    private static readonly RandomEvent[] highLevelEvents = {
        new("Restructuration imposée par la direction.", -10, 15, -5, 12),
        new("Fuite d'informations confidentielles.", -8, 12, -8, 10),
        new("Échec d'un projet stratégique majeur.", -12, 14, -15, 8),
    };
}