using System;
using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// Handles getting, detecting and handling input for you.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput;

    private InputDeviceType activeInputDevice = InputDeviceType.Keyboard;

    /// <summary>
    /// New Old.
    /// </summary>
    public event Action<InputDeviceType, InputDeviceType> onDeviceChanged;

    private InputDeviceType previousDevice = InputDeviceType.Keyboard;

    public enum InputDeviceType
    {
        Keyboard,
        Gamepad,
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Multiple input managers detected, destroying this one.", gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    public InputDeviceType GetCurrentInputDevice()
    {
        return activeInputDevice;
    }


}
