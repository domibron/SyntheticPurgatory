using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Handles getting, detecting and handling input for you.
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Singleton for the <see cref="InputManager"/>.
    /// </summary>
    public static InputManager Instance { get; private set; }

    /// <summary>
    /// The player input to listen to the inputs.
    /// </summary>
    private PlayerInput playerInput;

    /// <summary>
    /// The currently active input device the player is using.
    /// </summary>
    private InputDeviceType activeInputDevice = InputDeviceType.Keyboard;

    /// <summary>
    /// Delegate for when a device is changed. Current device, last device.
    /// </summary>
    /// <param name="currentDevice">The current device being used.</param>
    /// <param name="lastDevice">The last device the player was using.</param>
    public delegate void InputDeviceChangedDelegate(InputDeviceType currentDevice, InputDeviceType lastDevice);

    /// <summary>
    /// Event for when the device changes.
    /// </summary>
    public event InputDeviceChangedDelegate onDeviceChanged;

    /// <summary>
    /// The last device that was active.
    /// </summary>
    private InputDeviceType previousDevice = InputDeviceType.Keyboard;

    /// <summary>
    /// All the types of input that the player can use.
    /// </summary>
    public enum InputDeviceType
    {
        Keyboard,
        Gamepad,
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("Multiple input managers detected, destroying this one.", gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (playerInput.currentControlScheme == InputDeviceType.Gamepad.ToString())
        {
            activeInputDevice = InputDeviceType.Gamepad;
        }
        else
        {
            activeInputDevice = InputDeviceType.Keyboard;
        }

        if (activeInputDevice != previousDevice)
        {
            onDeviceChanged?.Invoke(activeInputDevice, previousDevice);
            previousDevice = activeInputDevice;
        }
    }

    /// <summary>
    /// Get the current active input device type.
    /// </summary>
    /// <returns>The device type that is active.</returns>
    public InputDeviceType GetCurrentInputDevice()
    {
        return activeInputDevice;
    }


}
