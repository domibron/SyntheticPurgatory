using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

public class DebugConsole : MonoBehaviour
{
	public static DebugConsole instance;

	public GameObject consoleWindow;

	public GameObject content;

	public GameObject textPrefab;

	public TMP_InputField inputField;

	public Scrollbar vertScrollBar;

	bool consoleOpen = false;

	public List<object> commands = new List<object>();


	BaseCommands baseCommands;


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

		// InitCommands();
		baseCommands = new BaseCommands(instance);
	}

	// Start is called before the first frame update
	void Start()
	{
		consoleWindow.SetActive(consoleOpen);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote) || (consoleOpen && Input.GetKeyDown(KeyCode.Escape)))
		{
			consoleOpen = !consoleOpen;

			// if (UIManager.Instance.inGameMenu || UIManager.Instance.inDialogueMenu) consoleOpen = false;

			consoleWindow.SetActive(consoleOpen);

			//Time.timeScale = consoleOpen ? 0 : 1;
			// UIManager.Instance.inGameMenu = consoleOpen; // jank fix to get console to pause.


			if (consoleOpen)
			{
				inputField.ActivateInputField();
			}
		}


		if (consoleOpen)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				EndEdit();
			}
		}
	}

	public void TextToConsole(string text)
	{
		GameObject textObj = Instantiate(textPrefab, content.transform, false);

		textObj.GetComponent<TMP_Text>().text = text;

		vertScrollBar.value = 0;
	}

	public void EndEdit()
	{
		string command = inputField.text;

		if (string.IsNullOrEmpty(command))
		{
			TextToConsole("<color=red>Enter something valid!</color>");
			return;
		}

		int result = ParseCommand(command);

		if (result == 0) TextToConsole("Code 0");
		if (result == 1) TextToConsole("<color=red>Unknown Command!</color>");
		if (result == 2) TextToConsole("<color=red>INTERNAL ERROR</color>");

		inputField.text = "";

		inputField.ActivateInputField();

	}

	public int ParseCommand(string input)
	{
		string[] original = input.Split(' ');
		string command = original[0];
		string[] args = new string[original.Length];
		for (int i = 1; i < original.Length; i++)
		{
			args[i - 1] = original[i];
		}

		for (int i = 0; i < commands.Count; i++)
		{
			CommandBase commandBase = commands[i] as CommandBase;

			if (command.Contains(commandBase.Command, StringComparison.InvariantCultureIgnoreCase))
			{
				if (commands[i] as Command != null)
				{
					(commands[i] as Command).Invoke();
					return 0;
				}

				if (args.Length <= 0 || args[0] == null) return 1;

				else if (commands[i] as Command<string> != null)
				{
					(commands[i] as Command<string>).Invoke(args[0]);
					return 0;
				}

				else if (commands[i] as Command<string[]> != null)
				{
					(commands[i] as Command<string[]>).Invoke(args);
					return 0;
				}

				else if (commands[i] as Command<string, string> != null)
				{
					try
					{
						(commands[i] as Command<string, string>).Invoke(args[0], args[1]);
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<string, string, string> != null)
				{
					try
					{
						(commands[i] as Command<string, string, string>).Invoke(args[0], args[1], args[2]);
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<float> != null)
				{
					(commands[i] as Command<float>).Invoke(float.Parse(args[0]));
					return 0;
				}

				else if (commands[i] as Command<float, float> != null)
				{
					try
					{
						(commands[i] as Command<float, float>).Invoke(float.Parse(args[0]), float.Parse(args[1]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<float, float, float> != null)
				{
					try
					{
						(commands[i] as Command<float, float, float>).Invoke(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<bool> != null)
				{
					(commands[i] as Command<bool>).Invoke(bool.Parse(args[0]));
					return 0;
				}

				else if (commands[i] as Command<bool, bool> != null)
				{
					try
					{
						(commands[i] as Command<bool, bool>).Invoke(bool.Parse(args[0]), bool.Parse(args[1]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<bool, bool, bool> != null)
				{
					try
					{
						(commands[i] as Command<bool, bool, bool>).Invoke(bool.Parse(args[0]), bool.Parse(args[1]), bool.Parse(args[2]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<int> != null)
				{
					try
					{
						(commands[i] as Command<int>).Invoke(int.Parse(args[0]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<int, int> != null)
				{
					try
					{
						(commands[i] as Command<int, int>).Invoke(int.Parse(args[0]), int.Parse(args[1]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}

				else if (commands[i] as Command<int, int, int> != null)
				{
					try
					{
						(commands[i] as Command<int, int, int>).Invoke(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
						return 0;
					}
					catch (Exception e)
					{
						print(e.Message);
						return 2;
					}
				}
			}
		}

		return 1;
	}

	public void DestroyUnityObject(UnityEngine.Object objectToDestroy)
	{
		Destroy(objectToDestroy);
	}
}
