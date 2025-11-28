using UnityEngine;

public class DeParentObject : MonoBehaviour
{
    void Awake()
    {
        transform.parent = null;
    }
}
