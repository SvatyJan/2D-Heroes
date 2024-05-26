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
    [SerializeField] float lookForAttackTargetRadius = 5f;
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
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetMovementDirection();
        checkForHighlight();

        if (stance == Stance.PASSIVE)
        {
            PassiveStanceBehaviour();
        }
        else if(stance == Stance.DEFENSIVE)
        {
            DefensiveStanceBehaviour();
        }
        else if (stance == Stance.AGRESSIVE)
        {
            //TODO:
            AttackStanceBehaviour();
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

    public Stance GetStance()
    {
        return stance;
    }

    public void SetStance(Stance stance)
    {
        this.stance = stance;
    }

    public GameObject GetFollowTarget()
    {
        return followTarget;
    }

    public void SetFollowTarget(GameObject newFollowTarget)
    {
        this.followTarget = newFollowTarget;
    }

    public void PassiveStanceBehaviour()
    {
        if (followTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, followTarget.transform.position, moveSpeed * Time.deltaTime);
            movement = followTarget.transform.position - transform.position;
        }
        else
        {
            // nema followTarget
            movement = new Vector2(0, 0);            
        }
    }

    public void DefensiveStanceBehaviour()
    {
        LookForAttackTarget();

        if(followTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, followTarget.transform.position, moveSpeed * Time.deltaTime);
            movement = followTarget.transform.position - transform.position;
        }

        if(attackTarget != null)
        {
            movement = attackTarget.transform.position - transform.position;
            if (Vector2.Distance(transform.position, attackTarget.transform.position) <= attackRadius)
            {
                animator.SetBool("Attacking", true);
            }
            else
            {
                animator.SetBool("Attacking", false);
            }
        }
    }

    public void AttackStanceBehaviour()
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

    public void LookForAttackTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, lookForAttackTargetRadius);

        if (attackTarget != null)
        {
            if (Vector2.Distance(transform.position, attackTarget.transform.position) >= lookForAttackTargetRadius)
            {                
                attackTarget = null;
                return;
            }
        }

        foreach (Collider2D collider in colliders)
        {
            if (ArrayContainsTag(collider.tag, attackTags))
            {
                attackTarget = collider.gameObject;
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
        // Set the color of the Gizmo
        Gizmos.color = Color.red;

        // Draw a wire sphere at the position of the GameObject this script is attached to
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
