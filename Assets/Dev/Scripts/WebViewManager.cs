using UnityEngine;
using UnityEngine.Networking;
using Firebase;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;

public class WebViewManager : MonoBehaviour, IAppsFlyerConversionData
{
    private string endpoint = "https://sturdy-clutch-f43.notion.site/742144532ce0403988b0689e3f942982"; // Укажите ваш endpoint

    private void Start()
    {
        // Инициализация Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            Debug.Log("Firebase is ready.");
        });
        
        // Подписка на события получения сообщений
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        
        RequestNotificationPermission();
        
        // Получаем URL из PlayerPrefs
        string url = PlayerPrefs.GetString("LastWebViewUrl", "https://default-url.com");

        // Обработка данных конверсии
        onConversionDataSuccess(url);

        // Открываем URL в браузере
        OpenUrlInBrowser(url);
    }
    

    private void RequestNotificationPermission()
    {
        FirebaseMessaging.RequestPermissionAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Notification permission granted.");
                // Здесь вы можете подписаться на уведомления или выполнить другие действия
            }
            else
            {
                Debug.LogError("Notification permission denied.");
            }
        });
    }
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);

        // Отправляем данные конверсии на сервер
        StartCoroutine(SendConversionData(conversionData));
    }
    
    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message: " + e.Message.From);
        // Обработка уведомления
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
