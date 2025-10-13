using System;
using System.Collections;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public event Action<bool> onAlerted;

    /// <summary>
    /// Whether or not this enemy has begun activating, cannot be contacted if true
    /// </summary>
    private bool activated;
    /// <summary>
    /// Offset from the pivot point to start the raycast from
    /// </summary>
    [SerializeField]
    private Vector3 viewPointOffset;
    /// <summary>
    /// Object to search for and become alerted when seen
    /// </summary>
    private GameObject targetObject;

    /// <summary>
    /// Range the enemy can detect the target from
    /// </summary>
    [Header("Detection"), SerializeField]
    private float detectionRange = 15;
    /// <summary>
    /// Time it takes to detect target when it is in view
    /// </summary>
    [SerializeField]
    private float detectionTimer = 1;
    /// <summary>
    /// Current detection time
    /// </summary>
    private float currentDetection;

    /// <summary>
    /// Range that other enemies have to be in to be activated by this enemy
    /// </summary>
    [Header("Alerting Others"), SerializeField, Min(0.01f)]
    private float alertOthersRange = 20;
    /// <summary>
    /// Whether or to allow other activated enemies to activate other enemies
    /// </summary>
    public bool AcceptDetectChaining = false;
    /// <summary>
    /// How many activation links can be made from this enemy
    /// </summary>
    [Range(1, 100)]
    public int MaxDetectionChain = 3;
    /// <summary>
    /// Increase of delay per every activation chain
    /// </summary>
    private float activateDelayAddition = 0.4f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Health>().onHealthChanged += AlertFromDamage;

        targetObject = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        LayerMask obstacles = LayerMask.GetMask("Default", "Ground", "Player");

        RaycastHit hit; // Linecast would have been better here but oh well
        Physics.Raycast(transform.position + viewPointOffset, ((targetObject.transform.position + Vector3.up / 2) - (transform.position + viewPointOffset)).normalized, out hit, detectionRange, obstacles);
        if (hit.rigidbody != null)
        {
            if (hit.rigidbody.CompareTag("Player"))
            {
                currentDetection += Time.fixedDeltaTime;
            }
            else
            {
                currentDetection = Mathf.Max(currentDetection - Time.fixedDeltaTime * 2, 0);
            }
        }
        else
        {
            currentDetection = Mathf.Max(currentDetection - Time.fixedDeltaTime * 2, 0);
        }

        if (currentDetection >= detectionTimer)
        {
            BecomeAlert(true, MaxDetectionChain, 0);
        }


        //Debug.DrawLine(transform.position + viewPointOffset, hit.point);
    }


    private void AlertFromDamage(float oldHP, float newHP)
    {
        BecomeAlert(true, MaxDetectionChain, 0);
    }

    public void BecomeAlert(bool alertOthers, int currentChain, float activationDelay)
    {
        print(alertOthers);
        activated = true;
        if (alertOthers && currentChain > 0) { AlertOtherEnemies(currentChain, activationDelay + activateDelayAddition); }

        StartCoroutine(WaitForDelay(activationDelay));
    }

    private void AlertOtherEnemies(int chainDetectsLeft, float activationDelay)
    {
        chainDetectsLeft--;
        print("called");
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        LayerMask obstacles = LayerMask.GetMask("Default", "Ground");

        RaycastHit[] foundEnemies;
        foundEnemies = Physics.SphereCastAll(transform.position + viewPointOffset, alertOthersRange, Vector3.up, 1, enemyLayer);

        foreach (RaycastHit enemy in foundEnemies) // Check each found enemy (or object in enemy layer)
        {
            EnemyDetection enemyDetectScript;
            if (!((enemyDetectScript = enemy.transform.GetComponent<EnemyDetection>()) != null && enemyDetectScript != this && !enemyDetectScript.activated))
            {
                continue; // skip this enemy if it doesn't satisfy: 1. Having the detection component,  2. is NOT this instance of the script and  3. is NOT already activated
            }

            RaycastHit hit;
            Physics.Linecast(transform.position + viewPointOffset, enemy.transform.position + enemyDetectScript.viewPointOffset, out hit, obstacles);
            if (hit.collider == null)
            {
                //print(chainDetectsLeft);
                enemyDetectScript.BecomeAlert(AcceptDetectChaining, chainDetectsLeft, activationDelay);
            }
        }

    }


    /// <summary>
    /// Wait for set amount of seconds beforing activating
    /// </summary>
    /// <param name="delay">Delay before activating in seconds</param>
    IEnumerator WaitForDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        onAlerted?.Invoke(true);
        this.enabled = false;
    }

}
