using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Transform gunBarrel;
    [SerializeField] private BulletObjectPool bulletPool;
    [SerializeField] private float bulletSpeed = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    public void findGunBarrel() {
        this.gunBarrel = transform.Find("GunBarrel");
    }

    public void setGunBarrel(Transform barrel)
    {
       this.gunBarrel = barrel;
    }

    void Shoot()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        GameObject bullet = bulletPool.GetBullet();
        bullet.transform.position = gunBarrel.position;
        bullet.transform.rotation = gunBarrel.rotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = bullet.transform.right * bulletSpeed;
        }

        StartCoroutine(DeactivateBullet(bullet));
    }

    IEnumerator DeactivateBullet(GameObject bullet) {
        yield return new WaitForSeconds(2f);
        bulletPool.ReturnBullet(bullet);
    }
}
 