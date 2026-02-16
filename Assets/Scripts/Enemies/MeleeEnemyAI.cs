using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

// By Vince Pressey

public class MeleeEnemyAI : BaseEnemy
{
    /// <summary>
    /// Target object for the enemy
    /// </summary>
    private GameObject goal;
    /// <summary>
    /// Goal Location
    /// </summary>
    private GameObject goalPos;
    /// <summary>
    /// Current Target location of the enemy
    /// </summary>
    private GameObject curTargetPos;

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
    /// <summary>
    /// Variable for checking if enemy is currently attacking
    /// </summary>
    private bool isAttacking;
    /// <summary>
    /// Use alternative attack where they laucnh themselves at target
    /// </summary>
    [SerializeField]
    private bool launchAtTarget;

    private bool launching;
    private bool hasHitPlayer;
    private float launchCooldown = 1f;
    private float curLaunchCooldown;

    [SerializeField]
    private GameObject[] leftWheels;

    [SerializeField]
    private GameObject[] rightWheels;

    [SerializeField]
    private Animator animator;

    /// <summary>
    /// Position of the enemy on start
    /// </summary>
    private Vector3 startPosition;

    void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        goal = GameObject.FindGameObjectWithTag(Constants.PlayerTag);
        goalPos = Camera.main.gameObject;

        agent = GetComponent<NavMeshAgent>();

        startPosition = transform.position;

    }

    private void FixedUpdate()
    {
        // Base enemy class logic
        if (enemyKnockedBack)
        {
            return;
        }
        else if (enemyStunned)
        {
            agent.destination = transform.position;
            return;
        }

        // Movement
        if (Alerted)
        {
            MoveToTarget(goalPos.transform.position, isAttacking ? baseSpeed / 16 : baseSpeed); // Move to target
        }
        else
        {
            if (Vector3.Distance(transform.position, startPosition) > 2)
            {
                MoveToTarget(startPosition, isAttacking ? baseSpeed / 16 : baseSpeed); // Move back to start position
            }
            else
            {
                RotateWheels(0, 0);
            }
            return;
        }


        curAttackCooldown -= Time.fixedDeltaTime;
        if (CheckCanAttack()) { InitiateAttack(); } // Attacking

        curLaunchCooldown -= Time.fixedDeltaTime;
    }

    /// <summary>
    /// Moves the object towards the target using navmesh, stops and turn when close to the target
    /// </summary>
    public void MoveToTarget(Vector3 target, float aimedBaseSpeed)
    {
        agent.destination = target; // Set destination to goal's current position
        if (isAttacking)
        {
            agent.destination = (transform.position * 7 + target) / 8;
        }


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
        agent.speed = Mathf.Min(aimedBaseSpeed * (leftTreadSpeed + rightTreadSpeed) / turnReduction, aimedBaseSpeed);


        // Swap to alternative movement if close enough to target
        if (Vector3.Distance(transform.position, target) < attackRange)
        {
            nextToSpeed = Mathf.Min(nextToSpeed + 0.02f, 1); // Increase rate of turning 
            agent.speed = 0; // Stop movement

            Vector3 targetDir = target - transform.position; // Get target angle to turn towards
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, nextToSpeed * 0.04f, 0.0f); // Calculate next angle
            transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
        }
        else
        {
            nextToSpeed = Mathf.Max(nextToSpeed - 0.05f, 0); // Decrease rate of turning 
        }


        angularVelocity = transform.rotation.eulerAngles.y - oldRotation.y;
        oldRotation = transform.rotation.eulerAngles; // Save old angle

        RotateWheels(Mathf.Clamp(angularVelocity + agent.speed / 2, -1, 1), Mathf.Clamp(-angularVelocity + agent.speed / 2, -1, 1));
    }

    #region Attacking

    /// <summary>
    /// Checks if the target is within reach
    /// </summary>
    /// <returns>True if the target within range and not on cooldown</returns>
    public bool CheckCanAttack()
    {
        // Check if target within attacking distance
        if (Vector3.Distance(transform.position, goalPos.transform.position) > attackRange)
        {
            return false;
        }

        // Calculate angle between object forward and target
        Vector3 direction = goalPos.transform.position - transform.position;
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

        // Check if already attacking
        if (isAttacking)
        {
            return false;
        }

        if (launchAtTarget)
        {
            if (curLaunchCooldown > 0)
            {
                return false;
            }
        }
        
        // Return true if all checks weren't triggered
        return true;
    }

    /// <summary>
    /// Start the attack
    /// </summary>
    public void InitiateAttack()
    {
        if (launchAtTarget)
        {
            hasHitPlayer = false;
            launching = true;
            KnockbackAI(1);

            float distance = Vector3.Distance(goalPos.transform.position, transform.position);
            Vector3 targetDir = (goalPos.transform.position - transform.position) + Vector3.up / 2 * distance;
            GetComponent<Rigidbody>().AddForce(targetDir.normalized * Mathf.Clamp(distance * 2, 6, 8), ForceMode.VelocityChange);

            return;
        }

        Health healthscript;
        if (healthscript = goal.gameObject.GetComponent<Health>()) // Attack object if it has the health script attached
        {
            if (animator) animator.SetTrigger("Attack");
            isAttacking = true;
        }
    }

    public void AttemptAttack(bool endOfAttacks)
    {
        if (endOfAttacks)
        {
            curAttackCooldown = attackCooldown; // Reset attack cooldown
            isAttacking = false;
            return;
        }

        Collider[] hits = Physics.OverlapBox(transform.position + transform.forward, new Vector3(0.5f, 1.3f, 0.7f), Quaternion.identity);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == goal)
            {
                goal.GetComponent<Health>().AddToHealth(-damage);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (launchAtTarget)
        {
            if (collision.transform.CompareTag("Player") && !hasHitPlayer)
            {
                collision.transform.GetComponent<Health>().AddToHealth(-damage);
                hasHitPlayer = true;
            }
        }

    }

    #endregion Attacking

    public override void GetUp()
    {
        base.GetUp();
        curLaunchCooldown = launchCooldown;
    }

    /// <summary>
    /// Spin the attached tread/wheel objects
    /// </summary>
    /// <param name="leftSpeed">Speed of the left tread/wheels</param>
    /// <param name="rightSpeed">Speed of the right tread/wheels</param>
    private void RotateWheels(float leftSpeed, float rightSpeed)
    {
        foreach (GameObject wheel in leftWheels)
        {
            if (wheel.GetComponent<ScrollingTextureController>())
            {
                wheel.GetComponent<ScrollingTextureController>().ScrollSpeed = leftSpeed;
            }
            else
            {
                wheel.GetComponent<SimpleSpin>().spinSpeed = leftSpeed;
            }
                
        }

        foreach (GameObject wheel in rightWheels)
        {
            if (wheel.GetComponent<ScrollingTextureController>())
            {
                wheel.GetComponent<ScrollingTextureController>().ScrollSpeed = rightSpeed;
            }
            else
            {
                wheel.GetComponent<SimpleSpin>().spinSpeed = rightSpeed;
            }
        }

    }

    /// <summary>
    /// Called when first alerted
    /// </summary>
    private void BecomeAlerted(bool state)
    {
        if (animator) animator.SetBool("Passive", false);
        Alerted = state;
    }

}
