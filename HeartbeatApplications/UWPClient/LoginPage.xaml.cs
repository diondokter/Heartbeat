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
using Windows.UI.Popups;
using System.Threading.Tasks;
using Messages;
using System.Diagnostics;
using Protocol;
using Client;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPClient
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			this.InitializeComponent();
		}

		private string Username;
		private string Password;

		private void OnUsernameChanged(object sender, TextChangedEventArgs e)
		{
			Username = ((TextBox)sender).Text;
		}

		private void OnPasswordChanged(object sender, RoutedEventArgs e)
		{
			Password = ((PasswordBox)sender).Password;
		}

		private async void OnLoginClick(object sender, RoutedEventArgs e)
		{
			if (await CheckUsername() && await CheckPassword())
			{
				(bool Accepted, string Reason) = await NetworkManager.Login(Username, Password);

				if (!Accepted || Reason != null)
				{
					MessageDialog LoginDialog = new MessageDialog(Reason, Accepted ? "Successful" : "Failed");
					await LoginDialog.ShowAsync();
				}

				if (Accepted)
				{
					LoadMainPage();
				}
			}
		}

		private async void OnCreateAccountClick(object sender, RoutedEventArgs e)
		{
			if (await CheckUsername() && await CheckPassword())
			{
				(bool Accepted, string Reason) = await NetworkManager.CreateAccount(Username, Password);

				if (!Accepted || Reason != null)
				{
					MessageDialog LoginDialog = new MessageDialog(Reason, Accepted ? "Successful" : "Failed");
					await LoginDialog.ShowAsync();
				}

				if (Accepted)
				{
					LoadMainPage();
				}
			}
		}

		/// <summary>
		/// Checks if the username fits the specifications
		/// </summary>
		private async Task<bool> CheckUsername()
		{
			if (string.IsNullOrWhiteSpace(Username))
			{
				MessageDialog UsernameWarningDialog = new MessageDialog("The username field has not been filled in.", "Warning");
				await UsernameWarningDialog.ShowAsync();
				return false;
			}

			return true;
		}

		private async Task<bool> CheckPassword()
		{
			string WarningText = null;

			if (string.IsNullOrWhiteSpace(Password))
			{
				WarningText = "The password field has not been filled in.";
			}
			else if (Password.Length < 8)
			{
				WarningText = "Your password is too short. It must be at least 8 characters long.";
			}
			else if (!Password.Any(x => char.IsLower(x)))
			{
				WarningText = "Your password does not contain any lower case letters.";
			}
			else if (!Password.Any(x => char.IsUpper(x)))
			{
				WarningText = "Your password does not contain any upper case letters.";
			}
			else if (!Password.Any(x => char.IsNumber(x)))
			{
				WarningText = "Your password does not contain any numbers.";
			}

			if (WarningText != null)
			{
				MessageDialog PasswordWarningDialog = new MessageDialog(WarningText, "Warning");
				await PasswordWarningDialog.ShowAsync();
				return false;
			}

			return true;
		}

		private void LoadMainPage()
		{
			App.RootFrame.Navigate(typeof(MainPage));
		}
	}
}
