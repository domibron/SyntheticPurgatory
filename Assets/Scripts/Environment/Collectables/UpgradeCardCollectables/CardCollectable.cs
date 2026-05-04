using UnityEngine;

public class CardCollectable : CollectableBase
{
    public ModuleTier cardTeir = ModuleTier.Common;

    protected override void CollectItem()
    {
        if (!CanPlayerCollect()) return;

        if (Vector3.Distance(transform.position, targetTransform.position) > collectItemRange) return;

        ModuleManager.Instance.CollectModule(cardTeir);
        Destroy(gameObject);
    }

}
