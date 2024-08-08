using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageOnObjectsPlayer : MonoBehaviour
{
    public int waterLayer;

    private void OnTriggerEnter(Collider other)
    {
        //kill a acharater whenever water is touched
        if (other.gameObject.layer == waterLayer)
        {
            GetComponent<ThirdPersonMovement>().TakeDamage(100);
        }
    }

    void Start()
    {
        waterLayer = LayerMask.NameToLayer("water");
    }
}
