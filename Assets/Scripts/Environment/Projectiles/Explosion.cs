using System.Collections;
using UnityEngine;

/// <summary>
/// A expanding object that deals damage when anything touches it.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Explosion : MonoBehaviour
{
    /// <summary>
    /// How long for the explosion to expand to the desired size in seconds. 
    /// </summary>
    [SerializeField]
    float ExplosionSpeed = 1f;

    /// <summary>
    /// Skip the initial delay and immediately expand.
    /// </summary>
    [SerializeField]
    bool StartImmediately = true;

    /// <summary>
    /// How long to wait if <see cref="StartImmediately"/> is set to true.
    /// </summary>
    [SerializeField]
    float delay = 1f;

    /// <summary>
    /// How much damage this explosion will deal per entity.
    /// </summary>
    private float damage;

    /// <summary>
    /// The max size of the object, using scale.
    /// </summary>
    private float maxSize;

    /// <summary>
    /// Used for local time keeping for scaling the object.
    /// </summary>
    private float localTime = 0f;


    IEnumerator Expand()
    {
        if (!StartImmediately) yield return new WaitForSeconds(delay);

        while (localTime < 1)
        {
            localTime += Time.deltaTime * (1 / ExplosionSpeed);

            localTime = Mathf.Clamp01(localTime);

            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(maxSize, maxSize, maxSize), localTime);

            yield return null;
        }

        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        other.GetComponent<IDamageable>()?.TakeDamage(damage, transform.position);
    }

    /// <summary>
    /// Set up the explosion with the desired damage and radius.
    /// </summary>
    /// <param name="damage">The damage to deal per entity.</param>
    /// <param name="maxSize">The max size of the explosion.</param>
    public void SetUpExplosion(float damage, float maxSize)
    {
        this.damage = damage;
        this.maxSize = maxSize;

        transform.localScale = Vector3.zero;
        StartCoroutine(Expand());
    }
}
