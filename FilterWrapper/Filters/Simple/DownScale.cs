using Imager;
using System.Collections.Generic;

namespace FilterWrapper.Filters.Simple
{
	class DownScale: IImageFilter
	{
		List<Setting> settings = new List<Setting>();

		SettingInteger scale = new SettingInteger("Scale Down", "Factor to scale down", 2, 3, 2);
		SettingBoolean colorBlending = new SettingBoolean("Color Blending", "Disable color blending if image uses a palette.");

		public DownScale()
		{
			settings.Add(scale);
			settings.Add(colorBlending);
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
				return "Simple Downscaler";
			}
		}

		public string Name
		{
			get
			{
				return "Downscale";
			}
		}

		public FilterImage Execute(FilterImage input)
		{
			int newWidth = (int)((double)input.Width * (1.0 / scale.Value));
			int newHeight = (int)((double)input.Height * (1.0 / scale.Value));
			FilterImage newImage = new FilterImage(newWidth, newHeight);
			for (int x = 0; x < newImage.Width; x++)
			{
				for (int y = 0; y < newImage.Height; y++)
				{
					Dictionary<int, int> argbCount = new Dictionary<int, int>();
					// test imp 1, most common wins.
					FilterImage chunk = input.GrabSubImage(x * scale.Value, y * scale.Value, scale.Value, scale.Value);
					for (int cx = 0; cx < chunk.Width; cx++)
					{
						for (int cy = 0; cy < chunk.Height; cy++)
						{
							int pixel = chunk.PixelArgb(cx, cy);
							if (!argbCount.ContainsKey(pixel))
							{
								argbCount.Add(pixel, 1);
							}
							else
							{
								argbCount[pixel] = argbCount[pixel] + 1;
							}
						}
					}

					int finalPixel;
					if (argbCount.Count >= scale.Value * 2 - 1 && colorBlending.Value == true)
					{
						finalPixel = PixelMix(argbCount);
					}
					else
					{
						finalPixel = LeastCommonValue(argbCount);
					}
					newImage.SetPixelArgb(x, y, finalPixel);
				}
			}
			return newImage;
		}

		private int LeastCommonValue(Dictionary<int, int> argbs)
		{
			int _argb = 0;
			int count = int.MaxValue;
			foreach (KeyValuePair<int, int> entry in argbs)
			{
				if (entry.Value < count)
				{
					_argb = entry.Key;
					count = entry.Value;
				}
			}

			return _argb;
		}
		private int MostCommonValue(Dictionary<int, int> argbs)
		{
			int _argb = 0;
			int count = -1;
			foreach(KeyValuePair<int, int> entry in argbs)
			{
				if (entry.Value > count)
				{
					_argb = entry.Key;
					count = entry.Value;
				}
			}

			return _argb;
		}

		private int PixelMix(Dictionary<int, int> argbs)
		{
			if (argbs.Count == 0) { throw new System.Exception(); }
			int _argb = 0;
			sPixel[] pix = new sPixel[argbs.Count];
			int index = 0;
			foreach (KeyValuePair<int, int> entry in argbs)
			{
				int myColor = entry.Key;
				byte b = (byte)(myColor & 0xFF);
				byte g = (byte)((myColor >> 8) & 0xFF);
				byte r = (byte)((myColor >> 16) & 0xFF);
				byte a = (byte)((myColor >> 24) & 0xFF);

				pix[index] = sPixel.FromRGBA(r, g, b, a);
				index++;
			}

			sPixel finalPix = alphaWeightedAverage(pix);

			return finalPix.Color.ToArgb();
		}

		public static sPixel alphaWeightedAverage(sPixel[] pixs)
		{
			Dictionary<sPixel, double> alphaScaledPixs = new Dictionary<sPixel, double>(pixs.Length);
			double total = 0.0;

			double a = 0.0;
			double r = 0.0;
			double g = 0.0;
			double b = 0.0;


			foreach(sPixel pix in pixs)
			{
				double alphaPower = pix.Alpha / (255.0 * 1.25); // we want non-transparent pixels to count more towards the overall appearance.
				//double alphaPower = (pix.Alpha / 255.0) * (pix.Alpha / 255.0); //also an option.
				if (alphaScaledPixs.ContainsKey(pix))
				{
					alphaScaledPixs[pix] += alphaPower;
				}
				else
				{
					alphaScaledPixs.Add(pix, alphaPower);
				}
				total += alphaPower;
			}


			if (total == 0.0)
			{
				total = pixs.Length;
				foreach(KeyValuePair<sPixel, double> alphaScaledPix in alphaScaledPixs)
				{
					a += alphaScaledPix.Key.Alpha * 1.0;
					r += alphaScaledPix.Key.Red * 1.0;
					g += alphaScaledPix.Key.Green * 1.0;
					b += alphaScaledPix.Key.Blue * 1.0;
				}
			}
			else
			{
				foreach (KeyValuePair<sPixel, double> alphaScaledPix in alphaScaledPixs)
				{
					a += alphaScaledPix.Key.Alpha * alphaScaledPix.Value;
					r += alphaScaledPix.Key.Red * alphaScaledPix.Value;
					g += alphaScaledPix.Key.Green * alphaScaledPix.Value;
					b += alphaScaledPix.Key.Blue * alphaScaledPix.Value;
				}
			}
			return (new sPixel(
				(byte)((r) / total),
				(byte)((g) / total),
				(byte)((b) / total),
				(byte)((a) / total)
				));

		}
	}
}