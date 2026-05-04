using UnityEngine;

public class LightFlash : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToFlash;

    [SerializeField]
    private float onTime = 0.3f;

    [SerializeField]
    private float offTime = 0.5f;

    private float currentTime = 0;

    private bool isActive = false;

    private bool isFlashState = false;


    void Start()
    {
        StopFlashing();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (currentTime > 0) currentTime -= Time.deltaTime;
        else
        {
            if (isFlashState)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }
    }

    public void StartFlashing()
    {
        isActive = true;
        TurnOn();
    }

    public void StopFlashing()
    {
        isActive = false;
        TurnOff();
    }

    private void TurnOn()
    {
        isFlashState = true;
        currentTime = onTime;
        objectToFlash.SetActive(isFlashState);
    }

    private void TurnOff()
    {
        isFlashState = false;
        currentTime = offTime;
        objectToFlash.SetActive(isFlashState);
    }
}
