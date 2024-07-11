using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    //for moving speed
    public float speed;
    public bool isSprintEnabled = false;

    //jumping speed
    public float jumpSpeed = 15;

    //for smoother direction transition
    public float turnSmoothTime = 0.1f;
    public float turnSMoothVelocity;

    //for gravity
    private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    public Vector3 velocity;
    public Vector3 moveDir;

    void ApplyGravity()
    {

        if (controller.isGrounded && moveDir.y < 0.0f && (!Input.GetKey(KeyCode.W) || !Input.GetKey(KeyCode.A) ||
          !Input.GetKey(KeyCode.S) || !Input.GetKey(KeyCode.D)))
            moveDir.y = -1.0f;
        //when we move our character in the air, increase the gravity
        else if (!controller.isGrounded && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
          Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            moveDir.y += gravity * gravityMultiplier * 10 * Time.deltaTime;
        }
        else
            moveDir.y += gravity * gravityMultiplier * Time.deltaTime;

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
        if (Input.GetButtonDown("Jump"))
        {
            //if we jumped during movement, make a jump more impactful
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                moveDir.y += jumpSpeed*10;
            else
                moveDir.y += jumpSpeed;
        }

        //sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift)) {

            if (isSprintEnabled) {
                speed = 4.0f;
                isSprintEnabled = false;
            }
            else
            {
                speed = 6.0f;
                isSprintEnabled = true;
            }
                
        }
            

    }

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        //apply movement
        ApplyMovement();

        //apply gravity
        ApplyGravity();

        //apply movement
        controller.Move(moveDir.normalized * speed * Time.deltaTime);

    }
}
