using System;
using System.Collections;
using UnityEngine;

public class Reset : SequenceBase
{
    public override event Action OnThisSequenceEnd;

    public override float GetProgress()
    {
        return 1;
    }

    public override void StartSequence()
    {
        StartCoroutine(WaitAndResetAndStartAgain());
    }

    IEnumerator WaitAndResetAndStartAgain()
    {
        yield return new WaitForSeconds(1f);
        OnThisSequenceEnd?.Invoke();

        GetComponent<LevelGenerator>()?.DestroyAllRooms();
        GetComponent<Sequencer>()?.StartTheSequence();
    }
}
