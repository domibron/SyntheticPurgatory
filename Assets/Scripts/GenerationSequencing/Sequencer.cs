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

            LevelLoading.Instance?.SetIsOverriding(true);
            LevelLoading.Instance?.SetLoadingBarValue(GetOverallProgress());

            waitingForASequence = true;

            sequences[currentSequence].OnThisSequenceEnd += SequenceEnd;

            sequences[currentSequence].StartSequence();

            while (waitingForASequence)
            {
                LevelLoading.Instance?.SetIsOverriding(true);
                LevelLoading.Instance?.SetLoadingBarValue(GetOverallProgress());

                yield return null;
            }

            // currentSequence++;
        }

        currentSequence++;

        OnSequencesEnd?.Invoke();

        LevelLoading.Instance?.ReleaseLevelLoading();

        yield return null;
    }

    private float GetOverallProgress()
    {
        float totalProgress = 0;

        foreach (SequenceBase sequence in sequences)
        {
            totalProgress += sequence.GetProgress();
        }

        print(totalProgress / sequences.Count);
        return totalProgress / sequences.Count;
    }

    private void SequenceEnd()
    {
        sequences[currentSequence].OnThisSequenceEnd -= SequenceEnd;
        waitingForASequence = false;
    }
}
