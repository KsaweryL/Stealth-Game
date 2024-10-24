using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metrics : MonoBehaviour
{
    //related to passed time
    private System.DateTime startTime;
    private System.DateTime endOfGameTime;
    private System.TimeSpan totalElapsedTime;

    //related to each diamond
    Diamond[] allDiamonds;
    Dictionary<Diamond, System.DateTime> reachiongDiamondsStartTimes;
    Dictionary<Diamond, System.DateTime> reachiongDiamondsEndTimes;
    Dictionary<Diamond, System.TimeSpan> reachiongDiamondsTotalElapsedTimes;


    private void Start()
    {
        startTime = System.DateTime.UtcNow;
    }

    public Dictionary<Diamond, System.TimeSpan> GetElapsedTimes()
    {
        return reachiongDiamondsTotalElapsedTimes;
    }

    public System.TimeSpan GetTotalElapsedTime()
    {
        return totalElapsedTime;
    }

    public void UpdateStartTime()
    {
        startTime = System.DateTime.UtcNow;
        reachiongDiamondsStartTimes = new Dictionary<Diamond, System.DateTime>();

        foreach (Diamond diamond in allDiamonds)
            reachiongDiamondsStartTimes.Add(diamond, startTime);

        //Debug.Log("start initial: " + startTime);

    }

    public void UpdateInitialValues()
    {
        allDiamonds = GetComponentInParent<Game>().GetDiamonds();
        reachiongDiamondsEndTimes = new Dictionary<Diamond, System.DateTime>();
        reachiongDiamondsTotalElapsedTimes = new Dictionary<Diamond, System.TimeSpan>();
        
    }

    public System.TimeSpan UpdateTimeOnDiamond(Diamond diamond)
    {
        System.DateTime currentTime = System.DateTime.UtcNow;
        if (reachiongDiamondsTotalElapsedTimes.ContainsKey(diamond))
        {
            reachiongDiamondsEndTimes[diamond] = currentTime;
            reachiongDiamondsTotalElapsedTimes[diamond] = currentTime - startTime;
        }
        else
        {
            reachiongDiamondsEndTimes.Add(diamond, currentTime);
            reachiongDiamondsTotalElapsedTimes.Add(diamond, currentTime - startTime);
        }

        //Debug.Log("Current: " + currentTime + " \nstart: " + startTime);
        return currentTime - startTime;
    }
    public System.TimeSpan UpdateElapsedTime()
    {
        endOfGameTime = System.DateTime.UtcNow;
        totalElapsedTime = endOfGameTime - startTime;

        return totalElapsedTime;
    }

    

}
