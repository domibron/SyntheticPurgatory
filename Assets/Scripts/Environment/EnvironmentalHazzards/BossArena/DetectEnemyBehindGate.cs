using UnityEngine;

/// <summary>
/// Checks if any enemies are behind the area gate to prevent it from closing.
/// </summary>
public class DetectEnemyBehindGate : MonoBehaviour
{
    /// <summary>
    /// The enemy spawner in control of this gate to inform.
    /// </summary>
    [SerializeField]
    EnemySpawnAtGates enemySpawnAtGates;

    /// <summary>
    /// Is the left gate in the arena used to inform the controller which side has enemies.
    /// </summary>
    [SerializeField]
    bool isLeftGate = false;

    // Rather than using a trigger, you use a physics overlap box? ok...
    /// <summary>
    /// The trigger box to use the bounds for the physics check.
    /// </summary>
    [SerializeField]
    BoxCollider boxCollider;

    // private int count = 0;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        // UpdateState(count > 0);

        Collider[] colliders = Physics.OverlapBox(transform.position + boxCollider.center, boxCollider.size / 2f, transform.rotation, LayerMask.GetMask(Constants.EnemyLayer));

        UpdateState(colliders.Length > 0);
    }

    /// <summary>
    /// Informs the controller if there are enemies at the gate.
    /// </summary>
    /// <param name="state">Are enemies remaining behind the gate.</param>
    private void UpdateState(bool state)
    {
        if (isLeftGate)
            enemySpawnAtGates.LeftGateDetection(state);
        else
            enemySpawnAtGates.RightGateDetection(state);
    }
}
