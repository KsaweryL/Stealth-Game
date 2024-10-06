using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievedTimeUI : MonoBehaviour
{
    private TextMeshProUGUI AchievedTimeText;

    private void Start()
    {
        AchievedTimeText = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateAchievedTimeText()
    {
        AchievedTimeText.text = "Previously achieved time: " + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().GetElapsedTime().Seconds;
    }
}
