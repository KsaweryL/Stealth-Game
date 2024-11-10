using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    //related to % of obtaining the diamond
    Dictionary<Diamond, float> diamondAquisitionRate;
    Dictionary<Diamond, int> howManyTimesWasDiamondObtained;
    int round;


    private void Start()
    {
        startTime = System.DateTime.UtcNow;
        diamondAquisitionRate = new Dictionary<Diamond, float>();
        howManyTimesWasDiamondObtained = new Dictionary<Diamond, int>();
        round = 1;
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
        
        if (diamondAquisitionRate.ContainsKey(diamond))
        {
            //we will update the time only if its better
            if ((currentTime - startTime) < reachiongDiamondsTotalElapsedTimes[diamond])
            {
                reachiongDiamondsEndTimes[diamond] = currentTime;
                reachiongDiamondsTotalElapsedTimes[diamond] = currentTime - startTime;
            }
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

    public System.TimeSpan GetBiggestDiamondAcquisitionTime()
    {
        return reachiongDiamondsTotalElapsedTimes.Values.Max();
    }

    public float UpdateDiamondAquisitionRate(Diamond diamond, bool updateDiamondsCounter)
    {
        float diamondAquisitionRateVar = 0.0f;

        if (diamondAquisitionRate.ContainsKey(diamond))
        {
            if(updateDiamondsCounter)
                howManyTimesWasDiamondObtained[diamond] += 1;
            diamondAquisitionRate[diamond] = (float) howManyTimesWasDiamondObtained[diamond] / round;
            
        }
        else
        {   
            if(updateDiamondsCounter == false)
                return diamondAquisitionRateVar;

            howManyTimesWasDiamondObtained.Add(diamond, 1);
            diamondAquisitionRate.Add(diamond, (float) 1 /round);
        }

        diamondAquisitionRateVar = diamondAquisitionRate[diamond];

        return diamondAquisitionRateVar;
    }

    public Dictionary<Diamond, float> GetDiamondAquisitionRate()
    {
        return diamondAquisitionRate;
    }

    public void UpdateTheround()
    {
        UpdateAllDiamondAcquisitionRates();

        round++;
        
    }

    public void UpdateAllDiamondAcquisitionRates()
    {
        //update all diamonds
        //we can't do it directly, we need to copy the dictionary
        Dictionary<Diamond, float> diamondAquisitionRateCopy = new Dictionary<Diamond, float>(diamondAquisitionRate);

        foreach (KeyValuePair<Diamond, float> pair in diamondAquisitionRateCopy)
        {
            UpdateDiamondAquisitionRate(pair.Key, false);
        }
    }

}
