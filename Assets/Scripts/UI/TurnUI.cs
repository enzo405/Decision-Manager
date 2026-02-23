using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    void Start()
    {
        GameManager.Instance.OnTurnStarted += RefreshUI;
        RefreshUI();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTurnStarted -= RefreshUI;
    }

    void RefreshUI()
    {
        turnText.text = $"Semaine {GameManager.Instance.CurrentWeek} / {GameManager.Instance.totalWeeks}";
    }
}