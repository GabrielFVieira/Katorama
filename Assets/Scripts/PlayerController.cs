using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float gravity = -12;
    public float jumHeight = 1;
    [Range(0, 1)]
    public float airControllerPercent;
    private Animator anim;
    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float curSpeed;
    private float velY;
    bool running;
    int punchIndex;
    private CharacterController controller;
    private Transform cameraT;
    bool canMove = true;
	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
        cameraT = Camera.main.transform;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir, running);

        if(!canMove)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && !anim.IsInTransition(0))
            {
                canMove = true;
                anim.applyRootMotion = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && canMove)
        {
            anim.applyRootMotion = true;
            anim.SetTrigger("Dance");
            canMove = false;
            curSpeed = 0;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && canMove && controller.isGrounded)
        {
            //anim.applyRootMotion = true;


            if (Input.GetAxis("Horizontal") != 0)
            {
                anim.SetFloat("PunchSide", 1);

                if (Input.GetAxis("Horizontal") > 0)
                {
                    anim.SetFloat("PunchIndex", 0);
                }

                else
                    anim.SetFloat("PunchIndex", 1);
            }

            else
            {
                anim.SetFloat("PunchIndex", punchIndex);
                anim.SetFloat("PunchSide", 0);
            }

            anim.SetTrigger("Punch");

            if (punchIndex == 0)
                punchIndex = 1;

            else
                punchIndex = 0;

            canMove = false;
            curSpeed = 0;
        } 

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        float animSpeedPercent = ((running) ? curSpeed / runSpeed : curSpeed / walkSpeed * 0.5f);
        anim.SetFloat("SpeedPercent", animSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void Move(Vector2 inputDir, bool running)
    {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            if (canMove)
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        curSpeed = Mathf.SmoothDamp(curSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * curSpeed + Vector3.up * velY;

        Debug.Log(velocity);

        if (canMove)
            controller.Move(velocity * Time.deltaTime);

        curSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            velY = 0;
            anim.SetBool("Jump", false);
        }
    }

    void Jump()
    {
        if(controller.isGrounded)
        {
            float jumpVel;
            if (running)
                jumpVel = Mathf.Sqrt(-2 * gravity * jumHeight);

            else
                jumpVel = Mathf.Sqrt(-2 * gravity * (jumHeight * 0.6f));

            velY = jumpVel;
            anim.SetBool("Jump", true);
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
            return smoothTime;

        if (airControllerPercent == 0)
            return float.MaxValue;

        return smoothTime/airControllerPercent;
    }
}
