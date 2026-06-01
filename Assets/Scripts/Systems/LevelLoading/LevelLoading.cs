using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// This is how levels are loaded with the splash screen.
/// You just need to call the <see cref="LoadScene(string)"/> func to load the scene you want.
/// </summary>
public class LevelLoading : MonoBehaviour
{
	/// <summary>
	/// Singleton for the <see cref="LevelLoading"/>.
	/// </summary>
	public static LevelLoading Instance;

	// The loading screen UI.

	// TODO: scripts should hook into this, we should only manage loading a level.

	/// <summary>
	/// The object to show when hiding the level loading.
	/// </summary>
	public GameObject LoadingScreen;

	/// <summary>
	/// The progress bar for the loading screen.
	/// </summary>
	public Slider ProgressBar;

	/// <summary>
	/// The main menu scene name to load after boot strap.
	/// </summary>
	[SerializeField]
	private string MainMenuSceneName = "MainMenu";


	/// <summary>
	/// Check to stop reloading the same scene  multiple times.
	/// </summary>
	private bool isReloading = false;

	/// <summary>
	/// Are we waiting for things to load.
	/// </summary>
	public bool IsLoading = false;



	/// <summary>
	/// Debug to prevent level loading from loading to test loading screens and other things.
	/// </summary>
	public bool OverrideAll = false;


	/// <summary>
	/// Is something overriding the loading bar.
	/// </summary>
	private bool isOverridingLoadingBar = false;

	/// <summary>
	/// The override value for the loading bar if <see cref="isOverridingLoadingBar"/> is TRUE.
	/// </summary>
	private float loadingBarOverrideValue = 0;


	/// <summary>
	/// Are we waiting for something to release loading.
	/// </summary>
	private bool isHoldingLoading = false;

	/// <summary>
	/// Has the core of the level been loaded. (Just the scenes)
	/// </summary>
	private bool isCoreLoaded = false;

	// progress of loading the scene.
	/// <summary>
	/// The total progress of loading all the scenes requested.
	/// </summary>
	private float totalSceneProgress;

	// This is used to keep track of levels being loaded.
	/// <summary>
	/// List of all tracked level async loading. Used to track progress.
	/// </summary>
	List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

	#region Awake
	// sets the instance
	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			Instance = this;
			// prevents this of being destroyed on load.
			DontDestroyOnLoad(this.gameObject);
		}
	}
	#endregion



	#region Start
	// sets variables.
	private void Start()
	{
		if (OverrideAll) return;

		LoadingScreen.SetActive(false);

		LoadMainMenu();

		if (isOverridingLoadingBar)
		{
			ProgressBar.value = loadingBarOverrideValue;
		}

		// SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

	}
	#endregion



	#region Update
	private void Update()
	{
		if (OverrideAll) return;

		// stops reloading of reloading multiple times.
		isReloading = LoadingScreen.gameObject.activeSelf;

		if (isOverridingLoadingBar)
		{
			ProgressBar.value = loadingBarOverrideValue;
		}
	}
	#endregion



	#region LoadMainMenu
	/// <summary>
	/// A function to load scene with index of 1.
	/// </summary>
	public void LoadMainMenu()
	{
		LoadScene(new string[] { MainMenuSceneName });
	}
	#endregion


	#region LoadScene int
	/// <summary>
	/// Loads the scene with the given index async.
	/// </summary>
	/// <param name="indexNumber">build scene index</param>
	public void LoadScene(int indexNumber)
	{
		if (OverrideAll) return;

		isCoreLoaded = false;

		isHoldingLoading = true;

		IsLoading = true;
		LoadingScreen.gameObject.SetActive(true);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));

		if (SceneManager.sceneCount > 1)
		{
			for (int i = 1; i < SceneManager.sceneCount; i++)
				scenesLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)));
		}


		scenesLoading.Add(SceneManager.LoadSceneAsync(indexNumber, LoadSceneMode.Additive));


		StartCoroutine(GetSceneLoadProgress());
	}
	#endregion


	#region LoadScene string
	/// <summary>
	/// Loads the scene with the given index async.
	/// </summary>
	/// <param name="sceneName">build scene index</param>
	public void LoadScene(string sceneName)
	{
		if (OverrideAll) return;
		SetIsOverridingLoadingBar();
		isHoldingLoading = true;


		IsLoading = true;
		LoadingScreen.gameObject.SetActive(true);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));

		if (SceneManager.sceneCount > 1)
		{
			for (int i = 1; i < SceneManager.sceneCount; i++)
				scenesLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)));
		}

		scenesLoading.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));

		StartCoroutine(GetSceneLoadProgress());
	}
	#endregion


	#region LoadScene string[]
	/// <summary>
	/// Loads the scenes with the give names async.
	/// </summary>
	/// <param name="mapName">build scene name</param>
	public void LoadScene(string[] mapNames)
	{
		if (OverrideAll) return;
		SetIsOverridingLoadingBar();
		isHoldingLoading = true;


		IsLoading = true;
		LoadingScreen.gameObject.SetActive(true);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));

		if (SceneManager.sceneCount > 1)
		{
			for (int i = 1; i < SceneManager.sceneCount; i++)
				scenesLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)));
		}


		foreach (string map in mapNames)
		{
			scenesLoading.Add(SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive));

		}

		StartCoroutine(GetSceneLoadProgress());
	}
	#endregion



	#region Reload
	/// <summary>
	/// Used to reload the current scene loaded.
	/// </summary>
	public void Reload()
	{
		if (OverrideAll) return;
		isCoreLoaded = false;

		SetIsOverridingLoadingBar();
		isHoldingLoading = true;

		IsLoading = true;
		if (isReloading) return;
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));

		LoadingScreen.gameObject.SetActive(true);

		Scene savedScene = SceneManager.GetSceneAt(1);

		// unload the scenes
		if (SceneManager.sceneCount > 1)
		{
			for (int i = 1; i < SceneManager.sceneCount; i++)
				scenesLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i)));
		}



		// reload the scenes
		if (LevelCollection.CheckSceneInCollection(savedScene.name))
		{
			foreach (string map in LevelCollection.GetCollectionNameFromScene(savedScene.name))
			{
				scenesLoading.Add(SceneManager.LoadSceneAsync(map, LoadSceneMode.Additive));
			}
		}
		else
		{
			scenesLoading.Add(SceneManager.LoadSceneAsync(savedScene.buildIndex, LoadSceneMode.Additive));
		}


		StartCoroutine(GetSceneLoadProgress());
	}
	#endregion



	#region GetSceneLoadProgress
	/// <summary>
	/// Used to keep track of loading.
	/// </summary>
	/// <returns></returns>
	public IEnumerator GetSceneLoadProgress()
	{
		for (int i = 0; i < scenesLoading.Count; i++)
		{
			while (!scenesLoading[i].isDone)
			{
				totalSceneProgress = 0;

				//TODO: look at this https://docs.unity3d.com/ScriptReference/AsyncOperation-progress.html
				// Unity specified using allowSceneActivation to false to use loading bars.

				foreach (AsyncOperation operation in scenesLoading)
				{
					totalSceneProgress += operation.progress;
				}

				totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;

				ProgressBar.value = totalSceneProgress;

				yield return null;
			}
		}

		if (SceneManager.sceneCount > 1)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
		}
		else
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		}

		isCoreLoaded = true;

		// we should have a hold until release command here instead. Have a script to tell this its free to unlock.

		while (isHoldingLoading)
		{

			yield return null;
		}


		IsLoading = false;
		LoadingScreen.gameObject.SetActive(false);


	}
	#endregion

	/// <summary>
	/// Set if the loading bar is being overridden.
	/// </summary>
	/// <param name="isOverriding">True will mark as being overridden.</param>
	public void SetIsOverridingLoadingBar(bool isOverriding = false)
	{
		isOverridingLoadingBar = isOverriding;

		if (!isOverriding) SetLoadingBarValue();
	}

	/// <summary>
	/// Set the loading bar value if it was overridden.
	/// </summary>
	/// <param name="value">The value to set it to. (0-1)</param>
	public void SetLoadingBarValue(float value = 0f)
	{
		loadingBarOverrideValue = value;
	}

	/// <summary>
	/// Releases the loading screen so the player can play.
	/// </summary>
	public void ReleaseLevelLoading()
	{
		isHoldingLoading = false;
		Debug.Log("Released level loading");
		SetIsOverridingLoadingBar();
	}

	/// <summary>
	/// Get if all the requested scenes have been loaded.
	/// </summary>
	/// <returns>True if all the requested scenes were loaded.</returns>
	public bool IsCoreLoaded()
	{
		return isCoreLoaded;
	}
}
