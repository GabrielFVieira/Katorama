using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    #region Public Variables
    public Transform target;
    public float minDist;
    public float runDist;
    public float runDistMin;
    public bool canAttack;
    public float turnSmoothTime = 0.2f;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0, 1)]
    public float airControllerPercent;
    public string[] partsNames;
    public float[] partsDamage;
    public Hit[] partsCol;
    public float impactForce = 3;
    public bool canTurn = true;
    public float attackDelay;
    public bool died;
    #endregion

    #region Private Variables
    private Animator anim;
    private Camera cam;
    private float turnSmoothVelocity;
    private CharacterController controller;
    private NavMeshAgent agent;
    private float velY;
    private bool run;
    private bool move;
    private bool jumpPhysicsActive;
    private int attackSide;
    private bool attackChange;
    private Dictionary<string, Hit> parts = new Dictionary<string, Hit>();
    private Dictionary<string, float> dmg = new Dictionary<string, float>();
    private string attackPart;
    private bool collided;
    private bool punch;
    private bool kick;
    private bool controle;
    #endregion
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        canAttack = true;
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        for (int i = 0; i < partsNames.Length; i++)
        {
            parts.Add(partsNames[i], partsCol[i]);
            dmg.Add(partsNames[i], partsDamage[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        

        if(target.gameObject.GetComponent<KaratecaMovement>().died && GetComponent<PlayerHealth>().curHealth > 0)
        {
            if(!controle)
            {
                if (GetComponent<PlayerHealth>().curHealth / GetComponent<PlayerHealth>().maxHealth > 0.7f)
                    anim.SetTrigger("Win2");

                else
                    anim.SetTrigger("Win");

                controle = true;
            }
            return;
        }

        if (GetComponent<PlayerHealth>().curHealth <= 0 || target.GetComponent<PlayerHealth>().curHealth <= 0)
            return;

        velY += Time.deltaTime * gravity;

        if (controller.enabled)
        {
            //agent.destination = target.position;
            agent.SetDestination(target.position);
            controller.Move(new Vector3(0, velY, 0) * Time.deltaTime);
        }

        if(canTurn)
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

        float dist = Vector3.Distance(target.transform.position, transform.position);

        if (attackDelay > 0)
            attackDelay -= Time.deltaTime;

        if (attackDelay < 0)
            attackDelay = 0;

        if (canAttack && target.GetComponent<Animator>().GetBool("Fall") == false)
        {
            if (dist > minDist && dist < runDist)
            {
                anim.SetFloat("Speed", 1);
                anim.SetBool("Move", true);
                agent.speed = 1f;
                agent.isStopped = false;
            }

            else if (dist >= runDist)
            {
                anim.SetFloat("Speed", 1);
                anim.SetBool("Move", true);
                anim.SetBool("Running", true);
                agent.speed = 2f;
                agent.isStopped = false;
            }

            else if (dist <= minDist)
            {
                anim.SetBool("Move", false);
                anim.SetBool("Running", false);
                agent.isStopped = true;

                if (attackDelay <= 0)
                {
                    anim.SetFloat("AttackSide", attackSide);
                    agent.isStopped = true;
                    int dir = Random.Range(-1, 2);

                    anim.SetFloat("Dir", dir);

                    int attackType = Random.Range(0, 2);

                    if (attackType == 0)
                    {
                        punch = true;
                        anim.SetTrigger("Punch2");
                    }

                    else
                    {
                        kick = true;
                        anim.SetTrigger("Kick2");
                    }
                    attackChange = !attackChange;
                    anim.SetFloat("Dir", 0);
                    canTurn = false;
                    canAttack = false;
                    attackDelay = 2;
                    agent.isStopped = true;
                }
            }

            if (anim.GetBool("Running") && dist < runDistMin)
            {
                anim.SetBool("Running", false);
            }
        }

        else
        {
            anim.SetFloat("Speed", 0);
            anim.SetBool("Move", false);
            agent.isStopped = true;
        }


        if (controller.isGrounded)
        {
            velY = 0;
        }

        if (attackChange)
            attackSide = 1;

        else
            attackSide = 0;

        if (attackPart != "" && attackPart != null && !collided)
        {
            if (parts[attackPart].collided)
            {
                float multiplyDmg;

                if (parts[attackPart].enemyPart == "Head")
                    multiplyDmg = 1.8f;

                else
                    multiplyDmg = 1;

                if (punch)
                {
                    parts[attackPart].enemy.GetComponent<PlayerHealth>().TakeDamage(dmg[attackPart] * multiplyDmg, "Punch");

                    if (parts[attackPart].enemy.GetComponent<PlayerHealth>().CanReceiveHitKnockBack())
                    {
                        parts[attackPart].enemy.GetComponent<Animator>().SetTrigger("Hit");
                        parts[attackPart].enemy.transform.LookAt(new Vector3(transform.position.x, parts[attackPart].enemy.transform.position.y, transform.position.z));
                    }
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
                //Debug.Log(attackPart + " Coliided with the " + parts[attackPart].enemyPart + " of " + parts[attackPart].enemy.name + " giving an damage of " + dmg[attackPart]);
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
        canTurn = true;
    }

    public void EndAttackReceived()
    {
        AttackFalse();
        canAttack = true;
    }

    public void Died()
    {
        died = true;
    }
}
