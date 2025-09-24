using UnityEngine;

public class PlayerTempMovement : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField]
    private float friction = 10f;
    [SerializeField]
    private float ground_accelerate = 3f;
    [SerializeField]
    private float max_velocity_ground = 30f;
    [SerializeField]
    private float air_accelerate = 5f;
    [SerializeField]
    private float max_velocity_air = 30f;


    Vector3 currentVel = Vector3.zero;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public

    void FixedUpdate()
    {

    }


    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float max_velocity)
    {
        accelDir.Normalize();


        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.fixedDeltaTime;

        if (projVel + accelVel > max_velocity)
            accelVel = max_velocity - projVel;

        return prevVelocity + accelDir * accelVel;
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        float speed = prevVelocity.magnitude;
        if (speed != 0)
        {
            float drop = speed * friction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accelerate(accelDir, prevVelocity, ground_accelerate, max_velocity_ground);
    }

    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        return Accelerate(accelDir, prevVelocity, air_accelerate, max_velocity_air);
    }
}
