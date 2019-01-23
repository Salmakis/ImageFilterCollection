#region (c)2008-2015 Hawkynt
/*
 *  cImage 
 *  Image filtering library 
    Copyright (C) 2008-2015 Hawkynt

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

#region Modified Salmakis
/*
 *	Removed and simplified some of the interface / scale type things
*/
#endregion
using Imager.Filters;
using Imager.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Imager
{
	public partial class cImage
	{
		public cImage ApplyXbrz(int scale)
		{
			var result = new cImage(this.Width * scale, this.Height * scale);

			switch (scale)
			{
				case 2:
					libXBRz.ScaleImage(libXBRz.ScaleSize.TIMES2, this.GetImageData(), result.GetImageData(), Width, Height, 0, 0, Width, Height);
					break;
				case 3:
					libXBRz.ScaleImage(libXBRz.ScaleSize.TIMES3, this.GetImageData(), result.GetImageData(), Width, Height, 0, 0, Width, Height);
					break;
				case 4:
					libXBRz.ScaleImage(libXBRz.ScaleSize.TIMES4, this.GetImageData(), result.GetImageData(), Width, Height, 0, 0, Width, Height);
					break;
				case 5:
					libXBRz.ScaleImage(libXBRz.ScaleSize.TIMES5, this.GetImageData(), result.GetImageData(), Width, Height, 0, 0, Width, Height);
					break;
				default:
					break;
			}
			return (result);
		}
	}
}