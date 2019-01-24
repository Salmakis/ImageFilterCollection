
using FilterWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageFilter
{
	public partial class MainWindow
	{
		//check if the tilesize give into textbox is valid integer
		private void CheckTileSize(object sender, TextCompositionEventArgs e) {
			Regex regex = new Regex("[^0-9]+");
			if (regex.IsMatch(((TextBox)sender).Text)) {
				((TextBox)sender).Text = "";
			}
			if (regex.IsMatch(e.Text)) {
				e.Handled = true;
				return;
			}
		}

		private async Task ExecuteCurrentFilterTiled() {
			int tileSizeY;
			int tileSizeX;
			if (!int.TryParse(((TextBox)this.FindName("tileYbox")).Text, out tileSizeY) || !int.TryParse(((TextBox)this.FindName("tileXbox")).Text, out tileSizeX)) {
				MessageBox.Show("invalid tile dimensions");
				return;
			}
			if (tileSizeX == 0 || tileSizeX > this.inputImage.Width || tileSizeY == 0 || tileSizeY > this.inputImage.Height) {
				MessageBox.Show("Tiles should be at least 1 pixel (x & y) and not bigger than the input image");
				return;
			}

			converting = true;
			//maybe display its settings again to see the up to date settings
			DisplayCurrentFilterSettings();

			PushProgress("Creating target Image", 1);
			//just run the converter with the first tile, to see how big the output tiles are
			Task<FilterImage> testConvertTask = Task.Run(() => this.currentFilter.Execute(this.inputImage.GrabSubImage(0, 0, tileSizeX, tileSizeY)));
			FilterImage testOutputFilterImage = await testConvertTask;
			
			int scaleX = testOutputFilterImage.Width / tileSizeX;
			int scaleY = testOutputFilterImage.Height / tileSizeY;
			FilterImage outputImage = new FilterImage(this.inputImage.Width * scaleX, this.inputImage.Height * scaleY);
			
			int offsetX;
			int offsetY = 0;
			int tilesProcessed = 1;
			int tilesToProcess = (inputImage.Height / tileSizeY) * (inputImage.Width / tileSizeX);
			while (offsetY <= inputImage.Height - tileSizeY) {
				offsetX = 0;
				while (offsetX <= inputImage.Width - tileSizeX) {
					PushProgress($"converting tile: {tilesProcessed} / {tilesToProcess}", 2);
					Task <FilterImage> convertTileTask = Task.Run(() => this.currentFilter.Execute(this.inputImage.GrabSubImage(offsetX, offsetY, tileSizeX, tileSizeY)));
					FilterImage iOutputTile = await convertTileTask;
					outputImage.PutImage(offsetX * scaleX, offsetY * scaleY, iOutputTile);
					offsetX += tileSizeX;
					tilesProcessed++;
				}
				offsetY += tileSizeY;
			}

			//convert the result back to bitmap
			PushProgress("Fetching result", 3);
			var outputTask = Task.Run(() => FilterImageConvert.BitmapFromFilterImage(outputImage));
			var outputBitmap = await outputTask;

			//apply the output bitmap guiwise
			PushProgress("Rendering result", 3);
			SetOutputImage(outputBitmap);

			converting = false;

		}
	}
}
