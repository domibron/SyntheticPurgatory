using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Stores a sequence that can be enabled or disabled.
/// </summary>
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

/// <summary>
/// The controller for all the sequencers linked to this, will automatically start.
/// </summary>
public class Sequencer : MonoBehaviour
{

    /// <summary>
    /// All the sequences that will be ran in order.
    /// </summary>
    [SerializeField]
    private Sequence[] sequences;

    /// <summary>
    /// Are we currently waiting for a sequence to finish.
    /// </summary>
    private bool waitingForASequence = false;

    /// <summary>
    /// The current sequence index we are on.
    /// </summary>
    private int currentSequence = 0;

    /// <summary>
    /// Event for when all sequences have been completed.
    /// </summary>
    public event Action OnSequencesEnd;

    private Coroutine main;
    private Coroutine timeTracking;
    private bool keepTime = false;

    IEnumerator Start()
    {
        // Loading is missing. Just run the sequence and exit.
        if (LevelLoading.Instance == null)
        {
            StartCoroutine(StartSequence());
            yield break; // exit.
        }

        // Wait until the level is fully loaded.
        while (!LevelLoading.Instance.IsCoreLoaded())
        {
            yield return null;
        }

        StartTheSequence();
    }

    public void StartTheSequence()
    {
        main = StartCoroutine(StartSequence());
    }

    /// <summary>
    /// Sequence runner.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartSequence()
    {
        timeTracking = StartCoroutine(TrackTime());

        for (int i = 0; i < sequences.Length; i++)
        {
            if (currentSequence != i) currentSequence = i;

            if (!sequences[i].IsEnabled) continue;

            LevelLoading.Instance?.SetIsOverridingLoadingBar(true);
            LevelLoading.Instance?.SetLoadingBarValue(GetOverallProgress());

            waitingForASequence = true;

            sequences[currentSequence].SequenceBase.OnThisSequenceEnd += SequenceEnd;

            sequences[currentSequence].SequenceBase.StartSequence();

            while (waitingForASequence)
            {
                LevelLoading.Instance?.SetIsOverridingLoadingBar(true);
                LevelLoading.Instance?.SetLoadingBarValue(GetOverallProgress());

                yield return null;
            }

            // currentSequence++;
        }

        currentSequence++;

        OnSequencesEnd?.Invoke();

        LevelLoading.Instance?.ReleaseLevelLoading();

        keepTime = false;

        yield return null;
    }

    /// <summary>
    /// Get the progress for the sequencer.
    /// </summary>
    /// <returns></returns>
    private float GetOverallProgress()
    {
        float totalProgress = 0;
        int length = sequences.Length;


        foreach (Sequence sequence in sequences)
        {
            if (!sequence.IsEnabled) // skip disabled sequences.
            {
                length--;
                continue;
            }

            totalProgress += sequence.SequenceBase.GetProgress();
        }

        //print(totalProgress / sequences.Length);
        return totalProgress / length;
    }

    /// <summary>
    /// Once a sequence ends unsubscribe to their event and mark <see cref="waitingForASequence"/> as false.
    /// </summary>
    private void SequenceEnd()
    {
        sequences[currentSequence].SequenceBase.OnThisSequenceEnd -= SequenceEnd;
        waitingForASequence = false;
    }

    private IEnumerator TrackTime()
    {
        print("Time started: " + DateTime.Now.ToLongTimeString());
        DateTime startTime = DateTime.Now;
        TimeSpan timeSpan = DateTime.Now.Subtract(startTime);

        keepTime = true;


        while (keepTime)
        {
            timeSpan = DateTime.Now.Subtract(startTime);


            print($"Elapsed: h{timeSpan.Hours}:m{timeSpan.Minutes}:s{timeSpan.Seconds}:ms{timeSpan.Milliseconds}");

            yield return null;
        }

        timeSpan = DateTime.Now.Subtract(startTime);


        print($"Ended with: h{timeSpan.TotalHours}:m{timeSpan.Minutes}:s{timeSpan.Seconds}:ms{timeSpan.Milliseconds}");
    }

    public void RESETSEQ()
    {
        StopCoroutine(timeTracking);
        keepTime = false;
        StopCoroutine(main);
    }
}
