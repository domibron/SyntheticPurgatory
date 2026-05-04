using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//by    _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// © 2025 domibron

public class CommandBase
{
	private string _command;
	private string _commandDescription;
	private string _commandHelp;

	public string Command { get { return _command; } }
	public string CommandDescription { get { return _commandDescription; } }
	public string CommandHelp { get { return _commandHelp; } }

	public CommandBase(string command, string description, string helpMessage)
	{
		_command = command;
		_commandDescription = description;
		_commandHelp = helpMessage;
	}
}

public class Command : CommandBase
{
	private Action commandEvent;

	public Command(string command, string description, string helpMessage, Action commandEvent) : base(command, description, helpMessage)
	{
		this.commandEvent = commandEvent;
	}

	public void Invoke()
	{
		commandEvent.Invoke();
	}
}

public class Command<T1> : CommandBase
{
	private Action<T1> commandEvent;

	public Command(string command, string description, string helpMessage, Action<T1> commandEvent) : base(command, description, helpMessage)
	{
		this.commandEvent = commandEvent;
	}

	public void Invoke(T1 t1)
	{
		commandEvent.Invoke(t1);
	}
}

public class Command<T1, T2> : CommandBase
{
	private Action<T1, T2> commandEvent;

	public Command(string command, string description, string helpMessage, Action<T1, T2> commandEvent) : base(command, description, helpMessage)
	{
		this.commandEvent = commandEvent;
	}

	public void Invoke(T1 t1, T2 t2)
	{
		commandEvent.Invoke(t1, t2);
	}
}

public class Command<T1, T2, T3> : CommandBase
{
	private Action<T1, T2, T3> commandEvent;

	public Command(string command, string description, string helpMessage, Action<T1, T2, T3> commandEvent) : base(command, description, helpMessage)
	{
		this.commandEvent = commandEvent;
	}

	public void Invoke(T1 t1, T2 t2, T3 t3)
	{
		commandEvent.Invoke(t1, t2, t3);
	}
}
