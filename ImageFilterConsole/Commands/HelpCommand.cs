using FilterWrapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageFilterConsole
{
	class HelpCommand : ICommand
	{
		IImageFilter filter;

		public string Help => "usage: help <filtername>";

		public string Info => "Displays help about a filter";

		public string Command => "Help";

		public string Execute(string[] args)
		{	
			if (args.Length > 0)
			{
				var command = CommandManager.GetCommand(args[0]);
				if (null != command){
					return command.Help;
				}else{
					return $"There is no such filter or command: {args[0]}";
				}
			}else{
				return Help;
			}
		}
	}
}
