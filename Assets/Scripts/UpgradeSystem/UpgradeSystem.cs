using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum UpgradeType
{
    PlayerStats,
    Ranged,
    Melee,
    Misc,
}




public class StatUpgradeInfo
{
    public float UpgradeAmount;
    public float DowngradeAmount;
    public float? Minimum;
    public float? Maximum;

    public StatUpgradeInfo(float upgradeAmount, float downgradeAmount, float? minimum, float? maximum)
    {
        UpgradeAmount = upgradeAmount;
        DowngradeAmount = downgradeAmount;
        Minimum = minimum;
        Maximum = maximum;
    }

    public float GetLogAmount(float currentValue, int totalIncreases, int amountToAdd)
    {
        float current = currentValue;
        for (int i = 0; i < amountToAdd; i++)
        {
            current += UpgradeAmount / ((float)totalIncreases + (float)i);
        }

        return current;
    }

    // public float DecreaseLogAmount(float currentValue, int totalIncreases, int amountToRemove)
    // {
    //     float current = currentValue;
    //     for (int i = 0; i < amountToRemove; i++)
    //     {
    //         if (totalIncreases - i <= 0)
    //         {
    //             amountToRemove -= i - 1;
    //             break;
    //         }
    //         current -= UpgradeAmount / ((float)totalIncreases - (float)i);
    //     }


    //     if (totalIncreases <= 1 && amountToRemove > 0)
    //     {
    //         current = DowngradeValue(current, amountToRemove);
    //     }

    //     return current;
    // }

    public float UpgradePercentage(float currentValue, int amount)
    {
        float current = currentValue;
        for (int i = 1; i <= amount; i++)
        {
            current += current * UpgradeAmount;
        }

        return current;
    }

    public float DowngradePercentage(float currentValue, int amount)
    {
        float current = currentValue;
        for (int i = 1; i <= amount; i++)
        {
            current += current * DowngradeAmount;
        }

        return current;
    }

    public float UpgradePercentageWithBaseStats(float currentValue, float baseStat, int amount)
    {
        float current = currentValue;
        for (int i = 1; i <= amount; i++)
        {
            current += baseStat * UpgradeAmount;
        }

        return current;
    }

    public float DowngradePercentage(float currentValue, float baseStat, int amount)
    {
        float current = currentValue;
        for (int i = 1; i <= amount; i++)
        {
            current += baseStat * DowngradeAmount;
        }

        return current;
    }

    public float UpgradeValue(float currentValue, int amount)
    {
        float newValue = currentValue + (UpgradeAmount * amount);

        if (ExceedsMaximum(newValue))
        {
            return Maximum.Value;
        }

        return newValue;
    }

    public float DowngradeValue(float currentValue, int amount)
    {
        float newValue = currentValue + (UpgradeAmount * amount);

        if (ExceedsMinmum(newValue))
        {
            return Minimum.Value;
        }

        return newValue;
    }

    public bool ExceedsMinmum(float value)
    {
        if (!Minimum.HasValue) return false;

        if (DowngradeAmount < 0) // does downgrading go down.
        {
            if (value < Minimum.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (value > Minimum.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool ExceedsMaximum(float value)
    {
        if (!Maximum.HasValue) return false;

        if (Maximum > 0) // does upgrading go up.
        {
            if (value > Maximum.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (value < Maximum.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


public class StatUpgrades
{
    //public const UpgradeType upgradeType = UpgradeType.BaseStat;

    StatUpgradeInfo healthInfo = new(8, -15, 10, null);
    StatUpgradeInfo regenerationInfo = new(0.5f, -0.2f, 0f, null);
    StatUpgradeInfo speedInfo = new(1, -0.4f, 1, null);
    StatUpgradeInfo boostInfo = new(1.2f, -0.5f, 2, null);


    private enum StatUpgradeType
    {
        Health,
        Regeneration,
        Speed,
        SlideBoostForce,
    }

    public int GetRandomUpgradeID()
    {
        return UnityEngine.Random.Range(0, Enum.GetValues(typeof(StatUpgradeType)).Length);
    }

    public string GetUpgradeAmountAsString(int id, int amount)
    {
        StatUpgradeType statUpgradeType = (StatUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case StatUpgradeType.Health:
                return (healthInfo.UpgradeValue(pStats.MaxHealth, amount) - pStats.MaxHealth).ToString("F0");
            case StatUpgradeType.Regeneration:
                return (regenerationInfo.UpgradeValue(pStats.RegenerationAmount, amount) - pStats.RegenerationAmount).ToString("F2");
            case StatUpgradeType.Speed:
                return (speedInfo.GetLogAmount(pStats.GroundSpeed, pStats.SpeedUpgradeAmount, amount) - pStats.GroundSpeed).ToString("F2");
            case StatUpgradeType.SlideBoostForce:
                return (boostInfo.GetLogAmount(pStats.SlideBoostForce, pStats.BoostUpgradeAmount, amount) - pStats.SlideBoostForce).ToString("F2");
            default:
                return "";
        }
    }

    public string GetCurrentAsString(int id)
    {
        StatUpgradeType statUpgradeType = (StatUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case StatUpgradeType.Health:
                return pStats.MaxHealth.ToString("F0");
            case StatUpgradeType.Regeneration:
                return pStats.RegenerationAmount.ToString("F2");
            case StatUpgradeType.Speed:
                return pStats.GroundSpeed.ToString("F2");
            case StatUpgradeType.SlideBoostForce:
                return pStats.SlideBoostForce.ToString("F2");
            default:
                return "";
        }
    }

    public string GetDowngradeAmountAsString(int id, int amount)
    {
        StatUpgradeType statUpgradeType = (StatUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case StatUpgradeType.Health:
                return (healthInfo.DowngradeValue(pStats.MaxHealth, amount) - pStats.MaxHealth).ToString("F0");
            case StatUpgradeType.Regeneration:
                return (regenerationInfo.DowngradeValue(pStats.RegenerationAmount, amount) - pStats.RegenerationAmount).ToString("F2");
            case StatUpgradeType.Speed:
                return (speedInfo.DowngradeValue(pStats.GroundSpeed, amount) - pStats.GroundSpeed).ToString("F2");
            case StatUpgradeType.SlideBoostForce:
                return (boostInfo.DowngradeValue(pStats.SlideBoostForce, amount) - pStats.SlideBoostForce).ToString("F2");
            default:
                return "";
        }
    }

    public string GetUpgradeNameAsString(int id)
    {
        StatUpgradeType statUpgradeType = (StatUpgradeType)id;

        switch (statUpgradeType)
        {
            case StatUpgradeType.Health:
                return "Max Health";
            case StatUpgradeType.Regeneration:
                return "Regeneration Rate";
            case StatUpgradeType.Speed:
                return "Speed";
            case StatUpgradeType.SlideBoostForce:
                return "Boost Amount";
            default:
                return "";
        }

    }

    public void UpgradeWithID(int id, int amount)
    {
        StatUpgradeType statUpgradeType = (StatUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case StatUpgradeType.Health:
                pStats.MaxHealth = healthInfo.UpgradeValue(pStats.MaxHealth, amount);
                break;
            case StatUpgradeType.Regeneration:
                pStats.RegenerationAmount = regenerationInfo.UpgradeValue(pStats.RegenerationAmount, amount);
                break;
            case StatUpgradeType.Speed:
                pStats.GroundSpeed = speedInfo.GetLogAmount(pStats.GroundSpeed, pStats.SpeedUpgradeAmount, amount);
                pStats.AirSpeed = speedInfo.GetLogAmount(pStats.AirSpeed, pStats.SpeedUpgradeAmount, amount);
                pStats.SpeedUpgradeAmount += amount;
                break;
            case StatUpgradeType.SlideBoostForce:
                pStats.SlideBoostForce = boostInfo.GetLogAmount(pStats.SlideBoostForce, pStats.BoostUpgradeAmount, amount);
                pStats.AirBoostForce = boostInfo.GetLogAmount(pStats.AirBoostForce, pStats.BoostUpgradeAmount, amount);
                pStats.BoostUpgradeAmount += amount;
                break;
        }

        GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
    }

    public void DownGradeWithID(int id, int amount)
    {
        StatUpgradeType statUpgradeType = (StatUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case StatUpgradeType.Health:
                pStats.MaxHealth = healthInfo.DowngradeValue(pStats.MaxHealth, amount);
                break;
            case StatUpgradeType.Regeneration:
                pStats.RegenerationAmount = regenerationInfo.DowngradeValue(pStats.RegenerationAmount, amount);
                break;
            case StatUpgradeType.Speed:
                pStats.GroundSpeed = speedInfo.DowngradeValue(pStats.GroundSpeed, amount);
                pStats.AirSpeed = speedInfo.DowngradeValue(pStats.AirSpeed, amount);
                pStats.SpeedUpgradeAmount -= amount;
                break;
            case StatUpgradeType.SlideBoostForce:
                pStats.SlideBoostForce = boostInfo.DowngradeValue(pStats.SlideBoostForce, amount);
                pStats.AirBoostForce = boostInfo.DowngradeValue(pStats.AirBoostForce, amount);
                pStats.BoostUpgradeAmount -= amount;
                break;
        }

        GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

    }
}

public class RangedUpgrades
{
    //public const UpgradeType upgradeType = UpgradeType.Ranged;

    StatUpgradeInfo projectileDamage = new(1, -2, 2, null);
    StatUpgradeInfo firerate = new(-0.05f, 0.2f, 3f, 0.08f);
    StatUpgradeInfo magSize = new(2, -3, 4, null);
    StatUpgradeInfo reloadSpeed = new(-0.06f, 0.1f, 4, 0.1f);


    public enum CannonUpgradeType
    {
        Damage,
        Firerate,
        MagSize,
        ReloadSpeed,
    }

    public int GetRandomUpgradeID()
    {
        return UnityEngine.Random.Range(0, Enum.GetValues(typeof(CannonUpgradeType)).Length);
    }

    public string GetUpgradeAmountAsString(int id, int amount)
    {
        CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case CannonUpgradeType.Damage:
                return (projectileDamage.UpgradeValue(pStats.ProjectileDamage, amount) - pStats.ProjectileDamage).ToString("F0");
            case CannonUpgradeType.Firerate:
                return (firerate.UpgradePercentage(pStats.ProjectileFireRate, amount) - pStats.ProjectileFireRate).ToString("F2");
            case CannonUpgradeType.MagSize:
                return (magSize.UpgradeValue(pStats.ProjectileMagSize, amount) - pStats.ProjectileMagSize).ToString("F0");
            case CannonUpgradeType.ReloadSpeed:
                return (reloadSpeed.UpgradePercentage(pStats.ReloadTime, amount) - pStats.ReloadTime).ToString("F2");
            default:
                return "";
        }
    }

    public string GetCurrentAsString(int id)
    {
        CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case CannonUpgradeType.Damage:
                return pStats.ProjectileDamage.ToString("F0");
            case CannonUpgradeType.Firerate:
                return pStats.ProjectileFireRate.ToString("F2");
            case CannonUpgradeType.MagSize:
                return pStats.ProjectileMagSize.ToString("F0");
            case CannonUpgradeType.ReloadSpeed:
                return pStats.ReloadTime.ToString("F2");
            default:
                return "";
        }
    }

    public string GetDowngradeAmountAsString(int id, int amount)
    {
        CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case CannonUpgradeType.Damage:
                return (projectileDamage.DowngradeValue(pStats.ProjectileDamage, amount) - pStats.ProjectileDamage).ToString("F0");
            case CannonUpgradeType.Firerate:
                return (firerate.DowngradeValue(pStats.ProjectileFireRate, amount) - pStats.ProjectileFireRate).ToString("F2");
            case CannonUpgradeType.MagSize:
                return (magSize.DowngradeValue(pStats.ProjectileMagSize, amount) - pStats.ProjectileMagSize).ToString("F0");
            case CannonUpgradeType.ReloadSpeed:
                return (reloadSpeed.DowngradeValue(pStats.ReloadTime, amount) - pStats.ReloadTime).ToString("F2");
            default:
                return "";
        }
    }

    public string GetUpgradeNameAsString(int id)
    {
        CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

        switch (statUpgradeType)
        {
            case CannonUpgradeType.Damage:
                return "Cannon Damage";
            case CannonUpgradeType.Firerate:
                return "Fire-rate";
            case CannonUpgradeType.MagSize:
                return "Mag Size";
            case CannonUpgradeType.ReloadSpeed:
                return "Reload Speed";
            default:
                return "";
        }

    }

    public void UpgradeWithID(int id, int amount)
    {
        CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case CannonUpgradeType.Damage:
                pStats.ProjectileDamage = projectileDamage.UpgradeValue(pStats.ProjectileDamage, amount);
                break;
            case CannonUpgradeType.Firerate:
                pStats.ProjectileFireRate = firerate.UpgradePercentage(pStats.ProjectileFireRate, amount);
                break;
            case CannonUpgradeType.MagSize:
                pStats.ProjectileMagSize = Mathf.FloorToInt(magSize.UpgradeValue(pStats.ProjectileMagSize, amount));
                break;
            case CannonUpgradeType.ReloadSpeed:
                pStats.ReloadTime = reloadSpeed.UpgradePercentage(pStats.ReloadTime, amount);
                break;
        }

        GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
    }

    public void DownGradeWithID(int id, int amount)
    {
        CannonUpgradeType statUpgradeType = (CannonUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case CannonUpgradeType.Damage:
                pStats.ProjectileDamage = projectileDamage.DowngradeValue(pStats.ProjectileDamage, amount);
                break;
            case CannonUpgradeType.Firerate:
                pStats.ProjectileFireRate = firerate.DowngradeValue(pStats.ProjectileFireRate, amount);
                break;
            case CannonUpgradeType.MagSize:
                pStats.ProjectileMagSize = Mathf.FloorToInt(magSize.DowngradeValue(pStats.ProjectileMagSize, amount));
                break;
            case CannonUpgradeType.ReloadSpeed:
                pStats.ReloadTime = reloadSpeed.DowngradeValue(pStats.ReloadTime, amount);
                break;
        }

        GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

    }
}

public class MeleeUpgrades
{
    //public const UpgradeType upgradeType = UpgradeType.Ranged;

    StatUpgradeInfo meleeDamage = new(1, -1, 5, null);
    StatUpgradeInfo meleeAttackTime = new(-0.04f, 0.1f, 0.75f, 0.08f);
    StatUpgradeInfo kickAttackTime = new(-0.03f, 0.15f, 3f, 0.4f);
    StatUpgradeInfo enemyStagger = new(0.2f, -0.06f, 0, null);
    StatUpgradeInfo reach = new(0.2f, -0.2f, 0.5f, 4f);
    StatUpgradeInfo knockback = new(0.5f, -0.4f, 1, 40);


    public enum MeleeUpgradeType
    {
        Damage,
        MeleeAttackTime,
        KickAttackTime,
        EnemyStagger,
        Reach,
        Knockback,
    }

    public int GetRandomUpgradeID()
    {
        return UnityEngine.Random.Range(0, Enum.GetValues(typeof(MeleeUpgradeType)).Length);
    }

    public string GetUpgradeAmountAsString(int id, int amount)
    {
        MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case MeleeUpgradeType.Damage:
                return (meleeDamage.UpgradeValue(pStats.MeleeDamage, amount) - pStats.MeleeDamage).ToString("F2");
            case MeleeUpgradeType.MeleeAttackTime:
                return (meleeAttackTime.UpgradePercentage(pStats.MeleeAttackDelay, amount) - pStats.MeleeAttackDelay).ToString("F2");
            case MeleeUpgradeType.KickAttackTime:
                return (kickAttackTime.UpgradePercentage(pStats.KickAttackDelay, amount) - pStats.KickAttackDelay).ToString("F2");
            case MeleeUpgradeType.EnemyStagger:
                return (enemyStagger.GetLogAmount(pStats.MeleeStaggeTime, pStats.MeleeStaggerUpgradeAmount, amount) - pStats.MeleeStaggeTime).ToString("F2");
            case MeleeUpgradeType.Reach:
                return (reach.GetLogAmount(pStats.MeleeReach, pStats.MeleeReachUpgradeAmount, amount) - pStats.MeleeReach).ToString("F2");
            case MeleeUpgradeType.Knockback:
                return (knockback.UpgradeValue(pStats.KickForce, amount) - pStats.KickForce).ToString("F2");
            default:
                return "";
        }
    }

    public string GetCurrentAsString(int id)
    {
        MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case MeleeUpgradeType.Damage:
                return pStats.MeleeDamage.ToString("F2");
            case MeleeUpgradeType.MeleeAttackTime:
                return pStats.MeleeAttackDelay.ToString("F2");
            case MeleeUpgradeType.KickAttackTime:
                return pStats.KickAttackDelay.ToString("F2");
            case MeleeUpgradeType.EnemyStagger:
                return pStats.MeleeStaggeTime.ToString("F2");
            case MeleeUpgradeType.Reach:
                return pStats.MeleeReach.ToString("F2");
            case MeleeUpgradeType.Knockback:
                return pStats.KickForce.ToString("F2");
            default:
                return "";
        }
    }

    public string GetDowngradeAmountAsString(int id, int amount)
    {
        MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case MeleeUpgradeType.Damage:
                return (meleeDamage.DowngradeValue(pStats.MeleeDamage, amount) - pStats.MeleeDamage).ToString("F2");
            case MeleeUpgradeType.MeleeAttackTime:
                return (meleeAttackTime.DowngradeValue(pStats.MeleeAttackDelay, amount) - pStats.MeleeAttackDelay).ToString("F2");
            case MeleeUpgradeType.KickAttackTime:
                return (meleeAttackTime.DowngradeValue(pStats.KickAttackDelay, amount) - pStats.KickAttackDelay).ToString("F2");
            case MeleeUpgradeType.EnemyStagger:
                return (enemyStagger.DowngradeValue(pStats.MeleeAttackDelay, amount) - pStats.MeleeStaggeTime).ToString("F2");
            case MeleeUpgradeType.Reach:
                return (reach.DowngradeValue(pStats.MeleeReach, amount) - pStats.MeleeReach).ToString("F2");
            case MeleeUpgradeType.Knockback:
                return (knockback.DowngradeValue(pStats.KickForce, amount) - pStats.KickForce).ToString("F2");
            default:
                return "";
        }
    }

    public string GetUpgradeNameAsString(int id)
    {
        MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

        switch (statUpgradeType)
        {
            case MeleeUpgradeType.Damage:
                return "Melee Damage";
            case MeleeUpgradeType.MeleeAttackTime:
                return "Melee Rate";
            case MeleeUpgradeType.KickAttackTime:
                return "Kick Rate";
            case MeleeUpgradeType.EnemyStagger:
                return "Enemy Stagger Time";
            case MeleeUpgradeType.Reach:
                return "Melee Reach";
            case MeleeUpgradeType.Knockback:
                return "Knockback Force";
            default:
                return "";
        }

    }

    public void UpgradeWithID(int id, int amount)
    {
        MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case MeleeUpgradeType.Damage:
                pStats.MeleeDamage = meleeDamage.UpgradeValue(pStats.MeleeDamage, amount);
                break;
            case MeleeUpgradeType.MeleeAttackTime:
                pStats.MeleeAttackDelay = meleeAttackTime.UpgradePercentage(pStats.MeleeAttackDelay, amount);
                break;
            case MeleeUpgradeType.KickAttackTime:
                pStats.KickAttackDelay = kickAttackTime.UpgradePercentage(pStats.KickAttackDelay, amount);
                break;
            case MeleeUpgradeType.EnemyStagger:
                pStats.MeleeStaggeTime = enemyStagger.GetLogAmount(pStats.MeleeStaggeTime, pStats.MeleeStaggerUpgradeAmount, amount);
                pStats.MeleeStaggerUpgradeAmount += amount;
                break;
            case MeleeUpgradeType.Reach:
                pStats.MeleeReach = reach.GetLogAmount(pStats.MeleeReach, pStats.MeleeReachUpgradeAmount, amount);
                pStats.MeleeStaggerUpgradeAmount += amount;
                break;
            case MeleeUpgradeType.Knockback:
                pStats.KickForce = knockback.UpgradeValue(pStats.KickForce, amount);
                break;
        }

        GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
    }

    public void DownGradeWithID(int id, int amount)
    {
        MeleeUpgradeType statUpgradeType = (MeleeUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            case MeleeUpgradeType.Damage:
                pStats.MeleeDamage = meleeDamage.DowngradeValue(pStats.MeleeDamage, amount);
                break;
            case MeleeUpgradeType.MeleeAttackTime:
                pStats.MeleeAttackDelay = meleeAttackTime.DowngradeValue(pStats.MeleeAttackDelay, amount);
                break;
            case MeleeUpgradeType.KickAttackTime:
                pStats.KickAttackDelay = kickAttackTime.DowngradeValue(pStats.KickAttackDelay, amount);
                break;
            case MeleeUpgradeType.EnemyStagger:
                pStats.MeleeStaggeTime = enemyStagger.DowngradeValue(pStats.MeleeStaggeTime, amount);
                break;
            case MeleeUpgradeType.Reach:
                pStats.MeleeReach = reach.DowngradeValue(pStats.MeleeReach, amount);
                break;
            case MeleeUpgradeType.Knockback:
                pStats.KickForce = knockback.DowngradeValue(pStats.KickForce, amount);
                break;
        }

        GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

    }
}

public class MiscellaneousUpgrades
{
    //public const UpgradeType upgradeType = UpgradeType.BaseStat;

    StatUpgradeInfo scrapCarryMax = new(10, -10, 40, null);
    StatUpgradeInfo itemCollectionRange = new(0.5f, -0.2f, 0.5f, 10f);
    StatUpgradeInfo levelTimeLimit = new(2, -2, 20, 600);
    StatUpgradeInfo criticalChance = new(0.01f, -0.02f, 0, 1); // decimal percentage


    public enum MiscellaneousUpgradeType
    {
        ScrapCarry,
        ItemPickupRange,
        TimeLimit,
        CriticalChance,
    }

    public int GetRandomUpgradeID()
    {
        return UnityEngine.Random.Range(0, Enum.GetValues(typeof(MiscellaneousUpgradeType)).Length);
    }

    public string GetUpgradeAmountAsString(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

        switch (statUpgradeType)
        {
            case MiscellaneousUpgradeType.ScrapCarry:
                return (scrapCarryMax.UpgradeValue(mStats.MaxInventoryScrap, amount) - mStats.MaxInventoryScrap).ToString("F0");
            case MiscellaneousUpgradeType.ItemPickupRange:
                return (itemCollectionRange.UpgradeValue(mStats.CollectItemRange, amount) - mStats.CollectItemRange).ToString("F2");
            case MiscellaneousUpgradeType.TimeLimit:
                return (levelTimeLimit.UpgradeValue(mStats.MaxLevelTime, amount) - mStats.MaxLevelTime).ToString("F2");
            case MiscellaneousUpgradeType.CriticalChance:
                return ((criticalChance.UpgradeValue(mStats.CriticalHitChance, amount) - mStats.CriticalHitChance) * 100f).ToString("F2");
            default:
                return "";
        }
    }

    public string GetCurrentAsString(int id)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

        switch (statUpgradeType)
        {
            case MiscellaneousUpgradeType.ScrapCarry:
                return mStats.MaxInventoryScrap.ToString("F0");
            case MiscellaneousUpgradeType.ItemPickupRange:
                return mStats.CollectItemRange.ToString("F2");
            case MiscellaneousUpgradeType.TimeLimit:
                return mStats.MaxLevelTime.ToString("F2");
            case MiscellaneousUpgradeType.CriticalChance:
                return mStats.CriticalHitChance.ToString("F2");
            default:
                return "";
        }
    }

    public string GetDowngradeAmountAsString(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

        switch (statUpgradeType)
        {
            case MiscellaneousUpgradeType.ScrapCarry:
                return (scrapCarryMax.DowngradeValue(mStats.MaxInventoryScrap, amount) - mStats.MaxInventoryScrap).ToString("F0");
            case MiscellaneousUpgradeType.ItemPickupRange:
                return (itemCollectionRange.DowngradeValue(mStats.CollectItemRange, amount) - mStats.CollectItemRange).ToString("F2");
            case MiscellaneousUpgradeType.TimeLimit:
                return (levelTimeLimit.DowngradeValue(mStats.MaxLevelTime, amount) - mStats.MaxLevelTime).ToString("F2");
            case MiscellaneousUpgradeType.CriticalChance:
                return ((criticalChance.DowngradeValue(mStats.CriticalHitChance, amount) - mStats.CriticalHitChance) * 100f).ToString("F2");
            default:
                return "";
        }
    }

    public string GetUpgradeNameAsString(int id)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        switch (statUpgradeType)
        {
            case MiscellaneousUpgradeType.ScrapCarry:
                return "Max Inventory Scrap";
            case MiscellaneousUpgradeType.ItemPickupRange:
                return "Item Pickup Range";
            case MiscellaneousUpgradeType.TimeLimit:
                return "Time Limit";
            case MiscellaneousUpgradeType.CriticalChance:
                return "Critical Hit Chance";
            default:
                return "";
        }

    }

    public void UpgradeWithID(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

        switch (statUpgradeType)
        {
            case MiscellaneousUpgradeType.ScrapCarry:
                mStats.MaxInventoryScrap = Mathf.FloorToInt(scrapCarryMax.UpgradeValue(mStats.MaxInventoryScrap, amount));
                break;
            case MiscellaneousUpgradeType.ItemPickupRange:
                mStats.CollectItemRange = itemCollectionRange.UpgradeValue(mStats.CollectItemRange, amount);
                break;
            case MiscellaneousUpgradeType.TimeLimit:
                mStats.MaxLevelTime = levelTimeLimit.UpgradeValue(mStats.MaxLevelTime, amount);
                break;
            case MiscellaneousUpgradeType.CriticalChance:
                mStats.CriticalHitChance = criticalChance.UpgradeValue(mStats.CriticalHitChance, amount);
                break;
        }

        GameStatsManager.Instance.UpdateStats<MiscellaneousStats>(Stats.miscellaneous, mStats);
    }

    public void DownGradeWithID(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        MiscellaneousStats mStats = GameStatsManager.Instance.GetStats<MiscellaneousStats>(Stats.miscellaneous);

        switch (statUpgradeType)
        {
            case MiscellaneousUpgradeType.ScrapCarry:
                mStats.MaxInventoryScrap = Mathf.FloorToInt(scrapCarryMax.DowngradeValue(mStats.MaxInventoryScrap, amount));
                break;
            case MiscellaneousUpgradeType.ItemPickupRange:
                mStats.CollectItemRange = itemCollectionRange.DowngradeValue(mStats.CollectItemRange, amount);
                break;
            case MiscellaneousUpgradeType.TimeLimit:
                mStats.MaxLevelTime = levelTimeLimit.DowngradeValue(mStats.MaxLevelTime, amount);
                break;
            case MiscellaneousUpgradeType.CriticalChance:
                mStats.CriticalHitChance = criticalChance.DowngradeValue(mStats.CriticalHitChance, amount);
                break;
        }

        GameStatsManager.Instance.UpdateStats<MiscellaneousStats>(Stats.miscellaneous, mStats);

    }
}


public class UpgradeSystem : MonoBehaviour
{
    [Serializable]
    private class UpgradeData
    {
        public UpgradeType UpgradeType;
        public int UpgradeID;
        public int UpgradeAmount;

        public UpgradeData(UpgradeType type, int id, int amount)
        {
            UpgradeType = type;
            UpgradeID = id;
            UpgradeAmount = amount;
        }
    }

    [Serializable]
    private class UpgradeChoice
    {
        public UpgradeData Upgrade;
        public UpgradeData Downgrade;

        public UpgradeChoice(UpgradeData upgrade, UpgradeData downgrade)
        {
            Upgrade = upgrade;
            Downgrade = downgrade;
        }
    }

    StatUpgrades playerStatUpgrades = new();
    RangedUpgrades rangedUpgrades = new();
    MeleeUpgrades meleeUpgrades = new();
    MiscellaneousUpgrades miscellaneousUpgrades = new();


    UpgradeChoice[] upgradeChoices;

    [SerializeField]
    UpgradeItemUI[] upgradeItemUIs;


    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int ran = UnityEngine.Random.Range(0, 3);
            RandomUpgrades((CardTeir)ran);
            print(((CardTeir)ran).ToString());
        }
    }

    public void UpgradeSelected(UpgradeType upgradeType)
    {
        UpgradeChoice upgradeChoice = null;


        foreach (UpgradeChoice choice in upgradeChoices)
        {
            if (choice.Upgrade.UpgradeType == upgradeType) upgradeChoice = choice;
        }

        UpgradeStat(upgradeChoice);
    }

    private void RandomUpgrades(CardTeir cardTeir)
    {
        // int upAmount;
        // int downAmount;


        (int upAmount, int downAmount) = GameManager.Instance.GetUPandDOWNAmounts(cardTeir);



        upgradeChoices = GenerateUpgradeChoices(upAmount, downAmount);

        for (int i = 0; i < 4; i++)
        {
            upgradeItemUIs[i].SetText(GetDisplayText(upgradeChoices[i]));
        }
    }

    private string GetDisplayText(UpgradeChoice upgradeChoice)
    {
        string returnedText = "";

        string upgradeName = "NAME";
        string currentAmountForUpgradeType = "CURRENT AMOUNT";
        string upgradeAmount = "INCRASE AMOUNT";

        switch (upgradeChoice.Upgrade.UpgradeType)
        {
            case UpgradeType.PlayerStats:
                upgradeName = playerStatUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.UpgradeID);
                currentAmountForUpgradeType = playerStatUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.UpgradeID);
                upgradeAmount = playerStatUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
            case UpgradeType.Ranged:
                upgradeName = rangedUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.UpgradeID);
                currentAmountForUpgradeType = rangedUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.UpgradeID);
                upgradeAmount = rangedUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
            case UpgradeType.Melee:
                upgradeName = meleeUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.UpgradeID);
                currentAmountForUpgradeType = meleeUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.UpgradeID);
                upgradeAmount = meleeUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
            case UpgradeType.Misc:
                upgradeName = miscellaneousUpgrades.GetUpgradeNameAsString(upgradeChoice.Upgrade.UpgradeID);
                currentAmountForUpgradeType = miscellaneousUpgrades.GetCurrentAsString(upgradeChoice.Upgrade.UpgradeID);
                upgradeAmount = miscellaneousUpgrades.GetUpgradeAmountAsString(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
        }

        bool isUpgradeNegative = upgradeAmount[0] == '-';

        if (isUpgradeNegative) upgradeAmount = upgradeAmount.Substring(1);

        char sign = (isUpgradeNegative ? '-' : '+');

        returnedText += $"<color=green>+ {upgradeName} {currentAmountForUpgradeType} {sign} {upgradeAmount}</color>";


        if (upgradeChoice.Downgrade.UpgradeAmount <= 0) return returnedText;

        string downgradeName = "NAME";
        string currentAmountForDowngradeType = "CURRENT AMOUNT";
        string downgradeAmount = "DECREASE AMOUNT";

        switch (upgradeChoice.Downgrade.UpgradeType)
        {
            case UpgradeType.PlayerStats:
                downgradeName = playerStatUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.UpgradeID);
                currentAmountForDowngradeType = playerStatUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.UpgradeID);
                downgradeAmount = playerStatUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
            case UpgradeType.Ranged:
                downgradeName = rangedUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.UpgradeID);
                currentAmountForDowngradeType = rangedUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.UpgradeID);
                downgradeAmount = rangedUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
            case UpgradeType.Melee:
                downgradeName = meleeUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.UpgradeID);
                currentAmountForDowngradeType = meleeUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.UpgradeID);
                downgradeAmount = meleeUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
            case UpgradeType.Misc:
                downgradeName = miscellaneousUpgrades.GetUpgradeNameAsString(upgradeChoice.Downgrade.UpgradeID);
                currentAmountForDowngradeType = miscellaneousUpgrades.GetCurrentAsString(upgradeChoice.Downgrade.UpgradeID);
                downgradeAmount = miscellaneousUpgrades.GetDowngradeAmountAsString(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
        }


        bool isDowngradeNegative = downgradeAmount[0] == '-';

        if (isDowngradeNegative) downgradeAmount = downgradeAmount.Substring(1);

        sign = (isDowngradeNegative ? '-' : '+');

        returnedText += $"\n\n<color=red>- {downgradeName} {currentAmountForDowngradeType} {sign} {downgradeAmount}</color>";

        return returnedText;
    }

    private void UpgradeStat(UpgradeChoice upgradeChoice)
    {
        switch (upgradeChoice.Upgrade.UpgradeType)
        {
            case UpgradeType.PlayerStats:
                playerStatUpgrades.UpgradeWithID(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
            case UpgradeType.Ranged:
                rangedUpgrades.UpgradeWithID(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
            case UpgradeType.Melee:
                meleeUpgrades.UpgradeWithID(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
            case UpgradeType.Misc:
                miscellaneousUpgrades.UpgradeWithID(upgradeChoice.Upgrade.UpgradeID, upgradeChoice.Upgrade.UpgradeAmount);
                break;
        }

        if (upgradeChoice.Downgrade.UpgradeAmount <= 0) return;

        switch (upgradeChoice.Downgrade.UpgradeType)
        {
            case UpgradeType.PlayerStats:
                playerStatUpgrades.DownGradeWithID(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
            case UpgradeType.Ranged:
                rangedUpgrades.DownGradeWithID(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
            case UpgradeType.Melee:
                meleeUpgrades.DownGradeWithID(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
            case UpgradeType.Misc:
                miscellaneousUpgrades.DownGradeWithID(upgradeChoice.Downgrade.UpgradeID, upgradeChoice.Downgrade.UpgradeAmount);
                break;
        }
    }


    private UpgradeChoice[] GenerateUpgradeChoices(int upgradeAmount, int downgradeAmount)
    {
        UpgradeChoice[] choices = new UpgradeChoice[4];

        for (int i = 0; i < 4; i++)
        {
            choices[i] = GetRandomBaseOnCatagory((UpgradeType)i, upgradeAmount, downgradeAmount);
        }

        return choices;
    }

    private UpgradeChoice GetRandomBaseOnCatagory(UpgradeType upgradeType, int upgradeAmount, int downgradeAmount)
    {
        return new UpgradeChoice(GetRandomUpgradeForType(upgradeType, upgradeAmount), GetDowngradeExcludingType(upgradeType, downgradeAmount));
    }

    private UpgradeData GetRandomUpgradeForType(UpgradeType upgradeType, int amount)
    {
        UpgradeData upgradeData = null;

        switch (upgradeType)
        {
            case UpgradeType.Melee:
                return upgradeData = new UpgradeData(UpgradeType.Melee, meleeUpgrades.GetRandomUpgradeID(), amount);
            case UpgradeType.Misc:
                return upgradeData = new UpgradeData(UpgradeType.Misc, miscellaneousUpgrades.GetRandomUpgradeID(), amount);
            case UpgradeType.PlayerStats:
                return upgradeData = new UpgradeData(UpgradeType.PlayerStats, playerStatUpgrades.GetRandomUpgradeID(), amount);
            case UpgradeType.Ranged:
                return upgradeData = new UpgradeData(UpgradeType.Ranged, rangedUpgrades.GetRandomUpgradeID(), amount);
            default:
                return null;
        }
    }

    private UpgradeData GetDowngradeExcludingType(UpgradeType upgradeType, int amount)
    {
        int rand = UnityEngine.Random.Range(1, 4); // this only goes to 3 but we want that.
        /*
        1 - player stats
        2 - range stats
        3 - melee stats
        4 - misc stats
        */

        switch (upgradeType)
        {
            case UpgradeType.Melee:
                if (rand == 1)
                {
                    return GetDowngradeForType(UpgradeType.PlayerStats, amount);
                }
                else if (rand == 2)
                {
                    return GetDowngradeForType(UpgradeType.Ranged, amount);
                }
                else if (rand == 3)
                {
                    return GetDowngradeForType(UpgradeType.Misc, amount);
                }
                break;
            case UpgradeType.Misc:
                if (rand == 1)
                {
                    return GetDowngradeForType(UpgradeType.PlayerStats, amount);
                }
                else if (rand == 2)
                {
                    return GetDowngradeForType(UpgradeType.Ranged, amount);
                }
                else if (rand == 3)
                {
                    return GetDowngradeForType(UpgradeType.Melee, amount);
                }
                break;
            case UpgradeType.PlayerStats:
                if (rand == 1)
                {
                    return GetDowngradeForType(UpgradeType.Ranged, amount);
                }
                else if (rand == 2)
                {
                    return GetDowngradeForType(UpgradeType.Melee, amount);
                }
                else if (rand == 3)
                {
                    return GetDowngradeForType(UpgradeType.Misc, amount);
                }
                break;
            case UpgradeType.Ranged:
                if (rand == 1)
                {
                    return GetDowngradeForType(UpgradeType.PlayerStats, amount);
                }
                else if (rand == 2)
                {
                    return GetDowngradeForType(UpgradeType.Melee, amount);
                }
                else if (rand == 3)
                {
                    return GetDowngradeForType(UpgradeType.Misc, amount);
                }
                break;
        }
        return null;
    }

    private UpgradeData GetDowngradeForType(UpgradeType upgradeType, int amount)
    {
        UpgradeData upgradeData = null;

        switch (upgradeType)
        {
            case UpgradeType.Melee:
                return upgradeData = new UpgradeData(UpgradeType.Melee, meleeUpgrades.GetRandomUpgradeID(), amount);
            case UpgradeType.Misc:
                return upgradeData = new UpgradeData(UpgradeType.Misc, miscellaneousUpgrades.GetRandomUpgradeID(), amount);
            case UpgradeType.PlayerStats:
                return upgradeData = new UpgradeData(UpgradeType.PlayerStats, playerStatUpgrades.GetRandomUpgradeID(), amount);
            case UpgradeType.Ranged:
                return upgradeData = new UpgradeData(UpgradeType.Ranged, rangedUpgrades.GetRandomUpgradeID(), amount);
            default:
                return null;
        }
    }
}

