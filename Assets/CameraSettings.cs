using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraSettings : MonoBehaviour
{
    CinemachineFreeLook cinemachineCam;

    public Slider mouseSensitivitySlider;

    public void UpdateCinemachineXSpeed(float speed)
    {
        cinemachineCam.m_XAxis.m_MaxSpeed = speed;
    }

    private void Start()
    {
        mouseSensitivitySlider.value = StaticData.camSensitivityXAxis;
        cinemachineCam = GetComponent<CinemachineFreeLook>();

    }
    private void Update()
    {
        UpdateCinemachineXSpeed(mouseSensitivitySlider.value);

        StaticData.camSensitivityXAxis = mouseSensitivitySlider.value;
    }
}
