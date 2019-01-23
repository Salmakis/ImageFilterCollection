using FilterWrapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageFilterConsole
{
	class ListCommand : ICommand
	{
		public string Command => "List";

		public string Help => "Lists all Options and Filters";

		public string Info => "Lists all Options and Filters";

		public string Execute(string[] args)
		{
			var returnString = "Available commands:\n\n";
			var filterInfos = "\nAvailable Filters:\n\n";
			var commands = CommandManager.CommandList;
			foreach (ICommand command in commands)
			{
				if (command.GetType() == typeof(FilterCommand))
				{
					filterInfos += $"  {command.Command} - { command.Info}\n";
				}
				else
				{
					returnString += $"  {command.Command} - { command.Info}\n";
				}
			}

			returnString += filterInfos;
			return returnString;
		}		
	}
}
