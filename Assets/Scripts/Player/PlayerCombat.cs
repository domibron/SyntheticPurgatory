using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// Disable all combat abilities if enabled
    /// </summary>
    [SerializeField]
    private bool isDisabled = false;

    [SerializeField]
    GameObject projectilePrefab;

    [SerializeField]
    Transform projectileSpawnLocation;

    [SerializeField]
    float projectileSpeed = 10f;

    // [SerializeField]
    float projectileDamage = 12f;

    // [SerializeField]
    float projectileFireRate = 0.3f;

    // [SerializeField]
    int projectileMagSize = 20;

    float reloadTime = 2f;

    float currentReloadTime = 0f;

    bool isReloading = false;

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

    int currentAmmoCount = 0;

    // float currentKickCooldown = 0;

    float currentProjectileCooldown = 0f;
    float currentMeleeCooldown = 0f;
    float currentKickCooldown = 0f;

    // NEW CANNON
    // Charging
    bool isRecharging = false; // UI display or something.

    bool cursorGoingRight = true;

    int shotsPerFullCharge = 6; // 6 shots for a full charged bar (excluding overcharge)
    float chargePerShot = 0.5f; // How much to add or remove from current charge for a single shot.
    float currentChargeBar = 1f;

    float cursorScrollSpeed = 1;

    float currentCursorPosition = 0.5f;

    bool isChargeOnLeftSide = false;
    float chargeUpPos = 0.3f;
    float chargeSize = 0.2f; // full width is 0.5f since the bars are split in half.

    int missDenominator = 4;

    bool hasPressedCharge = false;

    float rechargeRatePerShot = 0.1f; // 1th of a second.

    // mainly used for the player's hud.
    public Action OnChargeSuccess;
    public Action OnChargeFail;
    public Action OnShowChargeBar;
    public Action OnHideChargeBar;

    // Firing




    // PlayerMovement playerMovement;
    Transform mainCamera;

    bool wantToFireRanged = false;
    bool wantToMelee = false;
    bool wantToKick = false;

    bool wantLeftCharge = false;
    bool wantRightCharge = false;
    // bool wantToReload = false;

    InputAction rangedWeaponInput;
    InputAction meleeWeaponInput;
    InputAction kickInput;
    InputAction ReloadInput;
    InputAction leftChargeInput;
    InputAction rightChargeInput;

    [SerializeField]
    bool showMeleeBox = false;

    [SerializeField]
    bool showKickBox = false;

    Animator animator;


    #region Awake
    #endregion
    void Awake()
    {
        currentAmmoCount = projectileMagSize;

        rangedWeaponInput = InputSystem.actions.FindAction("Attack");
        meleeWeaponInput = InputSystem.actions.FindAction("Melee");
        kickInput = InputSystem.actions.FindAction("Interact");
        ReloadInput = InputSystem.actions.FindAction("Reload");
        leftChargeInput = InputSystem.actions.FindAction("LeftCharge");
        rightChargeInput = InputSystem.actions.FindAction("RightCharge");


        animator = GetComponent<Animator>();

        // TODO: Move to stats read write thingy. // EPIK COMMENT
        chargePerShot = 1f / (float)shotsPerFullCharge;
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
        projectileFireRate = stats.ProjectileFireRate;
        projectileMagSize = stats.ProjectileMagSize;

        meleeAttackDelay = stats.MeleeAttackDelay;
        meleeDamage = stats.MeleeDamage;

        kickForce = stats.KickForce;
        kickAttackDelay = stats.KickAttackDelay;
        reloadTime = stats.ReloadTime;

        // TODO: calc charge heere.
    }

    #region Update
    #endregion
    // Update is called once per frame
    void Update()
    {
        if (isDisabled) return;

        if (currentKickCooldown > 0) currentKickCooldown -= Time.deltaTime;
        if (currentMeleeCooldown > 0) currentMeleeCooldown -= Time.deltaTime;
        if (currentProjectileCooldown > 0) currentProjectileCooldown -= Time.deltaTime;

        if (currentReloadTime > 0) currentReloadTime -= Time.deltaTime;
        else if (currentKickCooldown <= 0 && isReloading)
        {
            currentAmmoCount = projectileMagSize;
            isReloading = false;
        }

        PollInput();

        // Cursor stuff for charging.

        WeaponCharging();



        // end of cursor stuff



        if (wantToFireRanged && currentAmmoCount > 0 && currentProjectileCooldown <= 0)
        {
            FireProjectile();
        }

        if (wantToMelee && currentMeleeCooldown <= 0)
        {
            MeleeAttack();
        }

        if (wantToKick && currentKickCooldown <= 0)
        {
            KickAttack();
        }

        if (currentAmmoCount <= 0 && !isReloading)
        {
            Reload();
        }

    }

    private void WeaponCharging()
    {
        if (!isRecharging)
        {
            if (wantLeftCharge || wantRightCharge) // TODO: have input determine charge location.
            {
                ResetCharge();
                isRecharging = true;
            }
        }


        // passive recharge for the losers that cant hit any skill checks.
        if (currentChargeBar < 1)
        {
            currentChargeBar += Time.deltaTime * chargePerShot * rechargeRatePerShot;
        }

        // Cursor movement.
        if (cursorGoingRight)
        {
            currentCursorPosition += Time.deltaTime * cursorScrollSpeed;
        }
        else
        {
            currentCursorPosition -= Time.deltaTime * cursorScrollSpeed;
        }


        if (currentCursorPosition >= 1)
        {
            currentCursorPosition = 1;
            cursorGoingRight = false;
        }
        else if (currentCursorPosition <= 0)
        {
            currentCursorPosition = 0;
            cursorGoingRight = true;
        }


        if (hasPressedCharge && !wantLeftCharge && !wantRightCharge) hasPressedCharge = false;

        if (wantLeftCharge && !hasPressedCharge && isChargeOnLeftSide && currentCursorPosition < 0.5f)
        {
            if (IsCursourOverCheck())
            {
                // we pass
                currentChargeBar += chargePerShot;
                cursorGoingRight = true;
            }
            else
            {
                currentChargeBar += chargePerShot / (float)missDenominator;

            }


            if (currentCursorPosition < 0.5f)
                isChargeOnLeftSide = false;
            else
                isChargeOnLeftSide = true;

            hasPressedCharge = true;
            SetNewChargePos(isChargeOnLeftSide);
            // cursorGoingRight = !cursorGoingRight;
        }

        if (wantRightCharge && !hasPressedCharge && !isChargeOnLeftSide && currentCursorPosition > 0.5f)
        {
            if (IsCursourOverCheck())
            {
                // we pass
                currentChargeBar += chargePerShot;
                cursorGoingRight = false;
            }
            else
            {
                currentChargeBar += chargePerShot / (float)missDenominator;
            }

            if (currentCursorPosition < 0.5f)
                isChargeOnLeftSide = false;
            else
                isChargeOnLeftSide = true;

            hasPressedCharge = true;
            SetNewChargePos(isChargeOnLeftSide);
            // cursorGoingRight = !cursorGoingRight;
        }

        if (wantLeftCharge && wantRightCharge)
        {
            currentChargeBar = 0f;
        }

        if (currentChargeBar > 1)
        {
            currentChargeBar = 1f;
        }
        else if (currentChargeBar < 0f)
        {
            currentChargeBar = 0f;
        }
    }

    float GetChargeSize()
    {
        return 0.2f;
    }

    void SetNewChargePos(bool generateOnLeft)
    {
        chargeSize = GetChargeSize(); // 
        float halfOfChargeSize = chargeSize / 2f;

        if (generateOnLeft)
        {
            chargeUpPos = UnityEngine.Random.Range(0 + halfOfChargeSize, 0.5f - halfOfChargeSize);
        }
        else
        {
            chargeUpPos = UnityEngine.Random.Range(0.5f + halfOfChargeSize, 1f - halfOfChargeSize);
        }

        // set transforms and such but that will be handled elsewhere.
    }

    void ResetCharge()
    {
        currentCursorPosition = 0.5f;
        cursorGoingRight = true;
        isChargeOnLeftSide = false;
        hasPressedCharge = false;
        SetNewChargePos(isChargeOnLeftSide);
    }


    bool IsCursourOverCheck()
    {
        float lowerBound = chargeUpPos - (chargeSize / 2f);
        float upperBound = chargeUpPos + (chargeSize / 2f);

        if (currentCursorPosition >= lowerBound && currentCursorPosition <= upperBound)
        {
            return true;
        }

        return false;
    }

    public bool IsChargeOnLeftSide()
    {
        return isChargeOnLeftSide;
    }

    public float GetCursorPos()
    {
        return currentCursorPosition;
    }

    public float GetChargeUpSize()
    {
        return chargeSize;
    }

    public float GetChargeUpPos()
    {
        return chargeUpPos;
    }

    public float GetChargeAmount()
    {
        return currentChargeBar;
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
            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation((offsetPos - cam.position), cam.up),
                cam.localScale);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, meleeBounds);
        }

        if (showKickBox && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            Vector3 offsetPos = cam.position + (cam.forward * kickOffset.z) + (cam.right * kickOffset.x) + (cam.up * kickOffset.y);

            Gizmos.matrix = Matrix4x4.TRS(offsetPos,
                Quaternion.LookRotation((offsetPos - cam.position), cam.up),
                cam.localScale);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, kickBounds);
        }
    }

    #region Reload
    #endregion
    private void Reload()
    {
        isReloading = true;
        currentReloadTime = reloadTime;
    }
    #region KickAttack
    #endregion
    private void KickAttack()
    {
        // does knockback

        // if (currentKickCooldown > 0) return; // Dunno if i want to do timer check here or update?
        Collider[] hits = Physics.OverlapBox(mainCamera.position + (mainCamera.forward * kickOffset.z) + (mainCamera.right * kickOffset.x) + (mainCamera.up * kickOffset.y), kickBounds / 2f, transform.rotation);

        if (hits.Length > 0)
        {
            foreach (Collider c in hits)
            {
                Vector3 kickDir = c.transform.position - transform.position;
                c.GetComponent<IKickable>()?.KickObject(kickDir * kickForce, ForceMode.VelocityChange);
            }
        }

        Debug.Log("Kick!");

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
        currentAmmoCount--;
        currentProjectileCooldown = projectileFireRate;

        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnLocation.position, Quaternion.identity);
        projectile.GetComponent<ProjectileScript>().ProjectileDamage = projectileDamage;

        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, 999))
        {
            // we hit, so we fire towards target.
            Vector3 dirNeeded = (hit.point - projectile.transform.position).normalized;
            projectileRB.AddForce(dirNeeded * projectileSpeed, ForceMode.VelocityChange);
        }
        else
        {
            projectileRB.AddForce(mainCamera.forward * projectileSpeed, ForceMode.VelocityChange);
        }

        // projectile.GetComp<>().SetDamage();

        // Debug.Log("Fired ranged weapon");

        // set damage and so on.
    }

    #region PollInput
    #endregion
    void PollInput()
    {
        wantToFireRanged = rangedWeaponInput.IsPressed();
        wantToMelee = meleeWeaponInput.IsPressed();
        wantToKick = kickInput.IsPressed();
        // wantToReload = ReloadInput.IsPressed();
        wantLeftCharge = leftChargeInput.IsPressed();
        wantRightCharge = rightChargeInput.IsPressed();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmoCount;
    }

    public int GetMaxAmmo()
    {
        return projectileMagSize;
    }

    #region DisablePlayerCombat
    #endregion
    public void DisablePlayerCombat(bool state)
    {
        isDisabled = state;
    }
}
