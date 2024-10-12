using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    //anyone can access the value of this variable, but only object of this class can change it
    public int NumberOfDiamonds { get; private set; }

    public UnityEvent<PlayerInventory> OnDiamondCollected;

    public void ResetProperties()
    {
        NumberOfDiamonds = 0;
    }
    public void DiamondCollected()
    {
        NumberOfDiamonds++;

        if (GetComponentInParent<Game>().GetIsTrainingOn())
            GetComponent<MLPlayerAgent>().DiamondWasCollected();

        GetComponentInParent<Game>().UpdateCollectedDiamonds();


        //subscriber publisher type of function
        //in this case, should be assigned to the text in the main UI
        OnDiamondCollected.Invoke(this);
    }

    void Start()
    {
        NumberOfDiamonds = 0;
    }
}
