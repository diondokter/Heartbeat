using Client;
using Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
		private static DateTime StartDate;
		private static DateTime EndDate;
		private static UserData[] Data;
		private static BaseStationConnection SerialConnection;
		private static UserData? LastUserData;

		private DispatcherTimer Timer;

		public MainPage()
		{
			this.InitializeComponent();
			this.Loaded += OnLoaded;
			this.Unloaded += (sender, e) => Timer.Stop();
			this.SizeChanged += (sender, e) => LoadData();
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (ChosenUsername == null)
			{
				StartDate = DateTime.Today.AddDays(-7);
				EndDate = DateTime.Today.AddDays(1);

				StartDatePicker.Date = StartDate;
				EndDatePicker.Date = DateTime.Today;
				OnViewYourselfClick(null, null);
			}
			else
			{
				StartDatePicker.Date = StartDate;
				EndDatePicker.Date = EndDate.AddDays(-1);
				await ChooseUser(ChosenUsername, false);
				LoadData();
			}

			ConnectToBaseStationButton.Content = SerialConnection == null ? "Connect to Base Station" : "Disconnect from Base Station";

			if (Timer == null)
			{
				Timer = new DispatcherTimer();
				Timer.Interval = TimeSpan.FromSeconds(1);
				Timer.Tick += OnTimerTick;
			}

			Timer.Start();
		}

		private void LoadData()
		{
			if ((Data?.Count() ?? 0) == 0)
			{
				Data = null;
			}

			((DataPointSeries)HeartbeatChart.Series[0]).ItemsSource = Data;

			if (Data != null)
			{
				DateTimeAxis XAxis = (DateTimeAxis)((LineSeries)HeartbeatChart.Series[0]).ActualIndependentAxis;
				XAxis.ShowGridLines = true;

				XAxis.Minimum = StartDate;
				XAxis.Maximum = EndDate;

				TimeSpan Span = EndDate - StartDate;

				if (Span.TotalHours <= 48)
				{
					XAxis.IntervalType = DateTimeIntervalType.Hours;
					XAxis.Interval = Math.Round((EndDate - StartDate).TotalHours / HeartbeatChart.ActualWidth * 100);
				}
				else
				{
					XAxis.IntervalType = DateTimeIntervalType.Days;
					XAxis.Interval = Math.Max(1, Math.Round((EndDate - StartDate).TotalDays / HeartbeatChart.ActualWidth * 100));
				}

				UpdateText();
			}
		}

		private void UpdateText()
		{
			if (Data != null && Data.Where(x => x.Value > 0).Any())
			{
				CurrentlyViewingDisplay.Text = $"Currently viewing user: {ChosenUsername}\nHighest BPM value: {Data?.Max(x => x.Value) ?? float.NaN}\nLowest BPM Value: {Data?.Where(x => x.Value > 0).Min(x => x.Value) ?? float.NaN}\nAverage BPM Value: {Data?.Where(x => x.Value > 0).Average(x => x.Value) ?? float.NaN}";
			}
			else
			{
				CurrentlyViewingDisplay.Text = "";
			}

			if (LastUserData != null)
			{
				LiveBPMDisplay.Text = LastUserData.Value.Value.ToString("##0.0");
			}
			else
			{
				LiveBPMDisplay.Text = "Not Connected";
			}
		}

		private async void SearchUserTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			sender.ItemsSource = await NetworkManager.GetViewableUsers(10, sender.Text);
		}

		private async void SearchUserQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
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

			await ChooseUser(sender.Text);
		}

		private async Task ChooseUser(string Username, bool Update = true)
		{
			if (string.IsNullOrWhiteSpace(Username))
			{
				return;
			}

			ChosenUsername = Username;
			ChooseUserBox.Text = ChosenUsername;

			// We are not viewing ourselves, so disable the ability to add data.
			if (Username != NetworkManager.CurrentUsername)
			{
				ConnectToBaseStationButton.IsEnabled = false;

				if (SerialConnection != null)
				{
					OnConnectToBaseStationButtonClick(null, null);
				}
			}
			else
			{
				ConnectToBaseStationButton.IsEnabled = true;
			}

			//Unfocus the searchbox by focussing on the page
			HeartbeatChart.Focus(FocusState.Programmatic);

			if (Update)
			{
				await UpdateData();
			}
		}

		private async void OnViewYourselfClick(object sender, RoutedEventArgs e)
		{
			ChooseUserBox.Text = NetworkManager.CurrentUsername;
			await ChooseUser(ChooseUserBox.Text);
		}

		private async Task UpdateData()
		{
			if (string.IsNullOrWhiteSpace(ChosenUsername))
			{
				return;
			}

			Data = await NetworkManager.GetUserData(ChosenUsername, StartDate, EndDate);

			LoadData();
		}

		private async void OnEndDatePickerChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			if (e.NewDate < StartDatePicker.Date)
			{
				EndDatePicker.Date = StartDatePicker.Date;
			}

			EndDate = EndDatePicker.Date.AddDays(1).DateTime;
			await UpdateData();
		}

		private async void OnStartDatePickerChanged(object sender, DatePickerValueChangedEventArgs e)
		{
			if (e.NewDate > EndDatePicker.Date)
			{
				StartDatePicker.Date = EndDatePicker.Date;
			}

			StartDate = StartDatePicker.Date.DateTime;
			await UpdateData();
		}

		private async void OnUpdateDataClick(object sender, RoutedEventArgs e)
		{
			await UpdateData();
		}

		private void OnUserSettingsButtonClick(object sender, RoutedEventArgs e)
		{
			App.RootFrame.Navigate(typeof(UserSettingsPage));
		}

		private async void OnConnectToBaseStationButtonClick(object sender, RoutedEventArgs e)
		{
			if (SerialConnection == null)
			{
				BaseStationConnectionDialog Dialog = new BaseStationConnectionDialog();
				if (await Dialog.ShowAsync() == ContentDialogResult.Primary)
				{
					await Dialog.Connection.Initialize();
					SerialConnection = Dialog.Connection;
				}
			}
			else
			{
				SerialConnection.Dispose();
				SerialConnection = null;
				LastUserData = null;
				UpdateText();
			}

			ConnectToBaseStationButton.Content = SerialConnection == null ? "Connect to Base Station" : "Disconnect from Base Station";

		}

		private void OnTimerTick(object sender, object e)
		{
			if (!(SerialConnection?.Disposed ?? true))
			{
				while (SerialConnection.GetEntryCount() > 0)
				{
					(float TimeStamp, float BMPValue) Entry = SerialConnection.GetFirstEntry();
					float CurrentTime = SerialConnection.GetCurrentTime();

					DateTime Time = DateTime.Now - TimeSpan.FromSeconds(CurrentTime - Entry.TimeStamp);

					NetworkManager.AddUserData(Time, Entry.BMPValue);
					LastUserData = new UserData() { Username = NetworkManager.CurrentUsername, Time = Time, Value = Entry.BMPValue };
				}

				UpdateText();
			}
		}
	}
}
