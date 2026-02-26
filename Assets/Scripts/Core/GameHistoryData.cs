using System.Collections.Generic;

public static class GameHistoryData
{
    public static List<TurnRecord> History { get; private set; } = new List<TurnRecord>();

    public static void Clear() => History.Clear();

    public static void Record(string cardName, bool wasSuccess,
        int motivDelta, int stressDelta, int perfDelta, int turnoverDelta,
        int motivation, int stress, int performance, int turnover)
    {
        int improved = 0;
        if (motivDelta > 0) improved++;
        if (stressDelta < 0) improved++; // stress qui baisse = amélioration
        if (perfDelta > 0) improved++;
        if (turnoverDelta < 0) improved++; // turnover qui baisse = amélioration

        History.Add(new TurnRecord
        {
            cardName = cardName,
            wasSuccess = wasSuccess,
            motivation = motivation,
            stress = stress,
            performance = performance,
            turnover = turnover,
            wasGoodDecision = improved >= 2
        });
    }
}