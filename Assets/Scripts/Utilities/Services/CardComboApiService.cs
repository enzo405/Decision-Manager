using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CardComboApiService : MonoBehaviour
{
    public static CardComboApiService Instance { get; private set; }

    public List<CardCombo> AllCombos { get; private set; }


    void Awake()
    {
        Debug.Log("[CardComboApiService] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    public IEnumerator FetchAllCardCombos(Action<List<CardCombo>> onSuccess = null, Action<string> onError = null)
    {
        string locale = LocalizationSettings.SelectedLocale.Identifier.Code;

        yield return StartCoroutine(ApiClient.Get<List<CardCombo>>(
            $"/api/combos?locale={locale}",
            combos =>
            {
                AllCombos = combos;
                onSuccess?.Invoke(combos);
            },
            err => onError?.Invoke(err)
        ));
    }

}