using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TankEnemyAI : MonoBehaviour
{
    /// <summary>
    /// NavmeshAgent component of the enemy
    /// </summary>
    private NavMeshAgent agent;
    /// <summary>
    /// Target of the enemy
    /// </summary>
    private GameObject goal;

    private Rigidbody rb;

    /// <summary>
    /// Check if enemy has been alerted to player presence
    /// </summary>
    public bool Alerted = false;

    private bool isCharging;

    [SerializeField]
    private float maxChargeSpeed;

    private float chargeActivationTime = 2;

    private float chargeCharge = 0;

    private float currentChargeSpeed = 40;

    void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        goal = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        Invoke("ChargeAtTarget", 6);
    }



    void FixedUpdate()
    {
        if (isCharging) 
        { 

            rb.AddForce(transform.forward * currentChargeSpeed, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > currentChargeSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxChargeSpeed;
            }
            
            return;
        }

        if (agent.remainingDistance < 9)
        {
            float nextToSpeed = 4; //Mathf.Min(nextToSpeed + 0.02f, 1); // Increase rate of turning 
            agent.speed = 0; // Stop movement

            Vector3 targetDir = goal.transform.position - transform.position; // Get target angle to turn towards
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, nextToSpeed * 0.04f, 0.0f); // Calculate next angle
            transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
        }

        if (agent.remainingDistance < 10 && chargeCharge < chargeActivationTime)
        {
            chargeCharge += Time.fixedDeltaTime;
            if (chargeCharge > chargeActivationTime)
            {
                StartCoroutine(ChargeAtTarget());
            }
            return;
        }

        chargeCharge = Mathf.Max(0, chargeCharge - Time.fixedDeltaTime * 2);

        agent.destination = goal.transform.position;
    }

    private IEnumerator ChargeAtTarget()
    {
        float totalChargeTime = 0;
        isCharging = true;
        agent.enabled = false;
        rb.angularDamping = 100;
        currentChargeSpeed = maxChargeSpeed;

        yield return new WaitForSeconds(1);

        while (rb.linearVelocity.magnitude > 1)
        {
            totalChargeTime += 0.1f;
            if(totalChargeTime > 1.5f)
            {
                currentChargeSpeed = Mathf.Max(currentChargeSpeed - 5f, 0);
                rb.linearVelocity = Vector3.zero;
            }
            yield return new WaitForSeconds(0.1f);
        }

        isCharging = false;
        agent.enabled = true;
        rb.angularDamping = 15;
        chargeCharge = 0;

    }





    /// <summary>
    /// Called when first alerted
    /// </summary>
    private void BecomeAlerted(bool state)
    {
        Alerted = state;
    }
}
