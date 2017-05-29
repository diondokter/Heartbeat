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

namespace UWPClient
{
	public sealed partial class AddUserDialog : ContentDialog
	{
		public string Username;
		public bool Cancelled;

		public AddUserDialog()
		{
			this.InitializeComponent();

		}

		private void OnAddButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{

		}

		private void OnCancelButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Cancelled = true;
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			Username = ((TextBox)sender).Text;
		}

		private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				Hide();
			}
			else if(e.Key == Windows.System.VirtualKey.Escape)
			{
				Cancelled = true;
				Hide();
			}
		}
	}
}
