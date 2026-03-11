using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameHistoryManager : MonoBehaviour
{
    public static GameHistoryManager Instance { get; private set; }

    public List<TurnRecord> History { get; private set; } = new();

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
    }


    public void RecordTurn(Card card, bool wasSuccess,
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
            CardSlug = card.Slug,
            CardDisplayName = card.DisplayName,
            WasSuccess = wasSuccess,
            Motivation = motivation,
            Stress = stress,
            Performance = performance,
            Turnover = turnover,
            WasGoodDecision = wasGoodDecision
        });

        PlayerProgressionSystem.Instance.AddXP(wasGoodDecision);
        EventSystem.Instance.RegisterCardEvents(card);
    }
}