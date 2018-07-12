using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class KaratecaMovement : MonoBehaviour //NetworkBehavior
{
    #region Public Variables
    public float turnSmoothTime = 0.2f;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0, 1)]
    public float airControllerPercent;
    public string[] partsNames;
    public float[] partsDamage;
    public Hit[] partsCol;
    #endregion

    #region Private Variables
    private Animator anim;
    private Camera cam;
    private float turnSmoothVelocity;
    private CharacterController controller;
    private float velY;
    private bool run;
    private bool move;
    private bool jumpPhysicsActive;
    private int attackSide;
    private bool attackChange;
    private bool punch;
    private bool kick;
    private Dictionary<string, Hit> parts = new Dictionary<string, Hit>();
    private Dictionary<string, float> dmg = new Dictionary<string, float>();
    private string attackPart;
    private bool collided;
    #endregion

    public Transform camMiddle;
    public float impactForce = 3;
    public bool canAttack;
    public bool canTurn = true;
    private float curRot;
    private float curRotRef;
    private bool controle;
    private bool block;

    public bool died;
    // Use this for initialization
    void Start()
    {/*
        if (isLocalPlayer)
        {*/
            canTurn = true;
            anim = GetComponent<Animator>();
            canAttack = true;
            controller = GetComponent<CharacterController>();
            cam = Camera.main;
            cam.GetComponent<TPCamera>().target = camMiddle;
            cam.GetComponent<TPCamera>().player = anim;
            cam.GetComponent<TPCamera>().LockCursor();

            for (int i = 0; i < partsNames.Length; i++)
            {
                parts.Add(partsNames[i], partsCol[i]);
                dmg.Add(partsNames[i], partsDamage[i]);
            }
      //  }
    }

    // Update is called once per frame
    void Update()
    {/*
        if (!isLocalPlayer)
            return;
        */
        if(GameObject.FindGameObjectWithTag("Enemy") != null && GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyAI>().died && GetComponent<PlayerHealth>().curHealth > 0)
        {
            if(!controle)
            {
                if(GetComponent<PlayerHealth>().curHealth / GetComponent<PlayerHealth>().maxHealth > 0.7f)
                    anim.SetTrigger("Win2");

                else
                    anim.SetTrigger("Win");

                controller.enabled = false;
                controle = true;
            }
            return;
        }

        if (GetComponent<PlayerHealth>().curHealth <= 0)
        {
            cam.GetComponent<TPCamera>().pitchMinMax = new Vector2(0, cam.GetComponent<TPCamera>().pitchMinMax[1]);
            return;
        }

        if (Time.timeScale <= 0)
            return;

        float x = Input.GetAxis("Vertical");
        float z = Input.GetAxis("Horizontal");
        run = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (x < 0 && !run && anim.GetBool("Jump") == false)
            block = true;

        else
            block = false;

        anim.SetBool("Block", block);
        anim.SetFloat("Speed", x);
        anim.SetBool("Running", run);
        if (canAttack)
            anim.SetFloat("Dir", z);

        if (x != 0 || z != 0)
            move = true;
        else
            move = false;

        anim.SetBool("Move", move);

        velY += Time.deltaTime * gravity;

        if(controller.enabled)
            controller.Move(new Vector3(0, velY, 0) * Time.deltaTime);

        if (canAttack && canTurn)
        { 
            curRot = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.transform.localEulerAngles.y, ref curRotRef, cam.GetComponent<TPCamera>().rotationSmoothTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, curRot, transform.eulerAngles.z);
        }

        if (controller.isGrounded)
        {
            velY = 0;
            if (jumpPhysicsActive)
            {
                anim.SetBool("Jump", false);
                jumpPhysicsActive = false;
            }

        }

        if (attackChange)
            attackSide = 1;

        else
            attackSide = 0;

        if (Input.GetKeyDown(KeyCode.Mouse0) && canAttack && anim.GetBool("Jump") == false && anim.GetBool("Fall") == false)
        {
            anim.SetFloat("AttackSide", attackSide);
            block = false;
            if (z > 0)
                anim.SetFloat("Dir", 1);

            else if (z < 0)
                anim.SetFloat("Dir", -1);

            else
                anim.SetFloat("Dir", 0);

            punch = true;
            anim.SetTrigger("Punch2");

            attackChange = !attackChange;
            canAttack = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && canAttack && anim.GetBool("Jump") == false && anim.GetBool("Fall") == false)
        {
            block = false;

            if (z > 0)
                anim.SetFloat("Dir", 1);

            else if (z < 0)
                anim.SetFloat("Dir", -1);

            else
                anim.SetFloat("Dir", 0);

            //anim.SetFloat("AttackSide", attackSide);
            kick = true;
            anim.SetTrigger("Kick2");
            //attackChange = !attackChange;
            canAttack = false;
        }

        if (attackPart != "" && attackPart != null && !canAttack && !collided)
        {
            if (parts[attackPart].collided)
            {

                float multiplyDmg;

                if (parts[attackPart].enemyPart == "Head")
                    multiplyDmg = 1.8f;

                else
                    multiplyDmg = 1;

                //Debug.Log(attackPart + " Coliided with the " + parts[attackPart].enemyPart + " of " + parts[attackPart].enemy.name + " giving an damage of " + dmg[attackPart]);
                if (punch)
                {
                    parts[attackPart].enemy.GetComponent<PlayerHealth>().TakeDamage(dmg[attackPart] * multiplyDmg, "Punch");

                    if(parts[attackPart].enemy.GetComponent<PlayerHealth>().CanReceiveHitKnockBack())
                        parts[attackPart].enemy.GetComponent<Animator>().SetTrigger("Hit");
                }

                else if (kick)
                {
                    parts[attackPart].enemy.GetComponent<PlayerHealth>().TakeDamage(dmg[attackPart] * multiplyDmg, "Kick");

                    if (parts[attackPart].enemy.GetComponent<PlayerHealth>().CanReceiveHitKnockBack())
                    {
                        float angleOffset;

                        if (anim.GetFloat("Dir") > 0)
                            angleOffset = 225;

                        else if (anim.GetFloat("Dir") < 0)
                            angleOffset = 135;

                        else
                            angleOffset = 180;


                        Vector3 dir = new Vector3(parts[attackPart].enemy.transform.position.x - transform.position.x, 0, parts[attackPart].enemy.transform.position.z - transform.position.z).normalized;
                        parts[attackPart].enemy.transform.eulerAngles = new Vector3(parts[attackPart].enemy.transform.eulerAngles.x, transform.eulerAngles.y + angleOffset, parts[attackPart].enemy.transform.eulerAngles.z);
                        parts[attackPart].enemy.GetComponent<ImpactReceiver>().AddImpact(dir, impactForce);
                    }
                }
                collided = true;
            }
        }
    }

    public void AttackAgain()
    {/*
        if (!isLocalPlayer)
            return;
        */
        canAttack = true;
    }

    public void JumpPhysics()
    {/*
        if (!isLocalPlayer)
            return;
*/
        float jumpVel;
        if (run)
            jumpVel = Mathf.Sqrt(-2 * gravity * jumpHeight);

        else
            jumpVel = Mathf.Sqrt(-2 * gravity * (jumpHeight * 0.4f));

        velY = jumpVel;
        jumpPhysicsActive = true;
    }

    void Jump()
    {/*
        if (!isLocalPlayer)
            return;
        */
        if (controller.isGrounded && anim.GetBool("Fall") == false)
        {
            anim.SetBool("Jump", true);
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
            return smoothTime;

        if (airControllerPercent == 0)
            return float.MaxValue;

        return smoothTime / airControllerPercent;
    }

    public void Attack(string side)
    {/*
        if (!isLocalPlayer)
            return;
        */
        attackPart = side;
    }

    public void AttackFalse()
    {/*
        if (!isLocalPlayer)
            return;
        */
        collided = false;
        attackPart = "";
        punch = false;
        kick = false;
        /*
        foreach(Hit h in partsCol)
        {
            h.collided = false;
            h.enemy = null;
            h.enemyPart = "";
        }*/
    }

    public void EndAttackReceived()
    {
        AttackFalse();
        canAttack = true;
        canTurn = true;
    }

    public void Died()
    {
        died = true;
    }
}
