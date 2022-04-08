// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.CargoCommandReader
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;
using System.Threading;

namespace Microsoft.Band
{
    internal sealed class CargoCommandReader : CargoStreamBase, ICargoReader, IDisposable
    {
        private IDeviceTransport transport;
        private int byteCount;
        private object protocolLock;
        private int bytesTransferred;
        private CargoStatus? status;
        private ILoggerProvider loggerProvider;
        private CommandStatusHandling statusHandling;

        public CargoCommandReader(
          IDeviceTransport transport,
          int byteCount,
          object protocolLock,
          ILoggerProvider loggerProvider,
          CommandStatusHandling statusHandling)
        {
            this.transport = transport;
            this.byteCount = byteCount;
            this.protocolLock = protocolLock;
            this.statusHandling = statusHandling;
            this.loggerProvider = loggerProvider;
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

        public override bool CanRead => true;

        public int BytesRemaining
        {
            get
            {
                CheckIfDisposed();
                return byteCount - bytesTransferred;
            }
        }

        public CargoStatus CommandStatus => status.HasValue ? status.Value : throw new InvalidOperationException(BandResources.CargoCommandStatusUnavailable);

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            int num = transport.CargoReader.Read(buffer, offset, count);
            bytesTransferred += num;
            if (BytesRemaining == 0)
                FinalizeCommand();
            return num;
        }

        public void ReadExact(byte[] buffer, int offset, int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            transport.CargoReader.ReadExact(buffer, offset, count);
            bytesTransferred += count;
            if (BytesRemaining != 0)
                return;
            FinalizeCommand();
        }

        public byte[] ReadExact(int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            byte[] numArray = transport.CargoReader.ReadExact(count);
            bytesTransferred += count;
            if (BytesRemaining != 0)
                return numArray;
            FinalizeCommand();
            return numArray;
        }

        public void ReadExactAndDiscard(int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            transport.CargoReader.ReadExactAndDiscard(count);
            bytesTransferred += count;
            if (BytesRemaining != 0)
                return;
            FinalizeCommand();
        }

        public int Read(byte[] buffer) => Read(buffer, 0, buffer.Length);

        public byte ReadByte()
        {
            CheckIfDisposed();
            CheckIfEof(1);
            int num = transport.CargoReader.ReadByte();
            ++bytesTransferred;
            if (BytesRemaining != 0)
                return (byte)num;
            FinalizeCommand();
            return (byte)num;
        }

        public int ReadToEnd(byte[] buffer, int offset)
        {
            CheckIfDisposed();
            if (BytesRemaining == 0)
                return 0;
            int num = 0;
            transport.CargoReader.ReadExact(buffer, offset, BytesRemaining);
            bytesTransferred += BytesRemaining;
            if (BytesRemaining != 0)
                return num;
            FinalizeCommand();
            return num;
        }

        public void ReadToEndAndDiscard()
        {
            CheckIfDisposed();
            if (BytesRemaining == 0)
                return;
            transport.CargoReader.ReadExactAndDiscard(BytesRemaining);
            bytesTransferred += BytesRemaining;
            FinalizeCommand();
        }

        public short ReadInt16()
        {
            CheckIfDisposed();
            CheckIfEof(2);
            int num = transport.CargoReader.ReadInt16();
            bytesTransferred += 2;
            if (BytesRemaining != 0)
                return (short)num;
            FinalizeCommand();
            return (short)num;
        }

        public ushort ReadUInt16()
        {
            CheckIfDisposed();
            CheckIfEof(2);
            int num = transport.CargoReader.ReadUInt16();
            bytesTransferred += 2;
            if (BytesRemaining != 0)
                return (ushort)num;
            FinalizeCommand();
            return (ushort)num;
        }

        public int ReadInt32()
        {
            CheckIfDisposed();
            CheckIfEof(4);
            int num = transport.CargoReader.ReadInt32();
            bytesTransferred += 4;
            if (BytesRemaining != 0)
                return num;
            FinalizeCommand();
            return num;
        }

        public uint ReadUInt32()
        {
            CheckIfDisposed();
            CheckIfEof(4);
            int num = (int)transport.CargoReader.ReadUInt32();
            bytesTransferred += 4;
            if (BytesRemaining != 0)
                return (uint)num;
            FinalizeCommand();
            return (uint)num;
        }

        public long ReadInt64()
        {
            CheckIfDisposed();
            CheckIfEof(8);
            long num = transport.CargoReader.ReadInt64();
            bytesTransferred += 8;
            if (BytesRemaining != 0)
                return num;
            FinalizeCommand();
            return num;
        }

        public ulong ReadUInt64()
        {
            CheckIfDisposed();
            CheckIfEof(8);
            long num = (long)transport.CargoReader.ReadUInt64();
            bytesTransferred += 8;
            if (BytesRemaining != 0)
                return (ulong)num;
            FinalizeCommand();
            return (ulong)num;
        }

        public bool ReadBool32()
        {
            CheckIfDisposed();
            CheckIfEof(4);
            int num = transport.CargoReader.ReadBool32() ? 1 : 0;
            bytesTransferred += 4;
            if (BytesRemaining != 0)
                return num != 0;
            FinalizeCommand();
            return num != 0;
        }

        public Guid ReadGuid()
        {
            CheckIfDisposed();
            CheckIfEof(16);
            Guid guid = transport.CargoReader.ReadGuid();
            bytesTransferred += 16;
            if (BytesRemaining != 0)
                return guid;
            FinalizeCommand();
            return guid;
        }

        public string ReadString(int length)
        {
            CheckIfDisposed();
            CheckIfEof(8);
            int num = length * 2;
            string str = transport.CargoReader.ReadString(length);
            bytesTransferred += num;
            if (BytesRemaining != 0)
                return str;
            FinalizeCommand();
            return str;
        }

        public new void CopyTo(Stream stream, int count)
        {
            CheckIfDisposed();
            CheckIfEof(count);
            if (count == 0)
                return;
            transport.CargoReader.CopyTo(stream, count);
            bytesTransferred += count;
            if (BytesRemaining != 0)
                return;
            FinalizeCommand();
        }

        public new void CopyTo(Stream stream) => CopyTo(stream, BytesRemaining);

        private void FinalizeCommand()
        {
            status = new CargoStatus?(transport.CargoReader.ReadStatusPacket());
            BandClient.CheckStatus(status.Value, statusHandling, loggerProvider);
            Monitor.Exit(protocolLock);
            protocolLock = null;
        }

        private void CheckIfDisposed()
        {
            if (transport == null)
                throw new ObjectDisposedException(nameof(CargoCommandReader));
        }

        private void CheckIfEof(int count)
        {
            if (count > BytesRemaining)
                throw new EndOfStreamException();
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
