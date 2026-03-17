using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameOverUI : MonoBehaviour
{
    [Header("Header")]
    public Image titleIcon;
    public TextMeshProUGUI titleText;
    public Image titleIcon2;
    public TextMeshProUGUI reasonText;
    public Button replayButton;
    public Button mainMenuButton;

    [Header("Decisions")]
    public Transform decisionsList;
    public GameObject historyItemPrefab;
    public GameObject historyEventItemPrefab;

    [Header("Graph")]
    public RectTransform graphContainer;

    public void Start()
    {
        SetupHeader();
        SetupGraph();
        SetupDecisions();
        replayButton.onClick.AddListener(() => GameManager.Instance.NewGame());
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    private void SetupHeader()
    {
        bool isVictory = GameOverData.IsVictory;
        DefeatReason reason = GameOverData.Reason;

        titleText.text = isVictory ? "Victoire" : "Défaite";
        titleText.color = isVictory ? ColorUtilities.Green : ColorUtilities.Red;
        titleIcon.color = titleText.color;
        titleIcon2.color = titleText.color;
        reasonText.text = reason switch
        {
            DefeatReason.Burnout => "Ton équipe a atteint le burn-out. Le stress était trop élevé.",
            DefeatReason.MassiveDepartures => "Trop de départs. Le turnover a détruit l'équipe.",
            DefeatReason.PoorPerformance => "La performance est tombée trop bas. Objectifs non atteints.",
            _ => "Tu as maintenu l'équilibre sur 12 semaines. Bien joué !"
        };
    }

    private void SetupDecisions()
    {
        for (int i = 0; i < GameHistoryManager.Instance.History.Count; i++)
        {
            var record = GameHistoryManager.Instance.History[i];
            int turn = i + 1;

            // Decision item
            AddDecisionItem(turn, record.CardDisplayName, record.WasGoodDecision);

            // Random event linked to this turn
            if (GameHistoryManager.Instance.HistoryRandomEvents.TryGetValue(turn, out TurnEventRecord randomEvent))
            {
                AddDecisionItem(turn, randomEvent.Event.Name, randomEvent.FromTurnDecision, record.CardDisplayName);
            }
        }
    }

    private void AddDecisionItem(int turn, string text, bool wasGood)
    {
        GameObject item = Instantiate(historyItemPrefab, decisionsList);
        TextMeshProUGUI turnText = item.transform.Find("TurnNumber").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI msgText = item.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Image imageBg = item.transform.Find("BorderImage").GetComponent<Image>();

        turnText.text = turn.ToString();
        imageBg.color = wasGood ? ColorUtilities.Green : ColorUtilities.Red;
        msgText.text = text;
        msgText.color = wasGood ? ColorUtilities.Green : ColorUtilities.Red;
    }

    private void AddDecisionItem(int turn, string eventName, int fromTurn, string cardName)
    {
        Debug.Log($"Adding event item for event '{eventName}' linked to card '{cardName}' from turn {fromTurn} at turn {turn}");
        GameObject item = Instantiate(historyEventItemPrefab, decisionsList);
        TextMeshProUGUI turnText = item.transform.Find("TurnNumber").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI msgText = item.transform.Find("LabelGroup/Text").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI subTitleText = item.transform.Find("LabelGroup/SubTitleText").GetComponent<TextMeshProUGUI>();

        turnText.text = turn.ToString();
        msgText.text = $"Événement : {eventName}";
        subTitleText.text = $"Déclenché par : {cardName} (tour {fromTurn})";
        subTitleText.fontStyle = FontStyles.Italic;
    }

    private void SetupGraph()
    {
        Canvas.ForceUpdateCanvases();

        var history = GameHistoryManager.Instance.History;
        if (history.Count < 2) return;

        float width = graphContainer.rect.width;
        float height = graphContainer.rect.height;

        DrawCurve(history, r => r.Motivation, StatSystem.GetMaxMotivation, StatSystem.GetMinMotivation, new Color(0.29f, 0.56f, 0.85f), width, height);
        DrawCurve(history, r => r.Stress, StatSystem.GetMaxStress, StatSystem.GetMinStress, new Color(0.91f, 0.30f, 0.24f), width, height);
        DrawCurve(history, r => r.Performance, StatSystem.GetMaxPerformance, StatSystem.GetMinPerformance, new Color(0.18f, 0.80f, 0.44f), width, height);
        DrawCurve(history, r => r.Turnover, StatSystem.GetMaxTurnover, StatSystem.GetMinTurnover, new Color(0.90f, 0.49f, 0.13f), width, height);
    }

    private void DrawCurve(List<TurnRecord> history,
        Func<TurnRecord, int> getValue,
        float max, float min,
        Color color, float width, float height)
    {
        for (int i = 0; i < history.Count - 1; i++)
        {
            float x1 = (i / (float)(history.Count - 1)) * width;
            float y1 = Mathf.Clamp01((getValue(history[i]) - min) / (max - min)) * height;
            float x2 = ((i + 1) / (float)(history.Count - 1)) * width;
            float y2 = Mathf.Clamp01((getValue(history[i + 1]) - min) / (max - min)) * height;

            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        GameObject line = new("Line", typeof(Image));
        line.transform.SetParent(graphContainer, false);

        RectTransform rt = line.GetComponent<RectTransform>();
        Image img = line.GetComponent<Image>();
        img.color = color;

        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rt.sizeDelta = new Vector2(distance, 3f);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 0);
        rt.anchoredPosition = start;
        rt.localEulerAngles = new Vector3(0, 0,
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
}