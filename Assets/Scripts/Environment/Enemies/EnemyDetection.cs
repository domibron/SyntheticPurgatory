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
    private GameObject viewPointLocator;
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
    /// Allow AI to stop persuing the target if conditions are not met
    /// </summary>
    [Header("Agro Loss")]
    public bool canLoseAgro = false;
    /// <summary>
    /// Range the enemy starts losing agro
    /// </summary>
    [SerializeField]
    private float ignoreRange = 25;
    /// <summary>
    /// Time it takes to lose target when it is out of view
    /// </summary>
    [SerializeField]
    private float loseTimer = 10;
    /// <summary>
    /// Current lose time
    /// </summary>
    private float currentLose;

    /// <summary>
    /// Range that other enemies have to be in to be activated by this enemy
    /// </summary>
    [Header("Alerting Others"), SerializeField, Min(0.01f)]
    private float alertOthersRange = 20;
    /// <summary>
    /// Whether or to allow other activated enemies to activate other enemies
    /// </summary>
    public bool EnableDetectChaining = false;
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
        GetComponent<Health>().OnHealthChanged += AlertFromDamage;

        targetObject = GameObject.FindWithTag("Player");

        if (viewPointLocator == null)
        {
            Debug.LogWarning(transform.name + " has no view point locator assigned, script has been disabled");
            this.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        LayerMask obstacles = LayerMask.GetMask("Default", "Ground", "Player"); // Set layers the raycast can be stopped by

        float currentRange = activated ? ignoreRange : currentDetection;
        bool isDetecting = false;

        RaycastHit hit; // Get any objects between enemy and target, if not get player (provided they are within reach)
        Physics.Raycast(viewPointLocator.transform.position, 
            ((targetObject.transform.position + Vector3.up / 2) - (viewPointLocator.transform.position)).normalized,
            out hit, detectionRange, obstacles);

        if (hit.rigidbody != null) // Make sure something was hit before continuing
        {
            if (hit.rigidbody.CompareTag("Player")) // If object found is player
            {
                isDetecting = true;
            }
        }

        if (!activated)
        {
            DetectingTarget(isDetecting);
        }
        else if (canLoseAgro)
        {
            lostTarget(isDetecting);
        }
        
    }


    private void DetectingTarget(bool detected)
    {
        if (detected) { currentDetection += Time.fixedDeltaTime; } // Increase visibility timer
        else { currentDetection = Mathf.Max(currentDetection - Time.fixedDeltaTime * 2, 0); } // Reduce Visility timer

        if (currentDetection >= detectionTimer) // If player has been out of sight for long enough
        {

            BecomeAlert(true, MaxDetectionChain, 0); // Start activation sequence
        }
    }

    private void lostTarget(bool detected)
    {
        if (detected) { currentLose += Time.fixedDeltaTime; } // Increase visibility timer
        else { currentLose = Mathf.Max(currentLose - Time.fixedDeltaTime * 2, 0); } // Reduce Visility timer

        if (currentLose <= 0) // If player has been out of sight for long enough
        {
            BecomeInactive(); // Deactivate AI
        }
    }


    /// <summary>
    /// Instantly alert this enemy if they take damage
    /// </summary>
    private void AlertFromDamage(float oldHP, float newHP)
    {
        if (activated) { currentLose = loseTimer; return; } // Keep persuing player if hit when active

        BecomeAlert(true, MaxDetectionChain, 0); // Start activation sequence
    }

    /// <summary>
    /// Activation sequence for the enemy AI, alert other enemies if applicable
    /// </summary>
    /// <param name="alertOthers">Whether or not to alert other nearby enemies</param>
    /// <param name="currentChain">Remaining amount of detection links the enemies can make</param>
    /// <param name="activationDelay">Added delay for each linked group of enemies</param>
    public void BecomeAlert(bool alertOthers, int currentChain, float activationDelay)
    {
        activated = true;
        currentLose = loseTimer;

        if (alertOthers && currentChain > 0) { AlertOtherEnemies(currentChain, activationDelay + activateDelayAddition); }

        StartCoroutine(WaitForDelay(activationDelay));
    }

    /// <summary>
    /// Alert other visible unactivated enemies that are within range
    /// </summary>
    /// <param name="chainDetectsLeft">Remaining amount of detection links the enemies can make</param>
    /// <param name="activationDelay">Added delay for each linked group of enemies</param>
    private void AlertOtherEnemies(int chainDetectsLeft, float activationDelay)
    {
        chainDetectsLeft--;

        LayerMask enemyLayer = LayerMask.GetMask("Enemy"); // LayerMask for only enemies, used to collect all enemies within range
        LayerMask obstacles = LayerMask.GetMask("Default", "Ground"); // LayerMask for level surfaces, used to check that the enemy candidate is visible

        RaycastHit[] foundEnemies; // Get all enemies within a spherical range
        foundEnemies = Physics.SphereCastAll(viewPointLocator.transform.position, alertOthersRange, Vector3.up, 1, enemyLayer);
        foreach (RaycastHit enemy in foundEnemies) // Check each found enemy (or object in enemy layer)
        {
            EnemyDetection enemyDetectScript;
            if (!((enemyDetectScript = enemy.transform.GetComponent<EnemyDetection>()) != null && enemyDetectScript != this && !enemyDetectScript.activated))
            {
                continue; // skip this enemy if it doesn't satisfy: 1. Having the detection component,  2. is NOT this instance of the script and  3. is NOT already activated
            }

            RaycastHit hit; // Check for objects between this enemy and the other target enemy
            Physics.Linecast(viewPointLocator.transform.position, 
                enemyDetectScript.viewPointLocator.transform.position, out hit, obstacles);
            if (hit.collider == null)
            {
                // Start activation sequence on other enemy
                enemyDetectScript.BecomeAlert(EnableDetectChaining, chainDetectsLeft, activationDelay);
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

        onAlerted?.Invoke(true); // Invoke the onAlerted event

    }

    private void BecomeInactive()
    {
        activated = false;
        currentDetection = 0;

        onAlerted?.Invoke(false); // Invoke the onAlerted event
    }

    public void ChangeCanLoseAgro(bool newState)
    {
        canLoseAgro = newState;
    }

}
