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


    public void UpdateMovement(float horizontal, float vertical, bool jump, bool sprint, bool sneakingButton)
    {
        Debug.Log("Animation");

        isMovingForward = animator.GetBool("isMovingForward");
        isSneaking = animator.GetBool("isSneaking");
        isRunning = animator.GetBool("isRunning");
        jumpPressed = animator.GetBool("jumpPressed");

        animator.SetBool("jumpPressed", false);
        jumpPressed = false;

        if (horizontal != 0 || vertical != 0)
        {
            animator.SetBool("isMovingForward", true);
            isMovingForward = true;
        }
        else
        {
            animator.SetBool("isMovingForward", false);
            isMovingForward = false;
        }

        //standing to sneaking and vice versa
        if (sneakingButton)
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
        if (sprint)
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
        if (jump)
        {
            animator.SetBool("jumpPressed", true);
            jumpPressed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
