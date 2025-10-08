using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

// Im scared of this script.

/// <summary>
/// This is how levels are loaded with the splash screen.
/// You just need to call the loadScene func to load the scene you want.
/// </summary>
public class LevelLoading : MonoBehaviour
{
	// Instance so other scripts can call functions.
	public static LevelLoading Instance;

	// The loading screen UI.
	public GameObject LoadingScreen;
	public Slider ProgressBar;

	[SerializeField]
	private string MainMenuSceneName = "MainMenu";

	// used to stop loading the level multiple times when reloading is called more than once when loading.
	private bool isReloading = false;

	// if the level is being loaded.
	public bool IsLoading = false;

	// this prevents loading when enabled.
	public bool OverrideAll = false;

	// progress of loading the scene.
	private float totalSceneProgress;

	// This is used to keep track of levels being loaded.
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

		// SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

	}
	#endregion



	#region Update
	private void Update()
	{
		if (OverrideAll) return;

		// stops reloading of reloading multiple times.
		isReloading = LoadingScreen.gameObject.activeSelf;
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


	[Obsolete("This was for another project.", true)]
	public void LoadLevel(int levelID)
	{
		if (levelID == 1)
		{
			LoadScene(LevelCollections.Level1);
		}
		else if (levelID == 2)
		{
			LoadScene(LevelCollections.Level2);
		}
		else
		{
			Debug.LogError($"Failed to find level with id of {levelID}", this);
		}
	}


	#region LoadScene int
	/// <summary>
	/// Loads the scene with the given index async.
	/// </summary>
	/// <param name="indexNumber">build scene index</param>
	public void LoadScene(int indexNumber)
	{
		if (OverrideAll) return;

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

				foreach (AsyncOperation operation in scenesLoading)
				{
					totalSceneProgress += operation.progress;
				}

				totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;

				ProgressBar.value = totalSceneProgress;

				yield return null;
			}
		}

		IsLoading = false;
		LoadingScreen.gameObject.SetActive(false);

		if (SceneManager.sceneCount > 1)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
		}
		else
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		}
	}
	#endregion
}
