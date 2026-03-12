using System;
using System.Collections;
using UnityEngine;

public class ConfigApiService : MonoBehaviour
{
    public static ConfigApiService Instance { get; private set; }

    public Thresholds Thresholds { get; private set; }
    public DefeatConditions DefeatConditions { get; private set; }
    public StatsInit StatsInit { get; private set; }

    void Awake()
    {
        Debug.Log("[ConfigApiService] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator FetchThresholds(Action<Thresholds> onSuccess = null, Action<string> onError = null)
    {
        yield return StartCoroutine(ApiClient.Get<Thresholds>(
            "/api/config/thresholds",
            thresholds =>
            {
                Thresholds = thresholds;
                onSuccess?.Invoke(Thresholds);
            },
            (err) => onError?.Invoke(err)
        ));
    }

    public IEnumerator FetchDefeatConditions(Action<DefeatConditions> onSuccess = null, Action<string> onError = null)
    {
        yield return StartCoroutine(ApiClient.Get<DefeatConditions>(
            "/api/config/defeat-conditions",
            defeatConditions =>
            {
                DefeatConditions = defeatConditions;
                onSuccess?.Invoke(DefeatConditions);
            },
            (err) => onError?.Invoke(err)
        ));
    }

    public IEnumerator FetchStatsInit(Action<StatsInit> onSuccess = null, Action<string> onError = null)
    {
        yield return StartCoroutine(ApiClient.Get<StatsInit>(
            "/api/config/initial-stats",
            statsInit =>
            {
                StatsInit = statsInit;
                onSuccess?.Invoke(StatsInit);
            },
            (err) => onError?.Invoke(err)
        ));
    }
}