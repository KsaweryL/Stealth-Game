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
        //subscriber publisher type of function
        OnDiamondCollected.Invoke(this);
    }

    void Start()
    {
        NumberOfDiamonds = 0;
    }
}
