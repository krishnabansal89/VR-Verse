using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Threading;


public class GyroService : WebSocketBehavior
{
    private static Quaternion receivedRotation;
    private static bool dataReceived = false;
   

    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Data Recieved");
        GyroData data = JsonUtility.FromJson<GyroData>(e.Data);
        receivedRotation = new Quaternion(data.x, data.y, data.z, data.w);
        dataReceived = true;
        
    }

    public static Quaternion GetReceivedRotation()
    {
        if (dataReceived)
        {
            dataReceived = false;
            return receivedRotation;
        }
        return Quaternion.identity;
    }
}

public class WsServer : MonoBehaviour
{
    private WebSocketServer wss;
    public GameObject controlledObject;
    public Quaternion GyroRotation;
    void Start()
    {
        wss = new WebSocketServer("ws://0.0.0.0:8088");
        wss.AddWebSocketService<GyroService>("/Gyro");
        wss.Start();
        Debug.Log("WebSocket server started on ws://localhost:8080");
    }

    void Update()
    {
        Quaternion rotation = GyroService.GetReceivedRotation();
        if (rotation != Quaternion.identity)
        {
            Debug.Log("kuch to aaya he bc");
            controlledObject.transform.localRotation = rotation;
            GyroRotation = rotation;
        }
    }

    void OnApplicationQuit()
    {
        wss.Stop();
    }
}
