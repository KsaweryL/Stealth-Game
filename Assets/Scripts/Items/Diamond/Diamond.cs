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
            if (!GetComponentInParent<Game>().GetIsTrainingOn())
            {
                gameObject.SetActive(false);
            }
            else
                transform.localPosition = new Vector3(Random.Range(-3.0f, 3.0f), 1.5f, Random.Range(-3.0f, 3.0f));

        }
    }

}
