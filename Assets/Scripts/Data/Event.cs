public class Event
{
    public string Name { get; }
    public string CardSlug { get; }

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


    #region Effect on Stats

    public int MotivationDelta { get; }
    public int StressDelta { get; }
    public int PerformanceDelta { get; }
    public int TurnoverDelta { get; }

    #endregion
}
