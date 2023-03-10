using UnityEngine;
using System.Collections;

public class CameraAdjustSize : MonoBehaviour
{

    private float widthToBeSeen = 14.2f;

    void Start()
    {
        // Camera.main.orthographicSize = widthToBeSeen * Screen.height / Screen.width * 0.5f;
        //QualitySettings.SetQualityLevel(5, true);
    }

}
