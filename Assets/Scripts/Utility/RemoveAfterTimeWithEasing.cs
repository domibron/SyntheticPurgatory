using UnityEngine;


/// <summary>
/// Removes the object after a period of time but also scales the object in and out using easing.
/// </summary>
public class RemoveAfterTimeWithEasing : MonoBehaviour
{
    public float timeToWaitBeforeRemoving = 5f;

    public bool shouldEaseIn = true;

    public float easeInTime = 1f;
    public float easeOutTime = 1f;

    private float easingTime = 0f;

    private float localTime = 0f;

    private Vector3 targetScale;
    private Vector3 storedScale;

    void Awake()
    {
        targetScale = transform.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToWaitBeforeRemoving);
        storedScale = transform.localScale;

        if (shouldEaseIn) { transform.localScale = Vector3.zero; }
    }

    void Update()
    {
        localTime += Time.deltaTime;

        if (localTime <= easeInTime && shouldEaseIn)
        {
            easingTime += Time.deltaTime;

            if (shouldEaseIn)
            {
                storedScale = Vector3.Lerp(Vector3.zero, targetScale, EasingFunctions.EaseOutQuint(easingTime / easeInTime));
                transform.localScale = storedScale;
            }

        }
        else if (timeToWaitBeforeRemoving - localTime >= 0 && timeToWaitBeforeRemoving - localTime <= easeOutTime + 0.1f)
        {
            easingTime -= Time.deltaTime;

            storedScale = Vector3.Lerp(Vector3.zero, targetScale, EasingFunctions.EaseOutQuint(easingTime / easeOutTime));
            transform.localScale = storedScale;
        }

    }

    public Vector3 GetStoredScale()
    {
        return storedScale;
    }
}
