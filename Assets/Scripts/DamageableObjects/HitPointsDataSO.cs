using System;
using TMPro;
using UnityEngine;

public enum HitPointType
{
    armor,
    normal,
    crit,
}

[Serializable]
public class HitPointClass
{
    public HitPointType key;
    public TMP_ColorGradient gradient;
}

[CreateAssetMenu(fileName = "SO_HitPointsData", menuName = "ScriptableObjects/HitPoints/HitPointsData")]
public class HitPointsDataSO : ScriptableObject
{
    [SerializeField]
    HitPointClass[] data;

    public TMP_ColorGradient GetGradient(HitPointType key)
    {
        foreach (HitPointClass dum in data)
        {
            if (dum.key == key)
            {
                return dum.gradient;
            }
        }

        return null;
    }

}
