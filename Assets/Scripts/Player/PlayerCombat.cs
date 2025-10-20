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


    // PlayerMovement playerMovement;
    Transform mainCamera;

    bool wantToFireRanged = false;
    bool wantToMelee = false;
    bool wantToKick = false;
    // bool wantToReload = false;

    InputAction rangedWeaponInput;
    InputAction meleeWeaponInput;
    InputAction KickInput;
    InputAction ReloadInput;

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
        KickInput = InputSystem.actions.FindAction("Interact");
        ReloadInput = InputSystem.actions.FindAction("Reload");

        animator = GetComponent<Animator>();
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

                c.transform.GetComponent<Health>()?.AddToHealth(-meleeDamage); // deal damage.
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
        wantToKick = KickInput.IsPressed();
        // wantToReload = ReloadInput.IsPressed();
    }

    #region DisablePlayerCombat
    #endregion
    public void DisablePlayerCombat(bool state)
    {
        isDisabled = state;
    }
}
