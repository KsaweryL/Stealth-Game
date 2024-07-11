using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera_old : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;       //transform is for position, rotation and scale of the object
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rorationSpeed;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //we need to find forward
        //transfrom in this case is a camera??
        Vector3 viewDir = player.position - new Vector3(transform.position.x, transform.position.y, transform.position.z);                        //Representation of 3D vectors and points
        orientation.forward = viewDir.normalized;

        //rotation player object
        float horizontalInput = Input.GetAxis("Horizontal");        //gets the input
        float verticalInput = Input.GetAxis("Vertical");
        //knowing where the "forward" is, defined by the camera earlier, moves by this direction using a player input
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
            //interpolates between 2 vectors in a particular time
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rorationSpeed);
    }
}
