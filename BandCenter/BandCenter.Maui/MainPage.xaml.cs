using System;
using Microsoft.Band;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

#if DEBUG
using System.Diagnostics;
#endif

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

			try
			{
                using IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);
				var version = await bandClient.GetFirmwareVersionAsync();

				// check current user heart rate consent
				if (bandClient.SensorManager.HeartRate.GetCurrentUserConsent() != UserConsent.Granted)
				{
					// user hasn’t consented, request consent
					await bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
				}

				// hook up to the Heartrate sensor ReadingChanged event
				bandClient.SensorManager.HeartRate.ReadingChanged += (sender, args) =>
				{
					// do work when the reading changes (i.e., update a UI element)
					count = args.SensorReading.HeartRate;
				};

				// start the Heartrate sensor
				await bandClient.SensorManager.HeartRate.StartReadingsAsync();
				await System.Threading.Tasks.Task.CompletedTask.WaitAsync(new TimeSpan(0, 0, 10));
				await bandClient.SensorManager.HeartRate.StopReadingsAsync();
			}
			catch (BandException ex)
			{
				// Handle a Band connection exception
#if DEBUG
				Debug.WriteLine(ex);
#endif
			}
#else
			count++;
#endif
			CounterLabel.Text = $"Current count: {count}";

			SemanticScreenReader.Announce(CounterLabel.Text);
		}
	}
}
