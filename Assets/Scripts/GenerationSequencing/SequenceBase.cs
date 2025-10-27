using System;
using UnityEngine;

public abstract class SequenceBase : MonoBehaviour
{
    public abstract event Action OnThisSequenceEnd;

    public abstract void StartSequence();
}
