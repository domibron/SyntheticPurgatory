using UnityEngine;

public interface IKickable
{
    public void KickObject(Vector3 forceAndDir, ForceMode forceMode = ForceMode.Force);
}
