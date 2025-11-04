using UnityEngine;

public class OnBossDeath : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BossDied()
    {
        LevelLoading.Instance.LoadMainMenu();
    }
}
