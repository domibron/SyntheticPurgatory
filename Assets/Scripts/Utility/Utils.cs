using System;
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
        float angle = UnityEngine.Random.Range(0, 359) * Mathf.PI / 180;
        float dirX = Mathf.Cos(angle);
        float dirZ = Mathf.Sin(angle);

        // Add force to object in the previously created direction, multiplied changeable force and additional random number for variety
        objectToThrow.GetComponent<Rigidbody>().AddForce
            (new Vector3(dirX * xzForce * UnityEngine.Random.Range(0.8f, 1.2f), yForce, dirZ * xzForce * UnityEngine.Random.Range(0.8f, 1.2f)), ForceMode.Impulse);

    }


    /// <summary>
    /// Checks if the flag enum only has one flag.
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="value">The enum you want to check.</param>
    /// <returns>True if it's contains only one flag.</returns>
    public static bool IsSingleFlag<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        ulong bits;

        switch (Type.GetTypeCode(Enum.GetUnderlyingType(typeof(TEnum))))
        {
            case TypeCode.SByte:
                bits = unchecked((byte)Convert.ToSByte(value));
                break;
            case TypeCode.Byte:
                bits = Convert.ToByte(value);
                break;
            case TypeCode.Int16:
                bits = unchecked((ushort)Convert.ToInt16(value));
                break;
            case TypeCode.UInt16:
                bits = Convert.ToUInt16(value);
                break;
            case TypeCode.Int32:
                bits = unchecked((ushort)Convert.ToInt32(value));
                break;
            case TypeCode.UInt32:
                bits = Convert.ToUInt32(value);
                break;
            case TypeCode.Int64:
                bits = unchecked((ushort)Convert.ToInt64(value));
                break;
            case TypeCode.UInt64:
                bits = Convert.ToUInt64(value);
                break;
            default:
                return false;
        }

        return bits != 0 && (bits & (bits - 1)) == 0;

    }
}
