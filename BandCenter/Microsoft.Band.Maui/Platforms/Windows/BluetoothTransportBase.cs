// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.BluetoothTransportBase
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: C:\Users\jjask\AppData\Local\Temp\Xiwoxyt\b1d4237fe8\lib\netcore451\Microsoft.Band.Store.dll

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;

namespace Microsoft.Band.Windows
{
    internal abstract class BluetoothTransportBase : IDisposable
    {
        protected RfcommDeviceService rfcommService;
        private bool isConnected;
        private ICargoStream cargoStream;
        private CargoStreamReader cargoReader;
        private bool disposed;
        protected ILoggerProvider loggerProvider;

        public event EventHandler<TransportDisconnectedEventArgs> Disconnected;

        protected BluetoothTransportBase(ILoggerProvider loggerProvider)
        {
            this.loggerProvider = loggerProvider;
            isConnected = false;
        }

        protected BluetoothTransportBase(RfcommDeviceService service, ILoggerProvider loggerProvider)
          : this(loggerProvider)
        {
            rfcommService = service;
        }

        public ICargoStream CargoStream
        {
            get
            {
                CheckIfDisposed();
                return cargoStream;
            }
        }

        public CargoStreamReader CargoReader
        {
            get
            {
                CheckIfDisposed();
                return cargoReader;
            }
        }

        public BandConnectionType ConnectionType => BandConnectionType.Bluetooth;

        protected void RaiseDisconnectedEvent(TransportDisconnectedEventArgs args)
        {
            EventHandler<TransportDisconnectedEventArgs> disconnected = Disconnected;
            if (disconnected == null)
                return;
            disconnected(this, args);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Connect(RfcommDeviceService service, ushort maxConnectAttempts = 1)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            CheckIfDisposed();
            if (IsConnected)
                return;
            if (maxConnectAttempts == 0)
                throw new ArgumentOutOfRangeException(nameof(maxConnectAttempts));
            loggerProvider.Log(ProviderLogLevel.Info, $"Socket.ConnectAsync()... Max Attempts: {maxConnectAttempts}, ConnectionServiceName: {service.ConnectionServiceName}");
            ushort num = 0;
            Exception innerException = null;
            Stopwatch stopwatch1 = Stopwatch.StartNew();
            StreamSocket socket;
            do
            {
                ++num;
                Stopwatch stopwatch2 = Stopwatch.StartNew();
                socket = new StreamSocket();
                try
                {
                    loggerProvider.Log(ProviderLogLevel.Info, $"Socket.ConnectAsync()... Attempt: {num}/{maxConnectAttempts}");
                    socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName).AsTask().Wait();
                    isConnected = true;
                    loggerProvider.Log(ProviderLogLevel.Info, $"Socket.ConnectAsync() succeeded: Attempt: {num}/{maxConnectAttempts}, Elapsed: {stopwatch2.Elapsed}");
                }
                catch (Exception ex)
                {
                    socket.Dispose();
                    if (innerException == null)
                        innerException = ex;
                    loggerProvider.LogException(ProviderLogLevel.Warning, ex, $"Socket.ConnectAsync() failed: Attempt: {num}/{maxConnectAttempts}, Elapsed: {stopwatch2.Elapsed}");
                }
            }
            while (!isConnected && num < maxConnectAttempts);
            if (isConnected)
            {
                cargoStream = new StreamSocketStream(socket);
                cargoReader = new CargoStreamReader(cargoStream, BufferServer.Pool_8192);
                loggerProvider.Log(ProviderLogLevel.Info, $"Socket.ConnectAsync() succeeded: Elapsed: {stopwatch1.Elapsed}, ConnectionServiceName: {service.ConnectionServiceName}");
            }
            else
            {
                loggerProvider.Log(ProviderLogLevel.Error, $"Socket.ConnectAsync() failed: Attempts: {num}, Elapsed: {stopwatch1.Elapsed}, ConnectionServiceName: {service.ConnectionServiceName}");
                throw new BandIOException(BandResources.ConnectionAttemptFailed, innerException);
            }
        }

        public virtual void Disconnect()
        {
            if (IsConnected)
            {
                isConnected = false;
                CargoStreamReader cargoReader = this.cargoReader;
                if (cargoReader != null)
                {
                    cargoReader.Dispose();
                    this.cargoReader = null;
                }
                cargoStream = null;
            }
            RaiseDisconnectedEvent(new TransportDisconnectedEventArgs(TransportDisconnectedReason.DisconnectCall));
        }

        public bool IsConnected => isConnected;

        protected void CheckIfDisconnected()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Transport not connected");
        }

        protected void CheckIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(BluetoothTransportBase));
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || disposed)
                return;
            Disconnect();
            ICargoStream cargoStream = this.cargoStream;
            if (cargoStream != null)
            {
                cargoStream.Dispose();
                this.cargoStream = null;
            }
            disposed = true;
        }
    }
}
