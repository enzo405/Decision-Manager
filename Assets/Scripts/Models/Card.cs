

using System;
using System.Collections.Generic;

public class Card
{
    public string Slug { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public CardType Type { get; set; }
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
    public List<CardRiskStatThreshold> StatThresholdsRisk { get; set; }
    public List<CardUnlockStatThreshold> StatThresholdsUnlock { get; set; }
    public List<string> RequiredCardSlugs { get; set; }


    public override bool Equals(object obj)
    {
        if (obj is Card other)
            return Slug == other.Slug;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Slug);
    }
}
