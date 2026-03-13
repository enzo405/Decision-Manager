public class Event
{
    public string Name { get; set; }
    public string CardSlug { get; set; }

    /// <summary>
    /// Range de tour entre lequel l'évenement pourra se déclencher
    /// Le premier élément du tuple représente le tour minimum après que la carte soit joué, et le second élément représente le tour maximum après que la carte soit joué.
    /// </summary>
    public WeekRangeDto WeekRange { get; set; }

    /// <summary>
    /// Message affiché lorsque l'évenement est déclenché
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Chance que cet évenement se déclenche
    /// </summary>
    public float Chance { get; set; }


    #region Effect on Stats

    public int MotivationDelta { get; set; }
    public int StressDelta { get; set; }
    public int PerformanceDelta { get; set; }
    public int TurnoverDelta { get; set; }

    #endregion
}


public class WeekRangeDto
{
    public int Min { get; set; }
    public int Max { get; set; }
}