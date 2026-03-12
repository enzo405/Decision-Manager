using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Slider progressBar;

    public void SetStatus(string message, float progress)
    {
        statusText.text = message;
        progressBar.value = progress;
    }
}