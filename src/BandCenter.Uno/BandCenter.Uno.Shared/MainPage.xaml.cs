using BandCenter.Uno.Controls;
using Microsoft.Band;
using Microsoft.Band.Admin.Phone;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BandCenter.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Loaded += MainPage_Loaded;
            this.InitializeComponent();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
            if (pairedBands == null || pairedBands.Length == 0)
                return;

            IBandInfo band = pairedBands[0];
            // TODO: Make sure band is available

            try
            {
                using var bandClient = await BandAdminClientManager.Instance.ConnectAsync(band);
                var version = await bandClient.GetFirmwareVersionAsync();

                var lastRun = await bandClient.GetLastRunStatisticsAsync();
                RunTile.Subtitle = $"{lastRun.EndTime:ddd M/dd}";
                RunTile.MetricMarkup = $"{lastRun.Distance / (1609.344*100):##0.0}<s>mi</s>";

                var lastSleep = await bandClient.GetLastSleepStatisticsAsync();
                var timeAsleep = TimeSpan.FromMilliseconds(lastSleep.TimeAsleep);
                SleepTile.Subtitle = $"{lastSleep.EndTime:ddd M/dd}, Actual sleep";
                SleepTile.MetricMarkup = $"{timeAsleep.Hours}<s>h</s> {timeAsleep.Minutes}<s>m</s>";

                var lastWorkout = await bandClient.GetLastWorkoutStatisticsAsync();
                WorkoutTile.Subtitle = $"{lastWorkout.EndTime:ddd M/dd}, Calories burned";
                WorkoutTile.MetricMarkup = $"{lastWorkout.Calories:N0}<s>cals</s>";

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
                    _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        HeartRateTile.MetricMarkup = $"{args.SensorReading.HeartRate} <s>bpm</s>";
                    });
                };

                // start the Heartrate sensor
                await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                await System.Threading.Tasks.Task.Delay(new TimeSpan(0, 0, 30));
                await bandClient.SensorManager.HeartRate.StopReadingsAsync();
                return;

                var strip = await bandClient.GetStartStripAsync();
                var defaultTiles = bandClient.GetDefaultTiles();
                foreach (var tile in defaultTiles)
                {
                    if (strip.Contains(tile.TileId))
                        continue;
                    strip.Add(tile);
                }
                await bandClient.SetStartStripAsync(strip);
                return;

                await bandClient.SendSmsNotificationAsync(10,
                    "Test Message",
                    "Howdy from the Admin SDK!",
                    DateTime.Now);
                return;
            }
            catch (BandAccessDeniedException ex)
            {
                // Handle a Band connection exception
                System.Diagnostics.Debug.WriteLine("Missing Bluetooth access");
            }
            catch (BandException ex)
            {
                // Handle a Band connection exception
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
