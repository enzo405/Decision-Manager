using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private RectTransform progressFill;
    [SerializeField] private TextMeshProUGUI progressLabel;

    public void SetProgress(string message, float progress)
    {
        Vector2 max = progressFill.anchorMax;
        max.x = progress;
        progressFill.anchorMax = max;
        progressLabel.text = $"{message}  {Mathf.RoundToInt(progress * 100)}%";
    }
}