using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button howToPlayButton;
    public Button quitButton;

    [Header("How To Play")]
    public GameObject howToPlayPanel;
    public Button closeButton;
    public TextMeshProUGUI howToPlayText;

    void Start()
    {
        playButton.onClick.AddListener(() => SceneManager.LoadScene("MainGame"));
        howToPlayButton.onClick.AddListener(() => howToPlayPanel.SetActive(true));
        closeButton.onClick.AddListener(() => howToPlayPanel.SetActive(false));
        quitButton.onClick.AddListener(() => Application.Quit());

        howToPlayText.text =
            "<b>🎯 Objectif</b>\n" +
            "Maintenir l'équilibre de ton équipe sur 12 semaines.\n\n" +
            "<b>📊 Les 4 stats</b>\n" +
            "• Motivation — l'engagement de l'équipe\n" +
            "• Stress — la pression ressentie\n" +
            "• Performance — la productivité\n" +
            "• Turnover — le risque de départs\n\n" +
            "<b>🃏 Les cartes</b>\n" +
            "Chaque tour, choisis une carte parmi 3.\n" +
            "Chaque carte a une probabilité de succès.\n" +
            "Le hasard simule l'imprévisibilité du monde professionnel.\n\n" +
            "<b>🏆 Victoire</b>\n" +
            "Survivre 12 semaines sans burn-out ni départs massifs.\n\n" +
            "<b>💀 Défaite</b>\n" +
            "Stress trop élevé, trop de départs, ou performance effondrée.";

        howToPlayPanel.SetActive(false);
    }
}