#region (c)2019 Annika Ryll
/*
    Image filtering library wrapper
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

using Imager;
using System.Collections.Generic;

namespace FilterWrapper.Filters
{
	class FilterHQ : IImageFilter
	{
		List<Setting> settings = new List<Setting>();

		SettingInteger scale = new SettingInteger("Scale", "Factor to scale", 2, 4, 2);
		SettingOption complex = new SettingOption("Complex", "Complex type", new string[] { "normal", "bold", "smart" });

		public FilterHQ()
		{
			settings.Add(scale);
			settings.Add(complex);
		}

		public IEnumerable<Setting> Settings { get { return settings; } }

		public string Info { get { return "This is the HQ filter by Maxim Stepin implemented by Hawkynt"; } }

		public string Name { get { return "HQ"; } }

		public FilterImage Execute(FilterImage input)
		{
			cImage cimage = input.tocImage();
			cimage = cimage.ApplyHQ(scale.Value,complex.GetSelected());
			return new FilterImage(cimage);
		}
	}
}
