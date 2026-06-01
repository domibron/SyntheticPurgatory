using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Throws nav lines.
/// </summary>
public class NavLineThrower : MonoBehaviour
{
    /// <summary>
    /// The navigation line prefab.
    /// </summary>
    [SerializeField]
    private GameObject navLinePrefab;

    /// <summary>
    /// The throw action to cache.
    /// </summary>
    InputAction throwAction;

    /// <summary>
    /// The spawn point of the navigation line when spawning.
    /// </summary>
    [SerializeField]
    Transform throwSpawnPoint;

    /// <summary>
    /// The cool down before spawning another.
    /// </summary>
    [SerializeField]
    float throwCoolDown = 1f;

    /// <summary>
    /// The current cool down in effect.
    /// </summary>
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

    /// <summary>
    /// Spawn and throw a navigation line.
    /// </summary>
    /// <param name="context">The input context.</param>
    private void OnThrowAction(InputAction.CallbackContext context)
    {
        if (coolDown > 0) return;

        coolDown = throwCoolDown;

        GameObject navLine = Instantiate(navLinePrefab, throwSpawnPoint.position, throwSpawnPoint.parent.rotation);
        navLine.GetComponent<Rigidbody>().AddForce((throwSpawnPoint.forward + throwSpawnPoint.up * 0.2f).normalized * 10f, ForceMode.VelocityChange);

    }
}
