using System;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class UpgradeStats
{
    // public string ClassName = nameof(PlayerStats);
    public string VariableName = "MaxHealth";
    public float IncreaseAmount = 1;
    public int UpgradeCost = 1;
}

[Serializable]
public class UpgradeClassStats
{
    public string ClassName = nameof(PlayerStats);
    public UpgradeStats[] UpgradeStats = new UpgradeStats[0];
}

[CreateAssetMenu(menuName = "ScriptableObjects/UpgradeSystem/UpgradeAmounts", fileName = "SO_UpgradeAmounts")]
public class UpgradeAmountsSO : ScriptableObject
{
    [SerializeField]
    private UpgradeClassStats[] upgradeAmounts = new UpgradeClassStats[0];

    public ReadOnlyCollection<UpgradeClassStats> UpgradeAmounts { get => Array.AsReadOnly(upgradeAmounts); }

}
