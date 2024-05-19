using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //COMPONENTY
    [SerializeField] Rigidbody2D rb2d;

    //MOVEMENT
    public float moveSpeed = 5f;
    public Vector2 movement;
    public Vector2 mousePosition;
    public Camera cam;
    [SerializeField] float directionX;
    [SerializeField] float directionY;
    [SerializeField] GameObject unitSelectorTemplate;
    [SerializeField] GameObject unitDeselectorTemplate;
    private GameObject unitSelectorGameObject;
    private GameObject unitDeselectorGameObject;

    [SerializeField] GameObject flagTemplate;
    [SerializeField] public int maxFlags = 3;
    private int flagsPutDown = 0;
    public List<GameObject> flagList = new List<GameObject>();

    //ANIMATOR
    Animator animator;

    //UNIT SELECTOR
    private bool unitSelectorBool = false;
    private bool unitDeselectorBool = false;

    Ray ray;
    RaycastHit2D hit;

    //ATTACK
    [SerializeField] float damage;
    private float timeBtwAttack;
    [SerializeField] float startTimeBtwAttack;

    public bool blocking = false;
    public bool attackActive = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        LookAtMouse();
    }

    void Update()
    {
        /*if (Input.GetButtonDown("Fire1"))
        {
            PlayerMeleeAttack();
        }*/

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

        Movement();
        //Point();
        createFlag();
        orderDefend();
    }

    private void Movement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

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

    private void LookAtMouse()
    {
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

        rb2d.MovePosition(rb2d.position + movement * moveSpeed * Time.deltaTime);
        Vector2 lookDirection = mousePosition - rb2d.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;

        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);

        float horizontalFirepointX = lookDirection.x;
        float horizontalFirepointY = lookDirection.y;
        if (horizontalFirepointX >= 1) { horizontalFirepointX = 1; }
        if (horizontalFirepointX <= -1) { horizontalFirepointX = -1; }
        if (horizontalFirepointY >= 1) { horizontalFirepointY = 1; }
        if (horizontalFirepointY <= -1) { horizontalFirepointY = -1; }
    }

    /* Stary point system, nahrazuji za selection system.
       Soucasti je UnitSelector a UnitDeselector */
    private void Point()
    {
        //musi existovat jen jeden unitselector
        if (unitSelectorBool == false)
        {
            unitSelectorGameObject = Instantiate(unitSelectorTemplate);
            unitSelectorGameObject.transform.parent = this.gameObject.transform;
            unitSelectorGameObject.SetActive(false);
            unitSelectorBool = true;
        }

        //musi existovat jen jeden unitdeselector
        if (unitDeselectorBool == false)
        {
            unitDeselectorGameObject = Instantiate(unitDeselectorTemplate);
            unitDeselectorGameObject.transform.parent = this.gameObject.transform;
            unitDeselectorGameObject.SetActive(false);
            unitDeselectorBool = true;
        }

        KeyCode space = KeyCode.Space;
        KeyCode shift = KeyCode.LeftShift;

        if (Input.GetKey(shift) && Input.GetKey(space))
        {
            animator.SetBool("Pointing", true);
            unitDeselectorGameObject.SetActive(true);
            unitDeselectorGameObject.transform.position = mousePosition;
            unitSelectorGameObject.SetActive(false);
        }
        else if (Input.GetKey(space))
        {
            animator.SetBool("Pointing", true);
            unitSelectorGameObject.SetActive(true);
            unitSelectorGameObject.transform.position = mousePosition;
            unitDeselectorGameObject.SetActive(false);
        }
        else
        {
            animator.SetBool("Pointing", false);
            unitSelectorGameObject.SetActive(false);
            unitDeselectorGameObject.SetActive(false);
            Destroy(unitSelectorGameObject);
            Destroy(unitDeselectorGameObject);
            unitSelectorBool = false;
            unitDeselectorBool = false;
        }
    }

    /** Metoda pro polozeni vlajky na kurzoru hrace. */
    private void createFlag()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject flag = Instantiate(flagTemplate);
            flag.transform.position = mousePosition;
            flagList.Add(flag);
            flagsPutDown++;
        }
        if (flagsPutDown > maxFlags)
        {
            removeLastFlag();
        }
    }

    private void removeLastFlag()
    {
        GameObject firstFlag = flagList[0];
        flagList.Remove(firstFlag);
        flagsPutDown--;
        Destroy(firstFlag);
    }

    public void removeFlag(GameObject flag)
    {
        flagList.Remove(flag);
        flagsPutDown--;
        Destroy(flag);
    }

    /** Metoda pro prikazani kontrolujicich jednotek k obrane */
    private void orderDefend()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (hit.collider != null && hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.tag == "Player Flag"
                    || hit.collider.gameObject.tag == "Player Structure"
                    || hit.collider.gameObject.tag == "Player"
                    )
                {
                    List<GameObject> selectedUnits = GetComponent<UnitController>().GetSelectedUnits();

                    for (int i = 0; i < selectedUnits.Count(); i++)
                    {
                        if(selectedUnits[i].GetComponent<UnitBehavior>().GetFollowTarget() != null)
                        {
                            selectedUnits[i].GetComponent<UnitBehavior>().GetFollowTarget().transform.parent.GetComponent<ObjectFormationController>().RemoveUnit(selectedUnits[i]);
                            //getcomponent in parent
                        }
                        hit.collider.gameObject.GetComponent<ObjectFormationController>().AddUnit(selectedUnits[i]);
                    }
                }
            }
            else
            {
                // Zadny objekt nebyl nalezen
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