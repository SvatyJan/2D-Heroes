using System.Collections.Generic;
using UnityEngine;

public class UnitBehavior : MonoBehaviour
{
    public enum Behavior
    {
        IDLE,
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

    public enum Formation
    {
        CIRCLE,
        FRONT,
        BACK,
        LEFT,
        RIGHT
    }
    public Formation formation;

    public GameObject defendingTarget;

    public GameObject followTarget;
    public GameObject attackTarget;

    public string[] attackTags = { "Enemy Unit", "Enemy" };
    public string[] followTags = { "Player" };

    [SerializeField] float moveSpeed;
    [SerializeField] float followKeepDistance = 3f;
    [SerializeField] float guardKeepDistance = 0f;
    [SerializeField] float attackFollowRadius = 5f;
    [SerializeField] float attackRadius = 2f;

    //pohyb a animace
    Animator animator;
    [SerializeField] float directionX;
    [SerializeField] float directionY;
    Vector2 movement;

    private void Start()
    {
        formation = Formation.CIRCLE;
        behavior = Behavior.IDLE;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetMovementDirection();

        if (behavior == Behavior.IDLE)
        {
            Idle();
        }
        else if(behavior == Behavior.ATTACK)
        {
            Attack();
        }
        else if (behavior == Behavior.GUARD)
        {
            Guard();
        }
        //Debug.Log(behavior);

        if (stance == Stance.PASSIVE)
        {
            //do nothing
        }
        else if (stance == Stance.DEFENSIVE)
        {
            //útoè v malém radiusu, jen když pøijdou do range
        }
        else if (stance == Stance.AGRESSIVE)
        {
            LookForAttackTarget();
            //útoè ve velkém radiusu na potkání + utika za cilem
        }
    }

    public void Idle()
    {
        animator.SetFloat("Speed", 0);
    }
    public void Attack()
    {
        if (attackTarget != null)
        {
            if (Vector2.Distance(transform.position, attackTarget.transform.position) <= attackFollowRadius && Vector2.Distance(transform.position, attackTarget.transform.position) >= attackRadius)
            {
                transform.position = Vector2.MoveTowards(transform.position, attackTarget.transform.position, moveSpeed * Time.deltaTime);
                movement = attackTarget.transform.position - transform.position;
                if (Vector2.Distance(transform.position, attackTarget.transform.position) < attackRadius+(attackRadius/100*20))
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
        if(followTarget == null)
        {
            followTarget = null;
            behavior = Behavior.IDLE;
        }
        else
        {
            if (Vector2.Distance(transform.position, followTarget.transform.position) > guardKeepDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, followTarget.transform.position, moveSpeed * Time.deltaTime);
                movement = followTarget.transform.position - transform.position;
            }
            else if (Vector2.Distance(transform.position, followTarget.transform.position) == guardKeepDistance)
            {
                movement = new Vector2(0, 0);
            }
        }        
    }

    public void LookForAttackTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackFollowRadius);

        if (attackTarget != null)
        {
            if (Vector2.Distance(transform.position, attackTarget.transform.position) > attackFollowRadius)
            {
                behavior = Behavior.IDLE;
                
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
    }    private bool ArrayContainsTag(string tag, string[] tagArray)
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

    public void AddToGroup(GameObject hero)
    {
        //zjistim nejdriv jestli uz v nejake parte nahodou nejsem
        if(defendingTarget != null)
        {
            defendingTarget.GetComponent<StructureFormationController>().selectedUnits.Remove(this.gameObject);
        }

        List<GameObject> selectedUnits = hero.GetComponent<FormationController>().selectedUnits;
        followTarget = hero;

        bool jeUzVParte = false;

        if (selectedUnits.Count != 0)
        {
            foreach (GameObject followingUnit in selectedUnits)
            {
                if (this.name == followingUnit.name)
                {
                    jeUzVParte = true;
                    break;
                }
            }
        }

        // Pokud jednotka ještì není v kolekci, pøidá se do kolekce
        if (!jeUzVParte)
        {
            selectedUnits.Add(this.gameObject);
        }
    }

    public void RemoveFromGroup(GameObject hero)
    {
        List<GameObject> selectedUnits = hero.GetComponent<FormationController>().selectedUnits;

        if (selectedUnits.Count != 0)
        {
            behavior = Behavior.IDLE;
            selectedUnits.Remove(this.gameObject);
        }
    }

    public void defendNewTarget(GameObject newTarget)
    {
        behavior = Behavior.GUARD;
        followTarget = newTarget; //tenhle target actually musí být vytvoøený bod z toho objektu
        if(newTarget.tag == "Player Structure" || newTarget.tag == "Player Flag")
        {
            newTarget.GetComponent<StructureFormationController>().selectedUnits.Add(this.gameObject);
        }
    }

    public void GetMovementDirection()
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
