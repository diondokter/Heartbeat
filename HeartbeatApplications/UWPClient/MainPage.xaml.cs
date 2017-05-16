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
		private string ChosenUsername;

		public MainPage()
		{
			this.InitializeComponent();
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			StartDatePicker.Date = StartDatePicker.Date.AddDays(-7);
			OnViewYourselfClick(null, null);
		}

		private void LoadChartData(IEnumerable<UserData> Data)
		{
			((DataPointSeries)HeartbeatChart.Series[0]).ItemsSource = Data;

			DateTimeAxis XAxis = (DateTimeAxis)((LineSeries)HeartbeatChart.Series[0]).ActualIndependentAxis;

			if (XAxis != null)
			{
				XAxis.Interval = (XAxis.ActualMaximum.Value - XAxis.ActualMinimum.Value).TotalDays / 3;
				XAxis.IntervalType = DateTimeIntervalType.Days;
			}
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

		private void ChooseUser(string Username)
		{
			if (string.IsNullOrWhiteSpace(Username))
			{
				return;
			}

			ChosenUsername = Username;
			CurrentlyViewingDisplay.Text = $"Currently viewing user: {Username}";

			//Unfocus the searchbox by focussing on the page
			HeartbeatChart.Focus(FocusState.Programmatic);

			UpdateUserData();
		}

		private void OnViewYourselfClick(object sender, RoutedEventArgs e)
		{
			ChooseUserBox.Text = NetworkManager.CurrentUsername;
			ChooseUser(ChooseUserBox.Text);
		}

		private async void UpdateUserData()
		{
			if (string.IsNullOrWhiteSpace(ChosenUsername))
			{
				return;
			}

			UserData[] Data = await NetworkManager.GetUserData(ChosenUsername, StartDatePicker.Date.Date, EndDatePicker.Date.Date);

			LoadChartData(Data);
		}

		private void OnEndDatePickerChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			if (e.NewDate < StartDatePicker.Date)
			{
				EndDatePicker.Date = StartDatePicker.Date;
			}
		}

		private void OnStartDatePickerChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			if (e.NewDate > EndDatePicker.Date)
			{
				StartDatePicker.Date = EndDatePicker.Date;
			}
		}
	}
}
