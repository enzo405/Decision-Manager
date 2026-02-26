using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI reasonText;
    public TextMeshProUGUI statsText;
    public Button replayButton;

    public void Start()
    {
        // Récupère les données passées depuis GameManager
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

        replayButton.onClick.AddListener(() =>
        {
            StatSystem.Instance.Start();
            SceneManager.LoadScene("MainGame");
        }
    );
    }
}