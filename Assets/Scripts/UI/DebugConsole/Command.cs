using System.Collections.Generic;
using System.Linq;



public class Command
{
	public delegate void CommandAction(string[] args);

	public string Name;

	public string Description;

	public (string ShortDesc, string LongDesc)[] ArgDescriptions;

	public Dictionary<int, List<string>> ArgAutofill;

	public CommandAction Action;

	/// <summary>
	/// Is this a custom command created by the player?
	/// </summary>
	public bool IsCustomCommand;

	public string GetShortDescription(bool full = false)
	{
		string text = Name;
		if (ArgDescriptions.Length != 0)
		{
			text = text + " [" + string.Join("] [", ArgDescriptions.Select(((string shortDesc, string longDesc) c) => c.shortDesc).ToList()) + "]";
		}
		if (full)
		{
			text = text + " - " + Description;
		}
		return text;
	}

	public Command(string name, string description, CommandAction action, Dictionary<int, List<string>> argAutofill, params (string, string)[] argDescriptions)
	{
		Name = name;
		Description = description;
		ArgDescriptions = argDescriptions ?? new (string, string)[0];
		Action = action;
		ArgAutofill = argAutofill;
		for (int i = 0; i < ArgDescriptions.Length; i++)
		{
			if (ArgDescriptions[i].ShortDesc.StartsWith("bool"))
			{
				if (ArgAutofill == null)
				{
					ArgAutofill = new Dictionary<int, List<string>>();
				}

				ArgAutofill.Add(i, new List<string> { "true", "false" });
			}
			if (ArgDescriptions[i].ShortDesc.StartsWith("position"))
			{
				if (ArgAutofill == null)
				{
					ArgAutofill = new Dictionary<int, List<string>>();
				}
				ArgAutofill.Add(i, new List<string> { "player", "random", "#.#.#" });
			}
		}
	}
}