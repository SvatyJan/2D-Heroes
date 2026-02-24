using System.Collections;
using UnityEngine;

public class UnitBehavior : MonoBehaviour, IOwned
{
    [SerializeField] private Player owner;

    public Player Owner => owner;

    public void SetOwner(Player newOwner)
    {
        if (owner != null)
            owner.UnregisterUnit(this);

        owner = newOwner;

        if (owner != null)
            owner.RegisterUnit(this);

        UpdateVisualOwnerColor();
    }

    private void UpdateVisualOwnerColor()
    {
        if (owner == null) return;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = owner.PlayerColor;
    }
    
    public Stance stance;
    public enum Stance
    {
        AGRESSIVE = 1,
        DEFENSIVE = 2,
        PASSIVE = 3,
    }


    public Formation formation;
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
    private float lastAttackTime = -999f;

    //pohyb a animace
    Animator animator;
    [SerializeField] float directionX;
    [SerializeField] float directionY;
    Vector2 movement;

    [SerializeField] GameObject highlightCircle;
    public bool isHighlighted = false;
    [SerializeField] private bool selectable;

    [SerializeField] private bool lockActions = false;
    private Vector2 attackingPoint;

    private AttackBehaviour attackBehaviour;


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
        SetOwner(Owner);

        formation = Formation.CIRCLE;
        animator = GetComponent<Animator>();
        currentMoveSpeed = moveSpeed;
        lockActions = false;

        attackBehaviour = GetComponent<AttackBehaviour>();
        if(attackBehaviour == null)
        {
            Debug.LogError("UnitBehavior: No AttackBehaviour component found on this unit!");
        }
    }

    void Update()
    {
        checkForHighlight();
        getMovementDirection();

        if (lockActions == false)
        {
            if (stance == Stance.PASSIVE)
            {
                passiveStanceBehaviour();
            }
            else if (stance == Stance.DEFENSIVE)
            {
                defensiveStanceBehaviour();
            }
            else if (stance == Stance.AGRESSIVE)
            {
                attackStanceBehaviour();
            }
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
        //lookAtTargetobject(attackTarget);
        //attackTargetObject(attackTarget);
        handleAttack(attackTarget);
    }

    public void attackStanceBehaviour()
    {
        lookForAttackTarget(attackFollowRadius);
        attackAndMoveTowardsTargetObject(attackTarget);
    }

    public void lookForAttackTarget(float lookForAttackTargetRadius)
    {
        if (attackTarget != null &&
            Vector2.Distance(transform.position, attackTarget.transform.position) <= lookForAttackTargetRadius)
            return;

        attackTarget = null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, lookForAttackTargetRadius);

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

    private void handleAttack(GameObject target)
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (dist > attackRadius)
            return;

        if (Time.time < lastAttackTime + attackSpeedTime)
            return;

        lastAttackTime = Time.time;

        currentMoveSpeed = 0f;
        animator.SetBool("Attacking", true);

        Vector2 dir = target.transform.position - transform.position;
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
    }

    public void OnAttackHitFrame()
    {
        if (attackBehaviour != null)
            attackBehaviour.ExecuteAttack(attackTarget);
    }

    public void OnAttackFinished()
    {
        animator.SetBool("Attacking", false);
        currentMoveSpeed = moveSpeed;
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
        if (target != null && !animator.GetBool("Attacking"))
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.transform.position,
                currentMoveSpeed * Time.deltaTime);

            movement = target.transform.position - transform.position;
        }
        else
        {
            movement = Vector2.zero;
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
        if (target == null)
        {
            followTargetObject(followTarget);
            return;
        }

        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (dist > attackRadius && dist <= attackFollowRadius)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.transform.position,
                currentMoveSpeed * Time.deltaTime);

            movement = target.transform.position - transform.position;
        }
        else if (dist <= attackRadius)
        {
            handleAttack(target);
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

    public void attackPreparation()
    {
        if(attackTarget != null)
        {
            attackingPoint = new Vector2(attackTarget.transform.position.x, attackTarget.transform.position.y);
            currentMoveSpeed = 0f;
            lockActions = true;
        }
        else
        {
            currentMoveSpeed = moveSpeed;
            lockActions = false;
        }
    }

    public void attackFinish()
    {
        if (attackingPoint != null && Vector2.Distance(transform.position, attackingPoint) <= attackRadius)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackingPoint, 1f);

            foreach (Collider2D collider in colliders)
            {
                if (ArrayContainsTag(collider.tag, attackTags))
                {
                    float damage = GetComponent<Damage>().damage;
                    GetComponent<Damage>().DealDamage(damage, attackTarget);
                    break;
                }
            }
        }

        movement.x = 0f;
        movement.y = 0f;
        currentMoveSpeed = moveSpeed;
        lockActions = false;
    }

    public void SetAttackAnimationFalse()
    {
        animator.SetBool("Attacking", false);
    }

    // TODO: přepsat na attribut
    public void ModifyMoveSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
        currentMoveSpeed = moveSpeed;
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
