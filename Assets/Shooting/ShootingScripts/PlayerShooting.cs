using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Gun gun;

    private Animator animator;

    [Header("Crosshair")]
    public Texture2D cursorTexture;
    public Vector2 hotstop = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void EquipGun(bool equip)
    {
        animator.SetBool("gunOut", equip);
        gun.gunEquipped = equip;
        gun.gameObject.SetActive(equip);

        if(equip)
        {
            Cursor.SetCursor(cursorTexture, hotstop, cursorMode);
        }
        else
        {
            Cursor.SetCursor(null, hotstop, cursorMode);
        }
        
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Shoot");
        gun.Shoot();
    }

    public void OnReload()
    {
        gun.ReloadGun();
    }

    public void setGunBarrel(Gun barrel)
    {
       this.gun = barrel;
    }
}
 