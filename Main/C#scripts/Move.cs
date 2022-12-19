using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed;
    float OrginalSpeed;


    public float playerHeight;
    public LayerMask GroundType;
    public bool isgrounded;

    public float groundDrag;

    
    Vector3 velocity;
    float jumpCD;
    public float jumpCDMax;
    bool readyToJump;
    public float jumpForce;


    float headbobx;
    float headboby;
    public GameObject cam;
    Vector3 camResetPos;
    public Transform CamHolder;
    public Camera playerFPScam;


    float SprintingSpeed;
    float SprintHeadBob;
    bool isSprinting;


    public GameObject me;
    public float Crouching;
    bool isCrouching;

    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    bool exitingSlope;

    public LayerMask Wall;
    public float wallrunForce;
    public float wallrunSpeed;
    public float maxWallrunTime;
    float wallrunTimer, wallrunCD;
    public float wallCheckDistance;
    public float minJumpHeight;
    public bool isWallrunning;
    RaycastHit leftwallHit, rightwallHit;
    bool wallLeft, wallRight;
    public float wr_climbspeed;
    public float wr_zRotation;
    public float wj_upForce;
    public float wj_sideForce;

    public float climbSpeed;
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    float wallLookAngle;
    RaycastHit frontWallHit;
    bool isWallFront;
    public float cj_upForce;
    public float cj_backForce;
    public bool isWallClimbing;



    public Transform orientation;

    float hInput;
    float vInput;

    Vector3 moveDirection;

    Rigidbody rb;



    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        readyToJump = true;
        camResetPos = cam.transform.localPosition;
        isSprinting = false;
        OrginalSpeed = speed;
        exitingSlope = false;
        wallrunSpeed = speed;
    }

    void FixedUpdate()
    {
        jumpCD += 1f;
        speed = OrginalSpeed;

        if (isCrouching == true)
        {
            speed *= Crouching;
        }
        if(OnSlope() == true)
        {
            speed *= 0.8f;
        }



        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * vInput * SprintingSpeed + orientation.right * hInput;      
        SpeedControl();


        isgrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);

        if (isgrounded)     //ground checking
        {
            rb.drag = groundDrag;

            rb.AddForce(moveDirection.normalized * speed * 10f , ForceMode.Force);

            if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            rb.drag = 0;

            rb.AddForce(moveDirection.normalized * speed * 3f , ForceMode.Force);
        }


        rb.useGravity = !OnSlope();

        



        if(jumpCD >= jumpCDMax)
        {
            readyToJump = true;
            exitingSlope = false;
        }
            

        if (Input.GetKey(KeyCode.Space) && isgrounded && readyToJump == true)       //jump
        {
            readyToJump = false;
            jumpCD = 0f;
            exitingSlope = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);


        }

        if (OnSlope() && !exitingSlope)     //OnSlope
        {
            rb.AddForce(GetSlopeMoveDirection() * speed * 10f, ForceMode.Force);

            if (rb.velocity.y < 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); 

        }

        //Debug.Log(rb.velocity.y);

        if (Input.GetKey(KeyCode.LeftShift) )        //Sprinting
        {
            isSprinting = true;  
        }
        if(vInput == 0)
        {
            isSprinting = false;
        }


        if (isSprinting && !isCrouching  && (vInput != 0 || hInput != 0))
        {
            if (playerFPScam.fieldOfView < 75)
                playerFPScam.fieldOfView += 3f;

            SprintingSpeed = 1.8f;
            SprintHeadBob = 1.8f;
        }
        else
        {
            if (playerFPScam.fieldOfView > 60)
                playerFPScam.fieldOfView -= 3f;

            SprintingSpeed = 1;
            SprintHeadBob = 1;
        }



        if (Input.GetKey(KeyCode.LeftControl) && isgrounded && !isWallrunning)      //Crouching
        {
            me.transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);           
            if(isCrouching == false)
                me.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, transform.localPosition.z);
             
            isCrouching = true;
        }
        else
        {
            me.transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);

            if(isCrouching == true)
            {
                me.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, transform.localPosition.z);
                isCrouching = false;
            }
            
        }

        wallClimbableCheck();
        if (isWallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)       //wallClimbing
        {
            isWallClimbing = true;

            rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
            if (Input.GetKey(KeyCode.Space))
            {
                isWallClimbing = false;

                Vector3 cj_forceToApply = transform.up * cj_upForce + frontWallHit.normal * cj_backForce;

                rb.velocity = new Vector3(rb.velocity.z, 0f, rb.velocity.z);
                rb.AddForce(cj_forceToApply, ForceMode.Impulse);
            }

        }
        else
        {
            if(isgrounded)
                isWallClimbing = false;
        }

        CheckForWall();     //Wall Running 
        if((wallLeft || wallRight) && vInput > 0 && AboveGround() && isSprinting && !isCrouching && wallrunTimer > 0 && !isWallClimbing)
        {
            isWallrunning = true;
            rb.useGravity = false;
            wallrunTimer -= Time.deltaTime;
            wallrunCD = 0f;
            
            
            if (Input.GetKey(KeyCode.CapsLock))
            {
                rb.velocity = new Vector3(rb.velocity.x, wr_climbspeed, rb.velocity.z);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                rb.velocity = new Vector3(rb.velocity.x, -wr_climbspeed, rb.velocity.z);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }

            Vector3 wallNormal = wallRight ? rightwallHit.normal : leftwallHit.normal;
            
                                                   

            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
                wallForward = -wallForward;

            rb.AddForce(wallForward * wallrunForce  , ForceMode.Force);

            if(!(wallLeft && hInput > 0) || !(wallRight && hInput < 0))
                rb.AddForce(-wallNormal * 200 , ForceMode.Force);

            wr_zRotation = wallRight ? 15f : -15f;



            Vector3 forceToApply = transform.up * wj_upForce + wallNormal * wj_sideForce;

            
            if (Input.GetKey(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(forceToApply, ForceMode.Impulse);
            }
        }
        else
        {
            rb.useGravity = true;
            
            
            
            isWallrunning = false;
            wr_zRotation = 0f;

            wallrunCD += Time.deltaTime;
            if(wallrunCD >= 2f)
                wallrunTimer = maxWallrunTime;
        }







        if ((vInput != 0 || hInput != 0) && (isgrounded || isWallrunning) && isCrouching == false)       //head bobing
        {
            
            headbobx = Mathf.Sin(Time.time * 8 * SprintHeadBob) * 0.1f;
            headboby = Mathf.Cos(Time.time * 16 * SprintHeadBob) * 0.05f + 0.85f ;

            //cam.transform.localPosition = CamHolder.right * headbobx + CamHolder.up * headboby;
            cam.transform.localPosition = new Vector3(headbobx,headboby,0f);
        }
        else
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camResetPos, 0.5f * Time.time);
        }

    }

    void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;


        }
        else
        {
            Vector3 flatVal = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVal.magnitude > speed && isSprinting == false)
            {
                Vector3 limitedVel = flatVal.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);

            }
            else if (flatVal.magnitude > speed && isSprinting == true)
            {
                Vector3 limitedVel = flatVal.normalized * speed * SprintingSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);


            }

        }
        

    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit , playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.localPosition, orientation.right, out rightwallHit, wallCheckDistance, Wall);
        wallLeft = Physics.Raycast(transform.localPosition, -orientation.right, out leftwallHit, wallCheckDistance, Wall);
    }
    
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.localPosition, Vector3.down, minJumpHeight, GroundType);
    }
    private void wallClimbableCheck()
    {
        isWallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit ,detectionLength, Wall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
    }
    


     
}
