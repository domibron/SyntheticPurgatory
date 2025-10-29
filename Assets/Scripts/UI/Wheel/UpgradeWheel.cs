using System;
using UnityEngine;



[Serializable]
public class UpgradeWheelSlot
{
    public float ZRotationStart = 0;
    public UpgradeType UpgradeType;
    public float SegmentLength = 90f;
}

public class UpgradeWheel : MonoBehaviour
{
    [SerializeField]
    UpgradeWheelSlot[] upgradeWheelSlots;

    public event Action<UpgradeType> WheelResult;

    private bool wheelIsSpinning = false;

    [SerializeField]
    private float wheelSpinForce = 30f;

    private float currentWheelSpinForce = 0f;

    [SerializeField]
    private float friction = 1f;

    [SerializeField]
    private Transform wheelImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpinWheel();
        }

        if (wheelIsSpinning && currentWheelSpinForce > 0f)
        {
            currentWheelSpinForce -= Time.deltaTime * friction;
            wheelImage.transform.Rotate(new Vector3(0, 0, currentWheelSpinForce));
        }
        if (wheelIsSpinning && currentWheelSpinForce <= 0)
        {
            currentWheelSpinForce = 0;
            wheelIsSpinning = false;
            WheelSpinEnd();
        }
    }

    public void SpinWheel()
    {
        Vector3 wheelRotation = wheelImage.transform.localRotation.eulerAngles;
        wheelImage.transform.localRotation = Quaternion.Euler(wheelRotation.x, wheelRotation.y, UnityEngine.Random.Range(0f, 360f));
        currentWheelSpinForce = wheelSpinForce;
        wheelIsSpinning = true;
    }

    private void WheelSpinEnd()
    {
        // get random upgrade from list.
        WheelResult?.Invoke(UpgradeType.PlayerStats);
    }

}
