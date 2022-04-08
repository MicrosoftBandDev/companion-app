// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Windows.PushServiceTransport
// Assembly: Microsoft.Band.Windows, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 28C7C615-6BA3-4124-96FB-B8960DA222E2
// Assembly location: .\netcore451\Microsoft.Band.Windows.dll

using System;
using Windows.Devices.Bluetooth.Rfcomm;

namespace Microsoft.Band.Windows
{
    internal sealed class PushServiceTransport : BluetoothTransportBase, IReadableTransport, IDisposable
    {
        private readonly BluetoothDeviceInfo associatedBand;

        public PushServiceTransport(BluetoothDeviceInfo deviceInfo, ILoggerProvider loggerProvider) : base(loggerProvider)
        {
            associatedBand = deviceInfo;
        }

        public void Connect(ushort maxConnectAttempts = 1)
        {
            foreach (BluetoothDeviceInfo connectedDevice in BluetoothTransport.GetConnectedDevices(new Guid("{C742E1A2-6320-5ABC-9643-D206C677E580}"), loggerProvider))
            {
                string connectedId = connectedDevice.Peer.Id;
                int idx = connectedId.LastIndexOf('#');
                if (idx >= 0)
                    connectedId = connectedId[..idx];
                string associatedId = connectedDevice.Peer.Id;
                idx = associatedId.LastIndexOf('#');
                if (idx >= 0)
                    associatedId = associatedId[..idx];

                if (connectedId == associatedId)
                {
                    Connect(RfcommDeviceService.FromIdAsync(connectedDevice.Peer.Id).AsTask().Result, maxConnectAttempts);
                    CargoStream.ReadTimeout = int.MaxValue;
                    return;
                }
            }
            throw new BandIOException(StoreResources.PushServiceNotFound);
        }
    }
}
