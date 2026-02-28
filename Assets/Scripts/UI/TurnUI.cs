using UnityEngine;
using TMPro;

public class TurnUI : MonoBehaviour
{
    public TextMeshProUGUI turnText;

    public void Start()
    {
        GameManager.Instance.OnTurnStarted += RefreshUI;
        RefreshUI();
    }

    public void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTurnStarted -= RefreshUI;
    }

    public void RefreshUI()
    {
        turnText.text = $"Semaine {GameManager.Instance.CurrentWeek} / {GameManager.Instance.totalWeeks}";
    }
}