using System;
using Microsoft.Band;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

#if DEBUG
using Console = System.Diagnostics.Debug;
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
            if (pairedBands == null || pairedBands.Length == 0)
                return;

            IBandInfo band = pairedBands[0];
            // TODO: Make sure band is available

            try
            {
                using IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(band);
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
                    Dispatcher.Dispatch(() =>
                    {
                        CounterLabel.Text = $"Current heart rate: {args.SensorReading.HeartRate}";

                        SemanticScreenReader.Announce(CounterLabel.Text);
                    });
                };

                // start the Heartrate sensor
                //await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                //await System.Threading.Tasks.Task.Delay(new TimeSpan(0, 0, 30));
                //await bandClient.SensorManager.HeartRate.StopReadingsAsync();
                //return;

                Guid smsTileId = Guid.Parse("b4edbc35-027b-4d10-a797-1099cd2ad98a");
                await bandClient.NotificationManager.SendMessageAsync(smsTileId,
                    "Test Message",
                    "This isn't a real message",
                    DateTimeOffset.Now,
                    Microsoft.Band.Notifications.MessageFlags.ShowDialog,
                    true);
                return;

                Guid tileId = Guid.Parse("870671A2-E128-4EB6-9F42-D75FA9290B40");
                Guid tilePageId = Guid.Parse("870671A2-E128-4EB6-9F42-D75FA9290B41");
                Microsoft.Band.Tiles.Pages.PageLayout tilePage = new(new Microsoft.Band.Tiles.Pages.FilledPanel(
                    new Microsoft.Band.Tiles.Pages.TextButton()
                    {
                        ElementId = 0x1
                    }
                ));
                var tile = new Microsoft.Band.Tiles.BandTile(tileId)
                {
                    Name = "Band Companion",
                    SmallIcon = new StreamImageSource
                    {
                        Stream = async (token) =>
                        {
                            return System.IO.File.OpenRead(@"D:\Pictures\24_testB.png");
                        }
                    }.BandIconFromMauiImage(),
                    TileIcon = new StreamImageSource
                    {
                        Stream = async (token) =>
                        {
                            return System.IO.File.OpenRead(@"D:\Pictures\48_testB.png");
                        }
                    }.BandIconFromMauiImage(),
                    PageLayouts =
                    {
                        tilePage
                    }
                };

                var tiles = await bandClient.TileManager.GetTilesAsync();
                await bandClient.TileManager.RemoveTileAsync(tileId);
                bool tileAdded = await bandClient.TileManager.AddTileAsync(tile);
                if (!tileAdded)
                {
                    CounterLabel.Text = "Failed to add tile.";
                    return;
                }
                CounterLabel.Text = "Added tile.";

                await bandClient.TileManager.SetPagesAsync(tileId, new Microsoft.Band.Tiles.Pages.PageData[]
                {
                    new Microsoft.Band.Tiles.Pages.PageData(tilePageId, 0, new Microsoft.Band.Tiles.Pages.TextButtonData(0x1, "Howdy from Band Center!"))
                });

                tiles = await bandClient.TileManager.GetTilesAsync();
            }
            catch (BandException ex)
            {
                // Handle a Band connection exception
                Console.WriteLine(ex);
            }
#endif
        }
    }
}
