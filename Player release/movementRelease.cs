using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementRelease : MonoBehaviour
{
    //Objects in Pc
    [SerializeField] GameObject GroundCheckerFront;
    [SerializeField] GameObject GroundCheckerBack;
    [SerializeField] GameObject GroundCheckerRight;
    [SerializeField] GameObject GroundCheckerLeft;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayMid;
    [SerializeField] GameObject stepRayLower;
    public Transform orientation;


    //step climbing items
    public float stepSmooth = 6f;
    public float stepSmoothWeak = 3f;
    private float RayLowerStep = 0.51f;
    private float RayHighStep = 0.52f;
    private bool isStep;

    //ground layer
    public LayerMask steps;
    public LayerMask Slope;
    public LayerMask whatIsGround;

    //Ray range to detect ground
    private float detectorRange = 0.91f;

    //pc componenets
    private Rigidbody rb;
    private GameObject PCcam;
    private playerStatusRelease MyPS;
    private cameraRelease CamSc;

    //camera    
    private float RotY = 0.0f;
    private float RotX = 0.0f;
    private float RotZ = 0.0f;
    private float sensitivity = 50;
    [HideInInspector]public float sensitivityMultiplier = 1f;

    //Input
    [HideInInspector] public float zInput, xInput;
    [HideInInspector] public float mouseY, mouseX;

    //time
    private float wait = 0.5f;

    //grounded
    private bool isGrounded = false;

    //movement
    private bool isMoving = false;
    private float acceleration = 10f;
    private Vector3 inputVector;
    private Vector3 lastVelocity;

    //jump
    private bool NoJump = false;
    public float AirAcc = 1000.0f;
    private bool isJumping = false;

    private float downPull = -800.0f;
    private bool isSloped = false;

    //test
    //Crouch & Slide
    [HideInInspector] public bool isCrouching = false;
    private CapsuleCollider myCollider;
    [HideInInspector] public bool isSliding = false;
    [HideInInspector] public bool canSlide = true;


    void Start()
    {
        myCollider = GetComponent<CapsuleCollider>();  
        PCcam = GameObject.FindGameObjectWithTag("MainCamera");
        rb = this.GetComponent<Rigidbody>();
        MyPS = this.GetComponent<playerStatusRelease>();
        canSlide = true;
    }

    void Update()
    {
        Inputs();
        CameraInput();
    }

    void FixedUpdate()
    {
        GroundCheck();
        SlopeGround();
        Movement();
        handleCrouch();
        StopSlide();
        CameraMovement();
    }




    public void updateSenMult(float SM)
    {
        sensitivityMultiplier = SM;
    }




    //handle Inputs
    void Inputs()
    {
        //Movement Input check
        zInput = Input.GetAxisRaw("Vertical");
        xInput = Input.GetAxisRaw("Horizontal");
        if (isCrouching == false)
            inputVector = (transform.forward * zInput + transform.right * xInput).normalized * MyPS.MaxSpeed;
        if (isCrouching == true)
            inputVector = (transform.forward * zInput + transform.right * xInput).normalized * (MyPS.MaxSpeed * MyPS.CrouchingSpeedModifier);



        //makes jump happen
        if (Input.GetKeyDown(KeyCode.Space) && NoJump == false && isGrounded == true)
        {
            rb.useGravity = true;
            rb.AddForce(-MyPS.JumpForce * rb.mass * Vector3.down, ForceMode.Impulse);
            isJumping = true;
            //NoJump = true;
        }


        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded == true)
            StartCrouch();
        if(isCrouching == true)
            if (Input.GetKeyUp(KeyCode.LeftControl) || isGrounded == false)
                StopCrouch();
        

        //is player making inputs        
        if (inputVector.x > 0.1 ||
            inputVector.x < -0.1 ||
            inputVector.z > 0.1 ||
            inputVector.z < -0.1 ||
            Input.GetKeyDown(KeyCode.Space))
            isMoving = true;
        else
            isMoving = false;
    }

    //handle camera
    void CameraInput()
    {
        //camera look
        if(PauseMenu.gameIsPaused == false)
        {
            mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensitivityMultiplier;
            mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensitivityMultiplier;
        }

        //camere look
        // Calculations
        RotX -= mouseY;
        RotX = Mathf.Clamp(RotX, -89f, 89f);
        RotY += mouseX;
    }





    void CameraMovement()
    {
        //Perform the rotations
        PCcam.transform.localRotation = Quaternion.Euler(RotX, RotY, RotZ);
        orientation.transform.localRotation = Quaternion.Euler(0, RotY, RotZ);
        rb.MoveRotation(orientation.transform.localRotation);
    }

    public void cameraReloils(float recoilSize)
    {
        RotX -= recoilSize;
        RotY += Random.Range(-recoilSize, recoilSize);
    }

    //crouch
    void StartCrouch()
    {
        isCrouching = true;
    }

    //uncrouch
    void StopCrouch()
    {

        isCrouching = false;
    }

    //handles when to crouch/slide and let go of crouch
    void handleCrouch()
    {
        Vector3 desiredCrouchHeight = new Vector3(0f, 0f ,0f);
        Vector3 CamPos = orientation.localPosition;
        if (isCrouching == true)
        {
            myCollider.height = 1;
            myCollider.center = new Vector3(0, -0.45f, 0);
            desiredCrouchHeight.y = 0.0f;
            if (rb.velocity.magnitude > 8f && canSlide == true && zInput > 0)
            {
                StartSlide();
            }
        }
        else
        {
            myCollider.height = 2;
            myCollider.center = new Vector3(0, 0, 0);
            desiredCrouchHeight.y = 0.85f;
        }

        CamPos.y = Mathf.MoveTowards(CamPos.y, desiredCrouchHeight.y, 10.0f * Time.fixedDeltaTime);
        orientation.localPosition = CamPos;
    }

    //handles ground slide
    void StartSlide()
    {
        canSlide = false;
        isSliding = true;
        rb.AddForce(rb.transform.forward * 10f * rb.mass, ForceMode.Impulse);

        isCrouching = true;
        StartCoroutine(unlockSlide());
    }

    //goes sliding false
    void StopSlide()
    {
        if(isSliding == true)
        {
            if (isGrounded == false || rb.velocity.magnitude < 6f || isCrouching == false)
            {
                isSliding = false;
                
            }
        }
    }

    //delay to stop sliding spam
    IEnumerator unlockSlide()
    {
        yield return new WaitForSecondsRealtime(2f);
        canSlide = true;
    }

    //handles movement
    void Movement()
    {
        //if rigidbody is moving the script can look for steps
        if (isMoving == true)
            stepClimb();

        if (isGrounded == true && isSliding == false)
        {
            //allow you rigidbody to jump again
            isJumping = false;

            //add velocity to the rigidbody
            rb.velocity = Vector3.Lerp(rb.velocity, inputVector, acceleration * Time.fixedDeltaTime);

            //stop character sliding down slopes (likely temporary fix)
            if (isMoving == false)
                rb.useGravity = false;
            else
                rb.useGravity = true;
        }
        else if (isGrounded == false)
        {
            rb.useGravity = true;
            //air movement
            rb.AddForce(Vector3.down * Time.deltaTime * 10);

            //Clamps the maxSpeed of the Z and X axis while in the air
            Vector2 XZ = new Vector2(rb.velocity.x, rb.velocity.z);
            Vector2 XZClamp = Vector2.ClampMagnitude(XZ, MyPS.AirMaxSpeed);
            rb.velocity = new Vector3(XZClamp.x, rb.velocity.y, XZClamp.y);

            //decrease air acceleration in case rigidbody is in a slope
            if (isSloped == false)
            {
                rb.AddForce(rb.transform.forward * zInput * AirAcc * Time.deltaTime);
                rb.AddForce(rb.transform.right * xInput * AirAcc * Time.deltaTime);
            }
            else
            {
                rb.AddForce(rb.transform.forward * zInput * AirAcc / 2 * Time.deltaTime);
                rb.AddForce(rb.transform.right * xInput * AirAcc / 2 * Time.deltaTime);
            }
        }

        if (isSloped == true)
        {
            rb.AddForce(rb.transform.up * downPull * Time.deltaTime);
        }
        lastVelocity = rb.velocity;

    }

    //let player jump
    void UnblockJump()
    {
        NoJump = false;
    }

    //checks too see if the ground type is climbeble
    void SlopeGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(GroundCheckerFront.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, Slope) ||
           Physics.Raycast(GroundCheckerBack.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, Slope) ||
           Physics.Raycast(GroundCheckerLeft.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, Slope) ||
           Physics.Raycast(GroundCheckerRight.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, Slope))
        {
            isSloped = true;
        }
        else
        {
            isSloped = false;
        }
    }

    //check if its walkable
    void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(GroundCheckerFront.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, whatIsGround) ||
            Physics.Raycast(GroundCheckerBack.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, whatIsGround) ||
            Physics.Raycast(GroundCheckerLeft.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, whatIsGround) ||
            Physics.Raycast(GroundCheckerRight.transform.position, transform.TransformDirection(Vector3.down), out hit, detectorRange, whatIsGround))
        {
            isGrounded = true;
        }
        else
            isGrounded = false;
    }

    //check for steps to climb
    void stepsdetector(float x, float y)
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(x, 0, y), out hitLower, RayLowerStep, steps))
        {

            RaycastHit hitMid;
            if (Physics.Raycast(stepRayMid.transform.position, transform.TransformDirection(x, 0, y), out hitMid, RayHighStep, steps))
            {

                RaycastHit hitUpper;
                if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(x, 0, y), out hitUpper, RayHighStep, steps))
                {

                    rb.position -= new Vector3(0f, -stepSmooth * Time.fixedDeltaTime, 0f);
                    rb.velocity = lastVelocity;
                }
            }
            else
            {
                rb.position -= new Vector3(0f, -stepSmoothWeak * Time.fixedDeltaTime, 0f);
                rb.velocity = lastVelocity;
            }
        }
    }

    //direction where the ray in step climb are sended depending on direction input
    void stepClimb()
    {
        //x is 30 deegre = 1f
        //y is 1f = front facing & -1f back facing
        if (zInput > 0)
        {
            //hit front
            stepsdetector(0f, 1f);
            //hit 30 
            stepsdetector(1f, 1f);
            //hit -30
            stepsdetector(-1f, 1f);
        }

        if (zInput < 0)
        {
            //hit back
            stepsdetector(0f, -1f);
            //hit 120
            stepsdetector(1f, -1f);
            //hit -120
            stepsdetector(-1f, -1f);
        }

        if (xInput > 0)
        {
            //hit right
            stepsdetector(3f, 1f);
            //hit 60
            stepsdetector(2f, 1f);
            //hit 150
            stepsdetector(2f, -1f);
        }

        if (xInput < 0)
        {
            //hit left
            stepsdetector(-3f, 1f);

            //hit -60
            stepsdetector(-2f, 1f);

            //hit -150
            stepsdetector(-2f, -1f);
        }

    }

}
