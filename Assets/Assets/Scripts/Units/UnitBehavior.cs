using System.Collections.Generic;
using UnityEngine;

public class UnitBehavior : MonoBehaviour
{
    public enum Stance
    {
        AGRESSIVE = 1,
        DEFENSIVE = 2,
        PASSIVE = 3,
    }
    public Stance stance;

    public enum Formation
    {
        CIRCLE = 1,
        FRONT = 2,
        BACK = 3,
        LEFT = 4,
        RIGHT = 5
    }
    public Formation formation;

    [SerializeField] private GameObject followTarget;
    [SerializeField] private GameObject attackTarget;

    public string[] attackTags = { "Enemy Unit", "Enemy" };
    public string[] followTags = { "Player", "Player Unit", "Player Structure", "Player Flag" };

    [SerializeField] float moveSpeed;
    [SerializeField] float guardKeepDistance = 0f;
    [SerializeField] float attackFollowRadius = 5f;
    [SerializeField] float attackRadius = 2f;

    //pohyb a animace
    Animator animator;
    [SerializeField] float directionX;
    [SerializeField] float directionY;
    Vector2 movement;

    [SerializeField] GameObject highlightCircle;
    public bool isHighlighted = false;
    [SerializeField] private bool selectable;

    public void setSelectable(bool selectable)
    {
        this.selectable = selectable;
    }

    public bool getSelectable()
    {
        return this.selectable;
    }

    private void Start()
    {
        formation = Formation.CIRCLE;
        stance = Stance.AGRESSIVE;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetMovementDirection();
        checkForHighlight();

        if (stance == Stance.PASSIVE)
        {
            Idle();
        }
        else if(stance == Stance.AGRESSIVE)
        {
            Attack();
        }
        else if (stance == Stance.DEFENSIVE)
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

    public void AddToGroup(GameObject controllingObject)
    {
        if(followTarget != null)
        {
            followTarget.GetComponent<ObjectFormationController>().RemoveUnit(this.gameObject);
        }
        controllingObject.GetComponent<ObjectFormationController>().AddUnit(this.gameObject);
    }

    public void RemoveFromGroup(GameObject controllingObject)
    {
        controllingObject.GetComponent<UnitController>().RemoveSelectedUnit(this.gameObject);
        followTarget = null;
        this.isHighlighted = false;
    }

    public Formation GetFormation()
    {
        return formation;
    }

    public void SetFormation(Formation formation)
    {
        this.formation = formation;
    }

    public GameObject GetFollowTarget()
    {
        return followTarget;
    }

    public void SetFollowTarget(GameObject newFollowTarget)
    {
        this.followTarget = newFollowTarget;
    }

    public Stance GetBehavior()
    {
        return stance;
    }

    public void SetBehavior(Stance newBehavior)
    {
        this.stance = newBehavior;
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
            stance = Stance.PASSIVE;
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
                stance = Stance.PASSIVE;
                
                attackTarget = null;
                return;
            }
        }

        foreach (Collider2D collider in colliders)
        {
            if (ArrayContainsTag(collider.tag, attackTags))
            {
                attackTarget = collider.gameObject;
                stance = Stance.AGRESSIVE;
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

    public void defendNewTarget(GameObject newTarget)
    {
        if(followTarget != null)
        {
            if(followTarget.tag == "PlayerMainFormationPoint")
            {
                GameObject followTargetParent = followTarget.GetComponentInParent<GameObject>();
                followTargetParent.GetComponent<ObjectFormationController>().RemoveUnit(this.gameObject);
                followTargetParent.GetComponent<ObjectFormationController>().RecalculateFormations();
            }
            else
            {
                followTarget.GetComponent<ObjectFormationController>().RemoveUnit(this.gameObject);
                followTarget.GetComponent<ObjectFormationController>().RecalculateFormations();
            }
        }
        if(!newTarget.GetComponent<ObjectFormationController>().GetFollowingUnits().Contains(this.gameObject))
        {
            followTarget = newTarget;
            stance = Stance.DEFENSIVE;
            newTarget.GetComponent<ObjectFormationController>().GetFollowingUnits().Add(this.gameObject);
            newTarget.GetComponent<ObjectFormationController>().RecalculateFormations();
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

    private void checkForHighlight()
    {
        if (isHighlighted == true)
        {
            highlightCircle.SetActive(true);
        }
        else
        {
            highlightCircle.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackFollowRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius + (attackRadius / 100 * 20));
    }
}
