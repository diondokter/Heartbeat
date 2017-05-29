using Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UWPClient
{
	public sealed partial class UserSettingsPage : Page
	{
		public UserSettingsPage()
		{
			this.InitializeComponent();

			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
			SystemNavigationManager.GetForCurrentView().BackRequested += OnUserSettingsPageBackRequested;

			Loaded += OnUserSettingsPageLoaded;
		}

		private async void OnUserSettingsPageLoaded(object sender, RoutedEventArgs e)
		{
			await UpdateList();
		}

		public async Task UpdateList()
		{
			Usernames.Source = (await NetworkManager.GetViewingUsers()).GroupBy(x => char.ToUpper(x.First())).OrderBy(x => x.Key);
		}

		private void OnUserSettingsPageBackRequested(object sender, BackRequestedEventArgs e)
		{
			if (!e.Handled)
			{
				SystemNavigationManager.GetForCurrentView().BackRequested -= OnUserSettingsPageBackRequested;
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
				e.Handled = true;
				App.RootFrame.GoBack();
			}
		}

		private async void OnAddUserButtonClick(object sender, RoutedEventArgs e)
		{
			AddUserDialog Dialog = new AddUserDialog();
			await Dialog.ShowAsync();

			if (!Dialog.Cancelled)
			{
				string FailReason = await NetworkManager.AddUserViewPermission(Dialog.Username);

				if (FailReason != null)
				{
					MessageDialog Message = new MessageDialog(FailReason, "Error");
					await Message.ShowAsync();
				}
				else
				{
					await UpdateList();
				}
			}
		}

		private async void OnRemoveUserButtonClick(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < UsernameList.SelectedItems.Count; i++)
			{
				NetworkManager.RemoveUserViewPermission((string)UsernameList.SelectedItems[i]);
			}

			await Task.Delay(100); // Wait for the server to process the info

			await UpdateList();
		}

		private void OnUsernameListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemoveButton.IsEnabled = UsernameList.SelectedItems.Any();
		}
	}
}
