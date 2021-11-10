// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandClientManager
// Assembly: Microsoft.Band.Windows, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 28C7C615-6BA3-4124-96FB-B8960DA222E2
// Assembly location: .\netcore451\Microsoft.Band.Windows.dll

using Microsoft.Band.Windows;
using System;
using System.Threading.Tasks;

namespace Microsoft.Band
{
    public class BandClientManager : IBandClientManager
    {
        private static readonly BandClientManager instance = new();
        private const int MaximumBluetoothConnectAttempts = 3;

        private BandClientManager()
        {
        }

        public static IBandClientManager Instance => instance;

        public Task<IBandInfo[]> GetBandsAsync() => GetBandsAsync(false);

        public Task<IBandInfo[]> GetBandsAsync(bool isBackground)
        {
            Guid serviceGuid = new(isBackground ? "{A502CA9B-2BA5-413C-A4E0-13804E47B38F}" : "{A502CA9A-2BA5-413C-A4E0-13804E47B38F}");
            return Task.Run(() => (IBandInfo[])BluetoothTransport.GetConnectedDevices(serviceGuid, new LoggerProviderStub()));
        }

        public async Task<IBandClient> ConnectAsync(IBandInfo bandInfo)
        {
            if (bandInfo == null)
                throw new ArgumentNullException(nameof(bandInfo));
            if (bandInfo is not BluetoothDeviceInfo bluetoothDeviceInfo)
                throw new ArgumentException(StoreResources.DeviceInfoNotBluetooth);
            BluetoothTransport deviceTransport = null;
            PushServiceTransport pushServiceTransport = null;
            BandStoreClient client = null;
            try
            {
                LoggerProviderStub loggerProvider = new();
                deviceTransport = await BluetoothTransport.CreateAsync(bandInfo, loggerProvider, 3).ConfigureAwait(false);
                pushServiceTransport = new PushServiceTransport(bluetoothDeviceInfo, loggerProvider);
                client = new BandStoreClient(bluetoothDeviceInfo, deviceTransport, pushServiceTransport, loggerProvider, StoreApplicationPlatformProvider.Current);
                client.InitializeCachedProperties();
                client.CheckFirmwareSdkBit(FirmwareSdkCheckPlatform.Windows, 0);
                loggerProvider.Log(ProviderLogLevel.Info, "Created BandClient(IBandInfo bandinfo)");
                loggerProvider = null;
            }
            catch
            {
                if (client != null)
                {
                    client.Dispose();
                }
                else
                {
                    deviceTransport?.Dispose();
                    pushServiceTransport?.Dispose();
                }
                throw;
            }
            return client;
        }
    }
}
