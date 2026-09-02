using UnityEngine;

public class SawnEnemyAI : EnemyBase
{

    [SerializeField]
    private GameObject[] leftWheels;

    [SerializeField]
    private GameObject[] rightWheels;

    /// <summary>
    /// Stored rotation from previous fixed frame
    /// </summary>
    private Vector3 oldRotation;

    /// <summary>
    /// Reduction of speed when turning
    /// </summary>
    [SerializeField]
    private float turnReduction = 5;
    /// <summary>
    /// Turn speed handler
    /// </summary>
    private float nextToSpeed = 0;

    [SerializeField]
    private Animator animator;

    //private void Start()
    //{

    //}


    public override void Movement()
    {
        MoveToTarget(playerObj.transform.position, isAttacking ? baseMoveSpeed / 16 : baseMoveSpeed); // Move to target
    }

    public override void UnalertedMovement()
    {
        if (Vector3.Distance(transform.position, startPosition) > 2)
        {
            MoveToTarget(startPosition, isAttacking ? baseMoveSpeed / 16 : baseMoveSpeed); // Move back to start position
        }
        else
        {
            RotateWheels(0, 0);
        }
        return;

    }

    public override bool CheckAttackViability()
    {
        curAttackCooldown -= Time.fixedDeltaTime;

        // Check if target within attacking distance
        if (Vector3.Distance(transform.position, playerObj.transform.position) > baseAttackRange)
        {
            return false;
        }
        print("1 within attacking");

        // Calculate angle between object forward and target
        Vector3 direction = playerObj.transform.position - transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        if (toRotation.y > baseAttackCone || toRotation.y < -baseAttackCone)
        {
            return false;
        }

        print("2 within  cone");
        // Check attack cooldown
        if (curAttackCooldown > 0)
        {
            return false;
        }

        print("3 cooldown");
        // Check if already attacking
        if (isAttacking)
        {
            return false;
        }
        print("4.0 not already");

        // Return true if all checks weren't triggered
        return true;

    }   

    public override void Attack()
    {
        base.Attack();

        //if (launchAtTarget)
        //{
        //    hasHitPlayer = false;
        //    launching = true;
        //    KnockbackAI(1);

        //    launchTrail.enabled = true;
        //    float distance = Vector3.Distance(goalPos.transform.position, transform.position);
        //    Vector3 targetDir = (goalPos.transform.position - transform.position) + Vector3.up / 2 * distance;
        //    GetComponent<Rigidbody>().AddForce(targetDir.normalized * Mathf.Clamp(distance * 2, 6, 8), ForceMode.VelocityChange);

        //    return;
        //}

        Health healthscript;
        if (healthscript = playerObj.gameObject.GetComponent<Health>()) // Attack object if it has the health script attached
        {
            if (animator) animator.SetTrigger("Attack");
            isAttacking = true;
        }

    }


    public void AttemptAttack(bool endOfAttacks)
    {
        print("ds");
        if (endOfAttacks)
        {
            curAttackCooldown = baseAttackCooldown; // Reset attack cooldown
            isAttacking = false;
            return;
        }

        Collider[] hits = Physics.OverlapBox(transform.position + transform.forward, new Vector3(0.5f, 1.3f, 0.7f), Quaternion.identity);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == playerObj)
            {
                playerObj.GetComponent<Health>().AddToHealth(-baseAttack);
                playerObj.GetComponent<IDamageDirection>()?.DamagedFrom(transform.position);
            }
        }

    }





    public void MoveToTarget(Vector3 target, float aimedBaseSpeed)
    {
        navAgent.destination = target; // Set destination to goal's current position
        if (isAttacking)
        {
            navAgent.destination = (transform.position * 7 + target) / 8;
        }


        return;
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
        navAgent.speed = Mathf.Min(aimedBaseSpeed * (leftTreadSpeed + rightTreadSpeed) / turnReduction, aimedBaseSpeed);


        // Swap to alternative movement if close enough to target
        if (Vector3.Distance(transform.position, target) < baseAttackActivationDistance)
        {
            nextToSpeed = Mathf.Min(nextToSpeed + 0.02f, 1); // Increase rate of turning 
            navAgent.speed = 0; // Stop movement

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

        RotateWheels(Mathf.Clamp(angularVelocity + navAgent.speed / 2, -1, 1), Mathf.Clamp(-angularVelocity + navAgent.speed / 2, -1, 1));
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



    public override void BecomeAlerted(bool state)
    {
        base.BecomeAlerted(state);

        if (animator) animator.SetBool("Passive", !state);
    }


}

