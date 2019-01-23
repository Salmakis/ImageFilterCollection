
using FilterWrapper;
using ImageFilterConsole;
using System;
using System.Drawing;
using System.Linq;

namespace ImageFilterConsoleDotNet4
{
	class Program
	{

		static CommandManager commandManager = new CommandManager();
		
		static void Main(string[] args)
		{	
			if (args.Length > 0){
				if (commandManager.IsFilter(args[0]) && args.Length >= 3){

					Bitmap bmpSrc;
					try
					{
						bmpSrc = new Bitmap(args[1], true);
					}
					catch (Exception e)
					{
						Console.WriteLine("err: " + e);
						return;
					}
					
					FilterImage input = FilterImageConvert.FilterImageFromBitmap(bmpSrc);
					bmpSrc.Dispose();

					FilterImage output = new FilterImage(0,0);

					Console.Write(commandManager.ExecuteCommand(args[0], args.Skip(1).ToArray(),ref output,input));
					Bitmap outputBitmap = FilterImageConvert.BitmapFromFilterImage(output);
					outputBitmap.Save(args[2]);
					outputBitmap.Dispose();
				}
				else{
					Console.Write(commandManager.ExecuteCommand(args[0], args.Skip(1).ToArray()));
				}
				
			}else{
				Console.Write(commandManager.ExecuteCommand("List", new string[] { }));
			}
		}
	}
}
