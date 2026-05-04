using UnityEngine;

public class WinGame : MonoBehaviour
{
    public void WinDaGame()
    {
        GameManager.Instance.statsHolder.outcome = true;
    }
}
