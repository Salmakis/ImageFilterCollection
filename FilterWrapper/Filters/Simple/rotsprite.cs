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

namespace FilterWrapper.Filters
{
	class rotsprite : IImageFilter
	{
		List<Setting> settings = new List<Setting>();

		SettingInteger rotateDegrees = new SettingInteger("Angle", "angle in degrees, clockwise", 15, 180, -180);

		public rotsprite()
		{
			settings.Add(rotateDegrees);
		}

		public IEnumerable<Setting> Settings { get { return settings; } }

		public string Info { get { return "This is the rotsprite alogorithn developed by Xenowhirl"; } }

		public string Name { get { return "rotsprite"; } }

		public FilterImage Execute(FilterImage input)
		{
			cImage cimage = input.tocImage();
			cimage = cimage.ApplyXbr(4, false);
			cimage = cimage.ApplyXbr(2, false);

            int normalizedDegs = rotateDegrees.Value < 0 ? rotateDegrees.Value + 360 : rotateDegrees.Value;

            double rotateRads = normalizedDegs * Math.PI / 180.0;


            FilterImage rot = RotateAndDownscale(new FilterImage(cimage), rotateRads);

			return rot;
		}

		public FilterImage RotateAndDownscale(FilterImage input, double angleRad)
		{
            int newWidth = (int)((double)input.Width /8);
            int newHeight = (int)((double)input.Height /8);
            FilterImage newImage = new FilterImage(newWidth, newHeight);

            int centerX = newWidth / 2;
            int centerY = newHeight / 2;

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    double dir = Math.Atan2(y - centerY, x - centerX);
                    double magnitude = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)) * 8;

                    dir = dir - angleRad;

                    int origX = (int)Math.Round((centerX * 8) + magnitude * Math.Cos(dir));
                    int origY = (int)Math.Round((centerY * 8) + magnitude * Math.Sin(dir));

                    if (origX >= 0 && origX < input.Width &&
                        origY >= 0 && origY < input.Height)
                    {
                        newImage.SetPixelArgb(x, y, input.PixelArgb(origX, origY));
                    }
                }
            }

            return newImage;
        }
	}
}
