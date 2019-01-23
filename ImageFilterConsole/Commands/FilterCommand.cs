using FilterWrapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageFilterConsole
{
	class FilterCommand : ICommand
	{	
		IImageFilter filter;

		public FilterCommand(IImageFilter filter)
		{
			this.filter = filter;
		}

		public string Help
		{
			get
			{	
				string helpText = $"usage:{filter.Name} <inputFile> <outputFile> [<settingname>:<settingvalue>] [<settingname>:<settingvalue>]\n";
				helpText += "available options for this filter:\n\n";
				
				foreach (Setting setting in filter.Settings)
				{
					helpText += $"  {setting.Name} - {setting.Info}\n";
					
					if (setting.GetType() == typeof(SettingInteger))
					{
						var settingInteger = (SettingInteger)setting;
						helpText += $"    -range:{settingInteger.MinValue} to :{settingInteger.MaxValue} default:{settingInteger.Value}\n";
					}else if (setting.GetType() == typeof(SettingBoolean))
					{
						var settingBool = (SettingBoolean)setting;
						helpText += $"    -true or false, default:{settingBool.Value}\n";
					}
					else if (setting.GetType() == typeof(SettingOption))
					{
						var settingOption = (SettingOption)setting;
						helpText += $"    -one of the following\n";
						foreach (string option in settingOption.Options)
						{
							helpText += $"      +{option}";
							if (option.Equals(settingOption.GetSelected(), StringComparison.InvariantCultureIgnoreCase)){
								helpText += " (default)\n";
							}else{
								helpText += "\n";
							}
						}
					}
				}
				return helpText;
			}
		}

		public string Info => this.filter.Info;

		public string Command => filter.Name;

		public string Execute(string[] args)
		{
			return Help;
		}

		public string ExecuteFilter(FilterImage inputImage,ref FilterImage outputImage, string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				var argSplit = args[i].Split(':');
				foreach (Setting setting in filter.Settings)
				{
					if (setting.Name.Equals(argSplit[0],StringComparison.InvariantCultureIgnoreCase))
					{
						setting.Set(argSplit[1]);
					}
				}
			}
			outputImage = filter.Execute(inputImage);
			return "Ok";
		}
	}
}
