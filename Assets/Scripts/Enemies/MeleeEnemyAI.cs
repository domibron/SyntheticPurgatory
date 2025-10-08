using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    public GameObject goal;
    public float baseSpeed = 3.5f;
    public float turnReduction = 5;

    void Start()
    {

    }

    public Vector3 oldRotation;
    private void FixedUpdate()
    {
        float leftTreadSpeed = 1;
        float rightTreadSpeed = 1;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.transform.position;
        float angularVelocity = transform.rotation.eulerAngles.y - oldRotation.y;
        if (angularVelocity > 0)
        {
            rightTreadSpeed /= Mathf.Max(0.01f, angularVelocity);
        }
        else if (angularVelocity < 0)
        {
            leftTreadSpeed /= Mathf.Max(0.01f, -angularVelocity);
        }

        agent.speed = Mathf.Min(baseSpeed * (leftTreadSpeed + rightTreadSpeed) / turnReduction, baseSpeed);

        print("left: " + leftTreadSpeed + "   right: " + rightTreadSpeed);
        oldRotation = transform.rotation.eulerAngles;
    }
}
