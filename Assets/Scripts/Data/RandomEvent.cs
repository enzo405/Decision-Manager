public class RandomEvent
{
    public string Message { get; }
    public int MotivationDelta { get; }
    public int StressDelta { get; }
    public int PerformanceDelta { get; }
    public int TurnoverDelta { get; }

    public RandomEvent(string message, int motivDelta, int stressDelta, int perfDelta, int turnoverDelta)
    {
        Message = message;
        MotivationDelta = motivDelta;
        StressDelta = stressDelta;
        PerformanceDelta = perfDelta;
        TurnoverDelta = turnoverDelta;
    }
}