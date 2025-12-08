using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sequence
{
    public bool IsEnabled = true;
    public SequenceBase SequenceBase = null;

    public Sequence()
    {
        IsEnabled = true;
        SequenceBase = null;
    }
}

public class Sequencer : MonoBehaviour
{
    [SerializeField]
    private Sequence[] sequences;

    private bool waitingForASequence = false;
    private int currentSequence = 0;

    public event Action OnSequencesEnd;

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        for (int i = 0; i < sequences.Length; i++)
        {
            if (currentSequence != i) currentSequence = i;

            if (!sequences[i].IsEnabled) continue;

            LevelLoading.Instance?.SetIsOverriding(true);
            LevelLoading.Instance?.SetLoadingBarValue(GetOverallProgress());

            waitingForASequence = true;

            sequences[currentSequence].SequenceBase.OnThisSequenceEnd += SequenceEnd;

            sequences[currentSequence].SequenceBase.StartSequence();

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

        foreach (Sequence sequence in sequences)
        {
            totalProgress += sequence.SequenceBase.GetProgress();
        }

        print(totalProgress / sequences.Length);
        return totalProgress / sequences.Length;
    }

    private void SequenceEnd()
    {
        sequences[currentSequence].SequenceBase.OnThisSequenceEnd -= SequenceEnd;
        waitingForASequence = false;
    }
}
