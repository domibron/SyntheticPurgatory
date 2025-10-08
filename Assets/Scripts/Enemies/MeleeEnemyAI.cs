using UnityEngine;
using UnityEngine.AI;

// By Vince Pressey

public class MeleeEnemyAI : MonoBehaviour
{
    /// <summary>
    /// NavmeshAgent component of the enemy
    /// </summary>
    private NavMeshAgent agent;
    /// <summary>
    /// Target of the enemy
    /// </summary>
    [SerializeField]
    private GameObject goal;


    /// <summary>
    /// Max speed for the enemy
    /// </summary>
    [Header("Movement"), SerializeField]
    private float baseSpeed = 3.5f;
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
    /// Range of the Enemy
    /// </summary>
    [Header("Attacking"), SerializeField]
    private float attackRange = 1.5f;
    /// <summary>
    /// Max angle for attack from centre
    /// </summary>
    [SerializeField]
    private float attackCone = 0.3f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        MoveToTarget(); // Movement

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
    /// <returns>True if the target within range and attack angle</returns>
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

        // Return true if previous two checks weren't triggered
        return true;
    }

    /// <summary>
    /// Start the attack
    /// </summary>
    public void InitiateAttack()
    {
        // Code here
    }

}
