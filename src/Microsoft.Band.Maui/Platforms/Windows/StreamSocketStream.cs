// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.StreamSocketStream
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: C:\Users\jjask\AppData\Local\Temp\Xiwoxyt\b1d4237fe8\lib\netcore451\Microsoft.Band.Store.dll

using System;
using System.IO;
using System.Threading;
using Windows.Networking.Sockets;

namespace Microsoft.Band.Windows
{
    internal sealed class StreamSocketStream : ICargoStream, IDisposable
    {
        private StreamSocket socket;
        private Stream inputStream;
        private Stream outputStream;
        private int readTimeout;
        private int writeTimeout;
        private bool isDisposed;

        public StreamSocketStream(StreamSocket socket)
        {
            this.socket = socket != null ? socket : throw new ArgumentNullException(nameof(socket));
            inputStream = socket.InputStream.AsStreamForRead(0);
            outputStream = socket.OutputStream.AsStreamForWrite(0);
        }

        public StreamSocket Socket
        {
            get
            {
                CheckIsDisposed();
                return socket;
            }
        }

        public int ReadTimeout
        {
            get => readTimeout;
            set => readTimeout = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public int WriteTimeout
        {
            get => writeTimeout;
            set => writeTimeout = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public CancellationToken Cancel { get; set; }

        public int Read(byte[] buffer, int offset, int count)
        {
            CheckIsDisposed();
            int num = 0;
            using (CancellationTokenSource cancellationTokenSource1 = new(readTimeout))
            {
                CancellationTokenSource cancellationTokenSource2 = null;
                if (Cancel != CancellationToken.None)
                    cancellationTokenSource2 = CancellationTokenSource.CreateLinkedTokenSource(Cancel, cancellationTokenSource1.Token);
                try
                {
                    num = inputStream.ReadAsync(buffer, offset, count, cancellationTokenSource2 != null ? cancellationTokenSource2.Token : cancellationTokenSource1.Token).Result;
                }
                catch (AggregateException ex)
                {
                    HandleAggregateIOException(ex);
                }
                finally
                {
                    cancellationTokenSource2?.Dispose();
                }
            }
            return num;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            CheckIsDisposed();
            using CancellationTokenSource cancellationTokenSource = new(writeTimeout);
            try
            {
                outputStream.WriteAsync(buffer, offset, count, cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException ex)
            {
                HandleAggregateIOException(ex);
            }
        }

        public void Flush()
        {
            CheckIsDisposed();
            using CancellationTokenSource cancellationTokenSource = new(writeTimeout);
            try
            {
                outputStream.FlushAsync(cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException ex)
            {
                HandleAggregateIOException(ex);
            }
        }

        private void HandleAggregateIOException(AggregateException e)
        {
            if (e.InnerExceptions.Count != 1)
                throw e;
            if (e.InnerException is OperationCanceledException)
            {
                if (Cancel != CancellationToken.None)
                    Cancel.ThrowIfCancellationRequested();
                throw new TimeoutException();
            }
            throw e.InnerException;
        }

        public void CheckIsDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(nameof(StreamSocketStream));
        }

        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            inputStream.Dispose();
            outputStream.Dispose();
            socket.Dispose();
        }
    }
}
