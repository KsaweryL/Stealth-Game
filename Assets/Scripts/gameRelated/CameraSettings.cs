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
        if (GetComponent<CinemachineFreeLook>())
        {
            cinemachineCam.m_XAxis.m_MaxSpeed = speed;
            float maxYSpeed = 1.3f;
            if (speed > 220.0f)
                cinemachineCam.m_YAxis.m_MaxSpeed = maxYSpeed * speed / mouseSensitivitySlider.maxValue;
            else
                cinemachineCam.m_YAxis.m_MaxSpeed = 0.0f;

        }
    }

    private void Start()
    {
        mouseSensitivitySlider.value = StaticData.camSensitivityXAxis;
        mouseSensitivitySlider.maxValue = 250.0f;
        if (GetComponent<CinemachineFreeLook>())
        {
            cinemachineCam = GetComponent<CinemachineFreeLook>();
        }

    }
    private void Update()
    {
        UpdateCinemachineXSpeed(mouseSensitivitySlider.value);

        StaticData.camSensitivityXAxis = mouseSensitivitySlider.value;
    }
}
