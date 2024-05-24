using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private WsServer _WsServer;
    private Quaternion _Rotation;
    public Camera _Camera;
    public Transform _Transform;
    [SerializeField] private float xOff;
    [SerializeField] private float yOff;
    [SerializeField] private float zOff;
    [SerializeField] private float wOff;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _Rotation.x = _WsServer.GyroRotation.y * xOff;
        _Rotation.y = _WsServer.GyroRotation.x * yOff;
        _Rotation.z = _WsServer.GyroRotation.z * zOff;
        _Rotation.w = _WsServer.GyroRotation.w * wOff;
        Debug.Log(_Rotation);
        _Camera.transform.localRotation = _Rotation; 
        _Transform.rotation = _Rotation;
    }
}
