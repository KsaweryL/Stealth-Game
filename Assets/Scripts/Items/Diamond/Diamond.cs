using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public void ResetProperties()
    {
        gameObject.SetActive(true);

    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        //it means that if it shoudl be executed if the collided object has the PlayerInventory component
        //aka only player has it
        if(playerInventory != null)
        {
            //Debug.Log("diamond collected");
            playerInventory.DiamondCollected();
            gameObject.SetActive(false);
            Debug.Log("Diamond should be deactived");
        }
    }

}
