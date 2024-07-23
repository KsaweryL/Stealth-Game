using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    bool isMovingForward;
    bool isSneaking;
    bool isRunning;
    bool jumpPressed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isMovingForward = animator.GetBool("isMovingForward");
        isSneaking = animator.GetBool("isSneaking");
        isRunning = animator.GetBool("isRunning");
        jumpPressed = animator.GetBool("jumpPressed");

        //initially, there is no jump - toDO - to fix
        animator.SetBool("jumpPressed", false);
        jumpPressed = false;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
          Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            animator.SetBool("isMovingForward", true);
            isMovingForward = true;
        }
        else
        {
            animator.SetBool("isMovingForward", false);
            isMovingForward = false;
        }

        //standing to sneaking and vice versa
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isSneaking)
            {
                animator.SetBool("isSneaking", false);
                isSneaking = false;
            }
            else
            {
                animator.SetBool("isSneaking", true);
                isSneaking = true;
            }

        }

        //walking/crouching to running
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            if (isRunning)
            {
                animator.SetBool("isRunning", false);
                isRunning = false;
            }
            else
            {
                animator.SetBool("isRunning", true);
                isRunning = true;
            }

        }

        //jumping
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetBool("jumpPressed", true);
            jumpPressed = true;
        }
        
    }
}
