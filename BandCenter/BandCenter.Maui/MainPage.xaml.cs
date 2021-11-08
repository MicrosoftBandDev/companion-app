using System;
using Microsoft.Band;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace BandCenter.Maui
{
	public partial class MainPage : ContentPage
	{
		int count = 0;

		public MainPage()
		{
			InitializeComponent();
		}

		private async void OnCounterClicked(object sender, EventArgs e)
		{
#if WINDOWS
			IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
			count = pairedBands.Length;
#else
			count++;
#endif
			CounterLabel.Text = $"Current count: {count}";

			SemanticScreenReader.Announce(CounterLabel.Text);
		}
	}
}
