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
using System;
using System.Collections.Generic;
using System.Text;

namespace FilterWrapper
{
	public partial class FilterImage
	{
		/// <summary>
		/// converts this image to an instance of Hawkynt cImage
		/// </summary>
		public cImage tocImage()
		{
			cImage image = new cImage(this.width, this.height);

			int y;
			int x;

			for (x = 0; x < this.width; x++)
			{
				for (y = 0; y < this.height; y++)
				{
					image.SetPixel(x, y, new sPixel(this.r[x * height + y], this.g[x * height + y], this.b[x * height + y], this.a[x * height + y]));
				}
			}

			return (image);
		}

		/// <summary>
		/// creates a new FilterImage form a Hawkynt cImage
		/// </summary>
		public static FilterImage FromCImage(cImage cimage){
			return new FilterImage(cimage);
		}
	
		/// <summary>
		/// creates a new FilterImage form a Hawkynt cImage
		/// </summary>
		/// <param name="image"></param>
		public FilterImage(cImage image)
		{
			this.width = image.Width;
			this.height = image.Height;
			this.r = new byte[width * height];
			this.g = new byte[width * height];
			this.b = new byte[width * height];
			this.a = new byte[width * height];

			int x;
			int y;
			sPixel pixel;

			for (x = 0; x < this.width; x++)
			{
				for (y = 0; y < this.height; y++)
				{
					pixel = image.GetPixel(x, y);
					this.r[x * height + y] = pixel.Red;
					this.g[x * height + y] = pixel.Green;
					this.b[x * height + y] = pixel.Blue;
					this.a[x * height + y] = pixel.Alpha;
				}
			}
		}
	}
}
