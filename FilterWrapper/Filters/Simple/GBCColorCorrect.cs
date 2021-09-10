using Imager;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FilterWrapper.Filters.Simple
{
    class GBCColorCorrect : IImageFilter
    {
		List<Setting> settings = new List<Setting>();

		SettingBoolean reduceColors = new SettingBoolean("15-bit", "Reduce colors to 15-bit RGB.", true);
		SettingInteger simulatedDarkness = new SettingInteger("Grey %", "Simulates the grey background of the LCD screen.", 5, 50, 0);
		SettingOption style = new SettingOption("Style", "Which color correcting method to use", new string[] { "Pokefan531", "Sameboy", "None"});


		public GBCColorCorrect()
		{
			settings.Add(reduceColors);
			settings.Add(simulatedDarkness);
			settings.Add(style);
		}

		public IEnumerable<Setting> Settings
		{
			get
			{
				return settings;
			}
		}

		public string Info
		{
			get
			{
				return "Convert to 15-bit RGB with that signature darkness.";
			}
		}

		public string Name
		{
			get
			{
				return "GBC Color Correction";
			}
		}

		public FilterImage Execute(FilterImage input)
		{
			FilterImage newImage = new FilterImage(input.Width, input.Height);
			for (int x = 0; x < input.Width; x++)
			{
				for (int y = 0; y < input.Height; y++)
				{
					int argb = GBCColor(input.PixelArgb(x, y));
					newImage.SetPixelArgb(x, y, argb);
				}
			}
			return newImage;
		}

		private int GBCColor(int normalColor)
		{
			byte b = (byte)(normalColor & 0xFF);
			byte g = (byte)((normalColor >> 8) & 0xFF);
			byte r = (byte)((normalColor >> 16) & 0xFF);
			byte a = (byte)((normalColor >> 24) & 0xFF);

			// reduce to 5-bit values
			if (reduceColors.Value == true)
			{
				b /= 8;
				g /= 8;
				r /= 8;

				b = scale_channel_with_curve(b);
				g = scale_channel_with_curve(g);
				r = scale_channel_with_curve(r);
			}

			sPixel scaledPixel = sPixel.FromRGBA(r, g, b, a);
			Color gameboyizedColor;

			if (style.GetSelected() == "Pokefan531")
			{
				gameboyizedColor = GBCShader(scaledPixel.Color);
			}
			else if (style.GetSelected() == "Sameboy")
			{
				// GB_COLOR_CORRECTION_EMULATE_HARDWARE
				int new_b = b;
				int new_g = Math.Min((g * 3 + b) / 4, 255);
				int new_r = r;

				//GB_COLOR_CORRECTION_PRESERVE_BRIGHTNESS
				if (reduceColors.Value == true)
				{
					int old_max = Math.Max(r, Math.Max(g, b));
					int new_max = Math.Max(new_r, Math.Max(new_g, new_b));
					int old_min = Math.Min(r, Math.Min(g, b));
					int new_min = Math.Min(new_r, Math.Min(new_g, new_b));

					new_b = PreserveBrightness(new_b, old_max, new_max, old_min, new_min);
					new_g = PreserveBrightness(new_g, old_max, new_max, old_min, new_min);
					new_r = PreserveBrightness(new_r, old_max, new_max, old_min, new_min);
				}

				sPixel correctedPixel = sPixel.FromRGBA(new_r, new_g, new_b, a);
				gameboyizedColor = correctedPixel.Color;
			}
			else
			{
				gameboyizedColor = scaledPixel.Color;
			}


			return SimulateGreyness(gameboyizedColor).ToArgb();
		}


		private Color GBCShader(Color realColor)
		{
			float blr = 0.0f;
			float blg = 0.0f;
			float blb = 0.0f;
			float r = 0.87f;
			float g = 0.66f;
			float b = 0.79f;
			float rg = 0.115f;
			float rb = 0.14f;
			float gr = 0.18f;
			float gb = 0.07f;
			float br = -0.05f;
			float bg = 0.225f;

			float[][] mat = {
				new float[] {r,  rg,  rb,  0, 0},        // red scaling factor
				new float[] {gr,  g,  gb,  0, 0},        // green scaling factor
				new float[] {br, bg,   b,  0, 0},        // blue scaling factor
				new float[] {blr,blg,blb,  0, 1}};       // translations

			// check http://graficaobscura.com/matrix/index.html for ideas.

			// gamma adjust.
			const float targetGamma = 2.2f;
			const float displayGammaInv = 1.0f / targetGamma;

			float normalR = (float)Math.Pow((float)(realColor.R / 255.0), targetGamma);
			float normalG = (float)Math.Pow((float)(realColor.G / 255.0), targetGamma);
			float normalB = (float)Math.Pow((float)(realColor.B / 255.0), targetGamma);

			float[] rbg = MatrixMultiplyColor(mat, normalR, normalG, normalB);

			// gamma unadjust
			float newR = (float)Math.Pow(Math.Max(rbg[0], 0), displayGammaInv);
			float newG = (float)Math.Pow(Math.Max(rbg[1], 0), displayGammaInv);
			float newB = (float)Math.Pow(Math.Max(rbg[2], 0), displayGammaInv);

			newR = Math.Max(Math.Min(newR, 1.0f), 0.0f);
			newG = Math.Max(Math.Min(newG, 1.0f), 0.0f);
			newB = Math.Max(Math.Min(newB, 1.0f), 0.0f);

			Color BoostedColor = Color.FromArgb(realColor.A, (int)(newR * 255.0), (int)(newG * 255.0), (int)(newB * 255.0));
			return BoostedColor;
		}

		private float[] MatrixMultiplyColor(float[][] mat, float r, float g, float b)
		{
			float newR = r * mat[0][0] + g * mat[1][0] + b * mat[2][0] + mat[3][0];
			float newG = r * mat[0][1] + g * mat[1][1] + b * mat[2][1] + mat[3][1];
			float newB = r * mat[0][2] + g * mat[1][2] + b * mat[2][2] + mat[3][2];

			return new float[3] { newR, newG, newB };
		}



		private Color SimulateGreyness(Color realColor)
		{
			double hue;
			double saturation;
			double lightness;

			hue = realColor.GetHue() / 360.0;
			if (hue != 0)
			{
				//if (System.Diagnostics.Debugger.IsAttached)
					//System.Diagnostics.Debugger.Break();
			}
			saturation = realColor.GetSaturation();
			lightness = realColor.GetBrightness();

			double darkFactor = Math.Max(1.0 - (simulatedDarkness.Value * 0.01) * lightness, 0.0);

			lightness *= darkFactor;

			lightness = Math.Max(Math.Min(lightness, 1.0f), 0.0f);

			Color darkScreenColor = ColorFromHSL(hue, saturation, lightness, realColor.A);
			return darkScreenColor;
		}

		public static Color ColorFromHSL(double h, double s, double l, int alpha = 255)
		{
			double r = 0, g = 0, b = 0;
			if (l != 0)
			{
				if (s == 0)
					r = g = b = l;
				else
				{
					double temp2;
					if (l < 0.5)
						temp2 = l * (1.0 + s);
					else
						temp2 = l + s - (l * s);

					double temp1 = 2.0 * l - temp2;

					r = GetColorComponent(temp1, temp2, h + 1.0 / 3.0);
					g = GetColorComponent(temp1, temp2, h);
					b = GetColorComponent(temp1, temp2, h - 1.0 / 3.0);
				}
			}
			return Color.FromArgb(alpha, (int)(255 * r), (int)(255 * g), (int)(255 * b));

		}

		private static double GetColorComponent(double temp1, double temp2, double temp3)
		{
			if (temp3 < 0.0)
				temp3 += 1.0;
			else if (temp3 > 1.0)
				temp3 -= 1.0;

			if (temp3 < 1.0 / 6.0)
				return temp1 + (temp2 - temp1) * 6.0 * temp3;
			else if (temp3 < 0.5)
				return temp2;
			else if (temp3 < 2.0 / 3.0)
				return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
			else
				return temp1;
		}

		//thanks, sameboy
		private byte PreserveBrightness(int x, int old_max, int new_max, int old_min, int new_min)
		{
			int new_x = x;

			if (new_max != 0)
			{
				new_x = (byte)(new_x * old_max / new_max);
			}

			if (new_min != 0xff) // this won't break because only g can be above FF right now.
			{
				new_x = (byte)(0xff - (0xff - new_x) * (0xff - old_min) / (0xff - new_min));
			}

			return (byte)new_x;
		}
		private byte scale_channel_with_curve(byte x)
		{
			byte[] curve = {	0,
								2,
								4,
								7,
								12,
								18,
								25,
								34,
								42,
								52,
								62,
								73,
								85,
								97,
								109,
								121,
								134,
								146,
								158,
								170,
								182,
								193,
								203,
								213,
								221,
								230,
								237,
								243,
								248,
								251,
								253,
								255 };
			if (x > curve.Length - 1) { throw new System.ArgumentOutOfRangeException("x", x, "entry must be less than 32 (5-bit)"); }
			return curve[x];
		}

}
}
