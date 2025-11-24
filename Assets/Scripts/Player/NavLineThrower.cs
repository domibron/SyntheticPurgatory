using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavLineThrower : MonoBehaviour
{
    [SerializeField]
    private GameObject navLinePrefab;

    InputAction throwAction;

    [SerializeField]
    Transform throwSpawnPoint;

    [SerializeField]
    float throwCoolDown = 1f;
    float coolDown = 0;


    void Awake()
    {
        throwAction = InputSystem.actions.FindAction("Throw");

        throwAction.performed += OnThrowAction;
    }


    void Update()
    {
        if (coolDown > 0) coolDown -= Time.deltaTime;
    }

    private void OnThrowAction(InputAction.CallbackContext context)
    {
        if (coolDown > 0) return;

        coolDown = throwCoolDown;

        GameObject navLine = Instantiate(navLinePrefab, throwSpawnPoint.position, throwSpawnPoint.parent.rotation);
        navLine.GetComponent<Rigidbody>().AddForce((throwSpawnPoint.forward + throwSpawnPoint.up * 0.2f).normalized * 10f, ForceMode.VelocityChange);

    }
}
