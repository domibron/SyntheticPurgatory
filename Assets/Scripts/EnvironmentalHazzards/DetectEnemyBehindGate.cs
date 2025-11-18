using UnityEngine;

public class DetectEnemyBehindGate : MonoBehaviour
{
    [SerializeField]
    EnemySpawnAtGates enemySpawnAtGates;

    [SerializeField]
    bool isLeftGate = false;

    private int count = 0;

    void Update()
    {
        UpdateState(count > 0);
    }

    void OnTriggerEnter(Collider other)
    {
        // UpdateState(true);
        count++;
    }

    void OnTriggerExit(Collider other)
    {
        count--;
    }



    private void UpdateState(bool state)
    {
        if (isLeftGate)
            enemySpawnAtGates.LeftGateDetection(state);
        else
            enemySpawnAtGates.RightGateDetection(state);
    }
}
