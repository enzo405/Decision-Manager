using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseOverlay;
    public Button pauseButton;
    public Button resumeButton;
    public Button mainMenuButton;

    void Start()
    {
        pauseButton.onClick.AddListener(() => pauseOverlay.SetActive(true));
        resumeButton.onClick.AddListener(() => pauseOverlay.SetActive(false));
        mainMenuButton.onClick.AddListener(() =>
        {
            pauseOverlay.SetActive(false);
            GameManager.Instance.ResetGameStats();
            SceneManager.LoadScene("MainMenu");
        });

        pauseOverlay.SetActive(false);
    }
}