using System;
using System.Collections.Generic;
using System.Text;

namespace ImageFilterConsole
{
	public interface ICommand
	{
		string Command { get; }
		string Execute(string[] args);
		string Info { get; }
		string Help { get; }
	}
}
