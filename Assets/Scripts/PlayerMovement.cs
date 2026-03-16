using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Vector2 moveInput { get; private set; }

    public GameObject gunBody;
    private Animator animator;

    [Header("Crosshair")]
    public Texture2D cursorTexture;
    public Vector2 hotstop = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    public void EquipGun(bool equip)
    {
        animator.SetBool("gunOut", equip);
        
        Gun gun = gunBody.GetComponentInChildren<Gun>();
        gun.canShoot = equip;
        if(equip)
        {
            Cursor.SetCursor(cursorTexture, hotstop, cursorMode);
        }
        else
        {
            animator.SetBool("isShooting", false);
            Cursor.SetCursor(null, hotstop, cursorMode);
        }
        
    }
}
