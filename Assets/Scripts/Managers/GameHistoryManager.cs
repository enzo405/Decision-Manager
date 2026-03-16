using System.Collections.Generic;
using UnityEngine;

public class GameHistoryManager : MonoBehaviour
{
    public static GameHistoryManager Instance { get; private set; }

    public List<TurnRecord> History { get; private set; } = new();
    public Dictionary<int, Event> HistoryRandomEvents { get; private set; } = new();

    public void Awake()
    {
        Debug.Log("[GameHistoryManager] Awake");
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
        GameManager.Instance.OnCardPlayedTriggered += RecordTurn;
        EventSystem.Instance.OnEventTriggered += RecordRandomEvent;
        GameManager.Instance.OnNewGameTriggered += Reset;
    }


    private void Reset()
    {
        History.Clear();
        HistoryRandomEvents.Clear();
    }

    private void RecordRandomEvent(Event randomEvent, int fromTurnDecision)
    {
        if (randomEvent == null) return;
        HistoryRandomEvents.Add(fromTurnDecision, randomEvent);
    }

    private void RecordTurn(Card card, bool wasSuccess,
        int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        int improved = 0;
        if (motivDelta > 0) improved++;
        if (stressDelta < 0) improved++;
        if (perfDelta > 0) improved++;
        if (turnoverDelta < 0) improved++;

        var wasGoodDecision = improved >= 2;
        History.Add(new TurnRecord
        {
            CardSlug = card.Slug,
            CardDisplayName = card.DisplayName,
            WasSuccess = wasSuccess,
            Motivation = StatSystem.Instance.Motivation,
            Stress = StatSystem.Instance.Stress,
            Performance = StatSystem.Instance.Performance,
            Turnover = StatSystem.Instance.Turnover,
            WasGoodDecision = wasGoodDecision
        });

        PlayerProgressionSystem.Instance.AddXP(wasGoodDecision);
        EventSystem.Instance.RegisterCardEvents(card);
    }
}