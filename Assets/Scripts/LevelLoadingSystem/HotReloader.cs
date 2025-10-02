using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron


/// <summary>
/// Allows developers working on levels to click play on their level and load the level loader.
/// DO NOT use this on scenes that are not on the build index!
/// </summary>
public class HotReloader : MonoBehaviour
{
    public static HotReloader instance;

    private bool isReloading = false;

    private string sceneNameToReload;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (LevelLoading.instance != null) return;

        sceneNameToReload = SceneManager.GetActiveScene().name;

        isReloading = true;

        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReloading) return;


        if (LevelLoading.instance == null) return;

        if (LevelLoading.instance.loading) return;


        if (SceneManager.GetActiveScene().buildIndex != 1) return;



        if (LevelCollections.CheckSceneInCollection(sceneNameToReload))
        {
            LevelLoading.instance.LoadLevel(LevelCollections.GetLevelIDFromSceneName(sceneNameToReload));
        }
        else
        {
            LevelLoading.instance.LoadScene(sceneNameToReload);
        }

        Debug.Log("Keep in mind that there will be errors from when the scene was first loaded!", this);

        Destroy(this.gameObject);
    }
}
