using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    Animator animator;

    //for moving speed
    private float speed;
    [Header("Speed values")]
    public float sprintingSpeed;
    public float walkingSpeed;
    public float crouchingSpeed;
    public float crouchingSprintingSpeed;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;

    [Header("Jumping")]
    //jumping speed
    public float jumpSpeed;
    private float ySpeed;

    [Header("Mlagent")]
    public bool turnOffControler;

    [Header("Other")]
    public bool isSprintEnabled = false;

    

    //whether we crouch or not
    bool isSneaking;

    //for smoother direction transition
    public float turnSmoothTime = 0.1f;
    public float turnSMoothVelocity;

    //for gravity
    [Header("gravity")]
    public float gravity = -9.81f;
    public float gravityMultiplier = 2.2f;
    public Vector3 velocity;
    public Vector3 moveDir;

    public void ResetCurrentHealth()
    {
        currentHealth = maxHealth;
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public bool GetIsSneaking()
    {
        return isSneaking;
    }
    public float GetSpeed()
    {
        return speed;
    }

    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }

    public Vector3 GetMoveDir()
    {
        return moveDir;
    }

    public bool IsSprintEnabled()
    {
        return isSprintEnabled;
    }
    public void TakeDamage(float damage)
    {
        //inform mlagent about the current health
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if(GetComponentInParent<Game>().GetIsTrainingOn())
                GetComponent<MLPlayerAgent>().PlayerHasLost();
        }
        else
            if (GetComponentInParent<Game>().GetIsTrainingOn())
                GetComponent<MLPlayerAgent>().DamageWasTaken();
    }

    public float ApplyGravity(CharacterController controller, Vector3 moveDirVariable, float ySpeedVariable, float time)
    {
        if (controller)
        {
            if (controller.isGrounded && moveDirVariable.y < 0.0f)
                ySpeedVariable = -1.0f;
            else
                ySpeedVariable += gravity * gravityMultiplier * time;
        }

        return ySpeedVariable;
    }

    public Vector3 ApplyRotation(Transform objectTransform, float horizontal, float vertical, bool enableCam)
    {
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;


        //if there is any movement on x/z axes
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (enableCam)
                targetAngle += cam.eulerAngles.y;
            //an angle with smoother transition
            float angle = Mathf.SmoothDampAngle(objectTransform.eulerAngles.y, targetAngle, ref turnSMoothVelocity, turnSmoothTime);
            objectTransform.rotation = Quaternion.Euler(0f, angle, 0f);      //we rotate our object around our target angle

            //quaterion.euler returns a proper quaterion (w,x,y,z) and to assing this rortation to our vector, we need to change vector to quaterion
            //( (in this case 0,0,1 to (0,0,0,1) (w,z,y,z) and do v' = q*v*q^-1, the quaterion is then translated to vector again. All of this is automacially
            //done by unity
            Vector3 quaterion = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir.x = quaterion.x;
            moveDir.z = quaterion.z;
        }

        return moveDir;
    }
    public void ApplyMovement(float horizontal, float vertical, bool jump, bool sprint, bool sneakingButton, bool enableCam, float speedMultiplier, bool sprintByHolding)
    {
        //reset x and z movement
        moveDir.x = 0f;
        moveDir.z = 0f;

        ApplyRotation(transform, horizontal, vertical, enableCam);


        //jumping
        //jump only when one is not sneaking
        bool shouldGroundedBeIncluded = false;
        if(GetComponentInParent<Game>().GetIsTrainingOn())
            shouldGroundedBeIncluded = true;

        if (jump && !isSneaking && (controller.isGrounded))
        {

            ySpeed += jumpSpeed;

            // Set the isJumping parameter in the Animator
            if (animator)
                animator.SetBool("jumpPressed", true);
        }

        //sneaking
        if (sneakingButton)
        {
            //sneaking is being turned off
            if (isSneaking)
            {
                if(isSprintEnabled)
                    speed = sprintingSpeed;
                else
                    speed = walkingSpeed;

                isSneaking = false;
            }
            //sneaking is being turned on
            else
            {
                if (isSprintEnabled)
                    speed = crouchingSprintingSpeed;
                else
                    speed = crouchingSpeed;

                isSneaking = true;
            }

        }

        //sprinting
        if (sprint) {

            //if sprint by pressing is enables
            if (sprintByHolding == false)
            {
                //sprinting is being turned off
                if (isSprintEnabled)
                {
                    if (isSneaking)
                        speed = crouchingSpeed;
                    else
                        speed = walkingSpeed;
                    isSprintEnabled = false;
                }
                //sprinting is being turned on
                else
                {
                    if (isSneaking)
                        speed = crouchingSprintingSpeed;
                    else
                        speed = sprintingSpeed;

                    isSprintEnabled = true;
                }
            }
            else
            {
                if (isSneaking)
                    speed = crouchingSprintingSpeed;
                else
                    speed = sprintingSpeed;

                isSprintEnabled = true;
            }
                
        }
        else if (sprintByHolding)
        {
            if (isSneaking)
                speed = crouchingSpeed;
            else
                speed = walkingSpeed;

            isSprintEnabled = false;
        }

        moveDir.y = ySpeed;

        //apply gravity
        ySpeed = ApplyGravity(controller, moveDir, ySpeed, Time.deltaTime);

        //apply final movement
        if (controller && !turnOffControler)
            controller.Move(moveDir.normalized * speed * speedMultiplier * Time.deltaTime);


        GetComponentInChildren<AnimationStateController>().UpdateMovement(horizontal, vertical, jump, sprint, sneakingButton, sprintByHolding);

    }

    void SetStandardSpeedValues()
    {
        if (walkingSpeed == 0)
            walkingSpeed = 4.0f;
        if (sprintingSpeed == 0)
            sprintingSpeed = 2.5f*walkingSpeed;
        if (crouchingSpeed == 0)
            crouchingSpeed = 0.85f * walkingSpeed;
        if (crouchingSprintingSpeed == 0)
            crouchingSprintingSpeed = 1.25f * walkingSpeed;

        speed = walkingSpeed;

        if (jumpSpeed == 0)
            jumpSpeed = 3;

        currentHealth = maxHealth;

    }

    // Start is called before the first frame update
    void Start()
    {
        isSneaking = false;

        SetStandardSpeedValues();

        if (!GetComponentInParent<Game>().GetIsTrainingOn())
        {
            //if the training is off, initiate the timer here
            if (GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>())
            {
                GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateInitialValues();
                GetComponentInParent<Game>().GetPlayer().GetComponent<Metrics>().UpdateStartTime();
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //for rotation
        //between -1 and 1
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool sprint = Input.GetKey(KeyCode.LeftShift);      //only when holding
        //bool sneakingButton = Input.GetKeyDown(KeyCode.LeftControl);
        bool sneakingButton = Input.GetKeyDown(KeyCode.E);

        //apply sound
        if (!GetComponentInParent<Game>().GetIsTrainingOn())
        {
            SoundFXManager.instance.ApplyRunningSound(horizontal, vertical, isSprintEnabled, isSneaking, ySpeed, GetComponentInChildren<RunningAudioSource>().GetComponent<AudioSource>(), true);
            SoundFXManager.instance.ApplySneakingSound(horizontal, vertical, isSprintEnabled, isSneaking, ySpeed, GetComponentInChildren<SneakingAudioSource>().GetComponent<AudioSource>());
            SoundFXManager.instance.ApplySneakingRunningSound(horizontal, vertical, isSprintEnabled, isSneaking, ySpeed, GetComponentInChildren<SneakingRunningAudioSource>().GetComponent<AudioSource>());
            SoundFXManager.instance.ApplyWalkingSound(horizontal, vertical, isSneaking, isSprintEnabled, ySpeed, GetComponentInChildren<WalkingAudioSource>().GetComponent<AudioSource>(), true);
            SoundFXManager.instance.ApplyJumpingSound(jump, GetComponentInChildren<JumpingAudioSource>().GetComponent<AudioSource>(), controller.isGrounded);
        }
        //apply movement only when game is not paused
        if (!GetComponentInParent<Game>().GetIsPauseMenuOn() && !GetComponentInParent<GameOver>().GetGameOver())
            ApplyMovement(horizontal, vertical, jump, sprint, sneakingButton, true, 1f, true);

        


    }
}
