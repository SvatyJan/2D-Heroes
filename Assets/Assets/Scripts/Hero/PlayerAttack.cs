using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float damage;
    private float timeBtwAttack;
    [SerializeField] float startTimeBtwAttack;

    public bool blocking = false;
    public bool attackActive = false;

    Animator animator;
    Vector2 mousePosition;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            PlayerMeleeAttack();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (!blocking)
            {
                blocking = true;
                animator.SetBool("Blocking", true);
            }
            else if (blocking)
            {
                blocking = false;
                animator.SetBool("Blocking", false);
            }
        }
    }

    private void PlayerMeleeAttack()
    {
        if (timeBtwAttack <= 0)
        {
            animator.SetBool("Attacking", true);
            //Debug.Log("You melee attack");

            //GameObject slasheffectinstance = Instantiate(slashEffect, transform.position, Quaternion.identity);
            //Destroy(slasheffectinstance,effectduration);

            /*Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRadius, whatIsEnemy);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                //enemiesToDamage[i].GetComponent<Stats>().TakeDamage(damage);
            }*/
            timeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        if (timeBtwAttack <= 0)
        {
            animator.SetBool("Attacking", true);
        }
    }

    public void ChangeBlocking()
    {
        if (blocking)
        {
            animator.SetBool("Blocking", false);
            blocking = false;
        }
        else
        {
            animator.SetBool("Blocking", true);
            blocking = true;
        }
    }

    public void SetAttackAnimationFalse()
    {
        animator.SetBool("Attacking", false);
        timeBtwAttack = 0;
    }

}
