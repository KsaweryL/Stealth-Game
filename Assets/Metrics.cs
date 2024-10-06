using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metrics : MonoBehaviour
{
    //related to passed time
    private System.DateTime startTime;
    private System.DateTime endOfGameTime;
    private System.TimeSpan totalElapsedTime;

    public System.TimeSpan GetElapsedTime()
    {
        endOfGameTime = System.DateTime.UtcNow;
        totalElapsedTime = endOfGameTime - startTime;

        return totalElapsedTime;
    }

    

}
