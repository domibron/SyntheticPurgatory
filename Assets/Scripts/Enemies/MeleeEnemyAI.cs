using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// By Vince Pressey

public class MeleeEnemyAI : BaseEnemy
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
    /// Check if enemy has been alerted to player presence
    /// </summary>
    public bool Alerted = false;

    /// <summary>
    /// Normal speed of the enemy
    /// </summary>
    [Header("Movement"), SerializeField]
    public float baseSpeed;
    /// <summary>
    /// Reduction of speed when turning
    /// </summary>
    [SerializeField]
    private float turnReduction = 5;
    /// <summary>
    /// Turn speed handler
    /// </summary>
    private float nextToSpeed = 0;
    /// <summary>
    /// Stored rotation from previous fixed frame
    /// </summary>
    private Vector3 oldRotation;

    /// <summary>
    /// Damage dealt on attack
    /// </summary>
    [Header("Attacking"), SerializeField]
    private float damage;
    /// <summary>
    /// Range of the Enemy
    /// </summary>
    [SerializeField]
    private float attackRange = 1.5f;
    /// <summary>
    /// Max angle for attack from centre
    /// </summary>
    [SerializeField]
    private float attackCone = 0.3f;
    /// <summary>
    /// Time between initialization of attacks
    /// </summary>
    [SerializeField]
    private float attackCooldown = 1;
    /// <summary>
    /// Current Cooldown of attack
    /// </summary>
    private float curAttackCooldown = 0;


    void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        goal = GameObject.FindWithTag("Player");
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

        curAttackCooldown -= Time.fixedDeltaTime;
        if (CheckCanAttack()) { InitiateAttack(); } // Attacking

    }

    /// <summary>
    /// Moves the object towards the target using navmesh, stops and turn when close to the target
    /// </summary>
    public void MoveToTarget()
    {
        agent.destination = goal.transform.position; // Set destination to goal's current position


        // Variables for seperate tread speeds
        float leftTreadSpeed = 1;
        float rightTreadSpeed = 1;

        float angularVelocity = transform.rotation.eulerAngles.y - oldRotation.y; // Get angle difference from previous frame
        if (angularVelocity > 0) // Checks if enemy is turning left
        {
            rightTreadSpeed /= Mathf.Max(0.01f, angularVelocity); // Reduces right tread speed
        }
        else if (angularVelocity < 0) // Checks if enemy is turning right
        {
            leftTreadSpeed /= Mathf.Max(0.01f, -angularVelocity); // Reduces left tread speed
        }

        // Set new speed based on speed of the treads
        agent.speed = Mathf.Min(baseSpeed * (leftTreadSpeed + rightTreadSpeed) / turnReduction, baseSpeed);


        // Swap to alternative movement if close enough to target
        if (Vector3.Distance(transform.position, goal.transform.position) < attackRange)
        {
            nextToSpeed = Mathf.Min(nextToSpeed + 0.02f, 1); // Increase rate of turning 
            agent.speed = 0; // Stop movement

            Vector3 targetDir = goal.transform.position - transform.position; // Get target angle to turn towards
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, nextToSpeed * 0.04f, 0.0f); // Calculate next angle
            transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
        }
        else
        {
            nextToSpeed = Mathf.Max(nextToSpeed - 0.05f, 0); // Decrease rate of turning 
        }

        //print("left: " + leftTreadSpeed + "   right: " + rightTreadSpeed);
        oldRotation = transform.rotation.eulerAngles; // Save old angle
    }


    /// <summary>
    /// Checks if the target is within reach
    /// </summary>
    /// <returns>True if the target within range and not on cooldown</returns>
    public bool CheckCanAttack()
    {
        // Check if target within attacking distance
        if (Vector3.Distance(transform.position, goal.transform.position) > attackRange)
        {
            return false;
        }

        // Calculate angle between object forward and target
        Vector3 direction = goal.transform.position - transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        if (toRotation.y > attackCone || toRotation.y < -attackCone)
        {
            return false;
        }

        // Check attack cooldown
        if (curAttackCooldown > 0)
        {
            return false;
        }

        // Return true if all checks weren't triggered
        return true;
    }


    /// <summary>
    /// Start the attack
    /// </summary>
    public void InitiateAttack()
    {
        curAttackCooldown = attackCooldown; // Reset attack cooldown

        Health healthscript;
        if (healthscript = goal.gameObject.GetComponent<Health>()) // Attack object if it has the health script attached
        {
            healthscript.AddToHealth(-damage);
        }
    }


    /// <summary>
    /// Called when first alerted
    /// </summary>
    private void BecomeAlerted(bool state)
    {
        Alerted = state;
    }

}
