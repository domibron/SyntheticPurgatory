// using System;
// using System.Collections.Generic;
// using TMPro;
// using Unity.Mathematics;
// using UnityEngine;

// rest in peace v2 upgrade system (v1 was epic, v1.5 was a cool idea, now this one is deprecated becuase HOLY FUCK).

// public enum UpgradeType
// {
//     PlayerStats,
//     Ranged,
//     Melee,
//     Misc,
// }


// // function passing for upgrading and downgrading. also need to store current upgrade amounts.



// public class StatUpgradeInfo
// {
//     public float UpgradeAmount;
//     public float DowngradeAmount;
//     public float? DowngradeMax;
//     public float? UpgradeMax;

//     public StatUpgradeInfo(float upgradeAmount, float downgradeAmount, float? downgradeMax, float? upgradeMax)
//     {
//         UpgradeAmount = upgradeAmount;
//         DowngradeAmount = downgradeAmount;
//         DowngradeMax = downgradeMax;
//         UpgradeMax = upgradeMax;
//     }

//     [Obsolete("Cannot be asked to imp log, use other upgrade methods.", true)]
//     public float GetLogAmount(float currentValue, int totalIncreases, int amountToAdd)
//     {
//         float current = currentValue;
//         for (int i = 0; i < amountToAdd; i++)
//         {
//             current += UpgradeAmount / ((float)totalIncreases + (float)i);
//         }

//         return current;
//     }

//     // public float DecreaseLogAmount(float currentValue, int totalIncreases, int amountToRemove)
//     // {
//     //     float current = currentValue;
//     //     for (int i = 0; i < amountToRemove; i++)
//     //     {
//     //         if (totalIncreases - i <= 0)
//     //         {
//     //             amountToRemove -= i - 1;
//     //             break;
//     //         }
//     //         current -= UpgradeAmount / ((float)totalIncreases - (float)i);
//     //     }


//     //     if (totalIncreases <= 1 && amountToRemove > 0)
//     //     {
//     //         current = DowngradeValue(current, amountToRemove);
//     //     }

//     //     return current;
//     // }

//     public float UpgradePercentage(float currentValue, int amount)
//     {
//         float current = currentValue;
//         for (int i = 1; i <= amount; i++)
//         {
//             current += current * UpgradeAmount;
//         }

//         return current;
//     }

//     public float DowngradePercentage(float currentValue, int amount)
//     {
//         float current = currentValue;
//         for (int i = 1; i <= amount; i++)
//         {
//             current += current * DowngradeAmount;
//         }

//         return current;
//     }

//     public float UpgradePercentageWithBaseStats(float currentValue, float baseStat, int amount)
//     {
//         float current = currentValue;
//         for (int i = 1; i <= amount; i++)
//         {
//             current += baseStat * UpgradeAmount;
//         }

//         return current;
//     }

//     public float DowngradePercentage(float currentValue, float baseStat, int amount)
//     {
//         float current = currentValue;
//         for (int i = 1; i <= amount; i++)
//         {
//             current += baseStat * DowngradeAmount;
//         }

//         return current;
//     }

//     public float UpgradeValue(float currentValue, int amount)
//     {
//         float newValue = currentValue + (UpgradeAmount * amount);

//         if (ExceedsMaximum(newValue))
//         {
//             return UpgradeMax.Value;
//         }

//         return newValue;
//     }

//     public float DowngradeValue(float currentValue, int amount)
//     {
//         float newValue = currentValue + (DowngradeAmount * amount);

//         if (ExceedsMinimum(newValue))
//         {
//             return DowngradeMax.Value;
//         }

//         return newValue;
//     }

//     public bool ExceedsMinimum(float value)
//     {
//         if (!DowngradeMax.HasValue) return false;

//         if (DowngradeAmount < 0) // does downgrading go down.
//         {
//             if (value <= DowngradeMax.Value)
//             {
//                 return true;
//             }
//             else
//             {
//                 return false;
//             }
//         }
//         else
//         {
//             if (value >= DowngradeMax.Value)
//             {
//                 return true;
//             }
//             else
//             {
//                 return false;
//             }
//         }
//     }

//     public bool ExceedsMaximum(float value)
//     {
//         if (!UpgradeMax.HasValue) return false;

//         if (UpgradeAmount > 0) // does upgrading go up.
//         {
//             if (value >= UpgradeMax.Value)
//             {
//                 return true;
//             }
//             else
//             {
//                 return false;
//             }
//         }
//         else
//         {
//             if (value <= UpgradeMax.Value)
//             {
//                 return true;
//             }
//             else
//             {
//                 return false;
//             }
//         }
//     }
// }


// public class StatUpgrades
// {
//     //public const UpgradeType upgradeType = UpgradeType.BaseStat;

//     StatUpgradeInfo healthInfo = new(8, -15, 10, null);
//     StatUpgradeInfo regenerationInfo = new(0.5f, -0.2f, 0f, null);
//     StatUpgradeInfo speedInfo = new(1, -0.4f, 1, null);
//     StatUpgradeInfo slideBoostInfo = new(0.05f, -0.05f, 1.1f, null);
//     StatUpgradeInfo airBoostInfo = new(0.05f, -0.05f, 1.1f, null);


//     private enum StatUpgradeType
//     {
//         Health,
//         Regeneration,
//         Speed,
//         SlideBoostForce,
//         AirBoostInfo,
//     }

//     public int GetRandomUpgradeID()
//     {
//         return UnityEngine.Random.Range(0, Enum.GetValues(typeof(StatUpgradeType)).Length);
//     }

//     public string GetUpgradeAmountAsString(int id, int amount)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case StatUpgradeType.Health:
//                 return (healthInfo.UpgradeValue(pStats.MaxHealth, amount) - pStats.MaxHealth).ToString("F0");
//             case StatUpgradeType.Regeneration:
//                 return (regenerationInfo.UpgradeValue(pStats.RegenerationAmountStat, amount) - pStats.RegenerationAmountStat).ToString("F2");
//             case StatUpgradeType.Speed:
//                 return (speedInfo.UpgradeValue(pStats.GroundSpeedStat, amount) - pStats.GroundSpeedStat).ToString("F2");
//             case StatUpgradeType.SlideBoostForce:
//                 return ((slideBoostInfo.UpgradeValue(pStats.SlideBoostPercentageStat, amount) - pStats.SlideBoostPercentageStat) * 100f).ToString("F0") + "%";
//             case StatUpgradeType.AirBoostInfo:
//                 return ((airBoostInfo.UpgradeValue(pStats.AirBoostPercentageStat, amount) - pStats.AirBoostPercentageStat) * 100f).ToString("F0") + "%";
//             default:
//                 return "";
//         }
//     }

//     public string GetCurrentAsString(int id)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case StatUpgradeType.Health:
//                 return pStats.MaxHealth.ToString("F0");
//             case StatUpgradeType.Regeneration:
//                 return pStats.RegenerationAmountStat.ToString("F2");
//             case StatUpgradeType.Speed:
//                 return pStats.GroundSpeedStat.ToString("F2");
//             case StatUpgradeType.SlideBoostForce:
//                 return (pStats.SlideBoostPercentageStat * 100f).ToString("F0") + "%";
//             case StatUpgradeType.AirBoostInfo:
//                 return (pStats.AirBoostPercentageStat * 100f).ToString("F0") + "%";
//             default:
//                 return "";
//         }
//     }

//     public string GetDowngradeAmountAsString(int id, int amount)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case StatUpgradeType.Health:
//                 return (healthInfo.DowngradeValue(pStats.MaxHealth, amount) - pStats.MaxHealth).ToString("F0");
//             case StatUpgradeType.Regeneration:
//                 return (regenerationInfo.DowngradeValue(pStats.RegenerationAmountStat, amount) - pStats.RegenerationAmountStat).ToString("F2");
//             case StatUpgradeType.Speed:
//                 return (speedInfo.DowngradeValue(pStats.GroundSpeedStat, amount) - pStats.GroundSpeedStat).ToString("F2");
//             case StatUpgradeType.SlideBoostForce:
//                 return ((slideBoostInfo.DowngradeValue(pStats.SlideBoostPercentageStat, amount) - pStats.SlideBoostPercentageStat) * 100f).ToString("F0") + "%";
//             case StatUpgradeType.AirBoostInfo:
//                 return ((airBoostInfo.DowngradeValue(pStats.AirBoostPercentageStat, amount) - pStats.AirBoostPercentageStat) * 100f).ToString("F0") + "%";
//             default:
//                 return "";
//         }
//     }

//     public string GetUpgradeNameAsString(int id)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         switch (statUpgradeType)
//         {
//             case StatUpgradeType.Health:
//                 return "Max Health";
//             case StatUpgradeType.Regeneration:
//                 return "Regeneration Rate";
//             case StatUpgradeType.Speed:
//                 return "Speed";
//             case StatUpgradeType.SlideBoostForce:
//                 return "Slide Amount";
//             case StatUpgradeType.AirBoostInfo:
//                 return "Air Slide Amount";
//             default:
//                 return "";
//         }

//     }

//     public string GetNewValWithGradeAsString(bool isUpgrade, int id, int amount)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         if (isUpgrade)
//         {
//             switch (statUpgradeType)
//             {
//                 case StatUpgradeType.Health:
//                     return (healthInfo.UpgradeValue(pStats.MaxHealth, amount)).ToString("F0");
//                 case StatUpgradeType.Regeneration:
//                     return (regenerationInfo.UpgradeValue(pStats.RegenerationAmountStat, amount)).ToString("F2");
//                 case StatUpgradeType.Speed:
//                     return (speedInfo.UpgradeValue(pStats.GroundSpeedStat, amount)).ToString("F2");
//                 case StatUpgradeType.SlideBoostForce:
//                     return ((slideBoostInfo.UpgradeValue(pStats.SlideBoostPercentageStat, amount)) * 100f).ToString("F0") + "%";
//                 case StatUpgradeType.AirBoostInfo:
//                     return ((airBoostInfo.UpgradeValue(pStats.AirBoostPercentageStat, amount)) * 100f).ToString("F0") + "%";
//                 default:
//                     return "";
//             }
//         }
//         else
//         {
//             switch (statUpgradeType)
//             {
//                 case StatUpgradeType.Health:
//                     return (healthInfo.DowngradeValue(pStats.MaxHealth, amount)).ToString("F0");
//                 case StatUpgradeType.Regeneration:
//                     return (regenerationInfo.DowngradeValue(pStats.RegenerationAmountStat, amount)).ToString("F2");
//                 case StatUpgradeType.Speed:
//                     return (speedInfo.DowngradeValue(pStats.GroundSpeedStat, amount)).ToString("F2");
//                 case StatUpgradeType.SlideBoostForce:
//                     return ((slideBoostInfo.DowngradeValue(pStats.SlideBoostPercentageStat, amount)) * 100f).ToString("F0") + "%";
//                 case StatUpgradeType.AirBoostInfo:
//                     return ((airBoostInfo.DowngradeValue(pStats.AirBoostPercentageStat, amount)) * 100f).ToString("F0") + "%";
//                 default:
//                     return "";
//             }
//         }
//     }

//     public void UpgradeWithID(int id, int amount)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case StatUpgradeType.Health:
//                 pStats.MaxHealth = healthInfo.UpgradeValue(pStats.MaxHealth, amount);
//                 break;
//             case StatUpgradeType.Regeneration:
//                 pStats.RegenerationAmountStat = regenerationInfo.UpgradeValue(pStats.RegenerationAmountStat, amount);
//                 break;
//             case StatUpgradeType.Speed:
//                 pStats.GroundSpeedStat = speedInfo.UpgradeValue(pStats.GroundSpeedStat, amount);
//                 pStats.AirSpeed = speedInfo.UpgradeValue(pStats.AirSpeed, amount);
//                 pStats.SpeedUpgradeAmount += amount;
//                 break;
//             case StatUpgradeType.SlideBoostForce:
//                 pStats.SlideBoostPercentageStat = slideBoostInfo.UpgradeValue(pStats.SlideBoostPercentageStat, amount);
//                 pStats.SlideBoostUpgradeAmount += amount;
//                 break;
//             case StatUpgradeType.AirBoostInfo:
//                 pStats.AirBoostPercentageStat = airBoostInfo.UpgradeValue(pStats.AirBoostPercentageStat, amount);
//                 pStats.AirBoostUpgradeAmount += amount;
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
//     }

//     public void DownGradeWithID(int id, int amount)
//     {
//         StatUpgradeType statUpgradeType = (StatUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case StatUpgradeType.Health:
//                 pStats.MaxHealth = healthInfo.DowngradeValue(pStats.MaxHealth, amount);
//                 break;
//             case StatUpgradeType.Regeneration:
//                 pStats.RegenerationAmountStat = regenerationInfo.DowngradeValue(pStats.RegenerationAmountStat, amount);
//                 break;
//             case StatUpgradeType.Speed:
//                 pStats.GroundSpeedStat = speedInfo.DowngradeValue(pStats.GroundSpeedStat, amount);
//                 pStats.AirSpeed = speedInfo.DowngradeValue(pStats.AirSpeed, amount);
//                 pStats.SpeedUpgradeAmount -= amount;
//                 break;
//             case StatUpgradeType.SlideBoostForce:
//                 pStats.SlideBoostPercentageStat = slideBoostInfo.DowngradeValue(pStats.SlideBoostPercentageStat, amount);
//                 pStats.SlideBoostUpgradeAmount -= amount;
//                 break;
//             case StatUpgradeType.AirBoostInfo:
//                 pStats.AirBoostPercentageStat = airBoostInfo.DowngradeValue(pStats.AirBoostPercentageStat, amount);
//                 pStats.AirBoostUpgradeAmount -= amount;
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

//     }
// }

// public class RangedUpgrades
// {
//     //public const UpgradeType upgradeType = UpgradeType.Ranged;

//     StatUpgradeInfo projectileDamage = new(1, -2, 2, null);
//     StatUpgradeInfo rechargeRate = new(0.05f, -0.05f, 0.05f, 0.6f);
//     StatUpgradeInfo shotsPerFullCharge = new(1, -1, 5, null);
//     StatUpgradeInfo standardSecondsPerShot = new(-0.05f, 0.05f, 1f, 0.05f);
//     StatUpgradeInfo chargedSecondsPerShot = new(-0.05f, 0.05f, 1f, 0.05f);
//     StatUpgradeInfo delayAfterFireBeforeRecharging = new(-0.05f, 0.05f, 0.5f, 0.05f);
//     StatUpgradeInfo overheatForceCoolDown = new(-0.2f, 0.2f, 5f, 0.2f);
//     // StatUpgradeInfo firerate = new(-0.05f, 0.2f, 3f, 0.08f);
//     // StatUpgradeInfo magSize = new(2, -3, 4, null);
//     // StatUpgradeInfo reloadSpeed = new(-0.06f, 0.1f, 4, 0.1f);


//     public enum CannonUpgradeType
//     {
//         Damage,
//         RechargeRate,
//         ShotsPerFullCharge,
//         // StandardSecondsPerShot,
//         // ChargedSecondsPerShot,
//         // DelayAfterFireBeforeRecharging,
//         OverheatForceCooldown,
//     }

//     public int GetRandomUpgradeID()
//     {
//         return UnityEngine.Random.Range(0, Enum.GetValues(typeof(CannonUpgradeType)).Length);
//     }

//     public string GetUpgradeAmountAsString(int id, int amount)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case CannonUpgradeType.Damage:
//                 return (projectileDamage.UpgradeValue(pStats.ProjectileDamageStat, amount) - pStats.ProjectileDamageStat).ToString("F0");
//             case CannonUpgradeType.RechargeRate:
//                 return (rechargeRate.UpgradeValue(pStats.RechargeRateStat, amount) - pStats.RechargeRateStat).ToString("F2");
//             case CannonUpgradeType.ShotsPerFullCharge:
//                 return (shotsPerFullCharge.UpgradeValue(pStats.ShotsPerFullChargeStat, amount) - pStats.ShotsPerFullChargeStat).ToString("F0");
//             // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//             //     return (delayAfterFireBeforeRecharging.UpgradeValue(pStats.DelayAfterFireBeforeRecharging, amount) - pStats.DelayAfterFireBeforeRecharging).ToString("F2");
//             case CannonUpgradeType.OverheatForceCooldown:
//                 return (overheatForceCoolDown.UpgradeValue(pStats.OverheatForceCooldownStat, amount) - pStats.OverheatForceCooldownStat).ToString("F2");
//             default:
//                 return "";
//         }
//     }

//     public string GetCurrentAsString(int id)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case CannonUpgradeType.Damage:
//                 return pStats.ProjectileDamageStat.ToString("F0");
//             case CannonUpgradeType.RechargeRate:
//                 return pStats.RechargeRateStat.ToString("F2");
//             case CannonUpgradeType.ShotsPerFullCharge:
//                 return pStats.ShotsPerFullChargeStat.ToString("F0");
//             // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//             //     return pStats.DelayAfterFireBeforeRecharging.ToString("F2");
//             case CannonUpgradeType.OverheatForceCooldown:
//                 return pStats.OverheatForceCooldownStat.ToString("F2");
//             default:
//                 return "";
//         }
//     }

//     public string GetDowngradeAmountAsString(int id, int amount)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case CannonUpgradeType.Damage:
//                 return (projectileDamage.DowngradeValue(pStats.ProjectileDamageStat, amount) - pStats.ProjectileDamageStat).ToString("F0");
//             case CannonUpgradeType.RechargeRate:
//                 return (rechargeRate.DowngradeValue(pStats.RechargeRateStat, amount) - pStats.RechargeRateStat).ToString("F2");
//             case CannonUpgradeType.ShotsPerFullCharge:
//                 return (shotsPerFullCharge.DowngradeValue(pStats.ShotsPerFullChargeStat, amount) - pStats.ShotsPerFullChargeStat).ToString("F0");
//             // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//             //     return (delayAfterFireBeforeRecharging.DowngradeValue(pStats.DelayAfterFireBeforeRecharging, amount) - pStats.DelayAfterFireBeforeRecharging).ToString("F2");
//             case CannonUpgradeType.OverheatForceCooldown:
//                 return (overheatForceCoolDown.DowngradeValue(pStats.OverheatForceCooldownStat, amount) - pStats.OverheatForceCooldownStat).ToString("F2");
//             default:
//                 return "";
//         }
//     }

//     public string GetUpgradeNameAsString(int id)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         switch (statUpgradeType)
//         {
//             case CannonUpgradeType.Damage:
//                 return "Cannon Damage";
//             case CannonUpgradeType.RechargeRate:
//                 return "Recharge Rate";
//             case CannonUpgradeType.ShotsPerFullCharge:
//                 return "Shots Per Charge";
//             // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//             //     return "Recharge Delay";
//             case CannonUpgradeType.OverheatForceCooldown:
//                 return "Overheat Cooldown";
//             default:
//                 return "";
//         }

//     }

//     public string GetNewValWithGradeAsString(bool isUpgrade, int id, int amount)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         if (isUpgrade)
//         {
//             switch (statUpgradeType)
//             {
//                 case CannonUpgradeType.Damage:
//                     return (projectileDamage.UpgradeValue(pStats.ProjectileDamageStat, amount)).ToString("F0");
//                 case CannonUpgradeType.RechargeRate:
//                     return rechargeRate.UpgradeValue(pStats.RechargeRateStat, amount).ToString("F2");
//                 case CannonUpgradeType.ShotsPerFullCharge:
//                     return shotsPerFullCharge.UpgradeValue(pStats.ShotsPerFullChargeStat, amount).ToString("F0");
//                 // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//                 //     return delayAfterFireBeforeRecharging.UpgradeValue(pStats.DelayAfterFireBeforeRecharging, amount).ToString("F2");
//                 case CannonUpgradeType.OverheatForceCooldown:
//                     return overheatForceCoolDown.UpgradeValue(pStats.OverheatForceCooldownStat, amount).ToString("F2");
//                 default:
//                     return "";
//             }
//         }
//         else
//         {
//             switch (statUpgradeType)
//             {
//                 case CannonUpgradeType.Damage:
//                     return (projectileDamage.DowngradeValue(pStats.ProjectileDamageStat, amount)).ToString("F0");
//                 case CannonUpgradeType.RechargeRate:
//                     return rechargeRate.DowngradeValue(pStats.RechargeRateStat, amount).ToString("F2");
//                 case CannonUpgradeType.ShotsPerFullCharge:
//                     return shotsPerFullCharge.DowngradeValue(pStats.ShotsPerFullChargeStat, amount).ToString("F0");
//                 // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//                 //     return delayAfterFireBeforeRecharging.DowngradeValue(pStats.DelayAfterFireBeforeRecharging, amount).ToString("F2");
//                 case CannonUpgradeType.OverheatForceCooldown:
//                     return overheatForceCoolDown.DowngradeValue(pStats.OverheatForceCooldownStat, amount).ToString("F2");
//                 default:
//                     return "";
//             }
//         }
//     }

//     public void UpgradeWithID(int id, int amount)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case CannonUpgradeType.Damage:
//                 pStats.ProjectileDamageStat = projectileDamage.UpgradeValue(pStats.ProjectileDamageStat, amount);
//                 break;
//             case CannonUpgradeType.RechargeRate:
//                 pStats.RechargeRateStat = rechargeRate.UpgradeValue(pStats.RechargeRateStat, amount);
//                 break;
//             case CannonUpgradeType.ShotsPerFullCharge:
//                 pStats.ShotsPerFullChargeStat = shotsPerFullCharge.UpgradeValue(pStats.ShotsPerFullChargeStat, amount);
//                 break;
//             // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//             //     pStats.DelayAfterFireBeforeRecharging = delayAfterFireBeforeRecharging.UpgradeValue(pStats.DelayAfterFireBeforeRecharging, amount);
//             // break;
//             case CannonUpgradeType.OverheatForceCooldown:
//                 pStats.OverheatForceCooldownStat = overheatForceCoolDown.UpgradeValue(pStats.OverheatForceCooldownStat, amount);
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
//     }

//     public void DownGradeWithID(int id, int amount)
//     {
//         CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case CannonUpgradeType.Damage:
//                 pStats.ProjectileDamageStat = projectileDamage.DowngradeValue(pStats.ProjectileDamageStat, amount);
//                 break;
//             case CannonUpgradeType.RechargeRate:
//                 pStats.RechargeRateStat = rechargeRate.DowngradeValue(pStats.RechargeRateStat, amount);
//                 break;
//             case CannonUpgradeType.ShotsPerFullCharge:
//                 pStats.ShotsPerFullChargeStat = shotsPerFullCharge.DowngradeValue(pStats.ShotsPerFullChargeStat, amount);
//                 break;
//             // case CannonUpgradeType.DelayAfterFireBeforeRecharging:
//             //     pStats.DelayAfterFireBeforeRecharging = delayAfterFireBeforeRecharging.DowngradeValue(pStats.DelayAfterFireBeforeRecharging, amount);
//             //     break;
//             case CannonUpgradeType.OverheatForceCooldown:
//                 pStats.OverheatForceCooldownStat = overheatForceCoolDown.DowngradeValue(pStats.OverheatForceCooldownStat, amount);
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

//     }
// }

// public class MeleeUpgrades
// {
//     //public const UpgradeType upgradeType = UpgradeType.Ranged;

//     StatUpgradeInfo meleeDamage = new(1, -1, 5, null);
//     StatUpgradeInfo meleeAttackTime = new(-0.04f, 0.1f, 0.75f, 0.08f);
//     StatUpgradeInfo kickAttackTime = new(-0.03f, 0.15f, 3f, 0.4f);
//     StatUpgradeInfo enemyStagger = new(0.2f, -0.06f, 0, null);
//     StatUpgradeInfo reach = new(0.2f, -0.2f, 0.5f, 4f);
//     StatUpgradeInfo knockback = new(0.5f, -0.4f, 1, 40);


//     public enum MeleeUpgradeType
//     {
//         Damage,
//         MeleeAttackTime,
//         KickAttackTime,
//         EnemyStagger,
//         Reach,
//         Knockback,
//     }

//     public int GetRandomUpgradeID()
//     {
//         return UnityEngine.Random.Range(0, Enum.GetValues(typeof(MeleeUpgradeType)).Length);
//     }

//     public string GetUpgradeAmountAsString(int id, int amount)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case MeleeUpgradeType.Damage:
//                 return (meleeDamage.UpgradeValue(pStats.MeleeDamageStat, amount) - pStats.MeleeDamageStat).ToString("F2");
//             case MeleeUpgradeType.MeleeAttackTime:
//                 return (meleeAttackTime.UpgradePercentage(pStats.MeleeAttackDelayStat, amount) - pStats.MeleeAttackDelayStat).ToString("F2");
//             case MeleeUpgradeType.KickAttackTime:
//                 return (kickAttackTime.UpgradePercentage(pStats.KickAttackDelayStat, amount) - pStats.KickAttackDelayStat).ToString("F2");
//             case MeleeUpgradeType.EnemyStagger:
//                 return (enemyStagger.UpgradeValue(pStats.MeleeStagerTimeStat, amount) - pStats.MeleeStagerTimeStat).ToString("F2");
//             case MeleeUpgradeType.Reach:
//                 return (reach.UpgradeValue(pStats.MeleeReachStat, amount) - pStats.MeleeReachStat).ToString("F2");
//             case MeleeUpgradeType.Knockback:
//                 return (knockback.UpgradeValue(pStats.KickForceStat, amount) - pStats.KickForceStat).ToString("F2");
//             default:
//                 return "";
//         }
//     }

//     public string GetCurrentAsString(int id)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case MeleeUpgradeType.Damage:
//                 return pStats.MeleeDamageStat.ToString("F2");
//             case MeleeUpgradeType.MeleeAttackTime:
//                 return pStats.MeleeAttackDelayStat.ToString("F2");
//             case MeleeUpgradeType.KickAttackTime:
//                 return pStats.KickAttackDelayStat.ToString("F2");
//             case MeleeUpgradeType.EnemyStagger:
//                 return pStats.MeleeStagerTimeStat.ToString("F2");
//             case MeleeUpgradeType.Reach:
//                 return pStats.MeleeReachStat.ToString("F2");
//             case MeleeUpgradeType.Knockback:
//                 return pStats.KickForceStat.ToString("F2");
//             default:
//                 return "";
//         }
//     }

//     public string GetDowngradeAmountAsString(int id, int amount)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case MeleeUpgradeType.Damage:
//                 return (meleeDamage.DowngradeValue(pStats.MeleeDamageStat, amount) - pStats.MeleeDamageStat).ToString("F2");
//             case MeleeUpgradeType.MeleeAttackTime:
//                 return (meleeAttackTime.DowngradeValue(pStats.MeleeAttackDelayStat, amount) - pStats.MeleeAttackDelayStat).ToString("F2");
//             case MeleeUpgradeType.KickAttackTime:
//                 return (meleeAttackTime.DowngradeValue(pStats.KickAttackDelayStat, amount) - pStats.KickAttackDelayStat).ToString("F2");
//             case MeleeUpgradeType.EnemyStagger:
//                 return (enemyStagger.DowngradeValue(pStats.MeleeAttackDelayStat, amount) - pStats.MeleeStagerTimeStat).ToString("F2");
//             case MeleeUpgradeType.Reach:
//                 return (reach.DowngradeValue(pStats.MeleeReachStat, amount) - pStats.MeleeReachStat).ToString("F2");
//             case MeleeUpgradeType.Knockback:
//                 return (knockback.DowngradeValue(pStats.KickForceStat, amount) - pStats.KickForceStat).ToString("F2");
//             default:
//                 return "";
//         }
//     }

//     public string GetUpgradeNameAsString(int id)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         switch (statUpgradeType)
//         {
//             case MeleeUpgradeType.Damage:
//                 return "Melee Damage";
//             case MeleeUpgradeType.MeleeAttackTime:
//                 return "Melee Rate";
//             case MeleeUpgradeType.KickAttackTime:
//                 return "Kick Rate";
//             case MeleeUpgradeType.EnemyStagger:
//                 return "Enemy Stagger Time";
//             case MeleeUpgradeType.Reach:
//                 return "Melee Reach";
//             case MeleeUpgradeType.Knockback:
//                 return "Knockback Force";
//             default:
//                 return "";
//         }

//     }

//     public string GetNewValWithGradeAsString(bool isUpgrade, int id, int amount)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         if (isUpgrade)
//         {
//             switch (statUpgradeType)
//             {
//                 case MeleeUpgradeType.Damage:
//                     return (meleeDamage.UpgradeValue(pStats.MeleeDamageStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.MeleeAttackTime:
//                     return (meleeAttackTime.UpgradePercentage(pStats.MeleeAttackDelayStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.KickAttackTime:
//                     return (kickAttackTime.UpgradePercentage(pStats.KickAttackDelayStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.EnemyStagger:
//                     return (enemyStagger.UpgradeValue(pStats.MeleeStagerTimeStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.Reach:
//                     return (reach.UpgradeValue(pStats.MeleeReachStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.Knockback:
//                     return (knockback.UpgradeValue(pStats.KickForceStat, amount)).ToString("F2");
//                 default:
//                     return "";
//             }
//         }
//         else
//         {
//             switch (statUpgradeType)
//             {
//                 case MeleeUpgradeType.Damage:
//                     return (meleeDamage.DowngradeValue(pStats.MeleeDamageStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.MeleeAttackTime:
//                     return (meleeAttackTime.DowngradeValue(pStats.MeleeAttackDelayStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.KickAttackTime:
//                     return (meleeAttackTime.DowngradeValue(pStats.KickAttackDelayStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.EnemyStagger:
//                     return (enemyStagger.DowngradeValue(pStats.MeleeAttackDelayStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.Reach:
//                     return (reach.DowngradeValue(pStats.MeleeReachStat, amount)).ToString("F2");
//                 case MeleeUpgradeType.Knockback:
//                     return (knockback.DowngradeValue(pStats.KickForceStat, amount)).ToString("F2");
//                 default:
//                     return "";
//             }
//         }
//     }


//     public void UpgradeWithID(int id, int amount)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case MeleeUpgradeType.Damage:
//                 pStats.MeleeDamageStat = meleeDamage.UpgradeValue(pStats.MeleeDamageStat, amount);
//                 break;
//             case MeleeUpgradeType.MeleeAttackTime:
//                 pStats.MeleeAttackDelayStat = meleeAttackTime.UpgradePercentage(pStats.MeleeAttackDelayStat, amount);
//                 break;
//             case MeleeUpgradeType.KickAttackTime:
//                 pStats.KickAttackDelayStat = kickAttackTime.UpgradePercentage(pStats.KickAttackDelayStat, amount);
//                 break;
//             case MeleeUpgradeType.EnemyStagger:
//                 pStats.MeleeStagerTimeStat = enemyStagger.UpgradeValue(pStats.MeleeStagerTimeStat, amount);
//                 pStats.MeleeStaggerUpgradeAmount += amount;
//                 break;
//             case MeleeUpgradeType.Reach:
//                 pStats.MeleeReachStat = reach.UpgradeValue(pStats.MeleeReachStat, amount);
//                 pStats.MeleeStaggerUpgradeAmount += amount;
//                 break;
//             case MeleeUpgradeType.Knockback:
//                 pStats.KickForceStat = knockback.UpgradeValue(pStats.KickForceStat, amount);
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
//     }

//     public void DownGradeWithID(int id, int amount)
//     {
//         MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

//         PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

//         switch (statUpgradeType)
//         {
//             case MeleeUpgradeType.Damage:
//                 pStats.MeleeDamageStat = meleeDamage.DowngradeValue(pStats.MeleeDamageStat, amount);
//                 break;
//             case MeleeUpgradeType.MeleeAttackTime:
//                 pStats.MeleeAttackDelayStat = meleeAttackTime.DowngradeValue(pStats.MeleeAttackDelayStat, amount);
//                 break;
//             case MeleeUpgradeType.KickAttackTime:
//                 pStats.KickAttackDelayStat = kickAttackTime.DowngradeValue(pStats.KickAttackDelayStat, amount);
//                 break;
//             case MeleeUpgradeType.EnemyStagger:
//                 pStats.MeleeStagerTimeStat = enemyStagger.DowngradeValue(pStats.MeleeStagerTimeStat, amount);
//                 break;
//             case MeleeUpgradeType.Reach:
//                 pStats.MeleeReachStat = reach.DowngradeValue(pStats.MeleeReachStat, amount);
//                 break;
//             case MeleeUpgradeType.Knockback:
//                 pStats.KickForceStat = knockback.DowngradeValue(pStats.KickForceStat, amount);
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

//     }
// }

// public class MiscellaneousUpgrades
// {
//     //public const UpgradeType upgradeType = UpgradeType.BaseStat;

//     StatUpgradeInfo scrapCarryMax = new(10, -10, 40, null);
//     StatUpgradeInfo itemCollectionRange = new(0.5f, -0.2f, 0.5f, 10f);
//     StatUpgradeInfo levelTimeLimit = new(15, -10, 30, null);
//     StatUpgradeInfo criticalChance = new(0.01f, -0.02f, 0, 1); // decimal percentage


//     public enum MiscellaneousUpgradeType
//     {
//         ScrapCarry,
//         ItemPickupRange,
//         TimeLimit,
//         CriticalChance,
//     }

//     public int GetRandomUpgradeID()
//     {
//         return UnityEngine.Random.Range(0, Enum.GetValues(typeof(MiscellaneousUpgradeType)).Length);
//     }

//     public string GetUpgradeAmountAsString(int id, int amount)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

//         switch (statUpgradeType)
//         {
//             case MiscellaneousUpgradeType.ScrapCarry:
//                 return (scrapCarryMax.UpgradeValue(mStats.MaxInventoryScrapStat, amount) - mStats.MaxInventoryScrapStat).ToString("F0");
//             case MiscellaneousUpgradeType.ItemPickupRange:
//                 return (itemCollectionRange.UpgradeValue(mStats.MaxCollectionRangeStat, amount) - mStats.MaxCollectionRangeStat).ToString("F2");
//             case MiscellaneousUpgradeType.TimeLimit:
//                 return (levelTimeLimit.UpgradeValue(mStats.MaxLevelTimeStat, amount) - mStats.MaxLevelTimeStat).ToString("F2");
//             case MiscellaneousUpgradeType.CriticalChance:
//                 return ((criticalChance.UpgradeValue(mStats.CriticalHitChanceStat, amount) - mStats.CriticalHitChanceStat) * 100f).ToString("F0") + "%";
//             default:
//                 return "";
//         }
//     }

//     public string GetCurrentAsString(int id)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

//         switch (statUpgradeType)
//         {
//             case MiscellaneousUpgradeType.ScrapCarry:
//                 return mStats.MaxInventoryScrapStat.ToString("F0");
//             case MiscellaneousUpgradeType.ItemPickupRange:
//                 return mStats.MaxCollectionRangeStat.ToString("F2");
//             case MiscellaneousUpgradeType.TimeLimit:
//                 return mStats.MaxLevelTimeStat.ToString("F2");
//             case MiscellaneousUpgradeType.CriticalChance:
//                 return (mStats.CriticalHitChanceStat * 100f).ToString("F0") + "%";
//             default:
//                 return "";
//         }
//     }

//     public string GetDowngradeAmountAsString(int id, int amount)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

//         switch (statUpgradeType)
//         {
//             case MiscellaneousUpgradeType.ScrapCarry:
//                 return (scrapCarryMax.DowngradeValue(mStats.MaxInventoryScrapStat, amount) - mStats.MaxInventoryScrapStat).ToString("F0");
//             case MiscellaneousUpgradeType.ItemPickupRange:
//                 return (itemCollectionRange.DowngradeValue(mStats.MaxCollectionRangeStat, amount) - mStats.MaxCollectionRangeStat).ToString("F2");
//             case MiscellaneousUpgradeType.TimeLimit:
//                 return (levelTimeLimit.DowngradeValue(mStats.MaxLevelTimeStat, amount) - mStats.MaxLevelTimeStat).ToString("F2");
//             case MiscellaneousUpgradeType.CriticalChance:
//                 return ((criticalChance.DowngradeValue(mStats.CriticalHitChanceStat, amount) - mStats.CriticalHitChanceStat) * 100f).ToString("F0") + "%";
//             default:
//                 return "";
//         }
//     }

//     public string GetUpgradeNameAsString(int id)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         switch (statUpgradeType)
//         {
//             case MiscellaneousUpgradeType.ScrapCarry:
//                 return "Max Inventory Scrap";
//             case MiscellaneousUpgradeType.ItemPickupRange:
//                 return "Item Pickup Range";
//             case MiscellaneousUpgradeType.TimeLimit:
//                 return "Time Limit";
//             case MiscellaneousUpgradeType.CriticalChance:
//                 return "Critical Hit Chance";
//             default:
//                 return "";
//         }

//     }

//     public string GetNewValWithGradeAsString(bool isUpgrade, int id, int amount)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

//         if (isUpgrade)
//         {
//             switch (statUpgradeType)
//             {
//                 case MiscellaneousUpgradeType.ScrapCarry:
//                     return (scrapCarryMax.UpgradeValue(mStats.MaxInventoryScrapStat, amount)).ToString("F0");
//                 case MiscellaneousUpgradeType.ItemPickupRange:
//                     return (itemCollectionRange.UpgradeValue(mStats.MaxCollectionRangeStat, amount)).ToString("F2");
//                 case MiscellaneousUpgradeType.TimeLimit:
//                     return (levelTimeLimit.UpgradeValue(mStats.MaxLevelTimeStat, amount)).ToString("F2");
//                 case MiscellaneousUpgradeType.CriticalChance:
//                     return ((criticalChance.UpgradeValue(mStats.CriticalHitChanceStat, amount)) * 100f).ToString("F0") + "%";
//                 default:
//                     return "";
//             }
//         }
//         else
//         {
//             switch (statUpgradeType)
//             {
//                 case MiscellaneousUpgradeType.ScrapCarry:
//                     return (scrapCarryMax.DowngradeValue(mStats.MaxInventoryScrapStat, amount)).ToString("F0");
//                 case MiscellaneousUpgradeType.ItemPickupRange:
//                     return (itemCollectionRange.DowngradeValue(mStats.MaxCollectionRangeStat, amount)).ToString("F2");
//                 case MiscellaneousUpgradeType.TimeLimit:
//                     return (levelTimeLimit.DowngradeValue(mStats.MaxLevelTimeStat, amount)).ToString("F2");
//                 case MiscellaneousUpgradeType.CriticalChance:
//                     return ((criticalChance.DowngradeValue(mStats.CriticalHitChanceStat, amount)) * 100f).ToString("F0") + "%";
//                 default:
//                     return "";
//             }
//         }
//     }

//     public void UpgradeWithID(int id, int amount)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

//         switch (statUpgradeType)
//         {
//             case MiscellaneousUpgradeType.ScrapCarry:
//                 mStats.MaxInventoryScrapStat = Mathf.FloorToInt(scrapCarryMax.UpgradeValue(mStats.MaxInventoryScrapStat, amount));
//                 break;
//             case MiscellaneousUpgradeType.ItemPickupRange:
//                 mStats.MaxCollectionRangeStat = itemCollectionRange.UpgradeValue(mStats.MaxCollectionRangeStat, amount);
//                 break;
//             case MiscellaneousUpgradeType.TimeLimit:
//                 mStats.MaxLevelTimeStat = levelTimeLimit.UpgradeValue(mStats.MaxLevelTimeStat, amount);
//                 break;
//             case MiscellaneousUpgradeType.CriticalChance:
//                 mStats.CriticalHitChanceStat = criticalChance.UpgradeValue(mStats.CriticalHitChanceStat, amount);
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<MiscellaneousStats>(Stats.miscellaneous, mStats);
//     }

//     public void DownGradeWithID(int id, int amount)
//     {
//         MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

//         MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

//         switch (statUpgradeType)
//         {
//             case MiscellaneousUpgradeType.ScrapCarry:
//                 mStats.MaxInventoryScrapStat = Mathf.FloorToInt(scrapCarryMax.DowngradeValue(mStats.MaxInventoryScrapStat, amount));
//                 break;
//             case MiscellaneousUpgradeType.ItemPickupRange:
//                 mStats.MaxCollectionRangeStat = itemCollectionRange.DowngradeValue(mStats.MaxCollectionRangeStat, amount);
//                 break;
//             case MiscellaneousUpgradeType.TimeLimit:
//                 mStats.MaxLevelTimeStat = levelTimeLimit.DowngradeValue(mStats.MaxLevelTimeStat, amount);
//                 break;
//             case MiscellaneousUpgradeType.CriticalChance:
//                 mStats.CriticalHitChanceStat = criticalChance.DowngradeValue(mStats.CriticalHitChanceStat, amount);
//                 break;
//         }

//         GameStatsManager.Instance.UpdateStats<MiscellaneousStats>(Stats.miscellaneous, mStats);

//     }
// }

// // TODO: Refactor, this hurts me.
// public class UpgradeSystem : MonoBehaviour
// {
//     [Serializable]
//     private class UpgradeData
//     {
//         public UpgradeType UpgradeType;
//         public int ID;
//         public int Amount;

//         public UpgradeData(UpgradeType type, int id, int amount)
//         {
//             UpgradeType = type;
//             ID = id;
//             Amount = amount;
//         }
//     }

//     [Serializable]
//     private class UpgradeChoice
//     {
//         public UpgradeData Upgrade;
//         public UpgradeData Downgrade;

//         public UpgradeChoice(UpgradeData upgrade, UpgradeData downgrade)
//         {
//             Upgrade = upgrade;
//             Downgrade = downgrade;
//         }
//     }

//     StatUpgrades playerStatUpgrades = new();
//     RangedUpgrades rangedUpgrades = new();
//     MeleeUpgrades meleeUpgrades = new();
//     MiscellaneousUpgrades miscellaneousUpgrades = new();


//     UpgradeChoice[] upgradeChoices;

//     CardTier currentCardTier;

//     [SerializeField]
//     UpgradeItemUI[] upgradeItemUIs;

//     [SerializeField]
//     GameObject chooseUpgradeCardScreen;

//     [SerializeField]
//     TMP_Text cardScrappedText;

//     private float fadeDuration = 1f;

//     private float currentFadeTime = 0f;

//     [SerializeField]
//     GameObject upgradeChoicesScreen;

//     [SerializeField]
//     GameObject upgradedTheStatsScreen;

//     [SerializeField]
//     TMP_Text newStatsDisplay;

//     [SerializeField]
//     int commonOpenCost;

//     [SerializeField]
//     int commonOpenIncreaseAmount;

//     [SerializeField]
//     int rareOpenCost;

//     [SerializeField]
//     int rareOpenIncreaseAmount;

//     [SerializeField]
//     int epicOpenCost;

//     [SerializeField]
//     int epicOpenIncreaseAmount;

//     [SerializeField]
//     TMP_Text scrapThisCardText;

//     private enum ScreenType
//     {
//         OpenCard,
//         ChooseUpgrade,
//         StatsUpgraded,
//     }

//     void Start()
//     {
//         ShowScreen(ScreenType.OpenCard);
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             ShowScreen(ScreenType.OpenCard);
//             // int ran = UnityEngine.Random.Range(0, 3);
//             // OpenCard((CardTeir)ran);
//             // print(((CardTeir)ran).ToString());
//         }

//         if (currentFadeTime > 0) // Needs rework for new format
//         {
//             cardScrappedText.gameObject.SetActive(true);
//             currentFadeTime -= Time.deltaTime;
//             cardScrappedText.alpha = EasingFunctions.EaseOutQuint(currentFadeTime / fadeDuration);
//         }
//         else
//         {
//             cardScrappedText.gameObject.SetActive(false);
//         }
//     }

//     public int GetCardOpenCost(CardTier cardTeir)
//     {
//         switch (cardTeir)
//         {
//             case CardTier.Common:
//                 return commonOpenCost + (commonOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//             case CardTier.Rare:
//                 return rareOpenCost + (rareOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//             case CardTier.Epic:
//                 return epicOpenCost + (epicOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//         }

//         return -1;
//     }

//     private void ShowScreen(ScreenType screenType)
//     {
//         switch (screenType)
//         {
//             case ScreenType.OpenCard:
//                 chooseUpgradeCardScreen.SetActive(true);
//                 upgradeChoicesScreen.SetActive(false);
//                 upgradedTheStatsScreen.SetActive(false);
//                 break;
//             case ScreenType.ChooseUpgrade:
//                 chooseUpgradeCardScreen.SetActive(false);
//                 upgradeChoicesScreen.SetActive(true);
//                 upgradedTheStatsScreen.SetActive(false);
//                 break;
//             case ScreenType.StatsUpgraded:
//                 chooseUpgradeCardScreen.SetActive(false);
//                 upgradeChoicesScreen.SetActive(false);
//                 upgradedTheStatsScreen.SetActive(true);
//                 break;
//         }
//     }

//     public void UpgradeSelected(UpgradeType upgradeType)
//     {
//         UpgradeChoice upgradeChoice = null;

//         print(upgradeChoices.Length);

//         foreach (UpgradeChoice choice in upgradeChoices)
//         {
//             print("gabba" + choice);
//             if (choice.Upgrade.UpgradeType == upgradeType) upgradeChoice = choice;
//         }

//         newStatsDisplay.text = GetUpgradedDisplayText(upgradeChoice);
//         ShowScreen(ScreenType.StatsUpgraded);


//         UpgradeStat(upgradeChoice);
//     }

//     public void OpenCard(CardTier cardTier)
//     {
//         if (GameManager.Instance.GetCurrentScrapCount() < GetCardOpenCost(cardTier)) return; // cant open the card

//         if (GameManager.Instance.GetCardCount(cardTier) < 1) return; // if we have none of said card type.

//         currentCardTier = cardTier;
//         GameManager.Instance.RemoveFromDepositedScrap(GetCardOpenCost(currentCardTier));
//         GameManager.Instance.RemoveFromStoredCards(currentCardTier, 1);
//         RandomUpgrades(currentCardTier);
//         scrapThisCardText.text = "Worth: " + GetCardOpenCost(currentCardTier).ToString() + " Scrap";
//         ShowScreen(ScreenType.ChooseUpgrade);
//     }

//     public void ScrapCurrentCard()
//     {
//         int giveAmount = 0;

//         switch (currentCardTier)
//         {
//             case CardTier.Common:
//                 giveAmount = commonOpenCost + (commonOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//                 break;
//             case CardTier.Rare:
//                 giveAmount = rareOpenCost + (rareOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//                 break;
//             case CardTier.Epic:
//                 giveAmount = epicOpenCost + (epicOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//                 break;
//         }

//         GameManager.Instance.AddToDepositedScrap(giveAmount);

//         cardScrappedText.text = $"{currentCardTier.ToString()} scrapped for {giveAmount}";
//         currentFadeTime = fadeDuration;
//         ShowScreen(ScreenType.OpenCard);
//     }

//     public void ScrapCard(CardTier cardTier)
//     {
//         if (GameManager.Instance.GetCardCount(cardTier) < 1) return;

//         int giveAmount = 0;

//         switch (cardTier)
//         {
//             case CardTier.Common:
//                 giveAmount = commonOpenCost + (commonOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//                 break;
//             case CardTier.Rare:
//                 giveAmount = rareOpenCost + (rareOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//                 break;
//             case CardTier.Epic:
//                 giveAmount = epicOpenCost + (epicOpenIncreaseAmount * GameManager.Instance.GetCurrentDifficulty());
//                 break;
//         }

//         GameManager.Instance.RemoveFromStoredCards(cardTier, 1);
//         GameManager.Instance.AddToDepositedScrap(giveAmount);

//         cardScrappedText.text = $"{currentCardTier.ToString()} scrapped for {giveAmount}";
//         currentFadeTime = fadeDuration;
//     }

//     public void GoToOpenScreen()
//     {
//         ShowScreen(ScreenType.OpenCard);
//     }

//     private void RandomUpgrades(CardTier cardTeir)
//     {
//         // int upAmount;
//         // int downAmount;


//         (int upAmount, int downAmount) = GameManager.Instance.GetUPandDOWNAmounts(cardTeir);



//         upgradeChoices = GenerateUpgradeChoices(upAmount, downAmount);

//         for (int i = 0; i < 4; i++)
//         {
//             upgradeItemUIs[i].SetText(GetDisplayText(upgradeChoices[i]));
//         }
//     }

//     private string GetDisplayText(UpgradeChoice upgradeChoice)
//     {
//         string returnedText = "";

//         string upgradeName = "NAME";
//         string currentAmountForUpgradeType = "CURRENT AMOUNT";
//         string newAmount = "CURRENT AMOUNT";
//         string upgradeAmount = "INCRASE AMOUNT";

//         switch (upgradeChoice.Upgrade.UpgradeType)
//         {
//             case UpgradeType.PlayerStats:
//                 upgradeName = playerStatUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 currentAmountForUpgradeType = playerStatUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 newAmount = playerStatUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 upgradeAmount = playerStatUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//             case UpgradeType.Ranged:
//                 upgradeName = rangedUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 currentAmountForUpgradeType = rangedUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 newAmount = rangedUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 upgradeAmount = rangedUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//             case UpgradeType.Melee:
//                 upgradeName = meleeUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 currentAmountForUpgradeType = meleeUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 newAmount = meleeUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 upgradeAmount = meleeUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//             case UpgradeType.Misc:
//                 upgradeName = miscellaneousUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 currentAmountForUpgradeType = miscellaneousUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 newAmount = miscellaneousUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 upgradeAmount = miscellaneousUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//         }

//         bool isUpgradeNegative = upgradeAmount[0] == '-';

//         if (isUpgradeNegative) upgradeAmount = upgradeAmount.Substring(1);

//         char sign = (isUpgradeNegative ? '-' : '+');

//         returnedText += $"<color=green>+ {upgradeName} ({currentAmountForUpgradeType}) -> ({newAmount}) [{sign}{upgradeAmount}]</color>";


//         if (upgradeChoice.Downgrade.Amount <= 0) return returnedText;

//         string downgradeName = "NAME";
//         string currentAmountForDowngradeType = "CURRENT AMOUNT";
//         string downgradeAmount = "DECREASE AMOUNT";

//         switch (upgradeChoice.Downgrade.UpgradeType)
//         {
//             case UpgradeType.PlayerStats:
//                 downgradeName = playerStatUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 currentAmountForDowngradeType = playerStatUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 newAmount = playerStatUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 downgradeAmount = playerStatUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//             case UpgradeType.Ranged:
//                 downgradeName = rangedUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 currentAmountForDowngradeType = rangedUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 newAmount = rangedUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 downgradeAmount = rangedUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//             case UpgradeType.Melee:
//                 downgradeName = meleeUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 currentAmountForDowngradeType = meleeUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 newAmount = meleeUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 downgradeAmount = meleeUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//             case UpgradeType.Misc:
//                 downgradeName = miscellaneousUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 currentAmountForDowngradeType = miscellaneousUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 newAmount = miscellaneousUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 downgradeAmount = miscellaneousUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//         }


//         bool isDowngradeNegative = downgradeAmount[0] == '-';

//         if (isDowngradeNegative) downgradeAmount = downgradeAmount.Substring(1);

//         sign = (isDowngradeNegative ? '-' : '+');

//         returnedText += $"\n\n<color=red>- {downgradeName} ({currentAmountForDowngradeType}) -> ({newAmount}) [{sign}{downgradeAmount}]</color>";

//         return returnedText;
//     }

//     private string GetUpgradedDisplayText(UpgradeChoice upgradeChoice)
//     {
//         string returnedText = "";

//         string upgradeName = "NAME";
//         string newCurrent = "CURRENT AMOUNT";
//         string oldCurrent = "INCRASE AMOUNT";

//         switch (upgradeChoice.Upgrade.UpgradeType)
//         {
//             case UpgradeType.PlayerStats:
//                 upgradeName = playerStatUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 newCurrent = playerStatUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 oldCurrent = playerStatUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 break;
//             case UpgradeType.Ranged:
//                 upgradeName = rangedUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 newCurrent = rangedUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 oldCurrent = rangedUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 break;
//             case UpgradeType.Melee:
//                 upgradeName = meleeUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 newCurrent = meleeUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 oldCurrent = meleeUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 break;
//             case UpgradeType.Misc:
//                 upgradeName = miscellaneousUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.ID);
//                 newCurrent = miscellaneousUpgrades.GetNewValWithGradeAsString(true, upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 oldCurrent = miscellaneousUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.ID);
//                 break;
//         }

//         returnedText += $"<color=green>+ {upgradeName} ({oldCurrent}) -> ({newCurrent})</color>";


//         if (upgradeChoice.Downgrade.Amount <= 0) return returnedText;

//         string downgradeName = "NAME";
//         string newCurrentForDowngrade = "CURRENT AMOUNT";
//         string oldDowngradeCurrent = "DECREASE AMOUNT";

//         switch (upgradeChoice.Downgrade.UpgradeType)
//         {
//             case UpgradeType.PlayerStats:
//                 downgradeName = playerStatUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 newCurrentForDowngrade = playerStatUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 oldDowngradeCurrent = playerStatUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 break;
//             case UpgradeType.Ranged:
//                 downgradeName = rangedUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 newCurrentForDowngrade = rangedUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 oldDowngradeCurrent = rangedUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 break;
//             case UpgradeType.Melee:
//                 downgradeName = meleeUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 newCurrentForDowngrade = meleeUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 oldDowngradeCurrent = meleeUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 break;
//             case UpgradeType.Misc:
//                 downgradeName = miscellaneousUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.ID);
//                 newCurrentForDowngrade = miscellaneousUpgrades.GetNewValWithGradeAsString(false, upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 oldDowngradeCurrent = miscellaneousUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.ID);
//                 break;
//         }


//         returnedText += $"\n\n<color=red>- {downgradeName} ({oldDowngradeCurrent}) -> ({newCurrentForDowngrade})</color>";

//         return returnedText;
//     }

//     private void UpgradeStat(UpgradeChoice upgradeChoice)
//     {
//         switch (upgradeChoice.Upgrade.UpgradeType)
//         {
//             case UpgradeType.PlayerStats:
//                 playerStatUpgrades.UpgradeWithID(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//             case UpgradeType.Ranged:
//                 rangedUpgrades.UpgradeWithID(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//             case UpgradeType.Melee:
//                 meleeUpgrades.UpgradeWithID(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//             case UpgradeType.Misc:
//                 miscellaneousUpgrades.UpgradeWithID(upgradeChoice.Upgrade.ID, upgradeChoice.Upgrade.Amount);
//                 break;
//         }

//         if (upgradeChoice.Downgrade.Amount <= 0) return;

//         switch (upgradeChoice.Downgrade.UpgradeType)
//         {
//             case UpgradeType.PlayerStats:
//                 playerStatUpgrades.DownGradeWithID(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//             case UpgradeType.Ranged:
//                 rangedUpgrades.DownGradeWithID(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//             case UpgradeType.Melee:
//                 meleeUpgrades.DownGradeWithID(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//             case UpgradeType.Misc:
//                 miscellaneousUpgrades.DownGradeWithID(upgradeChoice.Downgrade.ID, upgradeChoice.Downgrade.Amount);
//                 break;
//         }
//     }


//     private UpgradeChoice[] GenerateUpgradeChoices(int upgradeAmount, int downgradeAmount)
//     {
//         UpgradeChoice[] choices = new UpgradeChoice[4];

//         for (int i = 0; i < 4; i++)
//         {
//             choices[i] = GetRandomBaseOnCatagory((UpgradeType)i, upgradeAmount, downgradeAmount);
//         }

//         return choices;
//     }

//     private UpgradeChoice GetRandomBaseOnCatagory(UpgradeType upgradeType, int upgradeAmount, int downgradeAmount)
//     {
//         return new UpgradeChoice(GetRandomUpgradeForType(upgradeType, upgradeAmount), GetDowngradeExcludingType(upgradeType, downgradeAmount));
//     }

//     private UpgradeData GetRandomUpgradeForType(UpgradeType upgradeType, int amount)
//     {
//         UpgradeData upgradeData = null;

//         switch (upgradeType)
//         {
//             case UpgradeType.Melee:
//                 return upgradeData = new UpgradeData(UpgradeType.Melee, meleeUpgrades.GetRandomUpgradeID(), amount);
//             case UpgradeType.Misc:
//                 return upgradeData = new UpgradeData(UpgradeType.Misc, miscellaneousUpgrades.GetRandomUpgradeID(), amount);
//             case UpgradeType.PlayerStats:
//                 return upgradeData = new UpgradeData(UpgradeType.PlayerStats, playerStatUpgrades.GetRandomUpgradeID(), amount);
//             case UpgradeType.Ranged:
//                 return upgradeData = new UpgradeData(UpgradeType.Ranged, rangedUpgrades.GetRandomUpgradeID(), amount);
//             default:
//                 return null;
//         }
//     }

//     private UpgradeData GetDowngradeExcludingType(UpgradeType upgradeType, int amount)
//     {
//         int rand = UnityEngine.Random.Range(1, 4); // this only goes to 3 but we want that.
//         /*
//         1 - player stats
//         2 - range stats
//         3 - melee stats
//         4 - misc stats
//         */

//         switch (upgradeType)
//         {
//             case UpgradeType.Melee:
//                 if (rand == 1)
//                 {
//                     return GetDowngradeForType(UpgradeType.PlayerStats, amount);
//                 }
//                 else if (rand == 2)
//                 {
//                     return GetDowngradeForType(UpgradeType.Ranged, amount);
//                 }
//                 else if (rand == 3)
//                 {
//                     return GetDowngradeForType(UpgradeType.Misc, amount);
//                 }
//                 break;
//             case UpgradeType.Misc:
//                 if (rand == 1)
//                 {
//                     return GetDowngradeForType(UpgradeType.PlayerStats, amount);
//                 }
//                 else if (rand == 2)
//                 {
//                     return GetDowngradeForType(UpgradeType.Ranged, amount);
//                 }
//                 else if (rand == 3)
//                 {
//                     return GetDowngradeForType(UpgradeType.Melee, amount);
//                 }
//                 break;
//             case UpgradeType.PlayerStats:
//                 if (rand == 1)
//                 {
//                     return GetDowngradeForType(UpgradeType.Ranged, amount);
//                 }
//                 else if (rand == 2)
//                 {
//                     return GetDowngradeForType(UpgradeType.Melee, amount);
//                 }
//                 else if (rand == 3)
//                 {
//                     return GetDowngradeForType(UpgradeType.Misc, amount);
//                 }
//                 break;
//             case UpgradeType.Ranged:
//                 if (rand == 1)
//                 {
//                     return GetDowngradeForType(UpgradeType.PlayerStats, amount);
//                 }
//                 else if (rand == 2)
//                 {
//                     return GetDowngradeForType(UpgradeType.Melee, amount);
//                 }
//                 else if (rand == 3)
//                 {
//                     return GetDowngradeForType(UpgradeType.Misc, amount);
//                 }
//                 break;
//         }
//         return null;
//     }

//     private UpgradeData GetDowngradeForType(UpgradeType upgradeType, int amount)
//     {
//         UpgradeData upgradeData = null;

//         switch (upgradeType)
//         {
//             case UpgradeType.Melee:
//                 return upgradeData = new UpgradeData(UpgradeType.Melee, meleeUpgrades.GetRandomUpgradeID(), amount);
//             case UpgradeType.Misc:
//                 return upgradeData = new UpgradeData(UpgradeType.Misc, miscellaneousUpgrades.GetRandomUpgradeID(), amount);
//             case UpgradeType.PlayerStats:
//                 return upgradeData = new UpgradeData(UpgradeType.PlayerStats, playerStatUpgrades.GetRandomUpgradeID(), amount);
//             case UpgradeType.Ranged:
//                 return upgradeData = new UpgradeData(UpgradeType.Ranged, rangedUpgrades.GetRandomUpgradeID(), amount);
//             default:
//                 return null;
//         }
//     }
// }

