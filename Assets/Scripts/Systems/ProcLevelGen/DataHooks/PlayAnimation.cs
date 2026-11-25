using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PlayAnimation : MonoBehaviour
{
    GameObject depot;

    GameObject player;

    void Start()
    {
        GetComponent<Sequencer>().OnSequencesEnd += () =>
        {
            StartCoroutine(SetUp());
        };
    }

    IEnumerator SetUp()
    {
        depot = GameObject.FindGameObjectWithTag("Depot");

        player = PlayerRefFetcher.Instance.gameObject;


        if (PlayerRefFetcher.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        PauseCanvas.SetPauseState(PauseCanvas.PauseState.OverridePaused);

        player.SetActive(false);

        LevelLoading.Instance?.ReleaseLevelLoading();

        yield return new WaitForEndOfFrame();

        if (depot == null)
        {
            Debug.LogError("Cannot find the depot");
            OnComplete();
            yield break;
        }

        depot.GetComponent<SequenceController>().OnSequenceEnd.AddListener(OnComplete);

        depot.GetComponent<PlayableDirector>().Play();
    }

    void OnComplete()
    {
        player?.SetActive(true);

        print("Player activated");

        PauseCanvas.SetPauseState(PauseCanvas.PauseState.RestoreFromOverride);
    }
}
