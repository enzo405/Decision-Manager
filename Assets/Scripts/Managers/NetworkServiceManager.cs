using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(CardApiService))]
[RequireComponent(typeof(PlayerApiService))]
[RequireComponent(typeof(ConfigApiService))]
public class NetworkServiceManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);

    public static NetworkServiceManager Instance { get; private set; }
    public bool IsReady { get; private set; } = false;
    private PlayerApiService _playerService;
    private CardApiService _cardService;
    private ConfigApiService _configApiService;
    private CardComboApiService _cardComboApiService;

    [SerializeField] private LoadingUI loadingUI;

    public string ApiBaseUrl { get; private set; }
    public string ApiKey { get; private set; }

    public void Awake()
    {
        Debug.Log("[NetworkServiceManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSavedLanguage();

        LoadConfig();

        _playerService = GetComponent<PlayerApiService>();
        _cardService = GetComponent<CardApiService>();
        _configApiService = GetComponent<ConfigApiService>();
        _cardComboApiService = GetComponent<CardComboApiService>();
    }

    public void Start()
    {
        Debug.Log("[NetworkServiceManager] Start");
        StartCoroutine(Initialize());
    }

    private void LoadSavedLanguage()
    {
        string savedCode = PlayerPrefs.GetString("selected_language", "en");
        Debug.Log($"[Localization] Loading saved language: {savedCode}");
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale(savedCode);
    }

    private IEnumerator Initialize()
    {
        // Wait for localization to be ready before using it
        yield return LocalizationSettings.InitializationOperation;

        string deviceId = SystemInfo.deviceUniqueIdentifier;

        // 1 — Create or retrieve player
        loadingUI.SetProgress(Loc("loading.step.player"), 0f);
        yield return StartCoroutine(_playerService.CreateOrGetPlayer(
            deviceId,
            onSuccess: player => Debug.Log($"[Network] Player initialized: {player.DeviceId}"),
            onError: error => Debug.LogError($"[Network] Player init failed with DeviceId {deviceId}: {error}")
        ));
        yield return _waitForSeconds0_5;

        // 2 — Fetch Config
        loadingUI.SetProgress(Loc("loading.step.config"), 0.25f);
        yield return StartCoroutine(_configApiService.FetchDefeatConditions(
            onSuccess: defeatConditions => Debug.Log($"[Network] Fetched defeat conditions."),
            onError: error => Debug.LogError($"[Network] FetchDefeatConditions failed: {error}")
        ));

        yield return StartCoroutine(_configApiService.FetchThresholds(
            onSuccess: thresholds => Debug.Log($"[Network] Fetched thresholds."),
            onError: error => Debug.LogError($"[Network] FetchThresholds failed: {error}")
        ));

        yield return StartCoroutine(_configApiService.FetchStatsInit(
            onSuccess: statsInit => Debug.Log($"[Network] Fetched initial stats."),
            onError: error => Debug.LogError($"[Network] FetchStatsInit failed: {error}")
        ));
        yield return _waitForSeconds0_5;

        // 3 — Initialize Cards
        loadingUI.SetProgress(Loc("loading.step.cards"), 0.50f);
        yield return StartCoroutine(_cardService.FetchAllCards(
            onSuccess: cards => Debug.Log($"[Network] Fetched {cards.Count} cards."),
            onError: error => Debug.LogError($"[Network] Card init failed: {error}")
        ));

        // 4 — Initialize CardCombos
        loadingUI.SetProgress(Loc("loading.step.cardCombo"), 0.75f);
        yield return StartCoroutine(_cardComboApiService.FetchAllCardCombos(
            onSuccess: combos => Debug.Log($"[Network] Fetched {combos.Count} card combos."),
            onError: error => Debug.LogError($"[Network] CardCombos init failed: {error}")
        ));

        loadingUI.SetProgress(Loc("loading.step.ready"), 1f);
        yield return _waitForSeconds0_5;

        loadingUI = null;
        SceneManager.LoadScene("MainMenu");
    }

    private string Loc(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("UI_Loading", key);
    }

    private void LoadConfig()
    {
        TextAsset file = Resources.Load<TextAsset>("ApiConfig");
        ApiConfig config = JsonUtility.FromJson<ApiConfig>(file.text);
        ApiBaseUrl = config.apiBaseUrl;
        ApiKey = config.apiKey;
    }
}