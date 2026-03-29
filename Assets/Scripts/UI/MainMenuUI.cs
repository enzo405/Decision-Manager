using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization.Settings;
using System.Collections;

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
        switchLanguageButton.onClick.AddListener(() => ToggleLanguage());

        GameManager.Instance.OnChangeLanguageTriggered += OnLanguageChanged;

        UpdateLanguageButton();

        RefreshUI();

        howToPlayPanel.SetActive(false);
    }

    public void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnChangeLanguageTriggered -= OnLanguageChanged;
    }

    private void OnLanguageChanged(string newCode)
    {
        UpdateLanguageButton();
        RefreshUI();
        StartCoroutine(RefetchCards());
        StartCoroutine(RefetchCombos());
    }

    private IEnumerator RefetchCards()
    {
        yield return StartCoroutine(CardApiService.Instance.FetchAllCards(
            onSuccess: _ => Debug.Log("[MainMenuUI] Cards refetched in new language."),
            onError: err => Debug.LogError($"[MainMenuUI] Card refetch failed: {err}")
        ));
    }
    
    private IEnumerator RefetchCombos()
    {
        yield return StartCoroutine(CardComboApiService.Instance.FetchAllCardCombos(
            onSuccess: _ => Debug.Log("[MainMenuUI] Combos refetched in new language."),
            onError: err => Debug.LogError($"[MainMenuUI] Combo refetch failed: {err}")
        ));
    }

    private void ToggleLanguage()
    {
        GameManager.Instance.ToggleLanguage();
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