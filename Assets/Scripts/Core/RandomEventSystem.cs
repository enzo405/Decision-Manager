using UnityEngine;
using System;
using System.Linq;

public class RandomEventSystem : MonoBehaviour
{
    public static RandomEventSystem Instance { get; private set; }

    public event Action<Event> OnEventTriggered;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RollEvent()
    {
        Event[] events = GameHistoryManager.Instance.Events
            .Where(e => e.Value == true)
            .Select(e => e.Key)
            .ToArray();

        if (events.Length == 0)
        {
            OnEventTriggered?.Invoke(null);
            return;
        }

        Event randomEvent = events[UnityEngine.Random.Range(0, events.Length)];

        bool isTriggered = UnityEngine.Random.value <= randomEvent.Chance;

        if (isTriggered)
        {
            TriggerEvent(randomEvent);
        }

        OnEventTriggered?.Invoke(isTriggered ? randomEvent : null);
    }

    private static void TriggerEvent(Event randomEvent)
    {
        int level = PlayerProgressionSystem.Instance.LevelThisGame;
        float negativeMultiplier = 1f + Mathf.Min(0.03f + (level * 0.02f), 0.20f); // 2% par niveau, max 20%

        StatSystem.Instance.ApplyEffects(
            Mathf.RoundToInt(randomEvent.MotivationDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.StressDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.PerformanceDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.TurnoverDelta * negativeMultiplier)
        );
    }
}