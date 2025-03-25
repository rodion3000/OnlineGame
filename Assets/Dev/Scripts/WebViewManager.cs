using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;

public class WebViewManager : MonoBehaviour, IAppsFlyerConversionData
{
    private string endpoint = "https://sturdy-clutch-f43.notion.site/742144532ce0403988b0689e3f942982"; // Укажите ваш endpoint

    private void Start()
    {
        
        // Получаем URL из PlayerPrefs
        string url = PlayerPrefs.GetString("LastWebViewUrl", "https://default-url.com");

        // Обработка данных конверсии
        onConversionDataSuccess(url);

        // Открываем URL в браузере
        OpenUrlInBrowser(url);
    }

    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);

        // Отправляем данные конверсии на сервер
        StartCoroutine(SendConversionData(conversionData));
    }
    
    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("onConversionDataFail", error);
    }

    private IEnumerator SendConversionData(string conversionData)
    {
        string jsonData = JsonUtility.ToJson(new { conversionData = conversionData });

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(endpoint, jsonData))
        {
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error sending conversion data: " + webRequest.error);
            }
            else
            {
                Debug.Log("Conversion data sent successfully: " + webRequest.downloadHandler.text);
            }
        }
    }

    private void OpenUrlInBrowser(string url)
    {
        // Проверяем, что URL валиден
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogError("URL is empty or null.");
        }
    }
}
