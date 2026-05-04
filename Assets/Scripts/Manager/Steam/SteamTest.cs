using System;
using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

    void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnDestroy()
    {
        Debug.Log("IM BEING FUCKING KILLED");
    }

    private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t param, bool bIOFailure)
    {
        if (param.m_bSuccess != 1 || bIOFailure)
        {
            Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
        }
        else
        {
            Debug.Log("The number of players playing your game: " + param.m_cPlayers);
        }
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t param)
    {
        if (param.m_bActive != 0)
        {
            Debug.Log("Steam overlay has been activated");
        }
        else
        {
            Debug.Log("Steam overlay has been closed");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
        //     m_NumberOfCurrentPlayers.Set(handle);
        //     Debug.Log("Called GetNumberOfCurrentPlayers()");
        // }
    }
}
