using UnityEngine;

public static class Utils
{
    public static float GetStat(float baseStat, float pA, float pB = 0, float pC = 0)
    {
        float mult = (1 + pA) * (1 + pB) * (1 + pC);
        float add = 1 + (pA + pB + pC);

        if (mult < 0) mult = 0;
        if (add < 0) add = 0;

        if (Mathf.Abs(1 - mult) < Mathf.Abs(1 - add))
            return baseStat * mult;
        else
            return baseStat * add;
    }
}
