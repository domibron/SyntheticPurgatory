using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    [SerializeField]
    private List<SequenceBase> sequences = new List<SequenceBase>();

    private bool waitingForASequence = false;
    private int currentSequence = 0;

    public event Action OnSequencesEnd;

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        for (int i = 0; i < sequences.Count; i++)
        {
            if (currentSequence != i) currentSequence = i;

            waitingForASequence = true;

            sequences[currentSequence].OnThisSequenceEnd += SequenceEnd;

            sequences[currentSequence].StartSequence();

            while (waitingForASequence) yield return null;

            // currentSequence++;
        }

        currentSequence++;

        OnSequencesEnd?.Invoke();

        yield return null;
    }

    private void SequenceEnd()
    {
        sequences[currentSequence].OnThisSequenceEnd -= SequenceEnd;
        waitingForASequence = false;
    }
}
