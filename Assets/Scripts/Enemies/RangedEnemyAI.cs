using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// By Vince Pressey

public class RangedEnemyAI : BaseEnemy
{
    /// <summary>
    /// NavmeshAgent component of the enemy
    /// </summary>
    private NavMeshAgent agent;
    /// <summary>
    /// Target of the enemy
    /// </summary>
    private GameObject goal;
    /// <summary>
    /// Position of the gun, used to make gun face the target over the body
    /// </summary>
    [SerializeField]
    private Transform gunPosition;
    /// <summary>
    /// Gun object, rotation is controlled by the gunPosition
    /// </summary>
    [SerializeField]
    private Transform gunObject;
    /// <summary>
    /// Projectile created when attacking
    /// </summary>
    [SerializeField]
    private GameObject projectileObject;

    /// <summary>
    /// Check if enemy has been alerted to player presence
    /// </summary>
    public bool Alerted = false;

    /// <summary>
    /// Max speed for the enemy
    /// </summary>
    [Header("Movement")]   // =============================================#
    public float BaseSpeed = 3.5f;
    /// <summary>
    /// Distance at which to stop following
    /// </summary>
    [SerializeField]
    private float minFollowRange = 10;
    /// <summary>
    /// Distance at which to start fleeing
    /// </summary>
    [SerializeField]
    private float maxFleeDistance = 8;
    /// <summary>
    /// Distance at which AI needs to change location due to being stuck
    /// </summary>
    [SerializeField]
    private float maxStuckDistance = 6;
    /// <summary>
    /// Speed multiplier when following target
    /// </summary>
    [SerializeField]
    private float followSpeedMult = 1.5f;
    /// <summary>
    /// Speed multiplier when fleeing from target
    /// </summary>
    [SerializeField]
    private float fleeSpeedMult = 1f;
    /// <summary>
    /// Speed multiplier when unable to move
    /// </summary>
    [SerializeField]
    private float stuckSpeedMult = 1.5f;

    /// <summary>
    /// Enable the ability of moving side to side while in range
    /// </summary>
    [Header("Dodging")]   // =============================================#
    public bool EnableDodge = true;
    /// <summary>
    /// Speed at which the enemy dodges side to side
    /// </summary>
    public float DodgeSpeed = 0.333f;
    /// <summary>
    /// Direction to move whilst in range
    /// </summary>
    private int dodgeDirection = 1;
    /// <summary>
    /// Direction to move whilst in range
    /// </summary>
    private float dodgeSwapTimer = 1;

    /// <summary>
    /// Range of the Enemy
    /// </summary>
    [Header("Attacking")]   // =============================================#
    public float damage = 15f;
    /// <summary>
    /// Range to be within before Enemy can start attacking
    /// </summary>
    [SerializeField]
    private float attackRange = 15f;
    /// <summary>
    /// Time between initialization of attacks
    /// </summary>
    [SerializeField]
    private float attackCooldown = 1.5f;
    /// <summary>
    /// Current Cooldown of attack
    /// </summary>
    private float curAttackCooldown = 0;
    /// <summary>
    /// Speed applied to projectile
    /// </summary>
    [SerializeField]
    private float projectileSpeed = 10;


    void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        goal = Camera.main.gameObject;
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (enemyKnockedBack)
        {
            return;
        }
        else if (enemyStunned)
        {
            agent.destination = transform.position;
            return;
        }

        if (!Alerted) { return; }

        MoveToTarget(); // Movement

        LookAtTarget(); // Aiming

        curAttackCooldown -= Time.fixedDeltaTime;
        if (CheckCanAttack()) { InitiateAttack(); } // Attacking
    }


    /// <summary>
    /// Move towards goal then stay a set distance away from it, flee when too close to target
    /// </summary>
    public void MoveToTarget()
    {
        agent.destination = goal.transform.position; // Set destination to goal's current position

        if (Vector3.Distance(agent.destination, transform.position) > minFollowRange) // Too far away from target
        {
            agent.speed = BaseSpeed * followSpeedMult;
        }
        else if(Vector3.Distance(agent.destination, transform.position) < maxFleeDistance) // Too close to target, start fleeing
        {
            Vector3 targetPos = goal.transform.position - ((goal.transform.position - transform.position).normalized * minFollowRange); 

            NavMeshHit myNavHit;
            if (NavMesh.SamplePosition(targetPos, out myNavHit, 100, -1)) // Check if target destination is on navmesh and store nearest point
            {

                if (Vector3.Distance(myNavHit.position, targetPos) > maxStuckDistance) // Target is too close and destination is out of reach, likely stuck in corner
                {
                    Vector3 oldPosition = (goal.transform.position - transform.position).normalized; 
                    agent.destination = goal.transform.position + (new Vector3(oldPosition.z, oldPosition.y, oldPosition.x) * 30); // Try another position
                    agent.speed = BaseSpeed * stuckSpeedMult;
                }
                else // Destination is within range to flee to, likely out in the open
                {
                    agent.destination = targetPos; // Go to further away point
                    agent.speed = BaseSpeed * fleeSpeedMult;
                }
            }
        }
        else // Within shooting range and out of fleeing range
        {
            if (EnableDodge) // Start dodging movement
            {
                dodgeSwapTimer -= Time.fixedDeltaTime; 
                if (dodgeSwapTimer < 0) 
                {
                    dodgeDirection *= -1; // Switch direction
                    dodgeSwapTimer = UnityEngine.Random.Range(1f, 3f); // Set random timer for next switch
                }

                agent.destination = transform.position + (2 * dodgeDirection * transform.right); // Target position to the left or right of current position
                agent.speed = BaseSpeed * DodgeSpeed; // Apply dodge speed
            }
            else // Don't dodge
            {
                agent.speed = 0f;
            }

        }

    }


    /// <summary>
    /// Slowly turn towards the target object and adjust gun to target player's Y coord
    /// </summary>
    private void LookAtTarget()
    {
        Vector3 targetDir = goal.transform.position - gunPosition.position; // Get target angle to turn towards
        Vector3 newDir = Vector3.RotateTowards(gunPosition.forward, targetDir, 0.05f, 0.0f); // Calculate next angle
        transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation


        // I hate math
        Vector3 relativePos = goal.transform.position - gunObject.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));
        gunObject.transform.rotation = rotation * Quaternion.Euler(0, -90, 0);
        gunObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, gunObject.transform.localRotation.eulerAngles.z));

    }


    /// <summary>
    /// Check availability to attack
    /// </summary>
    /// <returns>True if goal is within range and not on cooldown</returns>
    public bool CheckCanAttack()
    {
        // Check attack cooldown
        if (curAttackCooldown > 0)
        {
            return false;
        }

        // Check if goal is within range to shoot
        if (Vector3.Distance(transform.position, goal.transform.position) > attackRange)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Create projectile based on gun direction and player position
    /// </summary>
    private void InitiateAttack()
    {
        Vector3 projDir = gunObject.transform.right;

        GameObject projectile = Instantiate(projectileObject, gunPosition.position, Quaternion.identity); // Create projectile
        projectile.GetComponent<Rigidbody>().AddForce(projDir * projectileSpeed, ForceMode.VelocityChange); // Apply directional force

        projectile.GetComponent<ProjectileScript>().ProjectileDamage = damage; // Set damage of projectile

        curAttackCooldown = attackCooldown; // Restart attack cooldown
    }


    /// <summary>
    /// Called when first alerted
    /// </summary>
    private void BecomeAlerted(bool state)
    {
        Alerted = state;
    }

}
