using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float currentHealth;
    
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    [Header("Connection with other components")]
    public Transform player;
    public ThirdPersonMovement playerInfo;

    public void UpdateGradientColor()
    {
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInfo = player.GetComponent<ThirdPersonMovement>();
        slider.maxValue = playerInfo.maxHealth;

        fill.color = gradient.Evaluate(0f);
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = playerInfo.currentHealth;
        slider.value = slider.maxValue - currentHealth;

        UpdateGradientColor();
    }
}
