using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class GameOverUI : MonoBehaviour
{
    [Header("Header")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI reasonText;
    public Button replayButton;
    public Button mainMenuButton;

    [Header("Decisions")]
    public Transform decisionsList;
    public GameObject decisionItemPrefab;

    [Header("Graph")]
    public RectTransform graphContainer;

    public void Start()
    {
        SetupHeader();
        SetupDecisions();
        SetupGraph();
        replayButton.onClick.AddListener(() => GameManager.Instance.NewGame());
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    public void SetupHeader()
    {
        bool isVictory = GameOverData.IsVictory;
        DefeatReason reason = GameOverData.Reason;

        titleText.text = isVictory ? "Victoire !" : "Défaite";
        reasonText.text = reason switch
        {
            DefeatReason.Burnout => "Ton équipe a atteint le burn-out. Le stress était trop élevé.",
            DefeatReason.MassiveDepartures => "Trop de départs. Le turnover a détruit l'équipe.",
            DefeatReason.PoorPerformance => "La performance est tombée trop bas. Objectifs non atteints.",
            _ => "Tu as maintenu l'équilibre sur 12 semaines. Bien joué !"
        };
    }

    public void SetupDecisions()
    {
        foreach (var record in GameHistoryManager.Instance.History)
        {
            GameObject item = Instantiate(decisionItemPrefab, decisionsList);
            TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();

            string success = record.WasSuccess ? "Succès" : "Échec";
            text.text = $"{success} — {record.CardDisplayName}";
            text.color = record.WasGoodDecision ? Color.darkGreen : Color.softRed;
        }
    }

    public void SetupGraph()
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

    public void DrawCurve(List<TurnRecord> history,
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

    public void DrawLine(Vector2 start, Vector2 end, Color color)
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