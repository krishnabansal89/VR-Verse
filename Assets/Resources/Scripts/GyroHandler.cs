using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class GyroHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Gyroscope gyro;
    private bool isGyroSupported;
    public TMP_Text text ;
    private Quaternion rotation;
    void Start()
    {
        text.SetText("Hello There");
        isGyroSupported = SystemInfo.supportsGyroscope;
        if(isGyroSupported )
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGyroSupported)
        {
            rotation = GyroToUnity(gyro.attitude);
            transform.localRotation = rotation;
            text.SetText(transform.localRotation.ToString());
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
