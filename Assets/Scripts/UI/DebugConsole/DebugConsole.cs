using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;

[Serializable]
public class ConsoleSettings
{
	public float ConsoleHeight;

	public bool OnlyWatchForErrors;

	public bool Echo;

	public int MaxLogCount;

	public int MaxLogCommandHistory;

	public static ConsoleSettings defaultSettings => new ConsoleSettings
	{
		ConsoleHeight = 500f,
		OnlyWatchForErrors = true,
		Echo = true,
		MaxLogCount = 100,
		MaxLogCommandHistory = 100,
	};
}

public class DebugConsole : MonoBehaviour
{
	public static DebugConsole Instance { get; private set; }

	[SerializeField]
	private GameObject m_consoleRootObject;

	[SerializeField]
	private RectTransform m_consoleRect;

	[SerializeField]
	private RectTransform m_logWindowRect;

	[SerializeField]
	private TMP_Text m_logWindow;

	[SerializeField]
	private TMP_InputField m_inputField;

	[SerializeField]
	private TMP_Text m_descriptionText;

	[SerializeField]
	private Scrollbar m_vertScrollBar;

	[SerializeField]
	bool m_isConsoleActive = false;

	ConsoleSettings m_consoleSettings;

	public static List<Command> Commands = new List<Command>();

	const int k_maxConsoleMessages = 100;
	const int k_maxExecutedCommands = 100;

	const float k_inputFieldHeight = 40f;

	private Queue<string> m_logs = new Queue<string>(k_maxConsoleMessages);

	private Stack<string> m_executedCommands = new Stack<string>(k_maxExecutedCommands);

	public static string ConsoleConfigPath => Application.persistentDataPath + "\\ConsoleSettings.json";

	private int m_selectedCommandInHistory = 0;

	void OnDisable()
	{
		Application.logMessageReceived -= ApplicationLogCallback;
	}

	void OnEnable()
	{
		Application.logMessageReceived += ApplicationLogCallback;
	}


	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		ConsoleSettings consoleSettings = ReadConsoleSettings();
		if (consoleSettings != null)
		{
			m_consoleSettings = consoleSettings;
		}
		else
		{
			m_consoleSettings = ConsoleSettings.defaultSettings;
		}

		ApplySettings();
	}

	// Start is called before the first frame update
	void Start()
	{
		m_isConsoleActive = false;
		ClearInputField();

		if (Commands.Count <= 0)
		{
			RegisterAllCommands();
		}


		HandleConsoleActiveState();
	}

	// Update is called once per frame
	void Update()
	{
		HandleConsoleActiveState();


		if (m_isConsoleActive)
		{
			string[] argsNoEmpty = m_inputField.text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			string[] argsAll = m_inputField.text.Split(' ');

			HandleDescriptionText(argsAll);

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				TryFinishCommandPart(argsAll);
			}
			if (Input.GetKeyDown(KeyCode.Return))
			{
				TryExecuteCommand(argsNoEmpty);
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				GoToCommandHistory(1);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				GoToCommandHistory(-1);
			}

			m_vertScrollBar.value += Input.GetAxisRaw("Mouse ScrollWheel");
		}
	}

	private void SelectInputField()
	{
		m_inputField.ActivateInputField();

		m_inputField.Select();

		if (EventSystem.current != null)
			EventSystem.current.SetSelectedGameObject(m_inputField.gameObject);
	}

	public void ToggleActiveState()
	{
		m_isConsoleActive = !m_isConsoleActive;

		if (m_isConsoleActive)
		{
			UpdateLogWindow();

			SelectInputField();


			PauseCanvas.SetPauseState(PauseCanvas.PauseState.DebugConsolePaused);
		}
		else
		{
			if (PauseCanvas.CurrentPauseState == PauseCanvas.PauseState.DebugConsolePaused)
			{
				PauseCanvas.SetPauseState(PauseCanvas.PauseState.RestoreFromDebugPause);
			}

			m_selectedCommandInHistory = -1;

			ClearInputField();
			m_inputField.DeactivateInputField();

			HandleDescriptionText(new string[0]);

			((RectTransform)m_descriptionText.transform.parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
		}

		// m_consoleRootObject.SetActive(m_isConsoleActive);
	}

	public void SetCaretToEnd()
	{
		m_inputField.caretPosition = m_inputField.text.Length;
	}

	private void GoToCommandHistory(int change)
	{
		if (m_executedCommands.Count <= -1) return;

		m_selectedCommandInHistory += change;

		m_selectedCommandInHistory = Mathf.Clamp(m_selectedCommandInHistory, -1, m_executedCommands.Count - 1);

		if (m_selectedCommandInHistory == -1)
		{
			m_inputField.text = "";
			SetCaretToEnd();
			return;
		}

		m_inputField.text = m_executedCommands.ElementAt(m_selectedCommandInHistory);

		SetCaretToEnd();
	}

	private void ApplySettings()
	{
		m_logWindowRect.sizeDelta = new Vector2(1f, m_consoleSettings.ConsoleHeight - k_inputFieldHeight);
	}

	private void WriteConsoleSettings()
	{
		File.WriteAllText(ConsoleConfigPath, JsonConvert.SerializeObject(m_consoleSettings, Formatting.Indented, new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Error
		}));
		LogToConsole("Saved console settings to disk.");
	}

	private ConsoleSettings ReadConsoleSettings()
	{
		if (File.Exists(ConsoleConfigPath))
		{
#nullable enable
			ConsoleSettings? consoleSettings = JsonConvert.DeserializeObject<ConsoleSettings>(File.ReadAllText(ConsoleConfigPath));
			return consoleSettings;
#nullable restore
		}
		return null;
	}


	private void HandleConsoleActiveState()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote) || (m_isConsoleActive && Input.GetKeyDown(KeyCode.Escape)))
		{
			ToggleActiveState();
		}

		bool flag = m_consoleRect.anchoredPosition.y < (m_consoleSettings.ConsoleHeight) - 1;
		if (m_consoleRootObject.activeSelf != flag)
		{
			m_consoleRootObject.SetActive(flag);

			if (flag)
			{
				SelectInputField();
			}
		}

		float slideTarget = (m_isConsoleActive ? 0f : (m_consoleSettings.ConsoleHeight) + 10);
		m_consoleRect.anchoredPosition = new Vector2(0, Mathf.Lerp(m_consoleRect.anchoredPosition.y, slideTarget, Time.unscaledDeltaTime * 8f));
	}

	private void ClearInputField()
	{
		m_inputField.text = "";
		HandleDescriptionText(new string[0]);
	}


	private void ApplicationLogCallback(string condition, string stackTrace, LogType type)
	{
		if (m_consoleSettings.OnlyWatchForErrors && type != LogType.Error) return;

		switch (type)
		{
			case LogType.Error:
				LogToConsole($"<color=red> {condition}; {stackTrace} </color>");
				break;
			case LogType.Exception:
				LogToConsole($"<color=red> {condition}; {stackTrace} </color>");
				break;
			case LogType.Assert:
				LogToConsole($"<color=red> {condition}; {stackTrace} </color>");
				break;
			case LogType.Warning:
				LogToConsole($"<color=yellow> {condition}; {stackTrace} </color>");
				break;
			case LogType.Log:
				LogToConsole($"{condition}");
				break;
		}
	}


	public void TryFinishCommandPart(string[] args)
	{
		if (args.Length <= 0 || string.IsNullOrEmpty(args[0]))
		{
			return;
		}

		if (args.Length == 1)
		{
			{
				Command command = SearchFirst(args[0]);
				if (command != null)
				{
					m_inputField.text = command.Name + " ";
				}
			}
		}
		else
		{
			Command command = SearchExact(args[0]);
			if (command != null && command.ArgAutofill != null && command.ArgAutofill.TryGetValue(args.Length - 2, out var value))
			{
				string text = SearchArgumentAutofillFirst(args[^1], value);
				if (!string.IsNullOrEmpty(text))
				{
					m_inputField.text = string.Join(" ", args.Take(args.Length - 1)) + " " + text;
				}
			}
			char c = m_inputField.text.Substring(m_inputField.text.Length - 1, 1)[0];
			if (c == '.' || c == ',')
			{
				m_inputField.text += "0";
			}
			m_inputField.text += " ";
		}
		SetCaretToEnd();
	}

	public void HandleDescriptionText(string[] args)
	{
		m_descriptionText.text = "";
		// Vector3 bottomLeft = m_inputField.textComponent.textInfo.characterInfo[m_inputField.caretPosition].bottomLeft;
		// Vector3 vector = m_inputField.transform.TransformPoint(bottomLeft);
		// m_descriptionText.transform.localPosition = new Vector3(vector.x, 0);
		m_descriptionText.transform.parent.transform.position = new Vector3(0, Screen.height - m_consoleSettings.ConsoleHeight);
		((RectTransform)m_descriptionText.transform.parent).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_descriptionText.textBounds.size.y);
		//  = new Vector2(((RectTransform)m_descriptionText.transform.parent).sizeDelta.x, m_descriptionText.GetPreferredValues(m_descriptionText.text, m_descriptionText.rectTransform.rect.width, Mathf.Infinity).y);

		if (args.Length <= 0 || string.IsNullOrEmpty(args[0]))
		{
			return;
		}

		string text = "";
		if (args.Length == 1)
		{
			List<Command> list = Search(args[0]);
			if (list.Count <= 0)
			{
				text = $"<color=orange>Invalid command \"{args[0]}\"";
				m_descriptionText.text = text;
			}

			bool flag = true;
			foreach (Command command in list)
			{
				if (flag)
				{
					text += "<color=yellow>";
				}
				text += command.GetShortDescription(flag) + "\n";
				if (flag)
				{
					text += "</color>";
				}
				flag = false;
			}
		}
		else
		{
			Command command = SearchExact(args[0]);

			if (command == null)
			{
				text = $"<color=orange>Invalid command \"{args[0]}\"";
				m_descriptionText.text = text;
				return;
			}

			text = command.Name + " ";
			int num = args.Length - 2;
			for (int i = 0; i < command.ArgDescriptions.Length; i++)
			{
				if (i == num)
				{
					text += "<color=yellow>";
					text += $"[{command.ArgDescriptions[i].ShortDesc} - {command.ArgDescriptions[i].LongDesc}] ";
					text += "</color>";
				}
				else
				{
					text += $"[{command.ArgDescriptions[i].ShortDesc}] ";
				}
			}

			if (command.ArgAutofill != null && command.ArgAutofill.TryGetValue(num, out var value))
			{
				// value = SearchArgumentAutofill(args[^1], value);
				value = SearchArgumentAutofill(args[args.Length - 1], value);
				text += "\n";
				for (int i = 0; i < value.Count; i++)
				{
					if (i <= 0)
					{
						text += "<color=yellow>";
						text += value[i] + "\n";
						text += "</color>";
					}
					else
					{
						text += value[i] + "\n";
					}
				}
			}
		}

		m_descriptionText.text = text;
	}

	public void LogToConsole(string text)
	{
		if (!m_consoleSettings.Echo) return;

		// Create space.
		if (m_logs.Count >= m_consoleSettings.MaxLogCount)
		{
			m_logs.Dequeue();
		}

		// log.
		TimeSpan timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartupAsDouble);
		m_logs.Enqueue($"[<alpha=#55>{timeSpan.ToString("mm\\:ss")}<alpha=#FF>] {text}");

		// Update UI.
		UpdateLogWindow();
	}

	public void UpdateLogWindow()
	{
		m_logWindow.text = string.Join("\n", m_logs);

		m_vertScrollBar.value = -1;
	}

	public void TryExecuteCommand(string[] args, bool addCommandToHistory = true)
	{
		m_selectedCommandInHistory = -1;

		if (args.Length <= 0 || string.IsNullOrWhiteSpace(args[0]))
		{
			SelectInputField();
			return;
		}

		Command command = SearchExact(args[0]);
		if (command == null)
		{
			LogToConsole($"<color=orange>Invalid command \"{args[0]}\"</color>");
			if (addCommandToHistory)
			{
				SelectInputField();
				AddCommandToHistoryAndClearInput();
			}
			return;
		}

		try
		{
			command.Action(args);
		}
		catch (Exception e)
		{
			LogToConsole($"<color=orange>Command \"{args[0]}\" failed: {e.Message}</color>");
		}

		if (addCommandToHistory)
		{
			AddCommandToHistoryAndClearInput();
		}

		SelectInputField();
	}


	private void AddCommandToHistoryAndClearInput()
	{
		if (string.IsNullOrEmpty(m_inputField.text)) return;

		// Create space.
		if (m_executedCommands.Count >= m_consoleSettings.MaxLogCommandHistory)
		{
			m_executedCommands.Pop();
		}

		// Add to history.
		m_executedCommands.Push(m_inputField.text);

		// Clear input.
		m_inputField.text = "";
	}

	public static List<string> SearchArgumentAutofill(string input, List<string> fills)
	{
		return fills.Where((string c) => c.Contains(input)).ToList();
	}

	public static string SearchArgumentAutofillFirst(string input, List<string> fills)
	{
		return fills.FirstOrDefault((string c) => c.Contains(input));
	}

	public static List<Command> Search(string input)
	{
		return Commands.Where((Command c) => c.Name.Contains(input, StringComparison.OrdinalIgnoreCase)).ToList();
	}

	public static Command SearchFirst(string input)
	{
		return Commands.FirstOrDefault((Command c) => c.Name.Contains(input, StringComparison.OrdinalIgnoreCase));
	}

	public static Command SearchExact(string input)
	{
		return Commands.FirstOrDefault((Command c) => c.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
	}

	private int ParseInt(string s)
	{
		if (!int.TryParse(s, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid integer value! (0, 1, 5, etc)");
		}
		return result;
	}

	private float ParseFloat(string s)
	{
		if (!float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid float value! (2, 0.7, 14.1, etc)");
		}
		return result;
	}

	private bool ParseBool(string s)
	{
		if (!bool.TryParse(s, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid boolean value! (true/false)");
		}
		return result;
	}

	private Color ParseColor(string s)
	{
		if (!ColorUtility.TryParseHtmlString(s, out var color))
		{
			throw new Exception("\"" + s + "\" is not a valid color value! (#FFFFFF, etc)");
		}
		return color;
	}

	private TEnum ParseEnum<TEnum>(string s)
		where TEnum : struct, Enum
	{
		if (!Enum.TryParse<TEnum>(s, out var result))
		{
			throw new Exception("\"" + s + "\" is not a valid value!");
		}

		return result;
	}

	private Vector3 ParsePosition(string s)
	{
		switch (s)
		{
			case "player":
				return PlayerRefFetcher.Instance.transform.position;
			case "random":
				// TODO: use world size.
				return UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(0, 100);
			default:
				{
					string[] array = s.Split(',');
					if (array.Length == 3)
					{
						return new Vector3(ParseFloat(array[0]), ParseFloat(array[1]), ParseFloat(array[3]));
					}

					throw new Exception("\"" + s + "\" is not a valid position! (player/random/#,#,#)");
				}
		}
	}

	private void CheckForCheats()
	{
		if (!SteamStatsAndAchievements.AreAchievementsDisabled)
		{
#if UNITY_EDITOR
			Alert("<color=#ff006a>Achievements would have been disabled but since this is in the editor this was ignored.</color>");
#else
			SteamStatsAndAchievements.DisableAchievements();
			Alert("<color=#ff006a>WARNING: You used a cheat command. Achievements are disabled until you restart the game.</color>");
#endif
		}
	}

	public void Alert(string message)
	{
		if (!m_isConsoleActive)
		{
			ToggleActiveState();
		}
		ExecuteCommand("log " + message);
	}

	public void ExecuteCommand(string command)
	{
		TryExecuteCommand(command.Split(' ', StringSplitOptions.RemoveEmptyEntries));
	}

	private void RunCommandString(string commandStr)
	{
		commandStr = commandStr.Replace('_', ' ');
		string[] array = commandStr.Split(';');
		foreach (string text in array)
		{
			TryExecuteCommand(text.Split(' ', StringSplitOptions.RemoveEmptyEntries), false);
		}
	}

	private void CheckArgumentCount(string[] args, int desired)
	{
		if (args.Length <= desired)
		{
			throw new Exception(string.Format("Expected at least {0} argument{1}, but got {2}.", desired, (desired > 1) ? "s" : "", args.Length - 1));
		}
	}


	private void AddCustomCommand(string[] args)
	{
		string commandString = args[2];
		Commands.Add(new Command(args[0].Replace(' ', '.'), args[1].Replace('_', ' '), (string[] array) =>
		{
			RunCommandString(commandString);
		}, null)
		{
			IsCustomCommand = true
		});
	}



	public void RegisterAllCommands()
	{
		Commands.Add(new Command("help", "Shows the command list.", (string[] args) =>
		{
			List<string> list = new List<string>();

			foreach (Command command in Commands)
			{
				list.Add($"{command.GetShortDescription()} - <alpha=#48>{command.Description}<alpha=#FF>");
			}
			list.Add("Scroll to see all the commands.");
			LogToConsole(string.Join("\n", list));
		}, null));

		Commands.Add(new Command("log", "Adds text to the console history.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			string text = string.Join(" ", args.Skip(1));
			LogToConsole(text);
		}, null, ("string", "The text to log.")));


		Commands.Add(new Command("version", "Get the game's current version.", (string[] args) =>
		{
			LogToConsole("v" + Application.version);
		}, null));


		Commands.Add(new Command("noclip", "Allows you to move around without any collisions or gravity.", (string[] args) =>
		{
			CheckForCheats();

			bool specifiedActive = false;
			bool targetMode = false;

			if (args.Length > 1)
			{
				specifiedActive = true;
				targetMode = ParseBool(args[1]);
			}

			GameObject player = GameObject.FindGameObjectWithTag(Constants.PlayerTag);

			if (player == null)
			{
				throw new Exception("No player was found! Are in a active game right now?");
			}

			if (player.GetComponent<NoClipPlayerController>() || (specifiedActive && !targetMode))
			{
				Destroy(player.GetComponent<NoClipPlayerController>());

				if (player.transform.GetComponent<CharacterController>())
					player.transform.GetComponent<CharacterController>().enabled = true;
				if (player.transform.GetComponent<Rigidbody>())
					player.transform.GetComponent<Rigidbody>().isKinematic = false;

				LogToConsole("NoClip deactivated.");
			}
			else if ((specifiedActive && targetMode) || !specifiedActive)
			{
				if (player.transform.GetComponent<CharacterController>())
					player.transform.GetComponent<CharacterController>().enabled = false;
				if (player.transform.GetComponent<Rigidbody>())
					player.transform.GetComponent<Rigidbody>().isKinematic = true;

				player.AddComponent<NoClipPlayerController>();

				LogToConsole("NoClip activated.");
			}

		}, null, ("bool", "Optional set the noclip mode")));

		Commands.Add(new Command("givescrap", "Give scrap to yourself.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 1);
			int count = ParseInt(args[1]);

			if (RunManager.Instance == null)
			{
				throw new Exception("You are not currently in a game!");
			}

			RunManager.Instance.AddToDepositedScrap(count);

			LogToConsole($"Added {count} to your inventory! Now at {RunManager.Instance.GetCurrentScrapCount()} scrap!");


		}, null, ("int", "The amount of scrap to give yourself")));


		Commands.Add(new Command("giveupgradecards", "Give upgrade cards to yourself.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 2);

			ModuleTier moduleTier = ParseEnum<ModuleTier>(args[1]);
			int amount = ParseInt(args[2]);

			if (RunManager.Instance == null)
			{
				throw new Exception("You are not currently in a game!");
			}

			RunManager.Instance.AddToStoredModules(moduleTier, amount);

			LogToConsole($"Added {amount} of {moduleTier.ToString()} to your inventory! Now at {RunManager.Instance.GetModuleCount(moduleTier)} module{RunManager.Instance.GetModuleCount(moduleTier) > 1: 's' : ''}!");

		}, new Dictionary<int, List<string>>
		{
			{
				0,
				Enum.GetNames(typeof(ModuleTier)).ToList()
			}
		}, ("card tier", "The card tier to give"), ("int amount", "The quantity to give of that specified card tier")));

		Commands.Add(new Command("damageplayer", "Adds damage to the player character.", (string[] args) =>
		{
			CheckForCheats();
			CheckArgumentCount(args, 1);
			float damage = ParseFloat(args[1]);

			GameObject player = GameObject.FindGameObjectWithTag(Constants.PlayerTag);

			if (!player)
			{
				throw new Exception("No player was found! Are in a active game right now?");
			}

			Health h = player.GetComponent<Health>();

			LogToConsole($"Damaged player! Now at {h.GetHealthValue() - damage}.");

			h.AddToHealth(-damage);

		}, null, ("float", "Damage to apply.")));


		Commands.Add(new Command("reload", "Reloads the current scene.", (_) =>
		{
			if (LevelLoading.Instance)
			{
				if (LevelLoading.Instance.IsLoading)
				{
					throw new Exception("Cannot reload a scene when currently loading!");
				}

				LevelLoading.Instance.Reload();
			}
			else
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}

			LogToConsole("Reloading scene!");

		}, null));


		Commands.Add(new Command("destroy", "Destroys the current object in front of the camera.", (_) =>
		{
			CheckForCheats();
			if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f)), out RaycastHit hit, 1000f))
			{
				LogToConsole($"Destroyed {hit.transform.name}!");
				Destroy(hit.transform.gameObject);
			}
			else
			{
				LogToConsole("Nothing to destroy!");
			}

		}, null));

		Commands.Add(new Command("getplayerpos", "Get the current position of the player.", (_) =>
		{
			LogToConsole($"You're at: {Camera.main.transform.position}");

		}, null));

		Commands.Add(new Command("updateupgrademenu", "Updates the upgrade menu for anything that hasn't updated due to \"changes\".", (_) =>
		{
			CheckForCheats();

			if (!UpgradeMenuManager.Instance)
			{
				throw new Exception("You need to be on the hub world!");
			}

			UpgradeMenuManager.Instance.UpdateStatUI();

		}, null));


		Commands.Add(new Command("setconsoleheight", "Set the height of the console.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);
			float height = ParseFloat(args[1]);

			if (height < 100f)
			{
				height = 100f;
			}
			else if (height > (float)Screen.height)
			{
				height = Screen.height;
			}

			m_consoleSettings.ConsoleHeight = height;
			ApplySettings();

			LogToConsole($"Console height has been set to {height}.");

			WriteConsoleSettings();

		}, new Dictionary<int, List<string>>
		{
			{
				0,
				new List<string> { "500" }
			}
		}, ("float", "(Default: 500) The height of the console.")));


		Commands.Add(new Command("quit", "Quits the game. (Force unity to crash hard both engine and game)", (_) =>
		{
			Debug.Log("Shutting down gracefully...");
			UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
		}, null));


		Commands.Add(new Command("viewalllogs", "Shows all the logs that are being sent to the console.", (string[] args) =>
		{

			if (args.Length > 0)
			{
				m_consoleSettings.OnlyWatchForErrors = !ParseBool(args[1]);
			}
			else
			{
				m_consoleSettings.OnlyWatchForErrors = !m_consoleSettings.OnlyWatchForErrors;
			}

			ApplySettings();

			LogToConsole($"Full logging has been set to {!m_consoleSettings.OnlyWatchForErrors}.");

			WriteConsoleSettings();

		}, null, ("bool", "(Default: False) Set whether to see the logs or not.")));


		Commands.Add(new Command("echo", "Toggle command logging.", (string[] args) =>
		{
			if (args.Length > 1)
			{
				m_consoleSettings.Echo = ParseBool(args[1]);
			}
			else
			{
				m_consoleSettings.Echo = !m_consoleSettings.Echo;
			}


			if (enabled)
			{
				LogToConsole("Echo enabled.");
			}

			ApplySettings();

			WriteConsoleSettings();

		}, null, ("bool", "(Default: True) Enable logging?")));


		Commands.Add(new Command("clear", "Clears the console.", (_) =>
		{
			m_logs.Clear();

			UpdateLogWindow();

		}, null));


		Commands.Add(new Command("open", "Opens the file / folder.", (string[] args) =>
		{
			CheckArgumentCount(args, 1);

			switch (args[1])
			{
				default:
					throw new Exception($"\"{args[1]}\" is not a valid folder!");

				case "logs":
					OpenInFileBrowser(Application.consoleLogPath);
					LogToConsole("Opened logs file.");
					break;

				case "saves":
					OpenInFileBrowser(Application.persistentDataPath);
					LogToConsole("Opened saves file location.");
					break;
			}

		}, new Dictionary<int, List<string>>
		{
			{
				0,
				new List<string> { "logs", "saves" }
			}
		}, ("string type", "Which folder to go to")));


		Commands.Add(new Command("removeplayerprefs", "Deletes PlayerPrefs settings.", (string[] args) =>
		{
			CheckForCheats();

			if (args.Length > 1)
			{
				PlayerPrefs.DeleteKey(args[1]);
				LogToConsole("Deleted PlayerPrefs key \"" + args[1] + "\".");
			}
			else
			{
				PlayerPrefs.DeleteAll();
				LogToConsole("Deleted all PlayerPrefs keys.");
			}


		}, null, ("string key", "If provided a key, only delete that key. Otherwise delete all PlayerPrefs.")));
















		Commands.OrderBy(x => x.Name);
	}

	public void RegisterCommand(Command command)
	{
		if (SearchExact(command.Name) != null)
		{
			throw new Exception("There is a command already registered with that name!");
		}

		Commands.Add(command);

		Debug.Log($"Successfully registered {command.Name} command!");
	}

	public static void OpenInFileBrowser(string path)
	{
		Process.Start("explorer.exe", path.Replace("/", "\\"));
	}
}
