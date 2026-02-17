using UnityEngine;

public class DamageDirectionIndicatorPasser : MonoBehaviour, IDamageDirection
{

    private DamageDirectionIndicator damageDirectionIndicator;

    void Start()
    {
        damageDirectionIndicator = DamageDirectionIndicator.Instance;
    }

    public void DamagedFrom(Vector3 positionOfDamageSource)
    {
        damageDirectionIndicator.CreateDamageDirectionIndicator(positionOfDamageSource);
    }
}
