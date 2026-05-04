using System;
using TMPro;
using UnityEngine;


public enum HitPointType
{
    armor,
    normal,
    weak,
}

/// <summary>
/// Data for the hit point.
/// </summary>
[Serializable]
public class HitPointClass
{
    public HitPointType key;
    public TMP_ColorGradient gradient;
    public TMP_ColorGradient critGradient;
    public float multiplier;
}

/// <summary>
/// Hit point data defining all hit points. Stores gradient colour and multiplier values.
/// </summary>
[CreateAssetMenu(fileName = "SO_HitPointsData", menuName = "ScriptableObjects/HitPoints/HitPointsData")]
public class HitPointsDataSO : ScriptableObject
{
    /// <summary>
    /// Data for each hit point type.
    /// </summary>
    [SerializeField]
    HitPointClass[] data;

    /// <summary>
    /// Gets the gradient for the hit point with the provided key.
    /// </summary>
    /// <param name="key">The hit point type to get the gradient from.</param>
    /// <param name="critical">Whether it was a critical hit.</param>
    /// <returns>The gradient for use of display purposes.</returns>
    public TMP_ColorGradient GetGradient(HitPointType key, bool critical)
    {
        foreach (HitPointClass hitPointData in data)
        {
            if (hitPointData.key == key)
            {
                return critical ? hitPointData.critGradient : hitPointData.gradient;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the multiplier for the hit point type.
    /// </summary>
    /// <param name="key">The hit point type to get the multiplier for.</param>
    /// <returns>The multiplier value.</returns>
    public float GetMultiplier(HitPointType key)
    {
        foreach (HitPointClass hitPointData in data)
        {
            if (hitPointData.key == key)
            {
                return hitPointData.multiplier;
            }
        }

        return 1;
    }


}
