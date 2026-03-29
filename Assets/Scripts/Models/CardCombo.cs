using System.Collections.Generic;

public class CardCombo
{
    public string Name { get; set; }
    public string Message { get; set; }
    public int MotivationDelta { get; set; }
    public int StressDelta { get; set; }
    public int PerformanceDelta { get; set; }
    public int TurnoverDelta { get; set; }
    public List<string> TriggerSlugs { get; set; }
}
