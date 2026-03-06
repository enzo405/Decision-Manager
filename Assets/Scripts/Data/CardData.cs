using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "DecisionManager/Card")]
public class CardData : ScriptableObject
{
    [Header("Identity")]
    public string cardName;
    [TextArea] public string description;

    [Header("Requirements")]
    public int requiredLevel = 1;

    [Header("Success Chance")]
    [Range(0f, 1f)] public float successProbability = 0.75f;

    [Header("Effects on Success")]
    public int motivationEffect;
    public int stressEffect;
    public int performanceEffect;
    public int turnoverEffect;

    [Header("Effects on Failure")]
    public int motivationEffectOnFailure;
    public int stressEffectOnFailure;
    public int performanceEffectOnFailure;
    public int turnoverEffectOnFailure;

    [Header("Risk Level")]
    public RiskLevel riskLevel;

    [Header("Feedback Messages")]
    [TextArea] public string successMessage;
    [TextArea] public string failureMessage;

    [Header("Events Associated with this Card")]
    public Event[] Events;
}

public enum RiskLevel { Low, Medium, High }