using UnityEngine;

public class RiskUtilities
{
    private static readonly Color ColorRed = new(0.91f, 0.30f, 0.24f); // #E84C3D
    private static readonly Color ColorGreen = new(0.18f, 0.80f, 0.44f); // #2ECC70
    private static readonly Color ColorOrange = new(0.90f, 0.49f, 0.13f); // #E67E21

    public static Color GetRiskColor(RiskLevel riskLevel)
    {
        return riskLevel switch
        {
            RiskLevel.Low => ColorGreen,
            RiskLevel.Medium => ColorOrange,
            RiskLevel.High => ColorRed,
            _ => Color.white,
        };
    }

    public static string GetRiskLabel(RiskLevel riskLevel)
    {
        return riskLevel switch
        {
            RiskLevel.Low => "FAIBLE",
            RiskLevel.Medium => "MOYEN",
            RiskLevel.High => "ÉLEVÉ",
            _ => ""
        };
    }
}