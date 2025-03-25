using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;

public class AppManager : MonoBehaviour
{
    private const string endpoint = "https://sturdy-clutch-f43.notion.site/742144532ce0403988b0689e3f942982"; // Замените на ваш эндпоинт
    private const string webViewModeKey = "WebViewMode";
    private const string lastWebViewUrlKey = "LastWebViewUrl";
    private const string appName = "Plinking Books"; 

    private void Start()
    {
        // Инициализация AppsFlyer
        AppsFlyer.initSDK("YOUR_DEV_KEY", "com.DefaultCompany.OnlineGame");
        AppsFlyer.startSDK();

        // Проверка интернет-соединения и загрузка WebView
        StartCoroutine(CheckInternetAndLoadWebView());
    }

    private IEnumerator CheckInternetAndLoadWebView()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(endpoint))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                LoadNoInternetScene();
            }
            else
            {
                // Предполагаем, что ответ содержит URL для WebView
                string url = webRequest.downloadHandler.text; // Замените на правильный способ получения URL
                PlayerPrefs.SetString(lastWebViewUrlKey, url);
                PlayerPrefs.SetString(webViewModeKey, "WebView");
                

                // Загрузка сцены WebView
                LoadWebViewScene();
            }
        }
    }
    
    private void LoadWebViewScene()
    {
        SceneManager.LoadScene("WebViewScene");
    }

    private void LoadNoInternetScene()
    {
        SceneManager.LoadScene("NoInternetScene");
    }

    
}
