using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;

public class AppManager : MonoBehaviour, IAppsFlyerConversionData
{
    private const string endpoint = "https://your-endpoint-url.com"; // Замените на ваш эндпоинт
    private const string webViewModeKey = "WebViewMode";
    private const string lastWebViewUrlKey = "LastWebViewUrl";

    private void Start()
    {
        // Инициализация AppsFlyer
        AppsFlyer.initSDK("YOUR_DEV_KEY", "YOUR_APP_ID");
        AppsFlyer.startSDK();

        // Проверка режима WebView
        string mode = PlayerPrefs.GetString(webViewModeKey, "NoInternet");
        if (mode == "WebView")
        {
            StartCoroutine(CheckWebView());
        }
        else if (mode == "Fallback")
        {
            LoadFallbackScene();
        }
        else
        {
            LoadNoInternetScene();
        }
    }

    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        StartCoroutine(SendConversionData(conversionData));
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

    private IEnumerator CheckWebView()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(endpoint))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                LoadFallbackScene();
            }
            else
            {
                // Предполагаем, что ответ содержит URL для WebView
                string url = webRequest.downloadHandler.text; // Замените на правильный способ получения URL
                PlayerPrefs.SetString(lastWebViewUrlKey, url);
                PlayerPrefs.SetString(webViewModeKey, "WebView");
                LoadWebViewScene(url);
            }
        }
    }

    private void LoadWebViewScene(string url)
    {
        // Здесь вы можете передать URL в WebView
        SceneManager.LoadScene("WebViewScene");
    }

    private void LoadFallbackScene()
    {
        PlayerPrefs.SetString(webViewModeKey, "Fallback");
        SceneManager.LoadScene("FallbackScene");
    }

    private void LoadNoInternetScene()
    {
        SceneManager.LoadScene("NoInternetScene");
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
}
