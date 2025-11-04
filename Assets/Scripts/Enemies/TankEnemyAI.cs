using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//By Dr. Frankenstein
//This code bad currently, will fix later~

public class TankEnemyAI : BaseEnemy
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
    /// <summary>
    /// Stored rotation from previous fixed frame
    /// </summary>
    private Vector3 oldRotation;

    private bool isCharging;
    /// <summary>
    /// Max speed for the enemy
    /// </summary>
    [Header("Movement")]   // =============================================#
    public float BaseSpeed = 3.5f;

    [SerializeField]
    private float maxChargeSpeed = 8;

    [SerializeField]
    private float chargeActivationTime = 1f;

    private float chargeCharge = 0;

    private float currentChargeSpeed = 40;
    /// <summary>
    /// Offset from the pivot point to start the raycast from
    /// </summary>
    [SerializeField]
    private Vector3 viewPointOffset;

    private float chargeFinishTime = 0.6f;

    private float turnReductionMult = 1;

    private float regularTurnSpeed = 60;

    void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        goal = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }



    void FixedUpdate()
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

        Vector3 targetDir;
        Vector3 newDir;

        if (isCharging) 
        {
            if (rb.linearVelocity.magnitude < 0.5f) { return; }

            if (rb.linearVelocity.magnitude > 2)
            {
                targetDir = new Vector3(goal.transform.position.x, transform.position.y, goal.transform.position.z) - transform.position; // Get target angle to turn towards
                newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.008f, 0.0f); // Calculate next angle
                transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
            }

            Vector3 targetVel = transform.forward * currentChargeSpeed;
            rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);

            return;
        }
        else if (!agent.isActiveAndEnabled)
        {
            return;
        }

            targetDir = new Vector3(goal.transform.position.x, transform.position.y, goal.transform.position.z) - transform.position; // Get target angle to turn towards
        newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.03f, 0.0f); // Calculate next angle
        transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation

        if (agent.remainingDistance < 12)
        {
            agent.speed = 0.1f;
        }
        else
        {

            agent.speed = BaseSpeed / turnReductionMult;
        }



        if (chargeCharge < chargeActivationTime && agent.remainingDistance < 12)
        {
            LayerMask obstacles = LayerMask.GetMask("Default", "Ground", "Player"); // Set layers the raycast can be stopped by

            RaycastHit hit; // Get any objects between enemy and target, if not get player (provided they are within reach)
            Physics.SphereCast(transform.position + viewPointOffset, 0.75f, transform.forward, out hit, 12, obstacles);
            if (hit.rigidbody != null) // Make sure something was hit before continuing
            {
                if (hit.rigidbody.CompareTag("Player")) // If object found is player
                {
                    chargeCharge += Time.fixedDeltaTime;

                }
                else
                {
                    chargeCharge = Mathf.Max(0, chargeCharge - Time.fixedDeltaTime * 2);
                }

            }
            else
            {
                chargeCharge = Mathf.Max(0, chargeCharge - Time.fixedDeltaTime * 2);
            }
        }



        if (chargeCharge > chargeActivationTime)
        {
            StartCoroutine(ChargeAtTarget());
        }

        float angularVelocity = transform.rotation.eulerAngles.y - oldRotation.y; // Get angle difference from previous frame
        turnReductionMult = 1 + (angularVelocity * 2);
        //agent.angularSpeed = regularTurnSpeed * turnReductionMult * 2;

        if (agent.isActiveAndEnabled)
        {
            agent.destination = goal.transform.position;
        }

        oldRotation = transform.rotation.eulerAngles; // Save old angle
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isCharging || rb.linearVelocity.magnitude < 2)
        {
            return;
        }
        
        if (collision.transform.GetComponent<KickableObject>())
        {
            Vector3 kickDir = collision.transform.position - transform.position;
            collision.transform.GetComponent<IKickable>()?.KickObject(kickDir * rb.linearVelocity.magnitude / 4, ForceMode.VelocityChange);
        }
        else if (collision.transform.GetComponent<Health>())
        {
            collision.transform.GetComponent<Health>().AddToHealth(-15);
        }
        else
        {
            if (rb.linearVelocity.magnitude < 0.5f)
            {
                StopCharge();
            }

        }

    }



    private IEnumerator ChargeAtTarget()
    {
        float totalChargeTime = 0;
        isCharging = true;
        agent.enabled = false;
        rb.angularDamping = 100;
        rb.linearDamping = 0;
        rb.linearVelocity = Vector3.zero;
        currentChargeSpeed = maxChargeSpeed / 4;

        yield return new WaitForSeconds(0.1f);

        while (currentChargeSpeed < maxChargeSpeed)
        {
            totalChargeTime += 0.1f;
            if(totalChargeTime > 0.5f)
            {
                currentChargeSpeed = Mathf.Min(currentChargeSpeed + 0.3f, maxChargeSpeed);

            }
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.3f);

        totalChargeTime = 0;
        while (currentChargeSpeed > 0)
        {
            totalChargeTime += 0.1f;
            if (totalChargeTime > 0.5f)
            {
                currentChargeSpeed = Mathf.Max(currentChargeSpeed - 0.3f, 0);

            }
            yield return new WaitForSeconds(0.01f);
        }


        StopCharge();
    }

    private void StopCharge()
    {
        NavMeshHit myNavHit;
        NavMesh.SamplePosition(transform.position - viewPointOffset, out myNavHit, 100, -1);
        if (myNavHit.distance < 2)
        {
            rb.linearVelocity = Vector3.zero;
            StopCoroutine(ChargeAtTarget());
            isCharging = false;
            agent.enabled = true;
            rb.angularDamping = 15;
            rb.linearDamping = 20;
            chargeCharge = 0;
            StunAI(true, chargeFinishTime);
        }
        else
        {
            isCharging = false;
            rb.freezeRotation = false;
            rb.angularDamping = 0;

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
