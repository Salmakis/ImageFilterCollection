using FilterWrapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ImageFilterConsole
{
	public class CommandManager
	{
		static Dictionary<string, ICommand> commandList = new Dictionary<string, ICommand>();
		
		static FilterManager filterManager;

		static public IEnumerable<ICommand> CommandList { get => commandList.Values; }

		public bool IsFilter(string commandName)
		{
			if (commandList.ContainsKey(commandName.ToLower(CultureInfo.InvariantCulture)) &&
				commandList[commandName].GetType() == typeof(FilterCommand))
			{
				return true;
			}
			return false;				
		}

		static public ICommand GetCommand(string name)
		{
			if (commandList.ContainsKey(name.ToLower(CultureInfo.InvariantCulture)))
			{
				return commandList[name.ToLower(CultureInfo.InvariantCulture)];
			}
			return null;
		}

		public string ExecuteCommand(string commandName, string[] args){
			if (!(commandList.ContainsKey(commandName.ToLower(CultureInfo.InvariantCulture))))
			{
				return $"err: there is no such command '{commandName}', use 'list' to see the available commands";
			}
			var command = commandList[commandName.ToLower(CultureInfo.InvariantCulture)];
			return command.Execute(args);
		}

		public string ExecuteCommand(string commandName, string[] args, ref FilterImage output, FilterImage input)
		{	
			if (!(commandList.ContainsKey(commandName.ToLower(CultureInfo.InvariantCulture))))
			{	
				return $"err: there is no such command '{commandName}', use 'list' to see the available commands";
			}
			else
			{	
				try
				{
					if (null == output || null == input)
					{
						return ExecuteCommand(commandName, args);
					}
					else
					{
						var command = commandList[commandName.ToLower(CultureInfo.InvariantCulture)];
						if (command.GetType() == typeof (FilterCommand)){
							return ((FilterCommand)command).ExecuteFilter(input, ref output, args);
						}else{
							return command.Execute(args);
						}
					}
					
				}
				catch (Exception e)
				{
					return $"exception: {e.Message} \ninnerException: {e.InnerException?.Message}";
				}
			}
		}

		public CommandManager()
		{
			var commands = from type in Assembly.GetExecutingAssembly().GetTypes()
						   where typeof(ICommand).IsAssignableFrom(type)
						   select type;

			foreach (var command in commands)
			{
				//exclude filter command, cuz they handled below via filters itself
				if (command != typeof(ICommand) && command != typeof(FilterCommand))
				{
					ICommand newCommand = (ICommand)Activator.CreateInstance(command);
					commandList.Add(newCommand.Command.ToLower(CultureInfo.InvariantCulture), newCommand);
				}
			}

			filterManager = new FilterManager();

			foreach (var filter in filterManager.FoundFilters)
			{		
				ICommand newCommand = new FilterCommand(filter);
				commandList.Add(newCommand.Command.ToLower(CultureInfo.InvariantCulture), newCommand);
			}
		}
	}
}
