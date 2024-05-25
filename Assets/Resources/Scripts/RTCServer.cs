using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using WebSocketSharp;
public class RTCServer : MonoBehaviour
{
    // Start is called before the first frame update
    private RTCPeerConnection _peerConnection;
    private RTCDataChannel _channel;
    private List<RTCIceCandidate> iceCandidates = new List<RTCIceCandidate>();
    private Texture2D _texture;
    private WebSocket ws;
    void Start()
    {
        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += OnMessage;
        ws.Connect();
    }

    // Update is called once per frame

    void InitializedWebRTC()
    {
        var config = GetConfiguration();
        Debug.Log("Initialised WebRTC");
        _peerConnection = new RTCPeerConnection(ref config);
        _channel = _peerConnection.CreateDataChannel("data");

        _peerConnection.OnIceCandidate = candidate =>
        {
            iceCandidates.Add(candidate);
            ws.Send(JsonUtility.ToJson(candidate));
        };
        StartCoroutine(SendScreenCapture());

    }

    RTCConfiguration GetConfiguration()
    {
        return new RTCConfiguration
        {
            iceServers = new[]
            {
                new RTCIceServer
                {
                    urls = new[] { "stun:stun.l.google.com:19302" }
                }
            }
        };
    }
    void OnMessage(object sender, MessageEventArgs e)
    {
        if (e.Data != null)
        {
            Debug.Log("Message Recieved");
            Debug.Log(e.Data);
            if(e.Data.Contains("offer"))
            {
                var offer = JsonUtility.FromJson<RTCSessionDescription>(e.Data);
                _peerConnection.SetRemoteDescription(ref offer);
                StartCoroutine(CreateAnswer());

            }
            if (e.Data.Contains("candidate"))
            {
                var candidate = JsonUtility.FromJson<RTCIceCandidate>(e.Data);
                _peerConnection.AddIceCandidate(candidate);

            }
        }
    }
    IEnumerator CreateAnswer()
    {
        var answerOptions = new RTCOfferAnswerOptions { iceRestart = false };
        var answerOp = _peerConnection.CreateAnswer(ref answerOptions);
        yield return answerOp;
        RTCSessionDescription answer = answerOp.Desc;

        _peerConnection.SetLocalDescription(ref answer);
        ws.Send(JsonUtility.ToJson(answer));
    }
    IEnumerator SendScreenCapture()
    {
         yield return new WaitForEndOfFrame();
        if(_texture != null)
        {
            Destroy(_texture);
        }
        _texture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] screenBytes = _texture.EncodeToJPG();
        _channel.Send(screenBytes);


    }
    void OnDestroy()
    {
        _peerConnection.Close();
    }

}
