using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        //it means that if it shoudl be executed if the collided object has the PlayerInventory component
        //aka only player has it
        if(playerInventory != null)
        {
            playerInventory.DiamondCollected();
            gameObject.SetActive(false);
        }
    }

}
