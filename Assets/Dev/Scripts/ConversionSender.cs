using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AppsFlyerSDK;

[System.Serializable]
public class ConversionData
{
    public string af_id;
    public string bundle_id;
    public string os;
    public string store_id;
    public string locale;
    public string push_token;
    public string firebase_project_id;
}

[System.Serializable]
public class ApiResponse
{
    public bool ok;
    public string message;
    public string url;
    public long expires; // Используем long для timestamp
}

public class ConversionSender : MonoBehaviour, IAppsFlyerConversionData
{
    private string url = "https://your-api-endpoint.com"; // Замените на ваш эндпоинт

    void Start()
    {
        // Инициализация SDK AppsFlyer
        AppsFlyer.initSDK("mbuEj35ruc3QCoNtoq5pra", "com.DefaultCompany.OnlineGame", this); // Замените на ваши значения
        AppsFlyer.startSDK();
    }

    public void SendConversionData()
    {
        ConversionData conversionData = new ConversionData
        {
            af_id = "1688042316289-7152592750959506765",
            bundle_id = "com.example.app",
            os = "Android",
            store_id = "id643200239", // Замените на нужный store_id
            locale = "En",
            push_token = "dl28EJCAT4a7UNl86egX-U:APA91bEC1a5aGJL8ZyQHlm-B9togw60MLWP4_zU0ExSXLSa_HiL82Iurj0d-1zJmkMdUcvgCRXTrXtbWQHxmJh49BibLiqZVXPNyrCdZW-_ROTt98f0WCLtt531RYPhWSDOkykcaykE3",
            firebase_project_id = "8934278530"
        };

        StartCoroutine(SendRequest(conversionData));
    }

    private IEnumerator SendRequest(ConversionData conversionData)
    {
        string json = JsonUtility.ToJson(conversionData);
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                // Обработка успешного ответа
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                LogResponse(response);
            }
        }
    }

    private void LogResponse(ApiResponse response)
    {
        if (response.ok)
        {
            Debug.Log($"Успешный запрос: Status: 200 (OK)");
            Debug.Log($"URL: {response.url}");
            Debug.Log($"Expires: {response.expires}");
        }
        else
        {
            Debug.LogError($"Запрос завершился ошибкой: {response.message}");
        }
    }

    // Реализация методов интерфейса IAppsFlyerConversionData
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        
        // Здесь вы можете обработать данные конверсии, если это необходимо
        HandleConversionData(conversionDataDictionary);
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("onConversionDataFail", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

    private void HandleConversionData(Dictionary<string, object> conversionData)
    {
        // Пример обработки данных конверсии
        foreach (var item in conversionData)
        {
            Debug.Log($"Key: {item.Key}, Value: {item.Value}");
        }

        // Здесь вы можете добавить логику для перенаправления пользователя или выполнения других действий
    }
}