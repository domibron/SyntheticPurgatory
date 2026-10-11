using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Player combat controller.
/// </summary>
public class PlayerCombat : MonoBehaviour
{

    private PlayerDisabling m_playerDisabling;

    /// <summary>
    /// The player gun projectile prefab that is fired from the cannon.
    /// </summary>
    [SerializeField, FormerlySerializedAs("m_projectilePrefab")]
    GameObject m_projectilePrefab;

    /// <summary>
    /// The spawn point for the player's cannon projectile.
    /// </summary>
    [SerializeField, FormerlySerializedAs("projectileSpawnLocation")]
    Transform m_projectileSpawnLocation;

    /// <summary>
    /// The speed for the player's cannon projectile to fire at.
    /// </summary>
    [SerializeField, FormerlySerializedAs("projectileSpeed")]
    float m_projectileSpeed = 10f;

    /// <summary>
    /// How much damage the projectile will do. Stats set this.
    /// </summary>
    float m_projectileDamage = 12f;


    /// <summary>
    /// The melee box size.
    /// </summary>
    [SerializeField, FormerlySerializedAs("meleeBounds")]
    Vector3 m_meleeBounds = Vector3.one;

    /// <summary>
    /// The melee offset from the camera's position
    /// </summary>
    [SerializeField, FormerlySerializedAs("meleeOffset")]
    Vector3 m_meleeOffset = Vector3.forward;

    /// <summary>
    /// The melee attack interval. Stats set this.
    /// </summary>
    float m_meleeAttackDelay = 0.5f;

    /// <summary>
    /// The damage the melee will do per hit. Stats set this.
    /// </summary>
    float m_meleeDamage = 10f;

    /// <summary>
    /// The kick check bounding box size.
    /// </summary>
    [SerializeField, FormerlySerializedAs("bashBounds")]
    Vector3 m_bashBounds = Vector3.one;

    /// <summary>
    /// The offset for the kick bounding box.
    /// </summary>
    [SerializeField, FormerlySerializedAs("bashOffset")]
    Vector3 m_bashOffset = Vector3.forward;

    // [SerializeField]
    /// <summary>
    /// The force to apply to objects when they have been kicked. Stats set this.
    /// </summary>
    float m_bashForce = 10f;

    /// <summary>
    /// The bash attack interval. Stats set this.
    /// </summary>
    float m_bashAttackDelay = 0.5f;




    /// <summary>
    /// The current cannon fire cool down before next firing.
    /// </summary>
    float m_currentProjectileCoolDown = 0f;

    /// <summary>
    /// The current wait before the next melee can begin.
    /// </summary>
    float m_currentMeleeCoolDown = 0f;

    /// <summary>
    /// The current wait before the next kick can begin.
    /// </summary>
    float m_currentKickCoolDown = 0f;


    /// <summary>
    /// Is the cannon allowed to recharge.
    /// </summary>
    bool m_canStartRecharge = false;


    /// <summary>
    /// The current charge of the cannon.
    /// </summary>
    float m_currentGunChargeBar = 1f;

    /// <summary>
    /// How fast the cannon recharges. Charge per second. Stats sets this.
    /// </summary>
    float m_rechargeRate = 0.3f;

    /// <summary>
    /// The current charge amount of the melee.
    /// </summary>
    float m_currentMeleeChargeBar = 1f;

    /// <summary>
    /// The current charge amount of the bash.
    /// </summary>
    float m_currentBashChargeBar = 1f;


    /// <summary>
    /// How many shots before needing to recharge fully. Stats sets this.
    /// </summary>
    int m_shotsPerFullCharge = 12;

    /// <summary>
    /// How much to reduce the charge for the cannon per shot.
    /// </summary>
    float m_chargeDegradePerShot { get => 1f / m_shotsPerFullCharge; } // 8 shots before standard.



    /// <summary>
    /// How fast to fire when low on charge. Stats sets this.
    /// </summary>
    float m_standardSecondsPerShot = 0.4f;

    /// <summary>
    /// How fast the cannon fires when fully charged. Stats sets this.
    /// </summary>
    float m_chargedSecondsPerShot = 0.1f;

    /// <summary>
    /// The delay after a shot before the cannon can start recharging. Stats sets this.
    /// </summary>
    float m_delayAfterFireBeforeRecharging = 0.4f;

    /// <summary>
    /// The current wait time before the cannon can being recharging.
    /// </summary>
    float m_rechargeDelay = 0f;

    /// <summary>
    /// How long to disable the cannon before the player can fire again after full depletion. Stats sets this.
    /// </summary>
    float m_overheatForceCoolDown = 2.25f;

    /// <summary>
    /// The current overheat cool down for the cannon.
    /// </summary>
    float m_currentOverheatCoolDown = 0f;

    /// <summary>
    /// Has the cannon overheated requiring a forced cool down.
    /// </summary>
    bool m_isCannonOverheated = false;



    // TODO: move since this is visual and not related to the combat system.
    /// <summary>
    /// The cannon end to rotate when firing.
    /// </summary>
    [SerializeField, FormerlySerializedAs("gunSpinBit")]
    private Transform m_gunSpinBit;

    /// <summary>
    /// The velocity of the rotating cannon.
    /// </summary>
    private float m_velocity = 0f;

    /// <summary>
    /// How fast the spin the end of the cannon.
    /// </summary>
    [SerializeField, FormerlySerializedAs("spinRate")]
    private float m_spinRate = 20f;



    /// <summary>
    /// The main camera to base aiming off of.
    /// </summary>
    Transform m_mainCamera;



    /// <summary>
    /// Is the fire key being held.
    /// </summary>
    bool m_wantToFireRanged = false;

    /// <summary>
    /// Is the melee key being held.
    /// </summary>
    bool m_wantToMelee = false;

    /// <summary>
    /// Is the kick key being held.
    /// </summary>
    bool m_wantToBash = false;


    /// <summary>
    /// The fire cannon key to bind to.
    /// </summary>
    InputAction m_rangedWeaponInput;

    /// <summary>
    /// The melee key to bind to.
    /// </summary>
    InputAction m_meleeWeaponInput;

    /// <summary>
    /// The bash key to bind to.
    /// </summary>
    InputAction m_bashInput;





    /// <summary>
    /// Debug to show the melee attack box.
    /// </summary>
    [SerializeField, FormerlySerializedAs("showMeleeBox")]
    bool m_showMeleeBox = false;

    /// <summary>
    /// Debug to show the bash attack box.
    /// </summary>
    [SerializeField, FormerlySerializedAs("showBashBox")]
    bool m_showBashBox = false;

    /// <summary>
    /// The weapon animator to control.
    /// </summary>
    Animator m_animator;

    WeaponMovement m_weaponMovement;



    #region Mono Behaviour

    void Awake()
    {
        // currentAmmoCount = projectileMagSize;

        m_rangedWeaponInput = InputSystem.actions.FindAction("Attack");
        m_meleeWeaponInput = InputSystem.actions.FindAction("Melee");
        m_bashInput = InputSystem.actions.FindAction("Interact");

        m_playerDisabling = GetComponent<PlayerDisabling>();
        m_animator = GetComponent<Animator>();
        m_weaponMovement = GetComponentInChildren<WeaponMovement>(); // Only one on children so it should be fine.
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_mainCamera = Camera.main.transform;
        // playerMovement = GetComponent<PlayerMovement>();
    }



    // Update is called once per frame
    void Update()
    {
        // Code disabiler.
        if (m_playerDisabling.IsDisabled(PlayerDisabling.DisabledType.Combat)) return;

        if (m_currentKickCoolDown > 0) m_currentKickCoolDown -= Time.deltaTime;
        if (m_currentMeleeCoolDown > 0) m_currentMeleeCoolDown -= Time.deltaTime;
        if (m_currentProjectileCoolDown > 0) m_currentProjectileCoolDown -= Time.deltaTime;
        if (m_currentOverheatCoolDown > 0) m_currentOverheatCoolDown -= Time.deltaTime;

        // Reset weapon charge after cooling off.
        if (m_currentOverheatCoolDown <= 0 && m_isCannonOverheated)
        {
            m_currentGunChargeBar = 1f;
            m_isCannonOverheated = false;
        }



        PollInput();


        // Cannon firing.
        WeaponCharging();
        FireWeaponOnDemand();

        // Weapon spinning.
        m_velocity = Mathf.Clamp01(m_velocity);
        // TODO: fix later
        m_gunSpinBit.Rotate(Vector3.forward * m_velocity * m_spinRate); // * Mathf.Lerp(standardSecondsPerShot, chargedSecondsPerShot, EasingFunctions.EaseOutQuint(currentChargeBar)));


        if (m_wantToMelee && m_currentMeleeCoolDown <= 0)
        {
            MeleeAttack();
        }

        if (m_wantToBash && m_currentKickCoolDown <= 0)
        {
            BashAttack();
        }
    }

    private void FireWeaponOnDemand()
    {
        if (m_wantToFireRanged && m_currentOverheatCoolDown <= 0)
        {
            m_rechargeDelay = m_delayAfterFireBeforeRecharging;

            m_velocity = 1;

            if (m_currentProjectileCoolDown <= 0)
            {
                FireProjectile();
            }
        }
        else
        {
            m_velocity -= Time.deltaTime * 10f * Mathf.Lerp(m_standardSecondsPerShot, m_chargedSecondsPerShot, EasingFunctions.EaseOutQuint(m_currentGunChargeBar));
        }
    }

    void OnDrawGizmos()
    {
        if (m_showMeleeBox && Camera.main != null)
        {
            // Gizmos.matrix = Matrix4x4.identity; // reset the matrix.
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * m_meleeOffset.z) + (cam.right * m_meleeOffset.x) + (cam.up * m_meleeOffset.y);


            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation(cam.forward, cam.up),
                cam.localScale);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, m_meleeBounds);
        }

        if (m_showBashBox && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * m_bashOffset.z) + (cam.right * m_bashOffset.x) + (cam.up * m_bashOffset.y);

            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation(cam.forward, cam.up),
                cam.localScale);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, m_bashBounds);
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

        m_projectileDamage = stats.ProjectileDamageStat.GetCurrentValue();
        m_rechargeRate = 1f / stats.RechargeSecondsStat.GetCurrentValue();
        m_shotsPerFullCharge = (int)stats.ShotsPerFullChargeStat.GetCurrentValue();
        m_standardSecondsPerShot = stats.StandardSecondsPerShot;
        m_chargedSecondsPerShot = stats.ChargedSecondsPerShot;
        m_delayAfterFireBeforeRecharging = stats.DelayAfterFireBeforeRecharging;
        m_overheatForceCoolDown = stats.OverheatForceCoolDownStat.GetCurrentValue();
        // projectileFireRate = stats.ProjectileFireRate;
        // projectileMagSize = stats.ProjectileMagSize;

        m_meleeAttackDelay = stats.MeleeAttackDelayStat.GetCurrentValue();
        m_meleeDamage = stats.MeleeDamageStat.GetCurrentValue();
        m_meleeBounds.z = stats.MeleeReachStat.GetCurrentValue();


        m_bashForce = stats.BashForceStat.GetCurrentValue();
        m_bashAttackDelay = stats.BashAttackDelayStat.GetCurrentValue();
        // reloadTime = stats.ReloadTime;
        // rechargeRate = stats.ReloadTime;

        // TODO: calc charge here.
    }

    /// <summary>
    /// Check if the keys were pressed.
    /// </summary>
    void PollInput()
    {
        m_wantToFireRanged = m_rangedWeaponInput.IsPressed();
        m_wantToMelee = m_meleeWeaponInput.IsPressed();
        m_wantToBash = m_bashInput.IsPressed();
    }

    /// <summary>
    /// Handles the weapon recharging.
    /// </summary>
    private void WeaponCharging()
    {
        m_currentMeleeChargeBar = m_currentMeleeCoolDown / (m_meleeAttackDelay - 0.05f);
        m_currentBashChargeBar = m_currentKickCoolDown / (m_bashAttackDelay - 0.05f);
        if (m_currentOverheatCoolDown > 0) return;


        if (m_rechargeDelay <= 0)
        {
            m_canStartRecharge = true;
        }
        else if (m_rechargeDelay > 0)
        {
            m_rechargeDelay -= Time.deltaTime;
            m_canStartRecharge = false;
        }

        if (m_canStartRecharge)
        {
            m_currentGunChargeBar += Time.deltaTime * m_rechargeRate;
        }

        m_currentGunChargeBar = Mathf.Clamp01(m_currentGunChargeBar);
    }



    /// <summary>
    /// Does the bash attack.
    /// </summary>
    private void BashAttack()
    {
        // does knock back
        m_animator.SetTrigger("Bash");

        // if (currentKickCoolDown > 0) return; // Dunno if i want to do timer check here or update?
        Collider[] hits = Physics.OverlapBox(m_mainCamera.position + (m_mainCamera.forward * m_bashOffset.z) + (m_mainCamera.right * m_bashOffset.x) + (m_mainCamera.up * m_bashOffset.y), m_bashBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            foreach (Collider c in hits)
            {
                Vector3 kickDir = c.transform.position - transform.position;
                c.GetComponent<IKickable>()?.KickObject(kickDir * m_bashForce, ForceMode.VelocityChange);
            }
        }

        //Debug.Log("Kick!");

        m_currentKickCoolDown = m_bashAttackDelay;

    }


    /// <summary>
    /// Does the melee attack.
    /// </summary>
    private void MeleeAttack()
    {
        // does damage

        m_animator.SetTrigger("Melee");

        Collider[] hits = Physics.OverlapBox(m_mainCamera.position + (m_mainCamera.forward * m_meleeOffset.z) + (m_mainCamera.right * m_meleeOffset.x) + (m_mainCamera.up * m_meleeOffset.y), m_meleeBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            // damage
            foreach (Collider c in hits)
            {
                // print(c.gameObject.name);
                if (c.gameObject.CompareTag(Constants.PlayerTag)) continue; // if player, go away.
                c.GetComponent<IMeleeAble>()?.MeleeObject();


                c.transform.GetComponent<IDamageable>()?.TakeDamage(m_meleeDamage, m_mainCamera.position + m_mainCamera.forward); // deal damage.

            }
        }

        Debug.Log("Melee!");

        m_currentMeleeCoolDown = m_meleeAttackDelay;
    }


    /// <summary>
    /// Fires the cannon.
    /// </summary>
    private void FireProjectile()
    {
        m_weaponMovement.PushBack(0.01f);

        // Cooldown code / overheat code.
        if (m_currentGunChargeBar < m_chargeDegradePerShot)
        {
            m_currentOverheatCoolDown = m_overheatForceCoolDown;
            m_isCannonOverheated = true;
        }

        m_currentGunChargeBar -= m_chargeDegradePerShot;
        // currentProjectileCooldown = projectileFireRate;
        m_currentProjectileCoolDown = Mathf.Lerp(m_standardSecondsPerShot, m_chargedSecondsPerShot, EasingFunctions.EaseOutQuint(m_currentGunChargeBar / 2));


        // Projectile firing.
        GameObject projectile = Instantiate(m_projectilePrefab, m_mainCamera.position, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().ProjectileDamage = m_projectileDamage;

        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();
        projectileRB.AddForce(m_mainCamera.forward * m_projectileSpeed, ForceMode.VelocityChange);

        projectile.GetComponentInChildren<VisualFollowTarget>().SetVisualTargetLocation(m_projectileSpawnLocation.position, m_mainCamera.forward * m_projectileSpeed);

    }
    #endregion


    #region Getters
    /// <summary>
    /// Get the overheat cool down time as a value between 0 and 1.
    /// </summary>
    /// <returns></returns>
    public float GetOverheatCoolDownNormalized()
    {
        return m_currentOverheatCoolDown / m_overheatForceCoolDown;
    }

    /// <summary>
    /// Get the cannon charge amount. Already 0 - 1.
    /// </summary>
    /// <returns></returns>
    public float GetCannonChargeAmount()
    {
        return m_currentGunChargeBar;
    }

    /// <summary>
    /// Get the melee charge amount. Already 0 - 1.
    /// </summary>
    /// <returns></returns>
    public float GetMeleeChargeAmount()
    {
        return m_currentMeleeChargeBar;
    }


    /// <summary>
    /// Get the bash charge amount. Already 0 - 1.
    /// </summary>
    /// <returns></returns>
    public float GetBashChargeAmount()
    {
        return m_currentBashChargeBar;
    }

    #endregion
}
