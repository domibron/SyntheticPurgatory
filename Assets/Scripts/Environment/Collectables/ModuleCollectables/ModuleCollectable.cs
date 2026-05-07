using UnityEngine;
using UnityEngine.Serialization;

public class ModuleCollectable : CollectableBase
{
    public ModuleTier cardTier = ModuleTier.Common;

    /// <summary>
    /// Add the module to the player's inventory.
    /// </summary>
    protected override void CollectItem()
    {
        if (!CanTargetCollect()) return;

        if (Vector3.Distance(transform.position, targetTransform.position) > collectItemRange) return;

        ModuleManager.Instance.CollectModule(cardTier);
        Destroy(gameObject);
    }

}
