using System;
using TMPro;
using UnityEngine;

public enum HitPointType
{
    armor,
    normal,
    weak,
}

[Serializable]
public class HitPointClass
{
    public HitPointType key;
    public TMP_ColorGradient gradient;
    public TMP_ColorGradient critGradient;
    public float multiplier;
}

[CreateAssetMenu(fileName = "SO_HitPointsData", menuName = "ScriptableObjects/HitPoints/HitPointsData")]
public class HitPointsDataSO : ScriptableObject
{
    [SerializeField]
    HitPointClass[] data;

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
