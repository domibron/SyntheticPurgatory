using System;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class UpgradeAmount
{
    public string ClassName = nameof(PlayerStats);
    public string VariableName = "MaxHealth";
    public float IncreaseAmount = 1;
    public int UpgradeCost = 1;
}

[CreateAssetMenu(menuName = "ScriptableObjects/UpgradeSystem/UpgradeAmounts", fileName = "SO_UpgradeAmounts")]
public class UpgradeAmountsSO : ScriptableObject
{
    [SerializeField]
    private UpgradeAmount[] upgradeAmounts = new UpgradeAmount[0];

    public ReadOnlyCollection<UpgradeAmount> UpgradeAmounts { get => Array.AsReadOnly(upgradeAmounts); }

}
