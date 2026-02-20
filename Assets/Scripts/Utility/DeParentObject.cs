using UnityEngine;

public class DeParentObject : MonoBehaviour
{
    void Start()
    {
        transform.parent = null;
    }
}
