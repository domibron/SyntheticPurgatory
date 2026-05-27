using UnityEngine;

public static class Utils
{
    public static Vector3 GetLevelVectorY(Vector3 v, float y = 0)
    {
        return new Vector3(v.x, y, v.z);
    }

    /// <summary>
    /// Yeet an object in a random direction
    /// </summary>
    /// <param name="objectToThrow">Object that will be flung</param>
    /// <param name="xzForce">Force applied horizontally</param>
    /// <param name="yForce">Force applied vertically</param>
    public static void ThrowObject(GameObject objectToThrow, float xzForce, float yForce)
    {
        // Choose random direction
        float angle = Random.Range(0, 359) * Mathf.PI / 180;
        float dirX = Mathf.Cos(angle);
        float dirZ = Mathf.Sin(angle);

        // Add force to object in the previously created direction, multiplied changeable force and additional random number for variety
        objectToThrow.GetComponent<Rigidbody>().AddForce
            (new Vector3(dirX * xzForce * Random.Range(0.8f, 1.2f), yForce, dirZ * xzForce * Random.Range(0.8f, 1.2f)), ForceMode.Impulse);

    }
}
