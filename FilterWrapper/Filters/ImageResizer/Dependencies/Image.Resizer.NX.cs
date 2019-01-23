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
	/// <summary>
	/// 
	/// </summary>
	public partial class cImage
	{
		/// <summary>
		/// The NQ kernel.
		/// </summary>
		/// <param name="pattern">The pattern.</param>
		/// <param name="c0">The c0.</param>
		/// <param name="c1">The c1.</param>
		/// <param name="c2">The c2.</param>
		/// <param name="c3">The c3.</param>
		/// <param name="c4">The c4.</param>
		/// <param name="c5">The c5.</param>
		/// <param name="c6">The c6.</param>
		/// <param name="c7">The c7.</param>
		/// <param name="c8">The c8.</param>
		/// <returns></returns>
		internal delegate void NqKernel(byte pattern, sPixel c0, sPixel c1, sPixel c2, sPixel c3, sPixel c4, sPixel c5, sPixel c6, sPixel c7, sPixel c8, PixelWorker<sPixel> worker);

		/// <summary>
		/// The NQ filter itself
		/// </summary>
		internal delegate void NqFilter(PixelWorker<sPixel> worker, byte scx, byte scy, NqKernel kernel);

		/// <summary>
		/// Applies the NQ pixel scaler.
		/// </summary>
		/// <param name="type">The type of scaler to use.</param>
		/// <param name="mode">The mode.</param>
		/// <param name="filterRegion">The filter region, if any.</param>
		/// <returns>
		/// The rescaled image.
		/// </returns>
		public cImage ApplyHQ(int scale,string type)
		{

			NqKernel kernel;
			byte bScale = (byte)scale;

			switch (scale)
			{
				case 2:
				default:
					kernel = libHQ.Hq2xKernel;
					bScale = 2;
					break;
				case 3:
					kernel = libHQ.Hq3xKernel;
					break;
				case 4:
					kernel = libHQ.Hq4xKernel;
					break;
			}

			switch (type)
			{
				case "normal":
				default:
					return (this._RunLoop(null, bScale, bScale, worker => libHQ.ComplexFilter(worker, bScale, bScale, kernel)));
				case "bold":
					return (this._RunLoop(null, bScale, bScale, worker => libHQ.ComplexFilterBold(worker, bScale, bScale, kernel)));
				case "smart":
					return (this._RunLoop(null, bScale, bScale, worker => libHQ.ComplexFilterSmart(worker, bScale, bScale, kernel)));
			}
		}

		public cImage ApplyLQ(int scale, string type)
		{
			NqKernel kernel;
			byte bScale = (byte)scale;

			switch (scale)
			{
				case 2:
				default:
					kernel = libHQ.Lq2xKernel;
					bScale = 2;
					break;
				case 3:
					kernel = libHQ.Lq3xKernel;
					break;
				case 4:
					kernel = libHQ.Lq4xKernel;
					break;
			}

			switch (type)
			{
				case "normal":
				default:
					return (this._RunLoop(null, bScale, bScale, worker => libHQ.ComplexFilter(worker, bScale, bScale, kernel)));
				case "bold":
					return (this._RunLoop(null, bScale, bScale, worker => libHQ.ComplexFilterBold(worker, bScale, bScale, kernel)));
				case "smart":
					return (this._RunLoop(null, bScale, bScale, worker => libHQ.ComplexFilterSmart(worker, bScale, bScale, kernel)));
			}
		}
	}
}