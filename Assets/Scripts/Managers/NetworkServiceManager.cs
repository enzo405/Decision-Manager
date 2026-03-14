using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private LoadingUI loadingUI;

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

        _playerService = GetComponent<PlayerApiService>();
        _cardService = GetComponent<CardApiService>();
        _configApiService = GetComponent<ConfigApiService>();
    }

    public void Start()
    {
        Debug.Log("[NetworkServiceManager] Start");
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;

        // 1 — Create or retrieve player
        loadingUI.SetProgress("Connexion au serveur...", 0f);
        yield return StartCoroutine(_playerService.CreateOrGetPlayer(
            deviceId,
            onSuccess: player => Debug.Log($"[Network] Player initialized: {player.DeviceId}"),
            onError: error => Debug.LogError($"[Network] Player init failed with DeviceId {deviceId}: {error}")
        ));
        yield return _waitForSeconds0_5;

        // 2 - Fetch Config
        loadingUI.SetProgress("Chargement de la configuration...", 0.33f);
        yield return StartCoroutine(_configApiService.FetchDefeatConditions(
            onSuccess: defeatConditions => Debug.Log($"[Network] Fetched and initialized defeat conditions: Motivation={defeatConditions.Motivation.Min}/ {defeatConditions.Motivation.Max}; Stress={defeatConditions.Stress.Min}/ {defeatConditions.Stress.Max}; Performance={defeatConditions.Performance.Min}/ {defeatConditions.Performance.Max}; Turnover={defeatConditions.Turnover.Min}/ {defeatConditions.Turnover.Max}"),
            onError: error => Debug.LogError($"[Network] FetchDefeatConditions init failed: {error}")
        ));

        yield return StartCoroutine(_configApiService.FetchThresholds(
            onSuccess: thresholds => Debug.Log($"[Network] Fetched and initialized thresholds: BaseXp={thresholds.BaseXp}; Exponent={thresholds.Exponent}; MaxLevel={thresholds.MaxLevel}; XpBonusGoodDecision={thresholds.XpBonusGoodDecision}; XpPerTurn={thresholds.XpPerTurn}"),
            onError: error => Debug.LogError($"[Network] FetchThresholds init failed: {error}")
        ));

        yield return StartCoroutine(_configApiService.FetchStatsInit(
            onSuccess: statsInit => Debug.Log($"[Network] Fetched and initialized initial stats: InitialMotivation={statsInit.InitialMotivation}; InitialPerformance={statsInit.InitialPerformance}; InitialStress={statsInit.InitialStress}; InitialTurnover={statsInit.InitialTurnover}"),
            onError: error => Debug.LogError($"[Network] FetchStatsInit init failed: {error}")
        ));
        yield return _waitForSeconds0_5;


        // 3 - Initialize Cards
        loadingUI.SetProgress("Chargement des cartes...", 0.66f);
        yield return StartCoroutine(_cardService.FetchAllCards(
               onSuccess: cards => Debug.Log($"[Network] Fetched and initialized {cards.Count} cards."),
               onError: error => Debug.LogError($"[Network] Card init failed: {error}")
           ));

        Debug.Log("[Network] Initialization complete.");
        loadingUI.SetProgress("Prêt !", 1f);

        yield return _waitForSeconds0_5;
        loadingUI = null;
        SceneManager.LoadScene("MainMenu");
    }
}