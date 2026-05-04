using UnityEngine;

public class DetectEnemyBehindGate : MonoBehaviour
{
    [SerializeField]
    EnemySpawnAtGates enemySpawnAtGates;

    [SerializeField]
    bool isLeftGate = false;

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

    // void OnTriggerEnter(Collider other)
    // {
    //     // UpdateState(true);
    //     count++;
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     count--;
    // }



    private void UpdateState(bool state)
    {
        if (isLeftGate)
            enemySpawnAtGates.LeftGateDetection(state);
        else
            enemySpawnAtGates.RightGateDetection(state);
    }
}
