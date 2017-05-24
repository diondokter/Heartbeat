using Client;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace UWPClient
{
	public sealed partial class MainPage : Page
	{
		private static string ChosenUsername;
		private static DateTimeOffset StartDate;
		private static DateTimeOffset EndDate;
		private static IEnumerable<UserData> Data;

		public MainPage()
		{
			this.InitializeComponent();
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (ChosenUsername == null)
			{
				StartDate = DateTime.Today.AddDays(-7);
				EndDate = DateTime.Today;

				StartDatePicker.Date = StartDate;
				EndDatePicker.Date = EndDate;
				OnViewYourselfClick(null, null);
			}
			else
			{
				StartDatePicker.Date = StartDate;
				EndDatePicker.Date = EndDate;
				ChooseUser(ChosenUsername, false);
				LoadData();
			}
		}

		private void LoadData()
		{
			if (Data.Count() == 0)
			{
				Data = null;
			}

			((DataPointSeries)HeartbeatChart.Series[0]).ItemsSource = Data;

			if (Data != null)
			{
				DateTimeAxis XAxis = (DateTimeAxis)((LineSeries)HeartbeatChart.Series[0]).ActualIndependentAxis;

				XAxis.Minimum = null;
				XAxis.Maximum = null;

				XAxis.Minimum = StartDate.DateTime;
				XAxis.Maximum = EndDate.DateTime;
				XAxis.IntervalType = DateTimeIntervalType.Auto;
			}

			CurrentlyViewingDisplay.Text = $"Currently viewing user: {ChosenUsername}";
		}

		private async void SearchUserTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			sender.ItemsSource = await NetworkManager.GetViewableUsers(10, sender.Text);
		}

		private void SearchUserQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (args.ChosenSuggestion != null)
			{
				sender.Text = args.ChosenSuggestion.ToString();
			}
			else if (args.QueryText != null)
			{
				string ClosestName = ((string[])sender.ItemsSource).FirstOrDefault();
				sender.Text = ClosestName ?? "";
			}

			ChooseUser(sender.Text);
		}

		private void ChooseUser(string Username, bool Update = true)
		{
			if (string.IsNullOrWhiteSpace(Username))
			{
				return;
			}

			ChosenUsername = Username;
			ChooseUserBox.Text = ChosenUsername;

			//Unfocus the searchbox by focussing on the page
			HeartbeatChart.Focus(FocusState.Programmatic);

			if (Update)
			{
				UpdateData();
			}
		}

		private void OnViewYourselfClick(object sender, RoutedEventArgs e)
		{
			ChooseUserBox.Text = NetworkManager.CurrentUsername;
			ChooseUser(ChooseUserBox.Text);
		}

		private async void UpdateData()
		{
			if (string.IsNullOrWhiteSpace(ChosenUsername))
			{
				return;
			}

			Data = await NetworkManager.GetUserData(ChosenUsername, StartDatePicker.Date.Date, EndDatePicker.Date.Date);
			LoadData();
		}

		private void OnEndDatePickerChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			if (e.NewDate < StartDatePicker.Date)
			{
				EndDatePicker.Date = StartDatePicker.Date;
			}

			EndDate = EndDate.Date;
		}

		private void OnStartDatePickerChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			if (e.NewDate > EndDatePicker.Date)
			{
				StartDatePicker.Date = EndDatePicker.Date;
			}

			StartDate = StartDatePicker.Date;
		}

		private void OnUpdateDataClick(object sender, RoutedEventArgs e)
		{
			UpdateData();
		}

		private void OnUserSettingsButtonClick(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(typeof(UserSettingsPage));
		}
	}
}
