using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{
    //Damage, Speed, Health, DisableAI ('kick'), stunAI
    //public float damage;
    //public float baseSpeed;
    //public float health;

    protected Rigidbody rb;
    protected NavMeshAgent agent;

    public bool enemyKnockedBack;
    public bool enemyStunned;
    protected float knockbackTimer;

    protected float baseAngularDamping;
    protected float baseLinearDamping;

    protected bool isGettingUp;
    protected Vector3 targetGetupPosition;

    public bool isToaster = false; // Silly bullshit ignore or fix later


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        baseAngularDamping = rb.angularDamping;
        baseLinearDamping = rb.linearDamping;

        agent = transform.GetComponent<NavMeshAgent>();
    }

    protected virtual void Update()
    {
        if (isGettingUp)
        {
            GetUp();
        }

        knockbackTimer -= Time.deltaTime;
    }

    public void StunAI(bool stunned, float stunTime)
    {

        enemyStunned = stunned;

        if (!enemyStunned) { return; }

        CancelInvoke(nameof(ClearStun));
        Invoke(nameof(ClearStun), stunTime);
    }

    public void ClearStun()
    {
        enemyStunned = false;
    }



    public void KnockbackAI(float minimumTime = 0.3f, bool playerSourced = false)
    {
        rb.useGravity = true;
        rb.angularDamping = 0;
        rb.linearDamping = 0;
        knockbackTimer = minimumTime;

        enemyKnockedBack = true;
        agent.enabled = false;

        if (playerSourced && isToaster)
        {
            RunManager.Instance.statsHolder.todPunts++;
        }

    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (enemyKnockedBack && knockbackTimer < 0 && rb.linearVelocity.y > -0.1f)
        {
            LayerMask obstacles = LayerMask.GetMask("Default", "Ground"); // Set layers the raycast will detect

            RaycastHit hit;
            Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out hit, 2f);
            if (hit.collider)
            {
                rb.useGravity = false;
                rb.angularDamping = baseAngularDamping;
                rb.linearDamping = baseLinearDamping;

                targetGetupPosition = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                isGettingUp = true;
            }
        }


    }

    public virtual void GetUp()
    {
        isGettingUp = false;
        agent.enabled = true;
        enemyKnockedBack = false;
        //transform.position = Vector3.MoveTowards(transform.position, targetGetupPosition, 0.01f);
        //if (transform.position == targetGetupPosition)
        //{
        //    isGettingUp = false;
        //    agent.enabled = true;
        //    enemyKnockedBack = false;
        //}
    }
}
