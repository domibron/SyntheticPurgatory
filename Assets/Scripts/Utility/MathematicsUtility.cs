using UnityEngine;

public enum ArcType
{
    Any,
    DirectCurve,
    HighCurve,
}

public static class MathematicsUtility
{

    public static Quaternion GenerateProjectileAngle(Transform spawnPoint, float verticalOffset = 10f, float horizontalDisplacement = 10f, float verticalDisplacement = 10f)
    {
        return Quaternion.AngleAxis(verticalOffset, spawnPoint.right)
                * Quaternion.AngleAxis(UnityEngine.Random.Range(-horizontalDisplacement, horizontalDisplacement), spawnPoint.up)
                * Quaternion.AngleAxis(UnityEngine.Random.Range(-verticalDisplacement, verticalDisplacement), spawnPoint.right);
    }

    public static float GetAngleForFireProjectile(Vector3 startPos, Vector3 targetPos, float force, ArcType arcType = ArcType.Any)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        float distance = Vector2.Distance(new Vector2(startPos.x, startPos.z), new Vector2(targetPos.x, targetPos.z));
        float heightOffset = targetPos.y - startPos.y;
        //subtract to get direct curve and adding is high curve.
        //A = arctan((v^2 ± SQRT(v^4 - g(gx^2 + 2yv^2)))/gx)

        float angle01 = Mathf.Atan((Mathf.Pow(force, 2) + Mathf.Sqrt(Mathf.Pow(force, 4) - (gravity * (gravity * Mathf.Pow(distance, 2) + (2 * heightOffset) * Mathf.Pow(force, 2))))) / (gravity * distance));

        //print("Angle 01 : " + angle01);

        float angle02 = Mathf.Atan((Mathf.Pow(force, 2) - Mathf.Sqrt(Mathf.Pow(force, 4) - (gravity * (gravity * Mathf.Pow(distance, 2) + (2 * heightOffset) * Mathf.Pow(force, 2))))) / (gravity * distance));

        //print("Angle 02 : " + angle02);
        float angleResult;
        if (arcType == ArcType.Any)
            angleResult = Mathf.Min(angle01, angle02);
        else if (arcType == ArcType.DirectCurve)
            angleResult = angle02;
        else
            angleResult = angle01;

        //print("Angle : " + angleResult * Mathf.Rad2Deg);
        return angleResult * Mathf.Rad2Deg;
    }

    public static float GetStat(float baseStat, float percentageA, float percentageB = 0, float percentageC = 0)
    {
        float mult = (1 + percentageA) * (1 + percentageB) * (1 + percentageC);
        float add = 1 + (percentageA + percentageB + percentageC);

        if (mult < 0) mult = 0;
        if (add < 0) add = 0;

        if (Mathf.Abs(1 - mult) < Mathf.Abs(1 - add))
            return baseStat * mult;
        else
            return baseStat * add;
    }

}
