using System;
using UnityEngine;

public class ColorUtilities
{
    public static Color Blue => new(0.29f, 0.56f, 0.85f);
    public static Color Red => new(0.91f, 0.30f, 0.24f);
    public static Color Green => new(0.18f, 0.80f, 0.44f);
    public static Color Orange => new(0.90f, 0.49f, 0.13f);
    public static Color SoftRed => new(0.91f, 0.30f, 0.24f, 0.059f);
    public static Color SoftGreen => new(0.098f, 0.529f, 0.278f, 0.059f);
    public static Color SoftOrange => new(0.90f, 0.49f, 0.13f, 0.059f);

    public static Color SuccessColor => Green;
    public static Color FailColor => Red;

    public static Color GetRiskColorText(RiskLevel riskLevel)
    {
        return riskLevel switch
        {
            RiskLevel.Low => Green,
            RiskLevel.Medium => Orange,
            RiskLevel.High => Red,
            _ => Color.white,
        };
    }

    public static Color GetRiskColorBackground(RiskLevel riskLevel)
    {
        return riskLevel switch
        {
            RiskLevel.Low => SoftGreen,
            RiskLevel.Medium => SoftOrange,
            RiskLevel.High => SoftRed,
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