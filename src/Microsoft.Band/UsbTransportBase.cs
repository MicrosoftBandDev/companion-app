// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.UsbTransportBase
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal abstract class UsbTransportBase : IDeviceTransport, IReadableTransport, IDisposable
    {
        protected ICargoStream cargoStream;
        protected CargoStreamReader cargoReader;
        protected CargoStreamWriter cargoWriter;
        protected bool isDisposed;

        public event EventHandler<TransportDisconnectedEventArgs> Disconnected;

        public ICargoStream CargoStream => ReturnIfNotDisposed(cargoStream);

        public CargoStreamReader CargoReader => ReturnIfNotDisposed(cargoReader);

        public CargoStreamWriter CargoWriter => ReturnIfNotDisposed(cargoWriter);

        public abstract BandConnectionType ConnectionType { get; }

        public abstract bool IsConnected { get; }

        private T ReturnIfNotDisposed<T>(T value)
        {
            CheckIfDisposed();
            return value;
        }

        public abstract void Connect(ushort maxConnectAttempts = 1);

        public abstract void Disconnect();

        public void WriteCommandPacket(ushort commandId, uint argBufSize, uint dataStageSize, Action<ICargoWriter> writeArgBuf, bool flush)
        {
            CheckIfDisposed();
            CheckIfDisconnected();
            CargoWriter.WriteCommandPacket(commandId, argBufSize, dataStageSize, writeArgBuf, false, true);
        }

        protected void RaiseDisconnectedEvent(TransportDisconnectedEventArgs args)
        {
            EventHandler<TransportDisconnectedEventArgs> disconnected = Disconnected;
            if (disconnected == null)
                return;
            disconnected(this, args);
        }

        protected abstract void CheckIfDisposed();

        protected abstract void CheckIfDisconnected();

        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            Disconnect();
        }
    }
}
