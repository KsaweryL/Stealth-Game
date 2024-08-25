using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInParent<Game>().GetCamera().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //first camera does the movement, the update happens
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
