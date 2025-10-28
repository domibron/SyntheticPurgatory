using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BaseEnemy : MonoBehaviour
{
    //Damage, Speed, Health, DisableAI ('kick'), stunAI
    //public float damage;
    //public float baseSpeed;
    //public float health;

    private Rigidbody rb;
    private NavMeshAgent agent;

    public bool enemyKnockedBack;
    public bool enemyStunned;
    private float knockbackTimer;

    private float baseAngularDamping;
    private float baseLinearDamping;

    private bool isGettingUp;
    private Vector3 targetGetupPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        baseAngularDamping = rb.angularDamping;
        baseLinearDamping = rb.linearDamping;

        agent = transform.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isGettingUp)
        {
            GetUp();
        }

        knockbackTimer -= Time.deltaTime;
    }

    public void StunAI(bool stunned, float stunTime)
    {
        print("stunned");

        enemyStunned = stunned;

        if (!enemyStunned) { return; }

        CancelInvoke("ClearStun");
        Invoke("ClearStun", stunTime);
    }
    public void ClearStun()
    {
        print("unstun");
        enemyStunned = false;
    }



    public void KnockbackAI(float minimumTime = 0.3f)
    {
        print("knockback");

        rb.useGravity = true;
        rb.angularDamping = 0;
        rb.linearDamping = 0;

        knockbackTimer = minimumTime;

        enemyKnockedBack = true;
        agent.enabled = false;

    }
    private void OnCollisionStay(Collision collision)
    {
        if (enemyKnockedBack && knockbackTimer < 0 && rb.linearVelocity.y > -0.1f)
        {
            LayerMask obstacles = LayerMask.GetMask("Default", "Ground"); // Set layers the raycast will detect

            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out hit, 2);
            if (hit.collider)
            {
                rb.useGravity = false;
                rb.angularDamping = baseAngularDamping;
                rb.linearDamping = baseLinearDamping;

                print("unknocked");

                targetGetupPosition = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                isGettingUp = true;
            }
        }


    }

    private void GetUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetGetupPosition, 0.01f);
        if (transform.position == targetGetupPosition)
        {
            isGettingUp = false;
            agent.enabled = true;
            enemyKnockedBack = false;
        }
    }
}
