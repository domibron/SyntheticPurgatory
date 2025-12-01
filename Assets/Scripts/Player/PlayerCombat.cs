using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// Disable all combat abilities if enabled
    /// </summary>
    public bool IsDisabled = false;

    [SerializeField]
    LayerMask gunAlignmentLayers;

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    Transform projectileSpawnLocation;

    [SerializeField]
    float projectileSpeed = 10f;

    // [SerializeField]
    float projectileDamage = 12f;


    // float reloadTime = 2f;

    // float currentReloadTime = 0f;

    // bool isReloading = false;

    [SerializeField]
    Vector3 meleeBounds = Vector3.one;

    [SerializeField]
    Vector3 meleeOffset = Vector3.forward;

    // [SerializeField]
    float meleeAttackDelay = 0.5f;

    // [SerializeField]
    float meleeDamage = 10f;

    [SerializeField]
    Vector3 kickBounds = Vector3.one;

    [SerializeField]
    Vector3 kickOffset = Vector3.forward;

    // [SerializeField]
    float kickForce = 10f; // 

    // [SerializeField]
    float kickAttackDelay = 0.5f; //

    // int currentAmmoCount = 0;s

    // float currentKickCooldown = 0;

    float currentProjectileCooldown = 0f;
    float currentMeleeCooldown = 0f;
    float currentKickCooldown = 0f;

    // NEW CANNON
    // Charging
    bool isRecharging = false;

    // bool cursorGoingRight = true;

    // int shotsPerFullCharge = 6; // 6 shots for a full charged bar (excluding overcharge)
    // float chargePerShot = 0.5f; // How much to add or remove from current charge for a single shot.
    float currentGunChargeBar = 1f;
    float rechargeRate = 0.3f; // TODO: add to stats
    float currentMeleeChargeBar = 1f;
    float currentBashChargeBar = 1f;

    int shotsPerFullCharge = 12; // TODO: add to stats
    float chargeDegradePerShot { get => 1f / shotsPerFullCharge; } // 8 shots before standard.

    float standardSecondsPerShot = 0.4f; // TODO: add to stats
    float chargedSecondsPerShot = 0.1f;  // TODO: add to stats

    float delayAfterFireBeforeRecharging = 0.4f;  // TODO: add to stats
    float rechargeDelay = 0f;

    float overheatForceCoolDown = 2.25f; // TODO: add to stats
    float currentOverheatCoolDown = 0f;

    [SerializeField]
    private Transform gunSpinBit;

    private float velocity = 0f;

    [SerializeField]
    private float spinRate = 20f;

    // float cursorScrollSpeed = 1;

    // float currentCursorPosition = 0.5f;

    // bool isChargeOnLeftSide = false;
    // float chargeUpPos = 0.3f;
    // float chargeSize = 0.2f; // full width is 0.5f since the bars are split in half.

    // int missDenominator = 4;

    // bool hasPressedCharge = false;

    // float rechargeRatePerShot = 0.1f; // 1th of a second.

    // // mainly used for the player's hud.
    // public Action OnChargeSuccess;
    // public Action OnChargeFail;
    // public Action OnShowChargeBar;
    // public Action OnHideChargeBar;





    // PlayerMovement playerMovement;
    Transform mainCamera;

    bool wantToFireRanged = false;
    bool wantToMelee = false;
    bool wantToKick = false;

    InputAction rangedWeaponInput;
    InputAction meleeWeaponInput;
    InputAction kickInput;

    [SerializeField]
    bool showMeleeBox = false;

    [SerializeField]
    bool showKickBox = false;

    Animator animator;

    bool overheated = false;

    #region Awake
    #endregion
    void Awake()
    {
        // currentAmmoCount = projectileMagSize;

        rangedWeaponInput = InputSystem.actions.FindAction("Attack");
        meleeWeaponInput = InputSystem.actions.FindAction("Melee");
        kickInput = InputSystem.actions.FindAction("Interact");


        animator = GetComponent<Animator>();

        // // TODO: Move to stats read write thingy. // EPIK COMMENT
        // chargePerShot = 1f / (float)shotsPerFullCharge;s
    }

    #region Start
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main.transform;
        // playerMovement = GetComponent<PlayerMovement>();
    }

    #region UpdateVariablesWithStats
    #endregion

    public void UpdateVariablesWithStats(PlayerStats stats)
    {
        if (stats == null)
        {
            Debug.LogError("No player stats! Using default values!");
            stats = new PlayerStats();
            // return;
        }

        projectileDamage = stats.ProjectileDamage;
        rechargeRate = stats.RechargeRate;
        shotsPerFullCharge = (int)stats.ShotsPerFullCharge;
        standardSecondsPerShot = stats.StandardSecondsPerShot;
        chargedSecondsPerShot = stats.ChargedSecondsPerShot;
        delayAfterFireBeforeRecharging = stats.DelayAfterFireBeforeRecharging;
        overheatForceCoolDown = stats.OverheatForceCooldown;
        // projectileFireRate = stats.ProjectileFireRate;
        // projectileMagSize = stats.ProjectileMagSize;

        meleeAttackDelay = stats.MeleeAttackDelay;
        meleeDamage = stats.MeleeDamage;
        meleeBounds.z = stats.MeleeReach;


        kickForce = stats.KickForce;
        kickAttackDelay = stats.KickAttackDelay;
        // reloadTime = stats.ReloadTime;
        // rechargeRate = stats.ReloadTime;

        // TODO: calc charge heere.
    }

    #region Update
    #endregion
    // Update is called once per frame
    void Update()
    {
        if (IsDisabled) return;

        if (currentKickCooldown > 0) currentKickCooldown -= Time.deltaTime;
        if (currentMeleeCooldown > 0) currentMeleeCooldown -= Time.deltaTime;
        if (currentProjectileCooldown > 0) currentProjectileCooldown -= Time.deltaTime;
        if (currentOverheatCoolDown > 0) currentOverheatCoolDown -= Time.deltaTime;

        // did this so it can recharge the weapon before a shot can be fired. otherwise you only shoot one if this is a else if.
        if (currentOverheatCoolDown <= 0 && overheated)
        {
            currentGunChargeBar = 1f;
            overheated = false;
        }

        // if (currentReloadTime > 0) currentReloadTime -= Time.deltaTime;
        // else if (currentKickCooldown <= 0 && isReloading)
        // {
        //     currentAmmoCount = projectileMagSize;
        //     isReloading = false;
        // }

        PollInput();


        WeaponCharging();




        if (wantToFireRanged && currentOverheatCoolDown <= 0)
        {
            rechargeDelay = delayAfterFireBeforeRecharging;

            velocity = 1;

            if (currentProjectileCooldown <= 0)
            {
                FireProjectile();
            }
        }
        else
        {
            velocity -= Time.deltaTime * 10f * Mathf.Lerp(standardSecondsPerShot, chargedSecondsPerShot, EasingFunctions.EaseOutQuint(currentGunChargeBar));
        }

        velocity = Mathf.Clamp01(velocity);
        // TODO: fix later
        gunSpinBit.Rotate(Vector3.forward * velocity * spinRate); // * Mathf.Lerp(standardSecondsPerShot, chargedSecondsPerShot, EasingFunctions.EaseOutQuint(currentChargeBar)));


        if (wantToMelee && currentMeleeCooldown <= 0)
        {
            MeleeAttack();
        }

        if (wantToKick && currentKickCooldown <= 0)
        {
            BashAttack();
        }

        // if (currentChargeBar <= 0 && !isRecharging)
        // {
        //     // Reload();
        //     // ShowRechargeBar();
        // }

    }

    private void WeaponCharging()
    {
        currentMeleeChargeBar = currentMeleeCooldown / (meleeAttackDelay - 0.05f);
        currentBashChargeBar = currentKickCooldown / (kickAttackDelay - 0.05f);
        if (currentOverheatCoolDown > 0) return;

        if (rechargeDelay <= 0)
        {
            isRecharging = true;
        }
        else if (rechargeDelay > 0)
        {
            rechargeDelay -= Time.deltaTime;
            isRecharging = false;
        }

        if (isRecharging)
        {
            currentGunChargeBar += Time.deltaTime * rechargeRate;
        }
        currentGunChargeBar = Mathf.Clamp01(currentGunChargeBar);
    }

    public float GetGunChargeAmount()
    {
        return currentGunChargeBar;
    }
    public float GetMeleeChargeAmount()
    {
        return currentMeleeChargeBar;
    }
    public float GetBashChargeAmount()
    {
        return currentBashChargeBar;
    }


    #region OnDrawGizmos
    #endregion
    void OnDrawGizmos()
    {
        if (showMeleeBox && Camera.main != null)
        {
            // Gizmos.matrix = Matrix4x4.identity; // reset the matrix.
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * meleeOffset.z) + (cam.right * meleeOffset.x) + (cam.up * meleeOffset.y);

            // newTransform.position = cam.position + (cam.forward * meleeOffset.z) + (cam.right * meleeOffset.x) + (cam.up * meleeOffset.y);
            // Gizmos.matrix = Matrix4x4.TRS(offsetPos,
            //     Quaternion.LookRotation((offsetPos - cam.position), cam.up),
            //     cam.localScale);
            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation(cam.forward, cam.up),
                cam.localScale);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, meleeBounds);
        }

        if (showKickBox && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * kickOffset.z) + (cam.right * kickOffset.x) + (cam.up * kickOffset.y);

            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation(cam.forward, cam.up),
                cam.localScale);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, kickBounds);
        }
    }

    // #region Reload
    // #endregion
    // private void Reload()
    // {
    //     isReloading = true;
    //     currentReloadTime = reloadTime;
    // }
    #region BashAttack
    #endregion
    private void BashAttack()
    {
        // does knockback
        animator.SetTrigger("Bash");

        // if (currentKickCooldown > 0) return; // Dunno if i want to do timer check here or update?
        Collider[] hits = Physics.OverlapBox(mainCamera.position + (mainCamera.forward * kickOffset.z) + (mainCamera.right * kickOffset.x) + (mainCamera.up * kickOffset.y), kickBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            foreach (Collider c in hits)
            {
                Vector3 kickDir = c.transform.position - transform.position;
                c.GetComponent<IKickable>()?.KickObject(kickDir * kickForce, ForceMode.VelocityChange);
                // IKickable[] kickables = c.GetComponents<IKickable>();
                // if (kickables.Length > 0)
                // {
                //     foreach (var kickable in kickables)
                //     {
                //         kickable.KickObject(kickDir * kickForce, ForceMode.VelocityChange);
                //     }
                // }
            }
        }

        //Debug.Log("Kick!");

        currentKickCooldown = kickAttackDelay;

    }

    #region MeleeAttack
    #endregion
    private void MeleeAttack()
    {
        // does damage

        // if (currentMeleeCooldown > 0) return;
        animator.SetTrigger("Melee");

        Collider[] hits = Physics.OverlapBox(mainCamera.position + (mainCamera.forward * meleeOffset.z) + (mainCamera.right * meleeOffset.x) + (mainCamera.up * meleeOffset.y), meleeBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            // damage
            foreach (Collider c in hits)
            {
                // print(c.gameObject.name);
                if (c.gameObject.CompareTag(Constants.PlayerTag)) continue; // if player, go away.
                c.GetComponent<IMeleeable>()?.MeleeObject();
                // IMeleeable[] meleeables = c.GetComponents<IMeleeable>();
                // if (meleeables.Length > 0)
                // {
                //     foreach (var meleeable in meleeables)
                //     {
                //         meleeable.MeleeObject();
                //     }
                // }

                c.transform.GetComponent<IDamageable>()?.TakeDamage(meleeDamage, c.transform.position); // deal damage.

            }
        }

        Debug.Log("Melee!");

        currentMeleeCooldown = meleeAttackDelay;
    }

    #region FireProjectile
    #endregion
    private void FireProjectile()
    {
        if (currentGunChargeBar < chargeDegradePerShot)
        {
            currentOverheatCoolDown = overheatForceCoolDown;
            overheated = true;
        }

        currentGunChargeBar -= chargeDegradePerShot;
        // currentProjectileCooldown = projectileFireRate;
        currentProjectileCooldown = Mathf.Lerp(standardSecondsPerShot, chargedSecondsPerShot, EasingFunctions.EaseOutQuint(currentGunChargeBar / 2));

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnLocation.position, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().ProjectileDamage = projectileDamage;

        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, 999))
        {
            // we hit, so we fire towards target. we add a little offset to allow the projectile to not be aids. but this whole thing sucks.
            Vector3 dirNeeded = ((hit.point + (mainCamera.forward * 3f)) - projectile.transform.position).normalized;
            projectileRB.AddForce(dirNeeded * projectileSpeed, ForceMode.VelocityChange);
        }
        else
        {
            projectileRB.AddForce(mainCamera.forward * projectileSpeed, ForceMode.VelocityChange);
        }


        // projectile.GetComp<>().SetDamage();

        // Debug.Log("Fired ranged weapon");

    }

    public float GetOverheatCoolDownNormalized()
    {
        return currentOverheatCoolDown / overheatForceCoolDown;
    }

    #region PollInput
    #endregion
    void PollInput()
    {
        wantToFireRanged = rangedWeaponInput.IsPressed();
        wantToMelee = meleeWeaponInput.IsPressed();
        wantToKick = kickInput.IsPressed();
    }

    // public int GetCurrentAmmo()
    // {
    //     return currentAmmoCount;
    // }


    #region DisablePlayerCombat
    #endregion
    public void DisablePlayerCombat(bool state)
    {
        IsDisabled = state;
    }
}
