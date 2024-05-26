using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;
using Unity.VisualScripting;
using System;
using UnityEngine.AdaptivePerformance.Provider;

public class GyroHandler : MonoBehaviour
{
    private Gyroscope gyro;
    private bool isGyroSupported;
    public TMP_Text text;
    private Quaternion rotation;
    public Transform _transform;
    private WebSocket ws;
    private Texture2D texture;
    public RawImage rawImage;

    void Awake()
    {
        try
        {
            ws = new WebSocket("ws://192.168.175.180:8080/Gyro");
            ws.Connect();
            ws.OnMessage += OnMessage;
            //StartCoroutine(SendGyroData());
            text.SetText("Hello There");
            texture = new Texture2D(2, 2);
            isGyroSupported = SystemInfo.supportsGyroscope;
            if (isGyroSupported)
            {
                gyro = Input.gyro;
                gyro.enabled = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            text.SetText(e.ToString());
        }
    }

    void Update()
    {
        if (isGyroSupported)
        {
            rotation = GyroToUnity(gyro.attitude);
            transform.localRotation = rotation;
            text.SetText(transform.localRotation.ToString());
            _transform.localRotation = rotation;
        }
    }

    public static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    IEnumerator SendGyroData()
    {
        while (true)
        {
            if (isGyroSupported)
            {
                GyroData gyroData = new GyroData
                {
                    x = gyro.attitude.x,
                    y = gyro.attitude.y,
                    z = gyro.attitude.z,
                    w = gyro.attitude.w
                };
                string jsonData = JsonUtility.ToJson(gyroData);
                Debug.Log("GyroData JSON: " + jsonData);
                //ws.Send(jsonData);
                Debug.Log("GyroData Sent");
            }

            yield return new WaitForSeconds(1.0f / 120); // Send data at ~10 FPS
        }
    }
    private void OnMessage(object sender, MessageEventArgs e)
    {
        if (e.RawData != null)
        {
            texture.LoadImage(e.RawData);
            rawImage.texture = texture;
        }
    }

}

[Serializable]
struct GyroData
{
    public float x;
    public float y;
    public float z;
    public float w;
}
