using System.Collections;
using UnityEngine;

public class HideTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (PlayerRefFetcher.Instance == null) yield return new WaitForEndOfFrame();

        GameManager.Instance?.HideTimer();
    }
}
