using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using WebSocketSharp.Server;

public class SceneHandler : MonoBehaviour
{
    public Transform _transform;



    private void Start()
    {
    }

    public void SwitchToScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
    void setRotation(Quaternion rotation)
    {
        _transform.rotation = rotation;
    }
}
