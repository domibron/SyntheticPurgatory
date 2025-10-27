using UnityEngine;

public class HitPoint : MonoBehaviour, IDamageable
{
    [SerializeField]
    private HitPointType hitPointType = HitPointType.normal;

    [SerializeField]
    private HitPointsDataSO hitPointsData;

    private FloatingTextSystem floatingTextSystem;

    private Health health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponent<Health>();
        floatingTextSystem = health.GetComponent<FloatingTextSystem>();
    }

    void IDamageable.TakeDamage(float damage, Vector3 hitPosition)
    {
        health.AddToHealth(-Mathf.Abs(damage));

        floatingTextSystem.SpawnText(damage.ToString(), hitPointsData.GetGradient(hitPointType), targetSpawnPoint: hitPosition);
    }
}
