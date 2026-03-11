using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public static class ApiClient
{
    public static IEnumerator Get<T>(string endpoint, System.Action<T> onSuccess, System.Action<string> onError = null)
    {
        using var request = UnityWebRequest.Get(ApiConfig.BaseUrl + endpoint);
        request.SetRequestHeader("X-Api-Key", ApiConfig.ApiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        var result = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        onSuccess(result);
    }

    public static IEnumerator Post<TBody, TResponse>(string endpoint, TBody body, System.Action<TResponse> onSuccess, System.Action<string> onError = null)
    {
        var json = JsonConvert.SerializeObject(body);
        using var request = new UnityWebRequest(ApiConfig.BaseUrl + endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Api-Key", ApiConfig.ApiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        var result = JsonConvert.DeserializeObject<TResponse>(request.downloadHandler.text);
        onSuccess(result);
    }

    public static IEnumerator Put<TBody>(string endpoint, TBody body, System.Action onSuccess, System.Action<string> onError = null)
    {
        var json = JsonConvert.SerializeObject(body);
        using var request = new UnityWebRequest(ApiConfig.BaseUrl + endpoint, "PUT");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Api-Key", ApiConfig.ApiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            onError?.Invoke(request.error);
        else
            onSuccess();
    }
}