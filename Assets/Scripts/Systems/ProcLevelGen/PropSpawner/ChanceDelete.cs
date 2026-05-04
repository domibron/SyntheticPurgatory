using UnityEngine;
using Random = UnityEngine.Random;

public class ChanceDelete : MonoBehaviour
{
    /// <summary>
    /// Percentage chance for object to delete itself on awake
    /// </summary>
    [SerializeField, Range(0, 100)]
    private float noActivationChance = 40;

    private void Awake()
    {
        if (noActivationChance > Random.Range(0, 99)) { Destroy(this.gameObject); }
    }
}
