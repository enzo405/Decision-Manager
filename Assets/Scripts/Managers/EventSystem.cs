using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    // Events for current session
    public Dictionary<string, TurnEventRecord> Events { get; private set; } = new();

    public event Action<Event, int> OnEventTriggered;

    private Func<Event, int, string> BuildDictKey = (ev, week) => $"{ev.Name}_{week}";

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
        TurnEventRecord[] events = Events
            .Where(e => e.Value.IsActiv == true)
            .Select(e => e.Value)
            // TODO Distinct and sum the .Chance for identical events
            .ToArray();

        if (events.Length == 0)
        {
            OnEventTriggered?.Invoke(null, 0);
            return;
        }

        TurnEventRecord randomEvent = events[UnityEngine.Random.Range(0, events.Length)];

        bool isTriggered = UnityEngine.Random.value <= randomEvent.Event.Chance;

        if (isTriggered)
        {
            TriggerEvent(randomEvent);
            OnEventTriggered?.Invoke(randomEvent.Event, randomEvent.FromTurnDecision);
        }
        else
        {
            OnEventTriggered?.Invoke(null, 0);
        }
    }

    public void RegisterCardEvents(Card card)
    {
        int currentWeek = GameManager.Instance.CurrentWeek;

        foreach (TurnEventRecord ter in Events.Values.ToList())
        {
            string dictKey = BuildDictKey(ter.Event, ter.FromTurnDecision);
            Events[dictKey].IsActiv = GameHistoryManager.Instance.History
                .Where((tr, i) =>
                {
                    var weeksSincePlayed = currentWeek - (i + 1);
                    return weeksSincePlayed >= ter.Event.WeekRange.Min &&
                           weeksSincePlayed <= ter.Event.WeekRange.Max &&
                           tr.CardSlug == ter.Event.CardSlug;
                })
                .Any();
        }

        foreach (Event ev in card.Events)
        {
            var turnEventRecord = new TurnEventRecord(currentWeek, ev, ev.WeekRange.Min == 0);
            var key = BuildDictKey(ev, currentWeek);
            Events[key] = turnEventRecord;
        }
    }


    private static void TriggerEvent(TurnEventRecord randomEvent)
    {
        GameHistoryManager.Instance.RecordRandomEvent(randomEvent.Event, randomEvent.FromTurnDecision);

        int level = PlayerProgressionSystem.Instance.LevelThisGame;
        float negativeMultiplier = 1f + Mathf.Min(0.03f + (level * 0.02f), 0.15f); // 2% par niveau, max 15%

        StatSystem.Instance.ApplyEffects(
            Mathf.RoundToInt(randomEvent.Event.MotivationDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.Event.StressDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.Event.PerformanceDelta * negativeMultiplier),
            Mathf.RoundToInt(randomEvent.Event.TurnoverDelta * negativeMultiplier)
        );
    }
}