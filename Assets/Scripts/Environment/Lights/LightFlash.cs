using UnityEngine;

/// <summary>
/// Flash a object with the specified on and off times.
/// </summary>
public class LightFlash : MonoBehaviour
{
    /// <summary>
    /// The desired object to flash.
    /// </summary>
    [SerializeField]
    private GameObject objectToFlash;

    /// <summary>
    /// How long to show the object in seconds.
    /// </summary>
    [SerializeField]
    private float onTime = 0.3f;

    /// <summary>
    /// How long to hide the object in seconds.
    /// </summary>
    [SerializeField]
    private float offTime = 0.5f;

    /// <summary>
    /// The current time for the current state.
    /// </summary>
    private float currentTime = 0;

    /// <summary>
    /// Is this flash currently flashing.
    /// </summary>
    private bool isActive = false;

    /// <summary>
    /// The flash state. True = currently on.
    /// </summary>
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

    /// <summary>
    /// Begin the flashing process.
    /// </summary>
    public void StartFlashing()
    {
        isActive = true;
        TurnOn();
    }

    /// <summary>
    /// Stop the flashing process.
    /// </summary>
    public void StopFlashing()
    {
        isActive = false;
        TurnOff();
    }

    /// <summary>
    /// Set the states for on.
    /// </summary>
    private void TurnOn()
    {
        isFlashState = true;
        currentTime = onTime;
        objectToFlash.SetActive(isFlashState);
    }

    /// <summary>
    /// Set the states for off.
    /// </summary>
    private void TurnOff()
    {
        isFlashState = false;
        currentTime = offTime;
        objectToFlash.SetActive(isFlashState);
    }
}
