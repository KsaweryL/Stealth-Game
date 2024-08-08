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

    [Header("Other")]
    public bool isSprintEnabled = false;

    //whether we crouch or not
    bool isSneaking;

    //for smoother direction transition
    public float turnSmoothTime = 0.1f;
    public float turnSMoothVelocity;

    //for gravity
    private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
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

        }
        else
            GetComponent<MLPlayerAgent>().DamageWasTaken();
    }

    void ApplyGravity()
    {
        if (controller)
        {
            if (controller.isGrounded && moveDir.y < 0.0f)
                ySpeed = -1.0f;
            else
                ySpeed += gravity * Time.deltaTime;
        }

    }

    void ApplyMovement()
    {

        //for rotation
        //between -1 and 1
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //reset x and z movement
        moveDir.x = 0f;
        moveDir.z = 0f;


        //if there is any movement on x/z axes
        if (direction.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //an angle with smoother transition
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSMoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);      //we rotate our object around our target angle

            //quaterion.euler returns a proper quaterion (w,x,y,z) and to assing this rortation to our vector, we need to change vector to quaterion
            //( (in this case 0,0,1 to (0,0,0,1) (w,z,y,z) and do v' = q*v*q^-1, the quaterion is then translated to vector again. All of this is automacially
            //done by unity
            Vector3 quaterion = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir.x = quaterion.x;
            moveDir.z = quaterion.z;
            

        }

 
        //jumping
        //jump only when one is not sneaking
        if (Input.GetButtonDown("Jump") && !isSneaking)
        {

            ySpeed = jumpSpeed;

            // Set the isJumping parameter in the Animator
            if (animator)
                animator.SetBool("jumpPressed", true);
        }

        //sneaking
        if (Input.GetKeyDown(KeyCode.LeftControl))
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
        if (Input.GetKeyDown(KeyCode.LeftShift)) {

            //sprinting is being turned off
            if (isSprintEnabled) {
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

        moveDir.y = ySpeed;

    }

    void SetStandardSpeedValues()
    {
        if (sprintingSpeed == 0)
            sprintingSpeed = 8.0f;
        if (walkingSpeed == 0)
            walkingSpeed = 4.0f;
        if (crouchingSpeed == 0)
            crouchingSpeed = 3.0f;
        if (crouchingSprintingSpeed == 0)
            crouchingSprintingSpeed = 5.0f;

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
    }


    // Update is called once per frame
    void Update()
    {
        //apply movement
        ApplyMovement();

        //apply gravity
        ApplyGravity();

        //apply movement
        if (controller)
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        

    }
}
