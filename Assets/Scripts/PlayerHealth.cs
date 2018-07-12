using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
    public Image playerHealth;
    public Image enemyHealth;
    public float maxHealth = 100;
    public float curHealth;
    private Animator anim;
    private float calcHealth;
    private string lastAttackType;
    private bool control;
    private float sequenceHits;
    private float hitsTimer;
    public float maxHitTimer = 1.5f;
    public float maxHits = 6;
    public Sprite[] healthStates;
    // Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        curHealth = maxHealth;

       // playerHealth = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<Image>();
       // enemyHealth = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        calcHealth = curHealth / maxHealth;
        playerHealth.fillAmount = calcHealth;

        if (calcHealth > 0.7f)
            playerHealth.sprite = healthStates[0];

        else if(calcHealth > 0.3f && calcHealth <= 0.7f)
            playerHealth.sprite = healthStates[1];

        else
            playerHealth.sprite = healthStates[2];
    }

    // Update is called once per frame
    void Update () {
        if (control)
            return;

        if (curHealth < 0)
            curHealth = 0;

        if (curHealth > maxHealth)
            curHealth = maxHealth;
        
        if(curHealth > 0)
        {
            if(sequenceHits > 0)
            hitsTimer += Time.deltaTime;

            if(hitsTimer >= maxHits)
            {
                sequenceHits = 0;
                hitsTimer = 0;
            }

            if(sequenceHits >= maxHits)
            {
                anim.SetBool("Fall", true);
                anim.SetTrigger("FallT");
                GetComponent<CharacterController>().enabled = false;
                hitsTimer = 0;
                sequenceHits = 0;
            }
        }
        
        if (curHealth <= 0)
        {
            if (lastAttackType == "Punch")
                anim.SetTrigger("PFall");

            else if (lastAttackType == "Kick")
                anim.SetTrigger("KFall");

            else
                anim.SetTrigger("FFall");

            anim.SetBool("Died", true);

            control = true;
            GetComponent<CharacterController>().enabled = false;
        }

    }

    public void TakeDamage(float dmg, string attackType)
    {
        float damage;

        if (anim.GetBool("Block"))
        {
            damage = dmg / 2;
            sequenceHits += 0.5f;
        }

        else
        {
            damage = dmg;
            sequenceHits++;
        }

        curHealth -= damage;

        lastAttackType = attackType;

        if (GetComponent<EnemyAI>() != null)
        {
            GetComponent<EnemyAI>().canTurn = false;
            GetComponent<EnemyAI>().attackDelay = 0;
        }

        if (GetComponent<KaratecaMovement>() != null)
            GetComponent<KaratecaMovement>().canTurn = false;

        hitsTimer = 0;

        FindObjectOfType<LevelManager>().Comemorate(damage);
    }

    public void GetUp()
    {
        anim.SetBool("Fall", false);

        if (GetComponent<EnemyAI>() != null)
        {
            GetComponent<EnemyAI>().canAttack = true;
            GetComponent<EnemyAI>().canTurn = true;
        }

        if (GetComponent<KaratecaMovement>() != null)
        { 
            GetComponent<KaratecaMovement>().canTurn = true;
            GetComponent<KaratecaMovement>().canAttack = true;
        }

        GetComponent<CharacterController>().enabled = true;
    }

    public bool CanReceiveHitKnockBack()
    {
        if (sequenceHits < maxHits)
            return true;

        else
            return false;
    }
}
