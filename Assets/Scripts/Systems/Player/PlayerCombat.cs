using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Player combat controller.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// Disable the player combat and freezing it.
    /// </summary>
    private bool isDisabled = false;

    /// <summary>
    /// Layers the alignment raycast can hit to make the gun fire at the target location.
    /// </summary>
    [SerializeField]
    LayerMask gunAlignmentLayers;

    /// <summary>
    /// The player gun projectile prefab that is fired from the cannon.
    /// </summary>
    [SerializeField]
    GameObject projectilePrefab;

    /// <summary>
    /// The spawn point for the player's cannon projectile.
    /// </summary>
    [SerializeField]
    Transform projectileSpawnLocation;

    /// <summary>
    /// The speed for the player's cannon projectile to fire at.
    /// </summary>
    [SerializeField]
    float projectileSpeed = 10f;

    /// <summary>
    /// How much damage the projectile will do. Stats set this.
    /// </summary>
    float projectileDamage = 12f;


    /// <summary>
    /// The melee box size.
    /// </summary>
    [SerializeField]
    Vector3 meleeBounds = Vector3.one;

    /// <summary>
    /// The melee offset from the camera's position
    /// </summary>
    [SerializeField]
    Vector3 meleeOffset = Vector3.forward;

    /// <summary>
    /// The melee attack interval. Stats set this.
    /// </summary>
    float meleeAttackDelay = 0.5f;

    /// <summary>
    /// The damage the melee will do per hit. Stats set this.
    /// </summary>
    float meleeDamage = 10f;

    /// <summary>
    /// The kick check bounding box size.
    /// </summary>
    [SerializeField, FormerlySerializedAs("kickBounds")]
    Vector3 bashBounds = Vector3.one;

    /// <summary>
    /// The offset for the kick bounding box.
    /// </summary>
    [SerializeField, FormerlySerializedAs("kickOffset")]
    Vector3 bashOffset = Vector3.forward;

    // [SerializeField]
    /// <summary>
    /// The force to apply to objects when they have been kicked. Stats set this.
    /// </summary>
    float bashForce = 10f;

    /// <summary>
    /// The bash attack interval. Stats set this.
    /// </summary>
    float bashAttackDelay = 0.5f;



    float currentProjectileCoolDown = 0f;
    float currentMeleeCoolDown = 0f;
    float currentKickCoolDown = 0f;


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

    #region Mono Behaviour

    void Awake()
    {
        // currentAmmoCount = projectileMagSize;

        rangedWeaponInput = InputSystem.actions.FindAction("Attack");
        meleeWeaponInput = InputSystem.actions.FindAction("Melee");
        kickInput = InputSystem.actions.FindAction("Interact");


        animator = GetComponent<Animator>();
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
        if (isDisabled) return;

        if (currentKickCoolDown > 0) currentKickCoolDown -= Time.deltaTime;
        if (currentMeleeCoolDown > 0) currentMeleeCoolDown -= Time.deltaTime;
        if (currentProjectileCoolDown > 0) currentProjectileCoolDown -= Time.deltaTime;
        if (currentOverheatCoolDown > 0) currentOverheatCoolDown -= Time.deltaTime;

        // did this so it can recharge the weapon before a shot can be fired. otherwise you only shoot one if this is a else if.
        if (currentOverheatCoolDown <= 0 && overheated)
        {
            currentGunChargeBar = 1f;
            overheated = false;
        }


        PollInput();


        WeaponCharging();




        if (wantToFireRanged && currentOverheatCoolDown <= 0)
        {
            rechargeDelay = delayAfterFireBeforeRecharging;

            velocity = 1;

            if (currentProjectileCoolDown <= 0)
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


        if (wantToMelee && currentMeleeCoolDown <= 0)
        {
            MeleeAttack();
        }

        if (wantToKick && currentKickCoolDown <= 0)
        {
            BashAttack();
        }

        // if (currentChargeBar <= 0 && !isRecharging)
        // {
        //     // Reload();
        //     // ShowRechargeBar();
        // }

    }

    void OnDrawGizmos()
    {
        if (showMeleeBox && Camera.main != null)
        {
            // Gizmos.matrix = Matrix4x4.identity; // reset the matrix.
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * meleeOffset.z) + (cam.right * meleeOffset.x) + (cam.up * meleeOffset.y);


            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation(cam.forward, cam.up),
                cam.localScale);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, meleeBounds);
        }

        if (showKickBox && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * bashOffset.z) + (cam.right * bashOffset.x) + (cam.up * bashOffset.y);

            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation(cam.forward, cam.up),
                cam.localScale);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, bashBounds);
        }
    }

    #endregion


    #region Functions

    public void UpdateVariablesWithStats(PlayerStats stats)
    {
        if (stats == null)
        {
            Debug.LogError("No player stats! Using default values!");
            stats = new PlayerStats();
            // return;
        }

        projectileDamage = stats.ProjectileDamageStat.GetCurrentValue();
        rechargeRate = 1f / stats.RechargeSecondsStat.GetCurrentValue();
        shotsPerFullCharge = (int)stats.ShotsPerFullChargeStat.GetCurrentValue();
        standardSecondsPerShot = stats.StandardSecondsPerShot;
        chargedSecondsPerShot = stats.ChargedSecondsPerShot;
        delayAfterFireBeforeRecharging = stats.DelayAfterFireBeforeRecharging;
        overheatForceCoolDown = stats.OverheatForceCoolDownStat.GetCurrentValue();
        // projectileFireRate = stats.ProjectileFireRate;
        // projectileMagSize = stats.ProjectileMagSize;

        meleeAttackDelay = stats.MeleeAttackDelayStat.GetCurrentValue();
        meleeDamage = stats.MeleeDamageStat.GetCurrentValue();
        meleeBounds.z = stats.MeleeReachStat.GetCurrentValue();


        bashForce = stats.BashForceStat.GetCurrentValue();
        bashAttackDelay = stats.BashAttackDelayStat.GetCurrentValue();
        // reloadTime = stats.ReloadTime;
        // rechargeRate = stats.ReloadTime;

        // TODO: calc charge here.
    }


    void PollInput()
    {
        wantToFireRanged = rangedWeaponInput.IsPressed();
        wantToMelee = meleeWeaponInput.IsPressed();
        wantToKick = kickInput.IsPressed();
    }

    private void WeaponCharging()
    {
        currentMeleeChargeBar = currentMeleeCoolDown / (meleeAttackDelay - 0.05f);
        currentBashChargeBar = currentKickCoolDown / (bashAttackDelay - 0.05f);
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




    private void BashAttack()
    {
        // does knock back
        animator.SetTrigger("Bash");

        // if (currentKickCoolDown > 0) return; // Dunno if i want to do timer check here or update?
        Collider[] hits = Physics.OverlapBox(mainCamera.position + (mainCamera.forward * bashOffset.z) + (mainCamera.right * bashOffset.x) + (mainCamera.up * bashOffset.y), bashBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            foreach (Collider c in hits)
            {
                Vector3 kickDir = c.transform.position - transform.position;
                c.GetComponent<IKickable>()?.KickObject(kickDir * bashForce, ForceMode.VelocityChange);
            }
        }

        //Debug.Log("Kick!");

        currentKickCoolDown = bashAttackDelay;

    }


    private void MeleeAttack()
    {
        // does damage

        animator.SetTrigger("Melee");

        Collider[] hits = Physics.OverlapBox(mainCamera.position + (mainCamera.forward * meleeOffset.z) + (mainCamera.right * meleeOffset.x) + (mainCamera.up * meleeOffset.y), meleeBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            // damage
            foreach (Collider c in hits)
            {
                // print(c.gameObject.name);
                if (c.gameObject.CompareTag(Constants.PlayerTag)) continue; // if player, go away.
                c.GetComponent<IMeleeAble>()?.MeleeObject();


                c.transform.GetComponent<IDamageable>()?.TakeDamage(meleeDamage, mainCamera.position + mainCamera.forward); // deal damage.

            }
        }

        Debug.Log("Melee!");

        currentMeleeCoolDown = meleeAttackDelay;
    }



    private void FireProjectile()
    {
        if (currentGunChargeBar < chargeDegradePerShot)
        {
            currentOverheatCoolDown = overheatForceCoolDown;
            overheated = true;
        }

        currentGunChargeBar -= chargeDegradePerShot;
        // currentProjectileCooldown = projectileFireRate;
        currentProjectileCoolDown = Mathf.Lerp(standardSecondsPerShot, chargedSecondsPerShot, EasingFunctions.EaseOutQuint(currentGunChargeBar / 2));

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
    #endregion


    #region Getters
    public float GetOverheatCoolDownNormalized()
    {
        return currentOverheatCoolDown / overheatForceCoolDown;
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

    public bool IsCombatDisabled()
    {
        return isDisabled;
    }

    #endregion



    #region Setters
    public void DisablePlayerCombat(bool state = false)
    {
        isDisabled = state;
    }

    #endregion
}
