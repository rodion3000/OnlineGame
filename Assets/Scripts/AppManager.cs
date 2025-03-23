using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class AppManager : MonoBehaviour
{
    private const string endpoint = "https://your-endpoint-url.com"; // Замените на ваш эндпоинт
    private const string webViewModeKey = "WebViewMode";
    private const string lastWebViewUrlKey = "LastWebViewUrl";

    private void Start()
    {
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
}
