using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//By Dr. Frankenstein
//This code bad currently, will fix later~

public class TankEnemyAI : BaseEnemy
{
    /// <summary>
    /// Target of the enemy
    /// </summary>
    private GameObject goal;

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

    // private float turnReductionMult = 1;

    //private float regularTurnSpeed = 60;

    private float previousSpeed;

    private float chargingMinTime = 0.1f;
    private float currentCharging = 0;

    [SerializeField]
    private GameObject[] leftWheels;

    [SerializeField]
    private GameObject[] rightWheels;


    /// <summary>
    /// Position of the enemy on start
    /// </summary>
    private Vector3 startPosition;





    void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        goal = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        startPosition = transform.position;
    }



    void FixedUpdate()
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

        // Mid-charge logic
        if (isCharging)
        {
            HandleCharging();
            return;
        }
        else if (!agent.isActiveAndEnabled) { return; }

        // Movement
        if (Alerted)
        {
            MoveToTarget(goal.transform.position, true);

        }
        else
        {
            if (Vector3.Distance(transform.position, startPosition) > 2)
            {
                MoveToTarget(startPosition, false); // Move back to start position
            }
            else
            {
                RotateWheels(0, 0);
            }

            MoveToTarget(startPosition, false);
            return;
        }

        // Attacking initialisation
        if (CheckCanAttack())
        {
            StartCoroutine(ChargeAtTarget());
        }

    }



    private void MoveToTarget(Vector3 target, bool persuing)
    {
        int detectedTarget = CalculateChargeCharge();
        Vector3 targetDir = new Vector3(target.x, transform.position.y, target.z) - transform.position; // Get target angle to turn towards
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.03f, 0.0f); // Calculate next angle

        if (agent.isActiveAndEnabled)
        {
            agent.destination = target;
        }

        if (!persuing)
        {
            agent.speed = BaseSpeed;
            agent.angularSpeed = 80;

            oldRotation = transform.rotation.eulerAngles; // Save old angle
            previousSpeed = rb.linearVelocity.magnitude;
            return;
        }

        if (agent.remainingDistance < 12)
        {
            switch (detectedTarget)
            {
                case 0:
                    agent.speed = BaseSpeed;
                    agent.angularSpeed = 80;
                    break;
                case 1:
                    transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
                    agent.speed = 0.1f;
                    agent.angularSpeed = 0;
                    break;
                case 2:
                    transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
                    agent.speed = 0.1f;
                    agent.angularSpeed = 0;
                    break;
            }
        }
        else
        {
            agent.speed = BaseSpeed;
            agent.angularSpeed = 80;
        }

        oldRotation = transform.rotation.eulerAngles; // Save old angle
        previousSpeed = rb.linearVelocity.magnitude;
    }


    private int CalculateChargeCharge()
    {
        if (agent.remainingDistance < 12)
        {
            LayerMask obstacles = LayerMask.GetMask(Constants.DefaultLayer, Constants.PlayerLayer); // Set layers the raycast can be stopped by
            //Vector3 detectDirection = new Vector3(transform.forward.x, Mathf.Clamp(goal.transform.position.y - transform.position.y, -5, 4), transform.forward.z);

            RaycastHit hit; // Get any objects between enemy and target, if not get player (provided they are within reach)
            Physics.SphereCast(transform.position + viewPointOffset, 0.75f, transform.forward, out hit, 12, obstacles);
            if (hit.rigidbody != null) // Make sure something was hit before continuing
            {

                if (hit.rigidbody.CompareTag(Constants.PlayerTag)) // If object found is player
                {
                    chargeCharge += Time.fixedDeltaTime;
                    return 2;
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

            Physics.SphereCast(transform.position + viewPointOffset, 0.75f, (goal.transform.position - (transform.position + viewPointOffset)).normalized, out hit, 12, obstacles);
            if (hit.rigidbody != null) // Make sure something was hit before continuing
            {
                if (hit.rigidbody.CompareTag(Constants.PlayerTag)) // If object found is player
                {
                    chargeCharge = Mathf.Max(0, chargeCharge - Time.fixedDeltaTime * 2);
                    return 1;
                }
            }

        }

        chargeCharge = Mathf.Max(0, chargeCharge - Time.fixedDeltaTime * 2);
        return 0;

    }

    private void HandleCharging()
    {
        Vector3 targetDir;
        Vector3 newDir;

        currentCharging += Time.deltaTime;

        if (rb.linearVelocity.magnitude < 0.5f && currentCharging < chargingMinTime) { return; }

        if (rb.linearVelocity.magnitude > 2)
        {
            targetDir = new Vector3(goal.transform.position.x, transform.position.y, goal.transform.position.z) - transform.position; // Get target angle to turn towards
            newDir = Vector3.RotateTowards(transform.forward, targetDir, 0.008f, 0.0f); // Calculate next angle
            transform.rotation = Quaternion.LookRotation(newDir); // Apply rotation
        }

        Vector3 targetVel = transform.forward * currentChargeSpeed;
        rb.linearVelocity = new Vector3(targetVel.x, rb.linearVelocity.y, targetVel.z);

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!isCharging || rb.linearVelocity.magnitude < 2)
        {
            return;
        }

        // NOTE: Objects can have multiple of these scripts so its not advisable to only check against one due to our based component system.
        if (collision.transform.GetComponent<KickableObject>())
        {
            Vector3 kickDir = collision.transform.position - transform.position;
            collision.transform.GetComponent<IKickable>()?.KickObject(kickDir * rb.linearVelocity.magnitude / 4, ForceMode.VelocityChange);
        }
        else if (collision.transform.GetComponent<Health>())
        {
            collision.transform.GetComponent<Health>().AddToHealth(-15);
            collision.transform.GetComponent<IDamageDirection>()?.DamagedFrom(transform.position);
        }
        else
        {
            if (rb.linearVelocity.magnitude < 0.5f)
            {
                StopCharge(Mathf.Min(previousSpeed / 3, 1.5f));
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
            if (totalChargeTime > 0.5f)
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


        StopCharge(0);
    }

    private void StopCharge(float stunTime)
    {
        NavMeshHit myNavHit;
        NavMesh.SamplePosition(transform.position - viewPointOffset, out myNavHit, 100, -1);
        if (myNavHit.distance < 2)
        {
            StopCoroutine(ChargeAtTarget());
            isCharging = false;
            agent.enabled = true;
            rb.angularDamping = 15;
            rb.linearDamping = 0; //rb.linearDamping = 20;
            rb.linearVelocity = -transform.up;
            chargeCharge = 0;
            currentCharging = 0;
            StunAI(true, stunTime);
        }
        else
        {
            isCharging = false;
            rb.freezeRotation = false;
            rb.angularDamping = 0;

        }
    }

    private bool CheckCanAttack()
    {
        if (chargeCharge < chargeActivationTime)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Spin the attached wheel objects
    /// </summary>
    /// <param name="leftSpeed">Speed of the left wheels</param>
    /// <param name="rightSpeed">Speed of the right wheels</param>
    private void RotateWheels(float leftSpeed, float rightSpeed)
    {
        foreach (GameObject wheel in leftWheels)
        {
            wheel.GetComponent<SimpleSpin>().spinSpeed = leftSpeed;
        }

        foreach (GameObject wheel in rightWheels)
        {
            wheel.GetComponent<SimpleSpin>().spinSpeed = rightSpeed;
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
