using System.Collections;
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
        RIGHT = 5,
        CENTER = 6,
        STAR = 7
    }
    public Formation formation;

    [SerializeField] private GameObject followTarget;
    [SerializeField] private GameObject attackTarget;

    public string[] attackTags = { "Enemy Unit", "Enemy" };
    public string[] followTags = { "Player", "Player Unit", "Player Structure", "Player Flag" };

    [SerializeField] float moveSpeed;
    [SerializeField] float currentMoveSpeed;
    [SerializeField] float defendRadius = 3f;
    [SerializeField] float attackFollowRadius = 5f;
    [SerializeField] float attackRadius = 2f;
    [SerializeField] float retreatRadius = 10f;

    [SerializeField] float attackSpeedTime = 2f;
    [SerializeField] private bool attackingState = false;
    [SerializeField] private bool retreatingState = false;

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
        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        getMovementDirection();
        checkForHighlight();

        if (stance == Stance.PASSIVE)
        {
            passiveStanceBehaviour();
        }
        else if(stance == Stance.DEFENSIVE)
        {
            defensiveStanceBehaviour();
        }
        else if (stance == Stance.AGRESSIVE)
        {
            attackStanceBehaviour();
        }
    }

    public void addToGroup(GameObject controllingObject)
    {
        if(followTarget != null)
        {
            followTarget.GetComponent<ObjectFormationController>().RemoveUnit(this.gameObject);
        }
        controllingObject.GetComponent<ObjectFormationController>().AddUnit(this.gameObject);
    }

    public void removeFromGroup(GameObject controllingObject)
    {
        controllingObject.GetComponent<UnitController>().RemoveSelectedUnit(this.gameObject);
        followTarget = null;
        this.isHighlighted = false;
    }

    public Formation getFormation()
    {
        return formation;
    }

    public void setFormation(Formation formation)
    {
        this.formation = formation;
    }

    public Stance getStance()
    {
        return stance;
    }

    public void setStance(Stance stance)
    {
        this.stance = stance;
    }

    public GameObject getFollowTarget()
    {
        return followTarget;
    }

    public void setFollowTarget(GameObject newFollowTarget)
    {
        this.followTarget = newFollowTarget;
    }

    public void passiveStanceBehaviour()
    {
        followTargetObject(followTarget);
    }

    public void defensiveStanceBehaviour()
    {
        lookForAttackTarget(defendRadius);
        followTargetObject(followTarget);
        lookAtTargetobject(attackTarget);
        attackTargetObject(attackTarget);
    }

    public void attackStanceBehaviour()
    {
        lookForAttackTarget(defendRadius);
        attackAndMoveTowardsTargetObject(attackTarget);
    }

    public void lookForAttackTarget(float lookForAttackTargetRadius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, lookForAttackTargetRadius);

        if (attackTarget != null)
        {
            if (Vector2.Distance(transform.position, attackTarget.transform.position) <= lookForAttackTargetRadius)
            {
                return;
            }
            else
            {
                attackTarget = null;
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

    public void getMovementDirection()
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

    private void followTargetObject(GameObject target)
    {
        if(target != null && attackingState == false)
        {
            animator.SetBool("Attacking", false);
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentMoveSpeed * Time.deltaTime);
            movement = target.transform.position - transform.position;
        }
        else
        {
            movement = new Vector2(0, 0);
        }
    }

    private void attackTargetObject(GameObject target)
    {
        if (target != null)
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= attackRadius)
            {
                retreatingState = false;
                StartCoroutine(attackRoutine());
            }
        }
    }

    private void attackAndMoveTowardsTargetObject(GameObject target)
    {
        if (followTarget != null && Vector2.Distance(transform.position, followTarget.transform.position) <= 1f)
        {
            retreatingState = false;
        }

        if (retreatingState == true && followTarget != null)
        {
            followTargetObject(followTarget);
            return;
        }

        if (followTarget != null && Vector2.Distance(transform.position, followTarget.transform.position) >= retreatRadius)
        {
            retreatingState = true;
            return;
        }

        if (target != null)
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= attackFollowRadius 
                && Vector2.Distance(transform.position, target.transform.position) > attackRadius)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, currentMoveSpeed * Time.deltaTime);
                movement = target.transform.position - transform.position;
            }
            else if(Vector2.Distance(transform.position, target.transform.position) < attackRadius)
            {
                StartCoroutine(attackRoutine());
            }
        }
        else if (followTarget != null)
        {
            followTargetObject(followTarget);
        }
    }

    private void lookAtTargetobject(GameObject target)
    {
        if(target != null && movement.x == 0 && movement.y == 0)
        {
            movement = target.transform.position - transform.position;
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Horizontal", movement.x);
            movement = new Vector2(0, 0);
            animator.SetBool("Attacking", true);
        }
    }

    private IEnumerator attackRoutine()
    {
        if(attackingState == false)
        {
            attackingState = true;
            currentMoveSpeed = 0f;
            animator.SetBool("Attacking", true);
            yield return new WaitForSeconds(attackSpeedTime);
            currentMoveSpeed = moveSpeed;
            animator.SetBool("Attacking", false);
            attackingState = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackFollowRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, retreatRadius);
    }
}
