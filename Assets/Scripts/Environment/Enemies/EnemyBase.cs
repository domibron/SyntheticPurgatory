using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IKickable
{
    protected Rigidbody rb;
    protected NavMeshAgent navAgent;



    public bool enemyKnockedBack;
    public bool enemyStunned;
    protected float knockbackTimer;

    protected float baseAngularDamping;
    protected float baseLinearDamping;

    protected bool isGettingUp;
    protected Vector3 targetGetupPosition;



    private bool hasDetection = false;
    /// <summary>
    /// Check if enemy has been alerted to player presence
    /// </summary>
    public bool Alerted = false;


    #region Enemy Attributes
    public float baseMoveSpeed;
    public float baseAttack;
    public float baseAttackActivationDistance;
    public float baseAttackRange;
    public float baseAttackCooldown;
    public float curAttackCooldown;


    /// <summary>
    /// Max angle for attack from centre
    /// </summary>
    public float baseAttackCone = 0.3f;

    private float baseHealth = 60;

    private bool canBeStunned;
    private bool canBeKnockbacked;
    #endregion Enemy Attributes

    /// <summary>
    /// Position of the enemy on start
    /// </summary>
    public Vector3 startPosition;


    /// <summary>
    /// Player Object
    /// </summary>
    public GameObject playerObj;
    /// <summary>
    /// Player camera object
    /// </summary>
    private GameObject mainCamObj;


    /// <summary>
    /// Variable for checking if enemy is currently attacking
    /// </summary>
    public bool isAttacking;

    private Health healthComponent;


    protected virtual void Start()
    {
        GetComponent<EnemyDetection>().onAlerted += BecomeAlerted;

        rb = transform.GetComponent<Rigidbody>();
        navAgent = transform.GetComponent<NavMeshAgent>();

        if (transform.GetComponent<EnemyDetection>() != null)
        {
            if (transform.GetComponent<EnemyDetection>().enabled)
            {
                hasDetection = true;
            }

        }

        startPosition = transform.position;

        playerObj = GameObject.FindGameObjectWithTag(Constants.PlayerTag);
        mainCamObj = Camera.main.gameObject;

        healthComponent = transform.GetComponent<Health>();
        healthComponent.SetMaxHealth(baseHealth);
        healthComponent.Reset();
    }

    private void FixedUpdate()
    {

        // Check if enemy has detection component and is alerted before starting it's AI
        if (hasDetection && !Alerted) // (Always continues if no detection component is present)
        {
            UnalertedMovement();
            return;
        }

        Movement();

        if (CheckAttackViability())
        {
            Attack();
        }

    }

    public virtual void UnalertedMovement()
    {
        
    }

    public virtual void Movement()
    {

    }

    public virtual bool CheckAttackViability()
    {
        return false;
    }

    public virtual void Attack()
    {

    }



    #region Enemy Stun

    public void SetStunStatus(bool stunned, float stunTime = 2)
    {
        if (!canBeStunned)
        {
            enemyStunned = false;
            return;
        }

        if (!stunned)
        {
            CancelInvoke(nameof(ClearStun));
            ClearStun();
            return;
        }

        enemyStunned = true;
        CancelInvoke(nameof(ClearStun));
        Invoke(nameof(ClearStun), stunTime);
    }

    public void ClearStun()
    {
        enemyStunned = false;
    }

    #endregion Enemy Stun



    #region Enemy Knockback

    void IKickable.KickObject(Vector3 forceAndDir, ForceMode forceMode)
    {
        KnockbackAI(forceAndDir, 0.3f, forceMode, true);
    }

    public virtual void KnockbackAI(Vector3 forceAndDir, float minimumTime = 0.3f, ForceMode forceMode = ForceMode.VelocityChange, bool playerSourced = true)
    {
        if (!canBeKnockbacked)
        {
            enemyKnockedBack = false;
            return;
        }

        rb.useGravity = true;
        rb.angularDamping = 0;
        rb.linearDamping = 0;
        knockbackTimer = minimumTime;

        enemyKnockedBack = true;
        navAgent.enabled = false;


        Vector3 alteredForceDir;
        // Alter given force to have forced upward direction and to account for the mass of the object
        alteredForceDir = new Vector3(forceAndDir.x * 5 / (rb.mass / 2), Mathf.Max(forceAndDir.y, 5), forceAndDir.z * 5 / (rb.mass / 2));
        rb.AddForce(alteredForceDir, forceMode);


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
        navAgent.enabled = true;
        enemyKnockedBack = false;
        //transform.position = Vector3.MoveTowards(transform.position, targetGetupPosition, 0.01f);
        //if (transform.position == targetGetupPosition)
        //{
        //    isGettingUp = false;
        //    agent.enabled = true;
        //    enemyKnockedBack = false;
        //}
    }

    #endregion Enemy Knockback

    /// <summary>
    /// Called when alerted
    /// </summary>
    public  virtual void BecomeAlerted(bool state)
    {
        Alerted = state;
    }
}
