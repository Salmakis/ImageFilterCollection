#region (c)2019 Annika Ryll
/*
    Image filtering FilterManager - collects all IImageFilter and offert hem with settings
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
using System.Linq;
using System.Reflection;

namespace FilterWrapper
{
	public class FilterManager
	{
		Dictionary<string, IImageFilter> filters = new Dictionary<string, IImageFilter>();

		public FilterManager()
		{
			//get all classes that implements IImageFilter
			var filterImplementations = from type in Assembly.GetExecutingAssembly().GetTypes()
										where typeof(IImageFilter).IsAssignableFrom(type)
										select type;

			//create instances and put them into the dict
			foreach (var filterType in filterImplementations)
			{
				if (filterType != typeof(IImageFilter)){
					IImageFilter newFilter = (IImageFilter)Activator.CreateInstance(filterType);
					filters.Add(newFilter.Name, newFilter);
				}
			}
		}

		public IEnumerable<IImageFilter> FoundFilters { get => filters.Values; } 

		public IImageFilter GetFilterByName(string name)
		{
			return filters[name];
		}
	}
}
