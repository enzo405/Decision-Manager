using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization.Settings;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button collectionButton;
    public Button howToPlayButton;
    public Button switchLanguageButton;
    public TextMeshProUGUI switchLanguageText;
    public Button quitButton;

    [Header("How To Play")]
    public GameObject howToPlayPanel;
    public Button closeButton;
    public TextMeshProUGUI howToPlayText;

    public void Start()
    {
        playButton.onClick.AddListener(() => GameManager.Instance.NewGame());
        collectionButton.onClick.AddListener(() => SceneManager.LoadScene("Collection"));
        howToPlayButton.onClick.AddListener(() => howToPlayPanel.SetActive(true));
        closeButton.onClick.AddListener(() => howToPlayPanel.SetActive(false));
        quitButton.onClick.AddListener(() => Application.Quit());

        UpdateLanguageButton();
        switchLanguageButton.onClick.AddListener(ToggleLanguage);

        RefreshUI();

        howToPlayPanel.SetActive(false);
    }

    private void ToggleLanguage()
    {
        string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        string newCode = currentCode == "fr" ? "en" : "fr";

        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(newCode);

        PlayerPrefs.SetString("selected_language", newCode);
        PlayerPrefs.Save();

        RefreshUI();

        UpdateLanguageButton();
    }

    private void RefreshUI()
    {
        howToPlayText.text = LocalizationSettings.StringDatabase
            .GetLocalizedString("UI_HowToPlay", "howtoplay.body");
    }

    private void UpdateLanguageButton()
    {
        string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        switchLanguageText.text = currentCode == "fr" ? "EN" : "FR";
    }
}