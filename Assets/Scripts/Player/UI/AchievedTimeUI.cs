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
        AchievedTimeText.text = "Previously achieved time: " + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().GetElapsedTime().Minutes + " minutes and " + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().GetElapsedTime().Seconds + " seconds";
    }

    public void UpdateAchievedTimeTextCustom(string str)
    {
        AchievedTimeText = GetComponent<TextMeshProUGUI>();
        if (GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>())
            AchievedTimeText.text = str + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().GetElapsedTime().Minutes + " minutes and "+GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().GetElapsedTime().Seconds + " seconds";
    }
}
