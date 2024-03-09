using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootmanBehavior : AUnit
{
    public enum Behavior
    {
        IDLE,
        FOLLOW,
        ATTACK,
        GUARD
    }
    public Behavior behavior;

    public enum Stance
    {
        PASSIVE,
        DEFENSIVE,
        AGRESSIVE
    }
    public Stance stance;

    public GameObject followTarget;
    public GameObject attackTarget;

    public string[] attackTags = { "Enemy Unit", "Enemy" };
    public string[] followTags = { "Player" };

    [SerializeField] float defaultMoveSpeed = 3f;
    [SerializeField] float currentMoveSpeed = 3f;
    [SerializeField] float followKeepDistance = 3f;
    [SerializeField] float guardKeepDistance = 0f;
    [SerializeField] float attackFollowRadius = 5f;
    [SerializeField] float attackRadius = 2f;

    [SerializeField] bool followOnStart = true;

    //pohyb a animace
    Animator animator;
    [SerializeField] float directionX;
    [SerializeField] float directionY;
    Vector2 movement;

    void Start()
    {
        if (followOnStart == true)
        {
            behavior = Behavior.FOLLOW;
        }
        else
        {
            behavior = Behavior.IDLE;
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetMovementDirection();

        if (behavior == Behavior.IDLE)
        {
            Idle();
        }
        else if (behavior == Behavior.FOLLOW)
        {
            Follow();
        }
        else if (behavior == Behavior.ATTACK)
        {
            Attack();
        }
        else if (behavior == Behavior.GUARD)
        {
            Guard();
        }

        if (stance == Stance.PASSIVE)
        {
            //do nothing
        }
        else if (stance == Stance.DEFENSIVE)
        {
            //útoè v malém radiusu
        }
        else if (stance == Stance.AGRESSIVE)
        {
            LookForAttackTarget();
            //útoè ve velkém radiusu na potkání
        }
    }

    public void Idle()
    {

    }
    public void Follow()
    {
        GameObject[] hero;
        hero = GameObject.FindGameObjectsWithTag("Player");
        followTarget = hero[0];
        /*List<GameObject> hero = new List<GameObject>();
        foreach (GameObject possibleFollowTargets in GameObject.FindGameObjectsWithTag("Player"))
        {
            hero.Add(possibleFollowTargets);
        }
        followTarget = hero[0];*/
        if (followTarget != null)
        {
            behavior = Behavior.GUARD;
        }


        if (Vector2.Distance(transform.position, followTarget.transform.position) > followKeepDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, followTarget.transform.position, currentMoveSpeed * Time.deltaTime);
            movement = followTarget.transform.position - transform.position;
        }
        else if (Vector2.Distance(transform.position, followTarget.transform.position) == followKeepDistance)
        {
            movement = new Vector2(0, 0);
        }
        AddToGroup();

        /*if (attackTarget == null)
        {
            
        }
        else
        {
            behavior = Behavior.ATTACK;
        }*/

    }
    public void Attack()
    {
        if (attackTarget != null)
        {
            if (Vector2.Distance(transform.position, attackTarget.transform.position) <= attackFollowRadius && Vector2.Distance(transform.position, attackTarget.transform.position) >= attackRadius)
            {
                transform.position = Vector2.MoveTowards(transform.position, attackTarget.transform.position, currentMoveSpeed * Time.deltaTime);
                movement = attackTarget.transform.position - transform.position;
                if (Vector2.Distance(transform.position, attackTarget.transform.position) < attackRadius + (attackRadius / 100 * 20))
                {
                    animator.SetBool("Attacking", true);
                }
                else
                {
                    animator.SetBool("Attacking", false);
                }
            }
        }
    }
    public void Guard()
    {
        if (Vector2.Distance(transform.position, followTarget.transform.position) > guardKeepDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, followTarget.transform.position, currentMoveSpeed * Time.deltaTime);
            movement = followTarget.transform.position - transform.position;
        }
        else if (Vector2.Distance(transform.position, followTarget.transform.position) == guardKeepDistance)
        {
            movement = new Vector2(0, 0);
        }
    }

    public void LookForAttackTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackFollowRadius);

        if (attackTarget != null)
        {
            if (Vector2.Distance(transform.position, attackTarget.transform.position) > attackFollowRadius)
            {
                if (followTarget != null)
                {
                    behavior = Behavior.FOLLOW;
                }
                else
                {
                    behavior = Behavior.IDLE;
                }

                attackTarget = null;
                return;
            }
        }

        foreach (Collider2D collider in colliders)
        {
            if (ArrayContainsTag(collider.tag, attackTags))
            {
                attackTarget = collider.gameObject;
                behavior = Behavior.ATTACK;
                Debug.Log("Menim chovani na utok");
                break;
            }
        }
    }
    private bool ArrayContainsTag(string tag, string[] tagArray)
    {
        foreach (string t in tagArray)
        {
            if (tag.Equals(t))
            {
                return true;
            }
        }
        return false;
    }

    public override void AddToGroup()
    {
        List<GameObject> followingUnits = followTarget.GetComponent<UnitController>().GetSelectedUnits();
        Debug.Log(followingUnits.Count);
        bool jeUzVParte = false;

        if (followingUnits.Count != 0)
        {
            foreach (GameObject followingUnit in followingUnits)
            {
                if (this.name == followingUnit.name)
                {
                    jeUzVParte = true;
                    break;
                }
            }
        }

        // Pokud jednotka ještì není v kolekci a je v dosahu hrdiny, pøidá se do kolekce
        if (!jeUzVParte)
        {
            followingUnits.Add(this.gameObject);
            Debug.Log(this.name + " se pøidává k tvé partì!");
        }
    }

    public override void RemoveFromGroup()
    {
        throw new System.NotImplementedException();
    }

    public override void GetMovementDirection()
    {
        if (movement.x != 0)
        {
            directionX = movement.x;
            animator.SetFloat("Horizontal", directionX);
            animator.SetFloat("Vertical", movement.y);
        }
        if (movement.y != 0)
        {
            directionY = movement.y;
            animator.SetFloat("Vertical", directionY);
            animator.SetFloat("Horizontal", movement.x);
        }

        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.green;        
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y,0), keepDistance);*/
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackFollowRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius + (attackRadius / 100 * 20));
    }
}
