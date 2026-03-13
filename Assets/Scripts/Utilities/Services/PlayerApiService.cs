using System;
using System.Collections;
using UnityEngine;

public class PlayerApiService : MonoBehaviour
{
    public static PlayerApiService Instance { get; private set; }

    public Player CurrentPlayer { get; private set; }

    void Awake()
    {
        Debug.Log("[PlayerApiService] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator CreateOrGetPlayer(string deviceId, Action<Player> onSuccess = null, Action<string> onError = null)
    {
        // Try to get first
        yield return StartCoroutine(ApiClient.Get<Player>(
            $"/api/players/{deviceId}",
            player => CurrentPlayer = player,
            _ => { }
        ));

        if (CurrentPlayer != null)
        {
            onSuccess?.Invoke(CurrentPlayer);
            yield break;
        }

        // Player not found — create
        yield return StartCoroutine(ApiClient.Post<CreatePlayerDto, Player>(
            "/api/players",
            new CreatePlayerDto { DeviceId = deviceId },
            player =>
            {
                CurrentPlayer = player;
                onSuccess?.Invoke(CurrentPlayer);
            },
            (err) => onError?.Invoke(err)
        ));
    }

    public IEnumerator UpdateProgression(Action onSuccess = null, Action<string> onError = null)
    {
        var dto = new UpdateProgressionDto
        {
            CurrentXp = PlayerProgressionSystem.Instance.CurrentXP,
            CurrentLevel = PlayerProgressionSystem.Instance.CurrentLevel
        };

        yield return StartCoroutine(ApiClient.Put(
            $"/api/players/{CurrentPlayer.DeviceId}/progression",
            dto,
            () =>
            {
                CurrentPlayer.Progression.CurrentXP = dto.CurrentXp;
                CurrentPlayer.Progression.CurrentLevel = dto.CurrentLevel;
                onSuccess?.Invoke();
            },
            (err) => onError?.Invoke(err)
        ));
    }
}