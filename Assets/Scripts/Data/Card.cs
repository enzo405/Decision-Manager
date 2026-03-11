

using System.Collections.Generic;

public class Card
{
    public string Slug { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public int RequiredLevel { get; set; }
    public float SuccessProbability { get; set; }
    public int MotivationEffect { get; set; }
    public int StressEffect { get; set; }
    public int PerformanceEffect { get; set; }
    public int TurnoverEffect { get; set; }
    public int MotivationEffectOnFailure { get; set; }
    public int StressEffectOnFailure { get; set; }
    public int PerformanceEffectOnFailure { get; set; }
    public int TurnoverEffectOnFailure { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string SuccessMessage { get; set; }
    public string FailureMessage { get; set; }
    public List<Event> Events { get; set; }
}
