using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Gun gun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            gun.Shoot();
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            gun.ReloadGun();
        }
    }

    public void setGunBarrel(Gun barrel)
    {
       this.gun = barrel;
    }
}
 