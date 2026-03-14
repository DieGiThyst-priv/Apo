using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private BulletObjectPool bulletPool;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int magazineSize = 8;
    [SerializeField] private int remainingShots;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private GameObject reloadingObject;
    [SerializeField] private Animator animator;
    [SerializeField] private string reloadAnimationName = "Reload_First_Clip";
    private bool isReloading;
    private bool canShoot = true;

    private void Start()
    {
        remainingShots = magazineSize;
    }
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Shoot()
    {
        if (isReloading || !canShoot)
            return;
        if (remainingShots <= 0) {
            ReloadGun();
            return;
        }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        GameObject bullet = bulletPool.GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = bullet.transform.right * bulletSpeed;
        }

        remainingShots--;
        StartCoroutine(DeactivateBullet(bullet));
        StartCoroutine(fireRateDelay());
    }

    IEnumerator DeactivateBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(2f);
        bulletPool.ReturnBullet(bullet);
    }

    IEnumerator fireRateDelay() {
        canShoot = false;
        yield return new WaitForSeconds(1f / fireRate);
        canShoot = true;
    }

    public void ReloadGun() {
        if (isReloading)
            return;
        StartCoroutine(ReloadGunCoroutine());
    }

    private IEnumerator ReloadGunCoroutine()
    {
        reloadingObject.SetActive(true);
        isReloading = true;

        AnimatorStateInfo clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = clipInfo.length;

        animator.speed = clipLength / reloadTime;
        animator.Play(reloadAnimationName);

        yield return new WaitForSeconds(reloadTime);

        animator.speed = 1f;

        remainingShots = magazineSize;
        isReloading = false;
        reloadingObject.SetActive(false);
    }
}
