using System.Collections;
using UnityEngine;

public class HideTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (PlayerRefFetcher.Instance == null) yield return new WaitForEndOfFrame();

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("Cannot hide the timer because the game manager does not exist.");
            yield break;
        }

        while (!RunManager.Instance.IsTimerHidden())
            RunManager.Instance.HideTimer();
    }
}
