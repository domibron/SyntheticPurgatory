using UnityEngine;

public class CardCollectable : CollectableBase
{
    public CardTier cardTeir = CardTier.Common;

    protected override void CollectItem()
    {
        if (!CanPlayerCollect()) return;

        if (Vector3.Distance(transform.position, targetTransform.position) > collectItemRange) return;

        UpgradeCardManager.Instance.CollectUpgradeCard(cardTeir);
        Destroy(gameObject);
    }

}
