using UnityEngine;

public class ResetDroppablePlatformAuto : MonoBehaviour
{
    DroppablePlatform droppablePlatform;

    private bool hasFallen = false;

    [SerializeField]
    private float waitToReset = 15f;

    private float currentTimer = 0f;

    void Awake()
    {
        droppablePlatform = GetComponent<DroppablePlatform>();
    }


    // Update is called once per frame
    void Update()
    {
        if (hasFallen && currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
        }
        else if (hasFallen && currentTimer <= 0)
        {
            droppablePlatform.Rise();
            hasFallen = false;
        }
        else if (!hasFallen && droppablePlatform.HasDropped())
        {
            hasFallen = true;
            currentTimer = waitToReset;
        }
    }

}
