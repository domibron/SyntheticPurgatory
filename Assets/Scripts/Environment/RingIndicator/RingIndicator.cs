using UnityEngine;

/// <summary>
/// Expanding ring script for visual indicators for the player.
/// </summary>
public class RingIndicator : MonoBehaviour
{
    /// <summary>
    /// The object to scale.
    /// </summary>
    public GameObject damageRingGameObject;

    /// <summary>
    /// How long for the ring to grow for.
    /// </summary>
    private float ringGrowTime = 0;

    /// <summary>
    /// Current tracked time for the scaling of the ring.
    /// </summary>
    private float currentRingGrowTime = 0;

    /// <summary>
    /// The max size of the ring.
    /// </summary>
    private float endRingDiameter = 1;

    /// <summary>
    /// The min size of the ring. It will start with this value.
    /// </summary>
    private float startRingDiameter = 0;

    /// <summary>
    /// How long for the rink to shrink for.
    /// </summary>
    private float shrinkTime = 1f;

    /// <summary>
    /// The current tracked time for shrinking.
    /// </summary>
    private float currentShrinkTime = 1f;

    /// <summary>
    /// 1 is 1 scale unit per second.
    /// </summary>
    private float shrinkSpeed = 1f;

    /// <summary>
    /// Should be ring be visible.
    /// </summary>
    private bool showRing = false;

    void Update()
    {
        if (currentRingGrowTime < ringGrowTime)
            currentRingGrowTime += Time.deltaTime;

        if (currentShrinkTime > 0)
            currentShrinkTime -= Time.deltaTime * (1 / shrinkSpeed);

        damageRingGameObject.SetActive(showRing || (currentShrinkTime / shrinkTime) > 0);

        // Is... is this a todo or... like a explanation?
        // stop divide by zero error.
        if (showRing)
        {
            damageRingGameObject.transform.localScale = Vector3.LerpUnclamped(Vector3.one * startRingDiameter, Vector3.one * endRingDiameter, EasingFunctions.EaseOutBack(currentRingGrowTime / ringGrowTime));
        }
        else if ((currentShrinkTime / shrinkTime) > 0)
        {
            damageRingGameObject.transform.localScale = Vector3.one * endRingDiameter * EasingFunctions.EaseInOutCubic(currentShrinkTime / shrinkTime);
        }


    }


    /// <summary>
    /// Make the ring visible and scale it to the desired size.
    /// </summary>
    /// <param name="chargeTime">How long for grow effect to last.</param>
    /// <param name="endRadius">The size for the ring.</param>
    /// <param name="startRadius">The starting size of the ring.</param>
    public void ShowRing(float chargeTime, float endRadius, float startRadius = 0)
    {
        ringGrowTime = chargeTime;
        currentRingGrowTime = 0;

        endRingDiameter = endRadius * 2f;
        startRingDiameter = startRadius * 2f;

        showRing = true;
    }

    // this should also be in time. ~ wha?
    /// <summary>
    /// Shrink the ring and hide it once shrunk.
    /// </summary>
    /// <param name="shrinkSpeed">How fast to shrink the ring.</param>
    public void HideRing(float shrinkSpeed = 2f)
    {
        this.shrinkSpeed = shrinkSpeed;

        if (showRing)
            currentShrinkTime = shrinkTime;

        showRing = false;
        //ringDiameter = 1f;
    }
}
