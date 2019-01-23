using System.Drawing;
using System.Drawing.Imaging;
using FilterWrapper;

namespace ImageFilter
{
	public static partial class FilterImageConvert
	{

		public static FilterImage FilterImageFromBitmap(Bitmap bitmap)
		{
			FilterImage outputImage = new FilterImage(bitmap.Width, bitmap.Height);

			var bitmapData = bitmap.LockBits(
			  new Rectangle(0, 0, outputImage.Width, outputImage.Height),
			  ImageLockMode.ReadOnly,
			  PixelFormat.Format32bppArgb
			);
			var stride = bitmapData.Stride - bitmapData.Width * 4;

			int x, y;

			unsafe
			{
				var offset = (byte*)bitmapData.Scan0.ToPointer();
				for (y = 0; y < outputImage.Height; y++)
				{
					for (x = 0; x < outputImage.Width; x++)
					{
						outputImage.SetPixel(x, y,
							*(offset + 2),
							*(offset + 1),
							*(offset + 0),
							*(offset + 3)
						);
						
						offset += 4;
					}
					offset += stride;
				}
			}
			bitmap.UnlockBits(bitmapData);
			return (outputImage);
		}

		public static Bitmap BitmapFromFilterImage(FilterImage image)
		{	
			Bitmap outputBitmap = new Bitmap(image.Width, image.Height);

			var bitmapData = outputBitmap.LockBits(
			  new Rectangle(0, 0, image.Width, image.Height),
			  ImageLockMode.WriteOnly,
			  PixelFormat.Format32bppArgb
			);
			
			var fillBytes = bitmapData.Stride - bitmapData.Width * 4;
			unsafe
			{
				int x, y;
				var offset = (byte*)bitmapData.Scan0.ToPointer();
				for (y = 0; y < image.Height; y++)
				{
					for (x = 0; x < image.Width; x++)
					{
						*(offset + 3) = image.Alpha(x, y);
						*(offset + 2) = image.Red(x, y);
						*(offset + 1) = image.Green(x, y);
						*(offset + 0) = image.Blue(x, y);
						offset += 4;
					}
					offset += fillBytes;
				}
			}
			outputBitmap.UnlockBits(bitmapData);
			return (outputBitmap);
		}
	}
}