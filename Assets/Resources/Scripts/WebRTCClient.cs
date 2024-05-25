using UnityEngine;
using Unity.WebRTC;
using UnityEngine.UI;
using System.Collections;
using WebSocketSharp;


public class WebRTCClient : MonoBehaviour
{
    public RawImage displayImage;
    private RTCPeerConnection peerConnection;
    private RTCDataChannel dataChannel;
    private Texture2D receivedTexture;
    private WebSocket ws;

    void Start()
    {
        StartCoroutine(WebRTC.Update());
        InitializeWebRTC();

        ws = new WebSocket("ws://localhost:8080");
        ws.OnMessage += OnMessage;
        ws.Connect();
    }

    void InitializeWebRTC()
    {
        var config = GetConfiguration();
        peerConnection = new RTCPeerConnection(ref config);

        peerConnection.OnDataChannel = channel =>
        {
            dataChannel = channel;
            dataChannel.OnMessage = OnDataChannelMessage;
        };

        peerConnection.OnIceCandidate = candidate =>
        {
            ws.Send(JsonUtility.ToJson(candidate));
        };

        StartCoroutine(CreateOffer());
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

    void OnDataChannelMessage(byte[] bytes)
    {
        if (receivedTexture == null)
        {
            receivedTexture = new Texture2D(2, 2);
        }

        receivedTexture.LoadImage(bytes);
        displayImage.texture = receivedTexture;
    }

    void OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        if (e.Data.Contains("answer"))
        {
            var answer = JsonUtility.FromJson<RTCSessionDescription>(e.Data);
            peerConnection.SetRemoteDescription(ref answer);
        }
        else if (e.Data.Contains("candidate"))
        {
            var candidate = JsonUtility.FromJson<RTCIceCandidate>(e.Data);
            peerConnection.AddIceCandidate(candidate);
        }
    }

    IEnumerator CreateOffer()
    {
        var offerOptions = new RTCOfferAnswerOptions();

        // Add tracks to peer connection
        var track = (GetComponent<Camera>()).CaptureStreamTrack(1280, 720);
        peerConnection.AddTrack(track);


        var offerOp = peerConnection.CreateOffer(ref offerOptions);
        yield return offerOp;

        if (offerOp.IsError)
        {
            Debug.LogError($"Error creating offer: {offerOp.Error.message}");
            yield break;
        }

        RTCSessionDescription offer = offerOp.Desc;
        var setLocalDescOp = peerConnection.SetLocalDescription(ref offer);
        yield return setLocalDescOp;

        if (setLocalDescOp.IsError)
        {
            Debug.LogError($"Error setting local description: {setLocalDescOp.Error.message}");
            yield break;
        }

        ws.Send(JsonUtility.ToJson(offer));
    }


    void OnDestroy()
    {
        peerConnection.Close();
    }
}
