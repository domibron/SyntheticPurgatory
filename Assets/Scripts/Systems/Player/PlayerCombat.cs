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




    /// <summary>
    /// The current cannon fire cool down before next firing.
    /// </summary>
    float currentProjectileCoolDown = 0f;

    /// <summary>
    /// The current wait before the next melee can begin.
    /// </summary>
    float currentMeleeCoolDown = 0f;

    /// <summary>
    /// The current wait before the next kick can begin.
    /// </summary>
    float currentKickCoolDown = 0f;


    /// <summary>
    /// Is the cannon allowed to recharge.
    /// </summary>
    bool canStartRecharge = false;


    /// <summary>
    /// The current charge of the cannon.
    /// </summary>
    float currentGunChargeBar = 1f;

    /// <summary>
    /// How fast the cannon recharges. Charge per second. Stats sets this.
    /// </summary>
    float rechargeRate = 0.3f;

    /// <summary>
    /// The current charge amount of the melee.
    /// </summary>
    float currentMeleeChargeBar = 1f;

    /// <summary>
    /// The current charge amount of the bash.
    /// </summary>
    float currentBashChargeBar = 1f;


    /// <summary>
    /// How many shots before needing to recharge fully. Stats sets this.
    /// </summary>
    int shotsPerFullCharge = 12;

    /// <summary>
    /// How much to reduce the charge for the cannon per shot.
    /// </summary>
    float chargeDegradePerShot { get => 1f / shotsPerFullCharge; } // 8 shots before standard.



    /// <summary>
    /// How fast to fire when low on charge. Stats sets this.
    /// </summary>
    float standardSecondsPerShot = 0.4f;

    /// <summary>
    /// How fast the cannon fires when fully charged. Stats sets this.
    /// </summary>
    float chargedSecondsPerShot = 0.1f;

    /// <summary>
    /// The delay after a shot before the cannon can start recharging. Stats sets this.
    /// </summary>
    float delayAfterFireBeforeRecharging = 0.4f;

    /// <summary>
    /// The current wait time before the cannon can being recharging.
    /// </summary>
    float rechargeDelay = 0f;

    /// <summary>
    /// How long to disable the cannon before the player can fire again after full depletion. Stats sets this.
    /// </summary>
    float overheatForceCoolDown = 2.25f;

    /// <summary>
    /// The current overheat cool down for the cannon.
    /// </summary>
    float currentOverheatCoolDown = 0f;

    /// <summary>
    /// Has the cannon overheated requiring a forced cool down.
    /// </summary>
    bool isCannonOverheated = false;



    // TODO: move since this is visual and not related to the combat system.
    /// <summary>
    /// The cannon end to rotate when firing.
    /// </summary>
    [SerializeField]
    private Transform gunSpinBit;

    /// <summary>
    /// The velocity of the rotating cannon.
    /// </summary>
    private float velocity = 0f;

    /// <summary>
    /// How fast the spin the end of the cannon.
    /// </summary>
    [SerializeField]
    private float spinRate = 20f;



    /// <summary>
    /// The main camera to base aiming off of.
    /// </summary>
    Transform mainCamera;



    /// <summary>
    /// Is the fire key being held.
    /// </summary>
    bool wantToFireRanged = false;

    /// <summary>
    /// Is the melee key being held.
    /// </summary>
    bool wantToMelee = false;

    /// <summary>
    /// Is the kick key being held.
    /// </summary>
    bool wantToBash = false;


    /// <summary>
    /// The fire cannon key to bind to.
    /// </summary>
    InputAction rangedWeaponInput;

    /// <summary>
    /// The melee key to bind to.
    /// </summary>
    InputAction meleeWeaponInput;

    /// <summary>
    /// The bash key to bind to.
    /// </summary>
    InputAction bashInput;





    /// <summary>
    /// Debug to show the melee attack box.
    /// </summary>
    [SerializeField]
    bool showMeleeBox = false;

    /// <summary>
    /// Debug to show the bash attack box.
    /// </summary>
    [SerializeField]
    bool showBashBox = false;

    /// <summary>
    /// The weapon animator to control.
    /// </summary>
    Animator animator;



    #region Mono Behaviour

    void Awake()
    {
        // currentAmmoCount = projectileMagSize;

        rangedWeaponInput = InputSystem.actions.FindAction("Attack");
        meleeWeaponInput = InputSystem.actions.FindAction("Melee");
        bashInput = InputSystem.actions.FindAction("Interact");


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
        if (currentOverheatCoolDown <= 0 && isCannonOverheated)
        {
            currentGunChargeBar = 1f;
            isCannonOverheated = false;
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

        if (wantToBash && currentKickCoolDown <= 0)
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

        if (showBashBox && Camera.main != null)
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

    /// <summary>
    /// Updates the player variables with the player stats.
    /// </summary>
    /// <param name="stats">The player stats to get the stats from.</param>
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

    /// <summary>
    /// Check if the keys were pressed.
    /// </summary>
    void PollInput()
    {
        wantToFireRanged = rangedWeaponInput.IsPressed();
        wantToMelee = meleeWeaponInput.IsPressed();
        wantToBash = bashInput.IsPressed();
    }

    /// <summary>
    /// Handles the weapon recharging.
    /// </summary>
    private void WeaponCharging()
    {
        currentMeleeChargeBar = currentMeleeCoolDown / (meleeAttackDelay - 0.05f);
        currentBashChargeBar = currentKickCoolDown / (bashAttackDelay - 0.05f);
        if (currentOverheatCoolDown > 0) return;


        if (rechargeDelay <= 0)
        {
            canStartRecharge = true;
        }
        else if (rechargeDelay > 0)
        {
            rechargeDelay -= Time.deltaTime;
            canStartRecharge = false;
        }

        if (canStartRecharge)
        {
            currentGunChargeBar += Time.deltaTime * rechargeRate;
        }

        currentGunChargeBar = Mathf.Clamp01(currentGunChargeBar);
    }



    /// <summary>
    /// Does the bash attack.
    /// </summary>
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


    /// <summary>
    /// Does the melee attack.
    /// </summary>
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


    /// <summary>
    /// Fires the cannon.
    /// </summary>
    private void FireProjectile()
    {
        if (currentGunChargeBar < chargeDegradePerShot)
        {
            currentOverheatCoolDown = overheatForceCoolDown;
            isCannonOverheated = true;
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
    /// <summary>
    /// Get the overheat cool down time as a value between 0 and 1.
    /// </summary>
    /// <returns></returns>
    public float GetOverheatCoolDownNormalized()
    {
        return currentOverheatCoolDown / overheatForceCoolDown;
    }

    /// <summary>
    /// Get the cannon charge amount. Already 0 - 1.
    /// </summary>
    /// <returns></returns>
    public float GetCannonChargeAmount()
    {
        return currentGunChargeBar;
    }

    /// <summary>
    /// Get the melee charge amount. Already 0 - 1.
    /// </summary>
    /// <returns></returns>
    public float GetMeleeChargeAmount()
    {
        return currentMeleeChargeBar;
    }


    /// <summary>
    /// Get the bash charge amount. Already 0 - 1.
    /// </summary>
    /// <returns></returns>
    public float GetBashChargeAmount()
    {
        return currentBashChargeBar;
    }


    /// <summary>
    /// Is the combat disabled.
    /// </summary>
    /// <returns></returns>
    public bool IsCombatDisabled()
    {
        return isDisabled;
    }

    #endregion



    #region Setters
    /// <summary>
    /// Set if the combat is disabled. Blocks all combat and cool downs essentially disabling this script.
    /// </summary>
    /// <param name="state">True to disable the combat of the player.</param>
    public void DisablePlayerCombat(bool state = false)
    {
        isDisabled = state;
    }

    #endregion
}
