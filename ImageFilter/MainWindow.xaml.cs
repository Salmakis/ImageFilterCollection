using FilterWrapper;
using Imager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using System.IO;
using System.Drawing.Imaging;

namespace ImageFilter
{
	public partial class MainWindow : Window
	{
		public MainWindow() {
			InitializeComponent();
		}

		private void ToggleTilemode(object sender, RoutedEventArgs e) {
			CheckBox tileModeBox = (System.Windows.Controls.CheckBox)sender;
			((System.Windows.Controls.TextBox)this.FindName("tileXbox")).IsEnabled = tileModeBox.IsChecked ?? false;
			((System.Windows.Controls.TextBox)this.FindName("tileYbox")).IsEnabled = tileModeBox.IsChecked ?? false;
		}

		private void LoadFromFile(object sender, RoutedEventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "PNG files (*.png)|*.png";
			if (dialog.ShowDialog(this) == true) {
				var loadedBitmap = (Bitmap)System.Drawing.Image.FromFile(dialog.FileName);
				SetInputImage(loadedBitmap);
			}
		}

		private void LoadFromClipboard(object sender, RoutedEventArgs e) {
			if (Clipboard.ContainsImage()) {
				System.Drawing.Image img = ClipboardHelper.GetImageFromClipboard();
				if (null != img) {
					SetInputImage(new Bitmap(img));
				}
			}
		}

		private void SaveToClipboard(object sender, RoutedEventArgs e) {
			BitmapSource outImg = (BitmapSource)GetOutputImage();
			Bitmap bmp = ClipboardHelper.GetBitmap((BitmapSource)GetOutputImage());
			ClipboardHelper.SetClipboardImage(bmp, null, null);
		}
		
		private void SaveToFile(object sender, RoutedEventArgs e) {
			BitmapSource outImg = (BitmapSource)GetOutputImage();
			if (null != outImg) {
				SaveFileDialog dialog = new SaveFileDialog();
				dialog.Filter = "PNG files (*.png)|*.png";
				if (dialog.ShowDialog(this) == true) {
					using (var fileStream = new FileStream(dialog.FileName, FileMode.Create)) {
						BitmapEncoder encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create((BitmapSource)GetOutputImage()));
						encoder.Save(fileStream);
					}
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			ComboBox filterComboBox = ((System.Windows.Controls.ComboBox)this.FindName("FilterSelectComboBox"));

			foreach (var item in filterManager.FoundFilters) {
				filterComboBox.Items.Add(item.Name);
			}			
			filterComboBox.SelectedIndex = 0;
		}

		private void FilterSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ComboBox filterComboBox = (System.Windows.Controls.ComboBox)sender;
			string selectedFilterName = (string)filterComboBox.SelectedItem;
			var filter = filterManager.GetFilterByName(selectedFilterName);
			if (null != filter) {
				ChangeCurrentFilter(filter);
			}
		}

		private async void StartConvert(object sender, RoutedEventArgs e) {
			LockWindow();
			await ExecuteCurrentFilter();
			UnLockWindow();
		}
	}
}
