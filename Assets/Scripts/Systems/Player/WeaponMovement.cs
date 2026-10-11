using UnityEngine;


public class WeaponMovement : MonoBehaviour
{
    private Vector3 m_targetLocalPos;
    private Vector3 m_targetLocalRotation;

    private Vector3 m_pushForce;
    private Vector3 m_pushCurrent;

    private Vector3 m_rotationForce;
    private Vector3 m_rotationCurrent;
    private Vector3 m_setRotationOffset;

    void Awake()
    {
        m_targetLocalPos = transform.localPosition;
        m_targetLocalRotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        m_pushCurrent = Vector3.Lerp(m_pushCurrent, m_pushForce, 40f * Time.deltaTime); // Move camera towards punt.
        m_pushForce = Vector3.Lerp(m_pushForce, Vector3.zero, 10f * Time.deltaTime); // Slowly reset punt.

        transform.localPosition = m_targetLocalPos + m_pushCurrent;

        m_rotationCurrent = Vector3.Lerp(m_rotationCurrent, m_rotationForce, 40f * Time.deltaTime); // Move camera towards punt.
        m_rotationForce = Vector3.Lerp(m_rotationForce, Vector3.zero, 10f * Time.deltaTime); // Slowly reset punt.

        transform.localRotation = Quaternion.Euler(m_targetLocalRotation + m_rotationCurrent + m_setRotationOffset);
    }



    public void Push(Vector3 directionAndForce)
    {
        m_pushForce += directionAndForce;
    }

    public void PushBack(float force)
    {
        Push(Vector3.back * force);
    }

    public void PushForward(float force)
    {
        Push(Vector3.forward * force);
    }

    public void PushLeft(float force)
    {
        Push(Vector3.left * force);
    }

    public void PushRight(float force)
    {
        Push(Vector3.right * force);
    }

    public void PushUp(float force)
    {
        Push(Vector3.up * force);
    }

    public void PushDown(float force)
    {
        Push(Vector3.down * force);
    }



    public void SetRotationOffset(Vector3 angles)
    {
        m_setRotationOffset = angles;
    }

    public void Rotate(Vector3 directionAndForce)
    {
        m_pushForce += directionAndForce;
    }

    public void RotateLeft(float force)
    {
        Rotate(Vector3.down * force);
    }

    public void RotateRight(float force)
    {
        Rotate(Vector3.up * force);
    }

    public void RotateUp(float force)
    {
        Rotate(Vector3.left * force);
    }

    public void RotateDown(float force)
    {
        Rotate(Vector3.right * force);
    }

    public void RotateClockWise(float force)
    {
        Rotate(Vector3.back * force);
    }

    public void RotateCounterClockWise(float force)
    {
        Rotate(Vector3.forward * force);
    }
}
