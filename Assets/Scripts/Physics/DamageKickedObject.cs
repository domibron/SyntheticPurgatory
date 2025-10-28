using UnityEngine;

// By Vincent Pressey

public class DamageKickedObject : MonoBehaviour
{
    Rigidbody rb;
    Health healthScript;

    /// <summary>
    /// Minimum velocity required to activate contact damage
    /// </summary>
    [SerializeField]
    private float minVelocity = 3;
    /// <summary>
    /// Reduction of linear velocity when this object hits something, use to prevent multiple hits
    /// </summary>
    [SerializeField]
    private float contactVelReduction = 4;

    /// <summary>
    /// Damage multiplier for when this object hits a surface without Health
    /// </summary>
    [Header("Damage"), SerializeField]
    private float normalDamageMult = 2;
    /// <summary>
    /// Damage multiplier if an object with Health is hit, damage is dealt to both this and the other object
    /// </summary>
    [SerializeField]
    private float sharedDamageMult = 4;
    /// <summary>
    /// Cap at remaining HP of this object
    /// </summary>
    [SerializeField]
    private bool capAtRemainingHealth = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        healthScript = GetComponent<Health>();
    }
     
    private void OnCollisionEnter(Collision collision)
    {
        float objectVel = rb.linearVelocity.magnitude;
        if (objectVel > minVelocity) // Only deal damage if over certain velocity threshold
        {
            Health otherObjectHealth;
            if (otherObjectHealth = collision.gameObject.GetComponent<Health>()) // If the object collided with has health
            {
                float maxDamage = capAtRemainingHealth ? healthScript.ReturnHealthValue() : Mathf.Infinity; // Check whether to cap at remaining HP or not
                float sharedDamage = Mathf.Max(-objectVel * sharedDamageMult, -maxDamage); // Get highest damage from hit

                otherObjectHealth.AddToHealth(sharedDamage); // Deal damage to other object
                healthScript.AddToHealth(sharedDamage); // Deal damage to this object

                print(sharedDamage);
                rb.linearVelocity = rb.linearVelocity / contactVelReduction; // Lower velocity to prevent more damage from object rolling
            }
            else // If collided with a regular surface
            {
                healthScript.AddToHealth(-objectVel * normalDamageMult); // Deal damage based  
                rb.linearVelocity = rb.linearVelocity / contactVelReduction; // Lower velocity to prevent more damage from object rolling
            }

        }
    }

}
