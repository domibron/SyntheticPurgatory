using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    BaseStat,
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

    public void DownGrade(int id, int amount)
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

    public void DownGrade(int id, int amount)
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

    public void DownGrade(int id, int amount)
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

    StatUpgradeInfo healthInfo = new(8, -15, 10, null);
    StatUpgradeInfo regenerationInfo = new(0.5f, -0.2f, 0f, null);
    StatUpgradeInfo speedInfo = new(1, -0.4f, 1, null);
    StatUpgradeInfo boostInfo = new(1.2f, -0.5f, 2, null);


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

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            // case MiscellaneousUpgradeType.Health:
            //     return (healthInfo.UpgradeValue(pStats.MaxHealth, amount) - pStats.MaxHealth).ToString("F0");
            // case MiscellaneousUpgradeType.Regeneration:
            //     return (regenerationInfo.UpgradeValue(pStats.RegenerationAmount, amount) - pStats.RegenerationAmount).ToString("F2");
            // case MiscellaneousUpgradeType.Speed:
            //     return (speedInfo.GetLogAmount(pStats.GroundSpeed, pStats.SpeedUpgradeAmount, amount) - pStats.GroundSpeed).ToString("F2");
            // case MiscellaneousUpgradeType.SlideBoostForce:
            //     return (boostInfo.GetLogAmount(pStats.SlideBoostForce, pStats.BoostUpgradeAmount, amount) - pStats.SlideBoostForce).ToString("F2");
            default:
                return "";
        }
    }

    public string GetDowngradeAmountAsString(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        switch (statUpgradeType)
        {
            // case MiscellaneousUpgradeType.Health:
            //     return (healthInfo.DowngradeValue(pStats.MaxHealth, amount) - pStats.MaxHealth).ToString("F0");
            // case MiscellaneousUpgradeType.Regeneration:
            //     return (regenerationInfo.DowngradeValue(pStats.RegenerationAmount, amount) - pStats.RegenerationAmount).ToString("F2");
            // case MiscellaneousUpgradeType.Speed:
            //     return (speedInfo.DowngradeValue(pStats.GroundSpeed, amount) - pStats.GroundSpeed).ToString("F2");
            // case MiscellaneousUpgradeType.SlideBoostForce:
            //     return (boostInfo.DowngradeValue(pStats.SlideBoostForce, amount) - pStats.SlideBoostForce).ToString("F2");
            default:
                return "";
        }
    }

    public string GetUpgradeNameAsString(int id)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        switch (statUpgradeType)
        {
            // case MiscellaneousUpgradeType.Health:
            //     return "Max Health";
            // case MiscellaneousUpgradeType.Regeneration:
            //     return "Regeneration Rate";
            // case MiscellaneousUpgradeType.Speed:
            //     return "Speed";
            // case MiscellaneousUpgradeType.SlideBoostForce:
            //     return "Boost Amount";
            default:
                return "";
        }

    }

    public void UpgradeWithID(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        // PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        // switch (statUpgradeType)
        // {
        //     case MiscellaneousUpgradeType.Health:
        //         pStats.MaxHealth = healthInfo.UpgradeValue(pStats.MaxHealth, amount);
        //         break;
        //     case MiscellaneousUpgradeType.Regeneration:
        //         pStats.RegenerationAmount = regenerationInfo.UpgradeValue(pStats.RegenerationAmount, amount);
        //         break;
        //     case MiscellaneousUpgradeType.Speed:
        //         pStats.GroundSpeed = speedInfo.GetLogAmount(pStats.GroundSpeed, pStats.SpeedUpgradeAmount, amount);
        //         pStats.AirSpeed = speedInfo.GetLogAmount(pStats.AirSpeed, pStats.SpeedUpgradeAmount, amount);
        //         pStats.SpeedUpgradeAmount += amount;
        //         break;
        //     case MiscellaneousUpgradeType.SlideBoostForce:
        //         pStats.SlideBoostForce = boostInfo.GetLogAmount(pStats.SlideBoostForce, pStats.BoostUpgradeAmount, amount);
        //         pStats.AirBoostForce = boostInfo.GetLogAmount(pStats.AirBoostForce, pStats.BoostUpgradeAmount, amount);
        //         pStats.BoostUpgradeAmount += amount;
        //         break;
        // }

        // GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);
    }

    public void DownGrade(int id, int amount)
    {
        MiscellaneousUpgradeType statUpgradeType = (MiscellaneousUpgradeType)id;

        // PlayerStats pStats = GameStatsManager.Instance.GetStats<PlayerStats>(Stats.player);

        // switch (statUpgradeType)
        // {
        //     case MiscellaneousUpgradeType.Health:
        //         pStats.MaxHealth = healthInfo.DowngradeValue(pStats.MaxHealth, amount);
        //         break;
        //     case MiscellaneousUpgradeType.Regeneration:
        //         pStats.RegenerationAmount = regenerationInfo.DowngradeValue(pStats.RegenerationAmount, amount);
        //         break;
        //     case MiscellaneousUpgradeType.Speed:
        //         pStats.GroundSpeed = speedInfo.DowngradeValue(pStats.GroundSpeed, amount);
        //         pStats.AirSpeed = speedInfo.DowngradeValue(pStats.AirSpeed, amount);
        //         pStats.SpeedUpgradeAmount -= amount;
        //         break;
        //     case MiscellaneousUpgradeType.SlideBoostForce:
        //         pStats.SlideBoostForce = boostInfo.DowngradeValue(pStats.SlideBoostForce, amount);
        //         pStats.AirBoostForce = boostInfo.DowngradeValue(pStats.AirBoostForce, amount);
        //         pStats.BoostUpgradeAmount -= amount;
        //         break;
        // }

        // GameStatsManager.Instance.UpdateStats<PlayerStats>(Stats.player, pStats);

    }
}


public class UpgradeSystem : MonoBehaviour
{
    private class UpgradeData
    {
        UpgradeType upgradeType;
        int upgradeID;
        int upgradeAmount;

        public UpgradeData(UpgradeType type, int id, int amount)
        {
            upgradeType = type;
            upgradeID = id;
            upgradeAmount = amount;
        }
    }

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

    StatUpgrades statUpgrades = new();
    RangedUpgrades rangedUpgrades = new();
    MeleeUpgrades meleeUpgrades = new();
    MiscellaneousUpgrades miscellaneousUpgrades = new();

    UpgradeChoice[] upgradeChoices;

    void Start()
    {

    }

    void Update()
    {

    }
}

