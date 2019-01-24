using FilterWrapper;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageFilter
{
	public partial class MainWindow
	{

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		FilterManager filterManager = new FilterManager();
		IImageFilter currentFilter;

		FilterImage inputImage;
		bool converting;

		//perform the current selected filter with its current selected settings and input image
		private async Task ExecuteCurrentFilter()
		{
			if (null == inputImage)
			{
				//no image? dont do it silencly (no annoying message plz)
				return;
			}

			if (converting) {
				//only once
				return;
			}

			if ((((System.Windows.Controls.CheckBox)this.FindName("tileToggle")).IsChecked ?? false)) {
				await ExecuteCurrentFilterTiled();
				return;
			}
				
			converting = true;
			//maybe display its settings again to see the up to date settings
			DisplayCurrentFilterSettings();
			
			PushProgress("Executing the filter", 1);
			//convert bitmap to filterImage and perform the filter
			var convertTask = Task.Run(() => this.currentFilter.Execute(this.inputImage));
			var outputFilterImage = await convertTask;

			//convert the result back to bitmap
			PushProgress("Fetching result", 2);
			var outputTask = Task.Run(() => FilterImageConvert.BitmapFromFilterImage(outputFilterImage));
			var outputBitmap = await outputTask;

			//apply the output bitmap guiwise
			PushProgress("Rendering result", 3);
			SetOutputImage(outputBitmap);

			converting = false;
		}
		
		//put the given image into the right image view and set this output image for further work
		private void SetOutputImage(Bitmap outputBitmap)
		{
			var hBitmap = outputBitmap.GetHbitmap();
			var imgOut = ((System.Windows.Controls.Image)this.FindName("OutputImage"));

			if (imgOut.Source != null)
			{
				imgOut.Source = null;
			}

			try
			{
				var imageSource = Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromWidthAndHeight(outputBitmap.Width, outputBitmap.Height));

				imageSource.Freeze();
				imgOut.Source = imageSource;
			}
			finally
			{
				DeleteObject(hBitmap);

				imgOut.Width = outputBitmap.Width;
				imgOut.Height = outputBitmap.Height;

				var outputInfoLabel = ((System.Windows.Controls.Label)this.FindName("OutputInfoLabel"));
				outputInfoLabel.Content = $"x:{outputBitmap.Width} y:{outputBitmap.Height}";
			}
		}

		private ImageSource GetOutputImage() {
			var imgOut = ((System.Windows.Controls.Image)this.FindName("OutputImage"));
			return imgOut.Source;
			
		}

		//put the bitmap as input image and set it for next steps
		private void SetInputImage(Bitmap inputBitmap)
		{
			var hBitmap = inputBitmap.GetHbitmap();
			var imgIn = ((System.Windows.Controls.Image)this.FindName("InputImage"));

			if (imgIn.Source != null)
			{
				imgIn.Source = null;
			}

			try
			{
				var imageSource = Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromWidthAndHeight(inputBitmap.Width, inputBitmap.Height));

				imageSource.Freeze();
				imgIn.Source = imageSource;

			}
			finally
			{
				DeleteObject(hBitmap);

				imgIn.Width = inputBitmap.Width;
				imgIn.Height = inputBitmap.Height;

				var inputInfoLabel = ((System.Windows.Controls.Label)this.FindName("InputInfoLabel"));
				inputInfoLabel.Content = $"x:{inputBitmap.Width} y:{inputBitmap.Height}";
				inputImage = FilterImageConvert.FilterImageFromBitmap(inputBitmap);
			}
		}

		//change the current filter to the given new one
		private void ChangeCurrentFilter(IImageFilter newFilter)
		{
			ComboBox filterComboBox = ((System.Windows.Controls.ComboBox)this.FindName("FilterSelectComboBox"));
			var filter = filterManager.GetFilterByName((string)filterComboBox.SelectedItem);
			this.currentFilter = filter;

			DisplayCurrentFilterSettings();
		}

		//lock the window
		private void LockWindow() {
			System.Windows.Controls.Border blocker = ((System.Windows.Controls.Border)this.FindName("Blocker"));
			blocker.Visibility = Visibility.Visible;
		}

		//free up the window to be used again
		private void UnLockWindow() {
			System.Windows.Controls.Border blocker = ((System.Windows.Controls.Border)this.FindName("Blocker"));
			blocker.Visibility = Visibility.Hidden;
		}

		//adjust progress bar and infotext
		private void PushProgress(String text, int push) {
			System.Windows.Controls.Label label = ((System.Windows.Controls.Label)this.FindName("ProgressText"));
			System.Windows.Controls.ProgressBar bar = ((System.Windows.Controls.ProgressBar)this.FindName("ProgressBar"));
			bar.Value = push;
			label.Content = text;
		}

		//display filter settings in the UI
		private void DisplayCurrentFilterSettings()
		{
			//set the info label text
			var infoLabel = ((System.Windows.Controls.Label)this.FindName("FilterInfo"));
			infoLabel.Content = this.currentFilter?.Info ?? "no Filter Selected...";

			var settingsGrid = ((System.Windows.Controls.Grid)this.FindName("SettingsGrid"));
			//clear all row and setting-elementss if there are some
			settingsGrid.RowDefinitions.Clear();
			settingsGrid.Children.Clear();

			int rowCount = 0;

			if (null != currentFilter && currentFilter.Settings.Count() > 0)
			{
				foreach (Setting setting in currentFilter.Settings)
				{
					//add a row for this setting
					settingsGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

					//create label
					Label label = new Label();
					label.Content = setting.Name;
					label.ToolTip = setting.Info;
					label.SetValue(Grid.ColumnProperty, 0);
					label.SetValue(Grid.RowProperty, rowCount);
					settingsGrid.Children.Add(label);

					Control newSettingControl = null;

					if (setting.GetType() == typeof(SettingInteger))
					{
						label.Content = $"{setting.Name} : {((SettingInteger)setting).Value}";

						Slider slider = new Slider();
						slider.ToolTip = setting.Info;
						slider.Maximum = ((SettingInteger)setting).MaxValue;
						slider.Minimum = ((SettingInteger)setting).MinValue;
						slider.Value = ((SettingInteger)setting).Value;
						slider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;
						slider.AutoToolTipPlacement = System.Windows.Controls.Primitives.AutoToolTipPlacement.BottomRight;

						slider.ValueChanged += (object sender, RoutedPropertyChangedEventArgs<double> e) =>
						{
							((SettingInteger)setting).Value = (int)e.NewValue;
							label.Content = $"{setting.Name} : {((SettingInteger)setting).Value}";
							return;
						};
						newSettingControl = slider;
					}
					else if (setting.GetType() == typeof(SettingBoolean))
					{
						CheckBox checkBox = new CheckBox();
						checkBox.IsChecked = ((SettingBoolean)setting).Value;

						checkBox.Checked += (object sender, RoutedEventArgs e) =>
						{
							((SettingBoolean)setting).Value = true;
						};

						checkBox.Unchecked += (object sender, RoutedEventArgs e) =>
						{
							((SettingBoolean)setting).Value = false;
						};
						newSettingControl = checkBox;
					}else if(setting.GetType() == typeof(SettingOption))
					{
						ComboBox comboBox = new ComboBox();
						foreach (string option in ((SettingOption)setting).Options){
							comboBox.Items.Add(option);
						}

						comboBox.SelectedValue = ((SettingOption)setting).GetSelected();
						comboBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
						{
							string selectedFilterName = (string)((ComboBox)sender).SelectedItem;
							((SettingOption)setting).TrySetSelected(selectedFilterName);
						};

						newSettingControl = comboBox;
					}
					settingsGrid.Children.Add(newSettingControl);
					newSettingControl?.SetValue(Grid.ColumnProperty, 1);
					newSettingControl?.SetValue(Slider.IsSnapToTickEnabledProperty, true);
					newSettingControl?.SetValue(Slider.TickFrequencyProperty, 1.0);
					newSettingControl?.SetValue(Grid.RowProperty, rowCount);
					rowCount++;
				}
			}
		}
	}
}
