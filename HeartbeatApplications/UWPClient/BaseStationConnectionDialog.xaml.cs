using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace UWPClient
{
	public sealed partial class BaseStationConnectionDialog : ContentDialog
	{
		public BaseStationConnection Connection;
		private DeviceInformationCollection Devices;

		public BaseStationConnectionDialog()
		{
			this.InitializeComponent();
			this.Loaded += OnBaseStationConnectionDialogLoaded;
		}

		private async void OnBaseStationConnectionDialogLoaded(object sender, RoutedEventArgs e)
		{
			Devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
			DevicesListView.ItemsSource = Devices.Select(x => x.Name);
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Connection = new BaseStationConnection(Devices[DevicesListView.SelectedIndex].Id);
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{

		}

		private void DevicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			IsPrimaryButtonEnabled = DevicesListView.SelectedIndex > -1;
		}
	}
}
