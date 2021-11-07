// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.CargoCommandWriter
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;
using System.Threading;

namespace Microsoft.Band
{
    internal sealed class CargoCommandWriter : CargoStreamBase, ICargoWriter, IDisposable
    {
        private IDeviceTransport transport;
        private int byteCount;
        private object protocolLock;
        private int bytesTransferred;
        private CargoStatus? status;
        private ILoggerProvider loggerProvider;
        private CommandStatusHandling statusHandling;

        public CargoCommandWriter(
          IDeviceTransport transport,
          int byteCount,
          object protocolLock,
          ILoggerProvider loggerProvider,
          CommandStatusHandling statusHandling)
        {
            this.transport = transport;
            this.byteCount = byteCount;
            this.protocolLock = protocolLock;
            this.loggerProvider = loggerProvider;
            this.statusHandling = statusHandling;
            Monitor.Enter(protocolLock);
        }

        public override long Length => byteCount;

        public override long Position
        {
            get
            {
                CheckIfDisposed();
                return bytesTransferred;
            }
            set => throw new InvalidOperationException();
        }

        public override bool CanWrite => true;

        public int BytesRemaining
        {
            get
            {
                CheckIfDisposed();
                return byteCount - bytesTransferred;
            }
        }

        public CargoStatus CommandStatus => status.HasValue ? status.Value : throw new InvalidOperationException(BandResources.CargoCommandStatusUnavailable);

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            transport.CargoWriter.Write(buffer, offset, count);
            IncrementBytesTransferred(count);
        }

        public void Write(byte[] buffer) => Write(buffer, 0, buffer.Length);

        public new void WriteByte(byte b)
        {
            CheckIfDisposed();
            CheckIfEof(1);
            transport.CargoWriter.WriteByte(b);
            IncrementBytesTransferred(1);
        }

        public void WriteByte(byte b, int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            transport.CargoWriter.WriteByte(b, count);
            IncrementBytesTransferred(count);
        }

        public void WriteInt16(short i)
        {
            CheckIfDisposed();
            CheckIfEof(2);
            transport.CargoWriter.WriteInt16(i);
            IncrementBytesTransferred(2);
        }

        public void WriteUInt16(ushort i)
        {
            CheckIfDisposed();
            CheckIfEof(2);
            transport.CargoWriter.WriteUInt16(i);
            IncrementBytesTransferred(2);
        }

        public void WriteInt32(int i)
        {
            CheckIfDisposed();
            CheckIfEof(4);
            transport.CargoWriter.WriteInt32(i);
            IncrementBytesTransferred(4);
        }

        public void WriteUInt32(uint i)
        {
            CheckIfDisposed();
            CheckIfEof(4);
            transport.CargoWriter.WriteUInt32(i);
            IncrementBytesTransferred(4);
        }

        public void WriteInt64(long i)
        {
            CheckIfDisposed();
            CheckIfEof(8);
            transport.CargoWriter.WriteInt64(i);
            IncrementBytesTransferred(8);
        }

        public void WriteUInt64(ulong i)
        {
            CheckIfDisposed();
            CheckIfEof(8);
            transport.CargoWriter.WriteUInt64(i);
            IncrementBytesTransferred(8);
        }

        public void WriteBool32(bool b)
        {
            CheckIfDisposed();
            CheckIfEof(4);
            transport.CargoWriter.WriteBool32(b);
            IncrementBytesTransferred(4);
        }

        public void WriteGuid(Guid guid)
        {
            CheckIfDisposed();
            CheckIfEof(16);
            transport.CargoWriter.WriteGuid(guid);
            IncrementBytesTransferred(16);
        }

        public void WriteString(string s)
        {
            CheckIfDisposed();
            if (s.Length == 0)
                return;
            int count = s.Length * 2;
            CheckIfEof(count);
            transport.CargoWriter.WriteString(s);
            IncrementBytesTransferred(count);
        }

        public void WriteStringWithPadding(string s, int exactLength)
        {
            CheckIfDisposed();
            if (exactLength == 0)
                return;
            int count = exactLength * 2;
            CheckIfEof(count);
            transport.CargoWriter.WriteStringWithPadding(s, exactLength);
            IncrementBytesTransferred(count);
        }

        public void WriteStringWithTruncation(string s, int maxLength)
        {
            CheckIfDisposed();
            if (maxLength == 0)
                return;
            int count = Math.Min(s.Length, maxLength) * 2;
            CheckIfEof(count);
            transport.CargoWriter.WriteStringWithTruncation(s, maxLength);
            IncrementBytesTransferred(count);
        }

        public int CopyFromStream(Stream stream, int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            if (count == 0)
                return 0;
            int count1 = transport.CargoWriter.CopyFromStream(stream, count);
            IncrementBytesTransferred(count1);
            return count1;
        }

        public int CopyFromStream(Stream stream) => CopyFromStream(stream, BytesRemaining);

        public override void Flush()
        {
            CheckIfDisposed();
            transport.CargoWriter.Flush();
        }

        private void IncrementBytesTransferred(int count)
        {
            bytesTransferred += count;
            if (BytesRemaining != 0)
                return;
            transport.CargoWriter.Flush();
            status = new CargoStatus?(transport.CargoReader.ReadStatusPacket());
            BandClient.CheckStatus(status.Value, statusHandling, loggerProvider);
            Monitor.Exit(protocolLock);
            protocolLock = null;
        }

        private void CheckIfDisposed()
        {
            if (transport == null)
                throw new ObjectDisposedException(nameof(CargoCommandWriter));
        }

        private void CheckIfEof(int count)
        {
            if (count > BytesRemaining)
                throw new EndOfStreamException(BandResources.EofExceptionOnWrite);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || transport == null)
                return;
            transport = null;
            if (protocolLock == null)
                return;
            Monitor.Exit(protocolLock);
            protocolLock = null;
        }
    }
}
