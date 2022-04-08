// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.IReadableTransport
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal interface IReadableTransport : IDisposable
    {
        ICargoStream CargoStream { get; }

        CargoStreamReader CargoReader { get; }

        BandConnectionType ConnectionType { get; }

        event EventHandler<TransportDisconnectedEventArgs> Disconnected;

        void Connect(ushort maxConnectAttempts = 1);

        void Disconnect();

        bool IsConnected { get; }
    }
}
