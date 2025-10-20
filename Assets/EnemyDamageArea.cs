using UnityEngine;

// By Vince Pressey

public class EnemyDamageArea : MonoBehaviour
{
    /// <summary>
    /// The script for the health that damage will deduct from
    /// </summary>
    [SerializeField]
    private Health healthScript;

    /// <summary>
    /// Multiplier for the damage recieved when this area is hit
    /// </summary>
    [SerializeField]
    private float damageMultiplier = 1f;


    public void TakeDamage(float dmgValue)
    {
        int damageDealt = Mathf.RoundToInt(dmgValue * damageMultiplier);

        healthScript.AddToHealth(damageDealt);

    }

    public float GetMultiplier()
    {
        return damageMultiplier;
    }
}
