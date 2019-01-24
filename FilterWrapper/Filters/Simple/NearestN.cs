using System.Collections.Generic;

namespace FilterWrapper.Filters.Simple
{
    class NearestN : IImageFilter
	{
		List<Setting> settings = new List<Setting>();

		SettingInteger scale = new SettingInteger("Scale", "Factor to scale", 2, 8, 2);

		public NearestN() {
			settings.Add(scale);
		}

		public IEnumerable<Setting> Settings
		{
			get {
				return settings;
			}
		}

		public string Info
		{
			get {
				return "Simple NearestNeighbor";
			}
		}

		public string Name
		{
			get {
				return "NearestNeighbor";
			}
		}

		public FilterImage Execute(FilterImage input) {
			FilterImage newImage = new FilterImage(input.Width * scale.Value, input.Height * scale.Value);
			for (int x = 0; x < input.Width; x++) {
				for (int y = 0; y < input.Height; y++) {
					int argb = input.PixelArgb(x, y);
					for (int ix = 0; ix < scale.Value; ix++) {
						for (int iy = 0; iy < scale.Value; iy++) {
							newImage.SetPixelArgb(ix + x * scale.Value, iy + y * scale.Value, argb);
						}
					}
				}
			}
			return newImage;
		}
	}
}
