using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHistoryManager : MonoBehaviour
{
    public static GameHistoryManager Instance { get; private set; }

    public List<TurnRecord> History { get; private set; } = new();

    // Events for current session
    public Dictionary<Event, bool> Events { get; private set; } = new();

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

    public void Reset()
    {
        History.Clear();
        Events.Clear();
    }


    public void RecordTurn(string cardName, bool wasSuccess,
        int motivDelta, int stressDelta, int perfDelta, int turnoverDelta,
        int motivation, int stress, int performance, int turnover)
    {
        int improved = 0;
        if (motivDelta > 0) improved++;
        if (stressDelta < 0) improved++;
        if (perfDelta > 0) improved++;
        if (turnoverDelta < 0) improved++;

        var wasGoodDecision = improved >= 2;
        History.Add(new TurnRecord
        {
            cardName = cardName,
            wasSuccess = wasSuccess,
            motivation = motivation,
            stress = stress,
            performance = performance,
            turnover = turnover,
            wasGoodDecision = wasGoodDecision
        });

        PlayerProgressionSystem.Instance.AddXP(wasGoodDecision);
    }

    public void RegisterCardEvents(CardData card)
    {
        int currentWeek = GameManager.Instance.CurrentWeek;

        foreach (var key in Events.Keys.ToList())
        {
            Events[key] = History
                .Where((tr, i) =>
                {
                    var weeksSincePlayed = currentWeek - (i + 1);
                    return weeksSincePlayed >= key.WeekRange.Min &&
                           weeksSincePlayed <= key.WeekRange.Max &&
                           tr.cardName == key.CardName;
                })
                .Any();
        }

        foreach (Event ev in card.Events)
        {
            Events[ev] = ev.WeekRange.Min == 0;
        }
    }
}