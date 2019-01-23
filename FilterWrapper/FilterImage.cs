#region (c)2019 Annika Ryll
/*
    Image filtering FilterImage - a imageformat to use with FilterWrapper filters
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
using System.Drawing;

namespace FilterWrapper
{
	public partial class FilterImage
	{
		int width;
		int height;

		byte[] r;
		byte[] g;
		byte[] b;
		byte[] a;

		public int Width { get => width;}
		public int Height { get => height;}

		public void Reset(int width, int height)
		{
			this.width = width;
			this.height = height;
			this.r = new byte[width * height];
			this.g = new byte[width * height];
			this.b = new byte[width * height];
			this.a = new byte[width * height];
		}

		public int PixelArgb(int x, int y)
		{
			return ((a[x * height + y] << 24) + (r[x * height + y] << 16) + (g[x * height + y] << 8) + b[x * height + y]);
		}

		public Color PixelColor(int x, int y)
		{
			return Color.FromArgb(PixelArgb(x, y));
		}

		public FilterImage(int width, int height)
		{
			this.Reset(width, height);
		}

		public void SetPixel(int x, int y, byte r, byte g, byte b, byte a)
		{
			if (x > width || x < 0 || y > height || y < 0)
			{
				throw new IndexOutOfRangeException($"x:{x}, y:{y} image size: {width} * {height}");
			}

			this.r[x * height + y] = r;
			this.g[x * height + y] = g;
			this.b[x * height + y] = b;
			this.a[x * height + y] = a;
		}

		public byte Red(int x, int y){
			return this.r[x * height + y];
		}

		public byte Green(int x, int y)
		{
			return this.g[x * height + y];
		}

		public byte Blue(int x, int y)
		{
			return this.b[x * height + y];
		}

		public byte Alpha(int x, int y)
		{
			return this.a[x * height + y];
		}

		public void SetPixel(int x, int y,Color color)
		{
			if (x > width || x < 0 || y > height || y < 0)
			{
				throw new IndexOutOfRangeException($"x:{x}, y:{y} image size: {width} * {height}");
			}

			this.r[x * height + y] = color.R;
			this.g[x * height + y] = color.G;
			this.b[x * height + y] = color.B;
			this.a[x * height + y] = color.A;
		}
	}
}
