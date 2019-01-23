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

using System;
using System.Collections.Generic;
using System.Drawing;
using Imager.Filters;
using Imager.Interface;

namespace Imager
{
	public partial class cImage
	{
		public cImage ApplyXbr(int scale, bool alphaBlending)
		{
			switch (scale)
			{	
				case 4:
					return (this._RunLoop(null, 4, 4, worker => libXBR.Xbr4X(worker, true)));
					break;
				case 3:
					return (this._RunLoop(null, 3,3, worker => libXBR.Xbr3X(worker, true,false)));
					break;
				case 2:
				default:
					return (this._RunLoop(null, 2, 2, worker => libXBR.Xbr2X(worker, true)));
					break;
			}
		}
	}
}
