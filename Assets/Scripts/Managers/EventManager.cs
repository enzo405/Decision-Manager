using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // Events for current session
    public Dictionary<string, TurnEventRecord> Events { get; private set; } = new();

    private Func<Event, int, string> BuildDictKey = (ev, week) => $"{ev.Name}_{week}";

    public void Awake()
    {
        Debug.Log("[EventManager] Awake");
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
        GameManager.Instance.OnNewGameTriggered += Reset;
    }

    private void Reset()
    {
        Events.Clear();
    }

    public (TurnEventRecord, int) RollEvent()
    {
        List<TurnEventRecord> events = Events
            .Where(e => e.Value.IsActiv)
            .Select(e => e.Value)
            .ToList();

        if (events.Count == 0)
        {
            return (null, 0);
        }

        TurnEventRecord randomEvent = events[UnityEngine.Random.Range(0, events.Count)];

        bool isTriggered = UnityEngine.Random.value <= randomEvent.Event.Chance;

        if (isTriggered)
        {
            string dictKey = BuildDictKey(randomEvent.Event, randomEvent.FromTurnDecision);

            Events.Remove(dictKey); // Remove the event so it doesn't trigger again in the future
            return (randomEvent, GameManager.Instance.CurrentWeek);
        }
        else
        {
            return (null, 0);
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
}