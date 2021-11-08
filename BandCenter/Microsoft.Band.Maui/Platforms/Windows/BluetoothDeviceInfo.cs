// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.BluetoothDeviceInfo
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: .\netcore451\Microsoft.Band.Store.dll

using System;
using System.Security;
using Windows.Devices.Enumeration;

namespace Microsoft.Band.Windows
{
    internal sealed class BluetoothDeviceInfo : IBandInfo
    {
        public string Name { get; private set; }

        public BandConnectionType ConnectionType => BandConnectionType.Bluetooth;

        public DeviceInformation Peer { [SecurityCritical] get; [SecurityCritical] private set; }

        [SecurityCritical]
        public BluetoothDeviceInfo(DeviceInformation peer)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
            Name = peer.Name;
        }
    }
}
