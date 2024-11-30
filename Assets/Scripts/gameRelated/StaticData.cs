using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticData : MonoBehaviour
{
    //this class contains data that will be shared among various scenes

    //for currently available sounds
    public static float soundfxVolumeSliderValue = 0.4f;
    public static float masterVolumeSliderValue = 0.4f;
    public static float musicVolumeSliderValue = 0.4f;

    //for camera sensitivity
    public static float camSensitivityXAxis = 120.0f;

}
