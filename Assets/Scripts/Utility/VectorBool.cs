using System;


[Serializable]
public struct VectorBool
{
    public bool x;
    public bool y;
    public bool z;

    public VectorBool(bool x, bool y, bool z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
