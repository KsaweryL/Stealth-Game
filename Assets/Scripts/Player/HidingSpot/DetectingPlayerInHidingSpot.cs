using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectingPlayerInHidingSpot : MonoBehaviour
{

    public enum Layers {Hiding_spot = 10 };

    //for hiding
    [Header("Hiding mechanics")]
    public bool playerIsTouchingHidingSpot;
    public bool playerIsHidden;
    public int hidingSpot;
    public bool isSneaking;

    public bool IsPlayerHidden()
    {
        return playerIsHidden;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == hidingSpot)
        {
            //Debug.Log(other.gameObject.layer);
            playerIsTouchingHidingSpot = true;

            //visual indication to the player - increase the light of the hiding spot
            if (GetComponent<ThirdPersonMovement>().GetIsSneaking())
                other.gameObject.GetComponentInChildren<Light>().intensity = 1;
            else
                other.gameObject.GetComponentInChildren<Light>().intensity = 0.7f;
        }
        else
        {
            playerIsTouchingHidingSpot = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == hidingSpot)
        {
            playerIsTouchingHidingSpot = false;
            other.gameObject.GetComponentInChildren<Light>().intensity = 0.7f;
        }
    }

    private void UpdatePlayerTouchingHidingSpotStatus(bool touchingHidingSPotStatus)
    {
        playerIsTouchingHidingSpot = touchingHidingSPotStatus;
    }

    void UpdatePlayerHidingState()
    {
        isSneaking = GetComponent<ThirdPersonMovement>().GetIsSneaking();

        if (playerIsTouchingHidingSpot && isSneaking)
        {
            playerIsHidden = true;
            
        }
        else
            playerIsHidden = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        hidingSpot = LayerMask.NameToLayer("hidingSpot");
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHidingState();
    }
}
