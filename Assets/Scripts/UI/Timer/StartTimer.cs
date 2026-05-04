using System.Collections;
using UnityEngine;

public class StartTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (PlayerRefFetcher.Instance == null) yield return new WaitForEndOfFrame();

        if (GameManager.Instance != null) GameManager.Instance.StartTimer(); // TODO: move to level generator.

    }
}
