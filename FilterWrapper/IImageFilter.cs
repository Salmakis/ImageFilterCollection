#region (c)2019 Annika Ryll
/*
    Image filtering FilterManager - interface for imagefilters, implement this interface for your own filter
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

using System.Collections.Generic;

namespace FilterWrapper
{
	public interface IImageFilter
	{
		/// <summary>
		/// all the settings related to this filter (if there are some)
		/// </summary>
		IEnumerable<Setting> Settings { get; }

		/// <summary>
		/// shall execute this filter with its current settings on the provided input image and return a new or modified one
		/// </summary>
		FilterImage Execute(FilterImage input);

		/// <summary>
		/// display some infos like who made algo and who implemented it etc.
		/// </summary>
		string Info { get; }

		/// <summary>
		/// display a small name as 1 word (without whitespaces for console version)
		/// </summary>
		string Name { get; }
	}
}
