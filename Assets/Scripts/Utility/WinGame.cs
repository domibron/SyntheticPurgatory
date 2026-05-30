using UnityEngine;

public class WinGame : MonoBehaviour
{
    public void WinDaGame()
    {
        RunManager.Instance.statsHolder.outcome = true;
    }
}
