using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] private GameObject RightBarrel;
    [SerializeField] private GameObject LeftBarrel;
    [SerializeField] private GameObject UpBarrel;
    [SerializeField] private GameObject DownBarrel;
    [SerializeField] private GameObject Player;

    private Animator playerAnimator;
    private Vector2 Direction;
    private bool isReloading;
    public bool gunEquipped = false;
    private bool canShoot = true;
    private float angle;

    private void Awake()
    {
        remainingShots = magazineSize;
        playerAnimator = Player.GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Direction = (mousePos - transform.position);

        angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        mousePos.z = 0f;
        float animatorDirectionX = (mousePos.x - transform.position.x);
        float animatorMagnitudeX = Mathf.Sign(mousePos.x - transform.position.x);

        playerAnimator.SetFloat("mouseX", animatorDirectionX);
        playerAnimator.SetFloat("mouseMagnitudeX", animatorMagnitudeX);
        playerAnimator.SetFloat("mouseY", Direction.y);
    }

    public Transform GetActiveBarrel()
    {
        var clipInfo = playerAnimator.GetCurrentAnimatorClipInfo(0);
        
        string dominantClipName = "";
        float maxWeight = -1f;

        foreach (var info in clipInfo)
        {
            if (info.weight > maxWeight)
            {
                maxWeight = info.weight;
                dominantClipName = info.clip.name;
            }
        }
        if (dominantClipName.Contains("Up"))    return UpBarrel.transform;
        if (dominantClipName.Contains("Down"))  return DownBarrel.transform;
        if (dominantClipName.Contains("Left"))  return LeftBarrel.transform;
        if (dominantClipName.Contains("Right")) return RightBarrel.transform;

        return DownBarrel.transform; 
    }

    public void Shoot()
    {
        if (isReloading || !gunEquipped || !canShoot)
        return;
        if (remainingShots <= 0) {
            ReloadGun();
            return;
        }
        GameObject bullet = bulletPool.GetBullet();

        float scalar = 1f;
        float scaledDirectionX = Direction.x * scalar;

        Transform firePoint = GetActiveBarrel();
        
        bullet.transform.position = firePoint.position;

        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

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
