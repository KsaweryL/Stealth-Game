using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public void ResetProperties()
    {
        //toDO - Issue
        Debug.Log("Calling ResetProperties on " + gameObject.name);
        Debug.Log("Before SetActive(true): " + gameObject.activeSelf);
        Debug.Log("Is Diamond active in hierarchy? " + gameObject.activeInHierarchy);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Debug.Log("Renderer enabled: " + renderer.enabled);
        }
        else
        {
            Debug.Log("No Renderer component found on the Diamond.");
        }

        gameObject.SetActive(true);

        Debug.Log("After SetActive(true): " + gameObject.activeSelf);
        Debug.Log("Parent active state: " + (transform.parent != null ? transform.parent.gameObject.activeSelf.ToString() : "No parent"));
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
