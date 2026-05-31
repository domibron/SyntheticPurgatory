using System;
using UnityEngine;

/// <summary>
/// Base classed used in the sequencer. This is to initiate, keep track of and know when complete.
/// </summary>
public abstract class SequenceBase : MonoBehaviour
{
    /// <summary>
    /// Event called once the operation was completed.
    /// </summary>
    public abstract event Action OnThisSequenceEnd;

    /// <summary>
    /// Stat the operation.
    /// </summary>
    public abstract void StartSequence();

    /// <summary>
    /// Get the current progress ideally as a 0 - 1 decimal percentage.
    /// </summary>
    /// <returns>The current progress of this operation.</returns>
    public abstract float GetProgress();
}
