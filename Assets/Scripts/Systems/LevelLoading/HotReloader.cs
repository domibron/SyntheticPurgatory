using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Allows developers working on levels to click play on their level and load the level loader.
/// <br/>DO NOT use this on scenes that are not on the build index!
/// </summary>
public class HotReloader : MonoBehaviour
{
    // Stop duplicate instances from taking over loading.

    /// <summary>
    /// Singleton for the <see cref="HotReloader"/>.
    /// </summary>
    public static HotReloader Instance { get; private set; }

    /// <summary>
    /// Are we performing a reload of a scene.
    /// </summary>
    private bool isReloading = false;

    /// <summary>
    /// The name of the scene to reload.
    /// </summary>
    private string sceneNameToReload;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check to see if the level loading is there (persistent script from boot strap).
        if (LevelLoading.Instance != null)
        {
            // Disable this since we don't need check again.
            this.enabled = false;
            return;
        }

        sceneNameToReload = SceneManager.GetActiveScene().name;

        isReloading = true;

        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReloading) return;


        if (LevelLoading.Instance == null) return;

        if (LevelLoading.Instance.IsLoading) return;


        if (SceneManager.GetActiveScene().buildIndex != 1) return;



        if (LevelCollection.CheckSceneInCollection(sceneNameToReload))
        {
            LevelLoading.Instance.LoadScene(LevelCollection.GetCollectionNameFromScene(sceneNameToReload));
        }
        else
        {
            LevelLoading.Instance.LoadScene(sceneNameToReload);
        }

        // To remind developers that they did not load into the game correctly and errors may be from a hot reload.
        // Fun fact, developers did not read the error messages and still reported "errors" in the console when they were from before a reload.

        Debug.Log("Keep in mind that there will be errors from when the scene was first loaded!", this);
        Debug.LogWarning("Keep in mind that there will be errors from when the scene was first loaded!", this);
        Debug.LogError("Keep in mind that there will be errors from when the scene was first loaded!", this);

        Destroy(this.gameObject);
    }
}
