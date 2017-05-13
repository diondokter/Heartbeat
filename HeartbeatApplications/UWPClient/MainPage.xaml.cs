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
		public class HeartbeatData
		{
			public float Value { get; set; }
			public DateTime Time { get; set; }
		}

		public MainPage()
		{
			this.InitializeComponent();
			this.Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Random Rand = new Random();
			IEnumerable<HeartbeatData> Data = Enumerable.Range(0, 100).Select(x => new HeartbeatData() { Time = new DateTime(2017,1,1) + new TimeSpan(x / 24, x % 24,0,0,0), Value = (float)Rand.NextDouble() * 100 });

			LoadChartData(Data);
		}

		private void LoadChartData(IEnumerable<HeartbeatData> Data)
		{
			((DataPointSeries)HeartbeatChart.Series[0]).ItemsSource = Data;

			DateTimeAxis XAxis = (DateTimeAxis)((LineSeries)HeartbeatChart.Series[0]).ActualIndependentAxis;
			XAxis.Interval = (XAxis.ActualMaximum.Value - XAxis.ActualMinimum.Value).TotalDays / 3;
			XAxis.IntervalType = DateTimeIntervalType.Days;
		}
	}
}
