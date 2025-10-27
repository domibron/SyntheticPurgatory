using UnityEngine;

public class HitPoint : MonoBehaviour, IDamageable
{
    [SerializeField]
    private HitPointType hitPointType = HitPointType.normal;

    [SerializeField]
    private HitPointsDataSO hitPointsData;

    [SerializeField]
    private FloatingTextSystem floatingTextSystem;

    [SerializeField]
    private Health health;


    void IDamageable.TakeDamage(float damage, Vector3 hitPosition)
    {
        float totalDamage = damage * hitPointsData.GetMultiplier(hitPointType);

        health.AddToHealth(-Mathf.Abs(totalDamage));

        floatingTextSystem.SpawnText(Mathf.Abs(totalDamage).ToString("F0"), hitPointsData.GetGradient(hitPointType, false), 3, -10, hitPosition);
    }
}
