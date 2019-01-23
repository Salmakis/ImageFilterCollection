#region (c)2019 Annika Ryll
/*
    Image filtering settings
    Copyright (C) 2019 Annika Ryll

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace FilterWrapper
{
	public abstract class Setting
	{
		//some name for the value
		public string Name { get => name; }
		protected string name;

		//some info (like to use in mouseover tooltip or infobox)
		public string Info { get => info; }
		protected string info;

		public abstract void Set(string set);
	}

	public class SettingInteger : Setting
	{
		public int Value { get; set; }
		public int MaxValue { get; }
		public int MinValue { get; }

		public SettingInteger(string name, string info,int value, int maxValue, int minValue)
		{
			this.info = info;
			this.name = name;
			Value = value;
			MaxValue = maxValue;
			MinValue = minValue;
		}

		public override void Set(string set)
		{
			int value = int.Parse(set);
			if (value <= MaxValue && value >= MinValue){
				Value = value;
			}
		}
	}

	public class SettingBoolean : Setting
	{
		public bool Value { get; set; } = false;
		public SettingBoolean(string name, string info, bool value = false)
		{
			this.info = info;
			this.name = name;
			Value = value;
		}

		public override void Set(string set)
		{
			if (
				set.Equals("true",StringComparison.InvariantCultureIgnoreCase) ||
				set.Equals("1", StringComparison.InvariantCultureIgnoreCase) ||
				set.Equals("yes", StringComparison.InvariantCultureIgnoreCase) ||
				set.Equals("ja", StringComparison.InvariantCultureIgnoreCase) ||
				set.Equals("+", StringComparison.InvariantCultureIgnoreCase)
			)
			{
				Value = true;
			}else{
				Value = false;
			}
		}
	}

	public class SettingOption : Setting
	{
		public IEnumerable<string>Options { get => options; }
		private string[] options;
		public int SelectedOption { get; set; }

		public SettingOption(string name, string info,string[] options)
		{
			this.info = info;
			this.name = name;
			this.options = options;
		}

		public string GetSelected()
		{
			return (options[SelectedOption]);
		}

		public bool TrySetSelected(string selection)
		{
			for (int i = 0; i < options.Length; i++)
			{
				if (options[i].Equals(selection, StringComparison.InvariantCultureIgnoreCase)){
					SelectedOption = i;
					return true;
				}
			}
			return false;
		}

		public override void Set(string set)
		{
			TrySetSelected(set);
		}
	}
}
