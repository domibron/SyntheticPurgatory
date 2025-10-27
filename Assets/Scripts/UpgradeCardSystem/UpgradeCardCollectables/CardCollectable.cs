using UnityEngine;

public class CardCollectable : CollectableBase
{
    public CardTeir cardTeir = CardTeir.Common;

    protected override void CollectItem()
    {
        if (!CanPlayerCollect()) return;

        if (Vector3.Distance(transform.position, playerTransform.position) > collectItemRange) return;

        UpgradeCardManager.Instance.CollectUpgradeCard(cardTeir);
        Destroy(gameObject);
    }

}
