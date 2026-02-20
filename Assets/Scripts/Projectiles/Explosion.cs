using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    float ExplosionSpeed = 1f;

    [SerializeField]
    bool StartImmediately = true;

    [SerializeField]
    float delay = 1f;

    private float damage;

    private float radius;

    private float localTime = 0f;

    void Start()
    {

    }

    IEnumerator Expand()
    {
        if (!StartImmediately) yield return new WaitForSeconds(delay);

        while (localTime <= 1)
        {
            localTime += Time.deltaTime * (1 / ExplosionSpeed);

            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(radius, radius, radius), localTime);

            yield return null;
        }

        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        other.GetComponent<IDamageable>()?.TakeDamage(damage, transform.position);
    }

    public void SetUpExplosion(float damage, float radius)
    {
        this.damage = damage;
        this.radius = radius;

        transform.localScale = Vector3.zero;
        StartCoroutine(Expand());
    }
}
