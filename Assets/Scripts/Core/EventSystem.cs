using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    // Events for current session
    public Dictionary<Event, bool> Events { get; private set; } = new();

    public event Action<Event> OnEventTriggered;

    public void Awake()
    {
        Debug.Log("[EventSystem] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Reset()
    {
        Events.Clear();
    }

    public void RollEvent()
    {
        Event[] events = Events
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

    public void RegisterCardEvents(Card card)
    {
        int currentWeek = GameManager.Instance.CurrentWeek;

        foreach (var key in Events.Keys.ToList())
        {
            Events[key] = GameHistoryManager.Instance.History
                .Where((tr, i) =>
                {
                    var weeksSincePlayed = currentWeek - (i + 1);
                    return weeksSincePlayed >= key.WeekRange.Min &&
                           weeksSincePlayed <= key.WeekRange.Max &&
                           tr.CardSlug == key.CardSlug;
                })
                .Any();
        }

        foreach (Event ev in card.Events)
        {
            Events[ev] = ev.WeekRange.Min == 0;
        }
    }


    private static void TriggerEvent(Event randomEvent)
    {
        int level = PlayerProgressionSystem.Instance.LevelThisGame;
        float negativeMultiplier = 1f + Mathf.Min(0.03f + (level * 0.02f), 0.15f); // 2% par niveau, max 15%

        StatSystem.Instance.ApplyEffects(
            Mathf.RoundToInt(randomEvent.MotivationDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.StressDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.PerformanceDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.TurnoverDelta * negativeMultiplier)
        );
    }
}