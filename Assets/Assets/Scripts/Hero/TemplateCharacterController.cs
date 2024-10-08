using Cinemachine;
using UnityEngine;

public class TemplateCharacterController : MonoBehaviour, IController
{
    //COMPONENTY
    [SerializeField] Rigidbody2D rb2d;

    //MOVEMENT
    public float moveSpeed = 5f;
    public Vector2 movement;
    public Vector2 mousePosition;
    public Camera cam;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField] float directionX;
    [SerializeField] float directionY;

    //ANIMATOR
    Animator animator;

    void Awake()
    { 
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Controll()
    {
        LookAtMouse();
        Movement();
    }

    public void LookAtMouse()
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

    public void Movement()
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
}
