using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    Transform projectileSpawnLocation;

    [SerializeField]
    float projectileSpeed = 10f;

    [SerializeField]
    float projectileDamage = 20f;

    [SerializeField]
    float projectileFireRate = 0.3f;

    [SerializeField]
    int projectileMagSize = 20;

    [SerializeField]
    float meleeAttackDelay = 0.5f;

    [SerializeField]
    float meleeDamage = 10f;

    [SerializeField]
    float kickPushForce = 10f;

    [SerializeField]
    float kickAttackDelay = 0.5f;

    int currentAmmoCount = 0;


    float currentProjectileCooldown = 0f;
    float currentMeleeCooldown = 0f;
    float currentKickCooldown = 0f;


    // PlayerMovement playerMovement;
    Transform mainCamera;

    bool wantToFireRanged = false;
    bool wantToMelee = false;
    bool wantToKick = false;

    InputAction rangedWeaponInput;
    InputAction meleeWeaponInput;
    InputAction KickInput;

    // TODO reload

    void Awake()
    {
        currentAmmoCount = projectileMagSize;

        rangedWeaponInput = InputSystem.actions.FindAction("Attack");
        meleeWeaponInput = InputSystem.actions.FindAction("Melee");
        KickInput = InputSystem.actions.FindAction("Interact");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main.transform;
        // playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentKickCooldown > 0) currentKickCooldown -= Time.deltaTime;
        if (currentMeleeCooldown > 0) currentKickCooldown -= Time.deltaTime;
        if (currentProjectileCooldown > 0) currentProjectileCooldown -= Time.deltaTime;

        PollInput();

        if (wantToFireRanged && currentAmmoCount > 0 && currentProjectileCooldown <= 0)
        {
            FireProjectile();
        }

        if (wantToMelee && currentMeleeCooldown > 0)
        {
            MeleeAttack();
        }

        if (wantToKick && currentKickCooldown > 0)
        {
            KickAttack();
        }

    }

    private void KickAttack()
    {
        // does knockback

    }

    private void MeleeAttack()
    {
        // does damage

    }

    private void FireProjectile()
    {
        currentAmmoCount--;
        currentProjectileCooldown = projectileFireRate;

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnLocation.position, Quaternion.identity);

        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, 999))
        {
            // we hit, so we fire towards target.
            Vector3 dirNeeded = (hit.point - projectile.transform.position).normalized;
            projectileRB.AddForce(dirNeeded * projectileSpeed, ForceMode.VelocityChange);
        }
        else
        {
            projectileRB.AddForce(projectile.transform.forward * projectileSpeed, ForceMode.VelocityChange);
        }

        Debug.Log("Fired ranged weapon");

        // set damage and so on.
    }

    void PollInput()
    {
        wantToFireRanged = rangedWeaponInput.IsPressed();
        wantToMelee = meleeWeaponInput.IsPressed();
        wantToKick = KickInput.IsPressed();
    }
}
