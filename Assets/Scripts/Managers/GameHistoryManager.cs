using System.Collections.Generic;
using UnityEngine;

public class GameHistoryManager : MonoBehaviour
{
    public static GameHistoryManager Instance { get; private set; }

    public List<TurnRecord> History { get; private set; } = new();
    public Dictionary<int, TurnEventRecord> HistoryRandomEvents { get; private set; } = new();
    public Dictionary<int, CardCombo> HistoryCombo { get; private set; } = new();

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
        GameManager.Instance.OnEventTriggered += RecordRandomEvent;
        GameManager.Instance.OnCardComboTriggered += RecordCardCombo;
        GameManager.Instance.OnNewGameTriggered += Reset;
    }


    private void Reset()
    {
        History.Clear();
        HistoryRandomEvents.Clear();
    }

    private void RecordRandomEvent(TurnEventRecord randomEvent, int turn)
    {
        if (randomEvent == null) return;
        HistoryRandomEvents.Add(turn, randomEvent);
    }

    private void RecordCardCombo(CardCombo combo, int turn)
    {
        if (combo == null) return;
        HistoryCombo.Add(turn, combo);
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
            Motivation = StatManager.Instance.Motivation,
            Stress = StatManager.Instance.Stress,
            Performance = StatManager.Instance.Performance,
            Turnover = StatManager.Instance.Turnover,
            WasGoodDecision = wasGoodDecision
        });

        PlayerProgressionManager.Instance.AddXP(wasGoodDecision);
        EventManager.Instance.RegisterCardEvents(card);
    }
}