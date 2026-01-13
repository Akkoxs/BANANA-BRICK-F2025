using System;
using UnityEngine;

public class MouseAimingSubmarine : MonoBehaviour
{
    [Header("Aiming Settings")]
    [SerializeField] private Transform reticle;
    [SerializeField] private float aimRadius = 3f;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    [Header("Shooter and Ammo Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileTransform;
    [SerializeField] private float timeBetweenFiring = 0.5f;
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private float reloadTime = 3f;

    private float timer;
    private bool canFire;
    private bool shoot;
    private Vector3 mousePos;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    void Start()
    {
        Cursor.visible = false;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        currentAmmo = maxAmmo;
        canFire = true;
    }

    void Update()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - transform.position).normalized;
        Vector2 reticlePos = (Vector2)transform.position + (direction * aimRadius);
        if (reticle != null) { reticle.position = reticlePos; }

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= reloadTime)
            {
                // Reload complete
                currentAmmo = maxAmmo;
                isReloading = false;
                reloadTimer = 0f;
                canFire = true;
                Debug.Log("reload complete ammo: " + currentAmmo);
            }
            return;
        }

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (canFire && shoot)
        {
            if (currentAmmo > 0)
            {
                Torpedo projectile = Instantiate(projectilePrefab, projectileTransform.position, Quaternion.identity).GetComponent<Torpedo>();
                projectile.InitializeProjectile(mousePos, mainCamera);

                currentAmmo--;
                Debug.Log("ammo remaining: " + currentAmmo);

                shoot = false;
                canFire = false;

                if (currentAmmo <= 0)
                {
                    isReloading = true;
                    reloadTimer = 0f;
                    Debug.Log("reloading...");
                }
            }
            else
            {
                shoot = false;
                isReloading = true;
                reloadTimer = 0f;
            }
        }
        else if (!canFire && shoot)
        {
            shoot = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aimRadius);
    }

    public Vector3 GetMousePos()
    {
        return mousePos;
    }

    public void TriggerShoot(bool shoot)
    {
        this.shoot = shoot;
    }

    public void SetReloadSpeed(float newReloadSpeed)
    {
        timeBetweenFiring = newReloadSpeed;
    }
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public float GetReloadProgress()
    {
        if (!isReloading) return 1f;
        return reloadTimer / reloadTime;
    }
}