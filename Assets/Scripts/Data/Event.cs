public class Event
{
    public string EventName { get; }

    /// <summary>
    /// Range de tour entre lequel l'évenement pourra se déclencher
    /// Le premier élément du tuple représente le tour minimum après que la carte soit joué, et le second élément représente le tour maximum après que la carte soit joué.
    /// </summary>
    public (int Min, int Max) WeekRange { get; }

    /// <summary>
    /// Message affiché lorsque l'évenement est déclenché
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Chance que cet évenement se déclenche
    /// </summary>
    public float Chance { get; }


    public int CardId { get; }
    public string CardName { get; }

    #region Effect on Stats

    public int MotivationDelta { get; }
    public int StressDelta { get; }
    public int PerformanceDelta { get; }
    public int TurnoverDelta { get; }

    #endregion


    public Event(string eventName, (int Min, int Max) weekRange, string message, float chance, int motivationDelta, int stressDelta, int performanceDelta, int turnoverDelta, int cardId, string cardName)
    {
        EventName = eventName;
        WeekRange = weekRange;
        Message = message;
        Chance = chance;
        MotivationDelta = motivationDelta;
        StressDelta = stressDelta;
        PerformanceDelta = performanceDelta;
        TurnoverDelta = turnoverDelta;
        CardId = cardId;
        CardName = cardName;
    }
}