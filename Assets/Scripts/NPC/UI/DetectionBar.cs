using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class DetectionBar : MonoBehaviour
{
    public int currentlyDetectedTime;

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    [Header("Connection with other components")]
    public NPCFieldOfView NPCFOV;

    public void UpdateGradientColor()
    {
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        //it needs to know which NPC we mean here
        if (NPCFOV)
        {
            currentlyDetectedTime = NPCFOV.GetCurrentlyDetectedTimeNPCFOV();
            slider.maxValue = (int)Math.Round(NPCFOV.GetTimeToDetectNPCFOV() / 0.02);

            fill.color = gradient.Evaluate(0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NPCFOV)
        {
            //NPCFOV = GetComponentInParent<NPCFieldOfView>();
            currentlyDetectedTime = NPCFOV.GetCurrentlyDetectedTimeNPCFOV();
            slider.value = currentlyDetectedTime;

            UpdateGradientColor();
        }
    }
}
