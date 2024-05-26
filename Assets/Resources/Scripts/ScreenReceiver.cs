using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ScreenReceiver : MonoBehaviour
{
    public RawImage rawImage;
    private WebSocket ws;
    private Texture2D texture;

    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += OnMessage;
        ws.Connect();
        texture = new Texture2D(2, 2); // Placeholder texture
        GameObject dispatcherObject = new GameObject("MainThreadDispatcher");
        dispatcherObject.AddComponent<UnityMainThreadDispatcher>();

    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        try {
            if (e.RawData != null)
            {
                Debug.Log("Kuch to aaye he");
                UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
                {
                    Texture2D receivedTexture = new Texture2D(1, 1);
                    receivedTexture.LoadImage(e.RawData);
                    // Use the receivedTexture as needed
                    // For example, assign it to a UI element
                    rawImage.texture = receivedTexture;
                });
            }

        }
        catch (System.Exception ex) { Debug.Log(ex); }
    }

    void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
