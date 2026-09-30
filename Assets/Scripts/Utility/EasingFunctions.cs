using UnityEngine;


/// <summary>
/// All easing functions in this project. Unless hardcoded, then replace with these.
/// <br></br>Most taken from https://easings.net/#
/// </summary>

// Static prevents instanciation of this class.
public static class EasingFunctions
{
    /// <summary>
    /// A easing delegate that converts a 0 - 1 time and returns an easing value.
    /// </summary>
    /// <param name="t">The current time of the easing function.</param>
    /// <returns>The easing function output.</returns>
    public delegate float EasingDelegate(float t);

    public static float EaseOutBack(float t)
    {
        Mathf.Clamp01(t);

        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    public static float EaseInOutCubic(float t)
    {
        Mathf.Clamp01(t);

        return t < 0.5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }

    public static float EaseOutQuint(float t)
    {
        Mathf.Clamp01(t);

        return 1 - Mathf.Pow(1 - t, 5);
    }

    public static float EaseInExpo(float t)
    {
        Mathf.Clamp01(t);

        return t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
    }
}
