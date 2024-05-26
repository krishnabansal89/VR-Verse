using UnityEngine;
using WebSocketSharp;
using System.Collections;

public class ScreenSender : MonoBehaviour
{
    private WebSocket ws;
    private Texture2D screenImage;

    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.Connect();
        screenImage = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        StartCoroutine(SendScreenshots());
    }

    IEnumerator SendScreenshots()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Kuch to aaya he bc");

            try
            {
                // Capture the screen
                screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                screenImage.Apply();
                byte[] imageData = screenImage.EncodeToJPG(); // Use EncodeToPNG() if you prefer PNG

                // Send image data through WebSocket
                if (ws.ReadyState == WebSocketState.Open)
                {
                    ws.Send(imageData);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error during screen capture: " + ex.Message);
            }

            yield return new WaitForSeconds(0.033f); // Approximately 30 FPS
        }
    }

    void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }

        if (screenImage != null)
        {
            Destroy(screenImage);
        }
    }
}
