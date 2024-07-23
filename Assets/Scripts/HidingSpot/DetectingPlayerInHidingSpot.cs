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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == hidingSpot)
        {
            //Debug.Log(other.gameObject.layer);
            playerIsTouchingHidingSpot = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == hidingSpot)
        {
            playerIsTouchingHidingSpot = false;
        }
    }

    private void UpdatePlayerTouchingHidingSpotStatus(bool touchingHidingSPotStatus)
    {
        playerIsTouchingHidingSpot = touchingHidingSPotStatus;
    }

    void UpdatePlayerHidingState()
    {
        isSneaking = FindObjectOfType<ThirdPersonMovement>().IsSneaking();

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
