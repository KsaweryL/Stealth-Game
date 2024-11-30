using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class AchievedTimeUI : MonoBehaviour
{
    public TextMeshProUGUI AchievedTimeText;
    public TextMeshProUGUI AchievedTimeTextEndOfGame;
    private Diamond[] allDiamonds;
    int collectedDiamonds = 0;

    private void Start()
    {
        //AchievedTimeText = GetComponent<TextMeshProUGUI>();
        allDiamonds = GetComponentInParent<Game>().GetDiamonds();
    }
    public void UpdateAchievedTimeText()
    {
        AchievedTimeText.text = AchievedTimeText.text + "\n" + "Total time: " + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateElapsedTime().Minutes + " minutes and " + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateElapsedTime().Seconds + " seconds";

        //if there is end of game screen, update it as well
        if (AchievedTimeTextEndOfGame)
            AchievedTimeTextEndOfGame.text = AchievedTimeText.text;
    }

    public void UpdateDiamondText(Diamond diamond, string text)
    {
        System.TimeSpan timeToGetDiamond = GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateTimeOnDiamond(diamond);
        GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateDiamondAquisitionRate(diamond, true);

        Metrics metrics = GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>();

        AchievedTimeText.text = text;
        collectedDiamonds++;
        if (collectedDiamonds == allDiamonds.Length || GetComponentInParent<Game>().GetIsTrainingOn())
        {
            //AchievedTimeText.text = AchievedTimeText.text + "\nTotal time: " + metrics.UpdateElapsedTime();
            AchievedTimeText.text = AchievedTimeText.text + "\nTotal time: " + metrics.GetBiggestDiamondAcquisitionTime();
            collectedDiamonds = 0;
        }


         //we will need to rewrite the text
            
            Dictionary<Diamond, System.TimeSpan> reachiongDiamondsTotalElapsedTimes = metrics.GetElapsedTimes();
            Dictionary<Diamond, float> diamondsAquisitionRate = metrics.GetDiamondAquisitionRate();

            foreach (KeyValuePair<Diamond, System.TimeSpan> pair in reachiongDiamondsTotalElapsedTimes)
            {
                AchievedTimeText.text = AchievedTimeText.text + "\n" + pair.Key.name + " " + pair.Value;

                //update the percentage of chances that AI reaches the diamond in UI text
                //maybe later
                //if (diamondsAquisitionRate.ContainsKey(pair.Key) && GetComponentInParent<Game>().GetIsTrainingOn())
                //    AchievedTimeText.text = AchievedTimeText.text + ", acquisition rate: " + diamondsAquisitionRate[pair.Key]*100 + "%";
                
            }

        //if there is end of game screen, update it as well
        if (AchievedTimeTextEndOfGame)
            AchievedTimeTextEndOfGame.text = AchievedTimeText.text;

    }

    public void UpdateAchievedTimeTextCustom(string str)
    {
        AchievedTimeText = GetComponent<TextMeshProUGUI>();
        if (GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>())
            AchievedTimeText.text = str + GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateElapsedTime().Minutes + " minutes and "+GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateElapsedTime().Seconds + " seconds";
    }
}
