// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.CargoStreamWriter
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.Band
{
    internal sealed class CargoStreamWriter : CargoStreamBase, ICargoStreamWriter, ICargoWriter, IDisposable
    {
        private ICargoStream stream;
        private ILoggerProvider loggerProvider;
        private BufferPool bufferPool;
        private PooledBuffer buffer;
        private int bufferedBytes;

        public CargoStreamWriter(ICargoStream source, ILoggerProvider loggerProvider, BufferPool bufferPool)
        {
            stream = source;
            this.loggerProvider = loggerProvider;
            this.bufferPool = bufferPool;
        }

        public override bool CanWrite => true;

        private void WriteFromBuffer()
        {
            stream.Write(buffer.Buffer, 0, bufferedBytes);
            bufferedBytes = 0;
        }

        private void EnsureBufferSpace(int minCount)
        {
            if (buffer == null)
                buffer = bufferPool.GetBuffer();
            if (minCount <= bufferPool.BufferSize - bufferedBytes || bufferedBytes <= 0)
                return;
            WriteFromBuffer();
        }

        private void AdvanceBuffer(int count)
        {
            bufferedBytes += count;
            if (bufferedBytes != bufferPool.BufferSize)
                return;
            WriteFromBuffer();
            bufferedBytes = 0;
        }

        public void Write(byte[] source) => Write(source, 0, source.Length);

        public override void Write(byte[] source, int offset, int count)
        {
            CheckIfDisposed();
            EnsureBufferSpace(count);
            int num1 = 0;
            int num2;
            for (num2 = Math.Min(count - num1, bufferPool.BufferSize); num1 < count && num2 >= bufferPool.BufferSize; num2 = Math.Min(count - num1, bufferPool.BufferSize))
            {
                stream.Write(source, offset + num1, num2);
                num1 += num2;
            }
            if (num2 <= 0)
                return;
            Array.Copy(source, offset + num1, buffer.Buffer, bufferedBytes, num2);
            AdvanceBuffer(num2);
        }

        public new void WriteByte(byte value)
        {
            CheckIfDisposed();
            int num = 1;
            EnsureBufferSpace(num);
            buffer.Buffer[bufferedBytes] = value;
            AdvanceBuffer(num);
        }

        public void WriteByte(byte value, int count)
        {
            CheckIfDisposed();
            int num = 1;
            while (count-- > 0)
            {
                EnsureBufferSpace(num);
                buffer.Buffer[bufferedBytes] = value;
                AdvanceBuffer(num);
            }
        }

        public void WriteInt16(short i)
        {
            CheckIfDisposed();
            EnsureBufferSpace(2);
            BandBitConverter.GetBytes(i, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(2);
        }

        public void WriteUInt16(ushort i)
        {
            CheckIfDisposed();
            EnsureBufferSpace(2);
            BandBitConverter.GetBytes(i, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(2);
        }

        public void WriteInt32(int i)
        {
            CheckIfDisposed();
            EnsureBufferSpace(4);
            BandBitConverter.GetBytes(i, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(4);
        }

        public void WriteUInt32(uint i)
        {
            CheckIfDisposed();
            EnsureBufferSpace(4);
            BandBitConverter.GetBytes(i, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(4);
        }

        public void WriteInt64(long i)
        {
            CheckIfDisposed();
            EnsureBufferSpace(8);
            BandBitConverter.GetBytes(i, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(8);
        }

        public void WriteUInt64(ulong i)
        {
            CheckIfDisposed();
            EnsureBufferSpace(8);
            BandBitConverter.GetBytes(i, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(8);
        }

        public void WriteBool32(bool b)
        {
            CheckIfDisposed();
            EnsureBufferSpace(4);
            BandBitConverter.GetBytes(b ? 1 : 0, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(4);
        }

        public void WriteGuid(Guid guid)
        {
            CheckIfDisposed();
            EnsureBufferSpace(16);
            BandBitConverter.GetBytes(guid, buffer.Buffer, bufferedBytes);
            AdvanceBuffer(16);
        }

        public void WriteString(string s)
        {
            if (s.Length == 0)
                return;
            int minCount = s.Length * 2;
            if (minCount > buffer.Length)
                throw new BandIOException("Unsupported string length");
            CheckIfDisposed();
            EnsureBufferSpace(minCount);
            AdvanceBuffer(Encoding.Unicode.GetBytes(s, 0, s.Length, buffer.Buffer, bufferedBytes));
        }

        public void WriteStringWithPadding(string s, int exactLength)
        {
            int charCount = Math.Min(s.LengthTruncatedTrimDanglingHighSurrogate(exactLength - 1), exactLength - 1);
            int num = exactLength * 2;
            if (num > buffer.Length)
                throw new BandIOException("Unsupported string length");
            if (num == 0)
                return;
            CheckIfDisposed();
            EnsureBufferSpace(num);
            int danglingHighSurrogate = Encoding.Unicode.GetBytesTrimDanglingHighSurrogate(s, charCount, buffer.Buffer, bufferedBytes);
            Array.Clear(buffer.Buffer, bufferedBytes + danglingHighSurrogate, num - danglingHighSurrogate);
            AdvanceBuffer(num);
        }

        public void WriteStringWithTruncation(string s, int maxLength)
        {
            int charCount = Math.Min(s.LengthTruncatedTrimDanglingHighSurrogate(maxLength), maxLength);
            if (charCount == 0)
                return;
            int minCount = charCount * 2;
            if (minCount > buffer.Length)
                throw new BandIOException("Unsupported string length");
            CheckIfDisposed();
            EnsureBufferSpace(minCount);
            AdvanceBuffer(Encoding.Unicode.GetBytesTrimDanglingHighSurrogate(s, charCount, buffer.Buffer, bufferedBytes));
        }

        public int CopyFromStream(Stream stream, int count)
        {
            CheckIfDisposed();
            int num1 = 0;
            while (num1 < count)
            {
                int num2 = Math.Min(count - num1, bufferPool.BufferSize);
                EnsureBufferSpace(num2);
                int count1 = stream.Read(buffer.Buffer, bufferedBytes, num2);
                if (count1 != 0)
                {
                    num1 += count1;
                    AdvanceBuffer(count1);
                }
                else
                    break;
            }
            return num1;
        }

        public void WriteCommandPacket(
          ushort commandId,
          uint argBufSize,
          uint dataStageSize,
          Action<ICargoWriter> writeArgBuf,
          bool prependPacketSize,
          bool flush)
        {
            CheckIfDisposed();
            if (argBufSize > 0U)
            {
                if (writeArgBuf == null)
                    throw new ArgumentException("writeArgBuf must not be null", nameof(writeArgBuf));
                if (argBufSize > 55U)
                    throw new ArgumentOutOfRangeException(nameof(argBufSize));
            }
            DeviceCommands.LookupCommand(commandId, out Facility category, out TX isTXCommand, out byte index);
            loggerProvider.Log(ProviderLogLevel.Info, $"Device Command: Facility: {category}, TX: {isTXCommand}, Index: 0x{index:X2}, Arg Buf Size: {argBufSize}, Data Size: {dataStageSize}");
            try
            {
                if (prependPacketSize)
                    WriteByte((byte)(8U + argBufSize));
                WriteUInt16(12025);
                WriteUInt16(commandId);
                WriteUInt32(dataStageSize);
                if (argBufSize > 0U)
                    writeArgBuf(this);
                if (!flush)
                    return;
                Flush();
            }
            catch (Exception ex)
            {
                throw new BandIOException(BandResources.StreamWriteFailure, ex);
            }
        }

        public override void Flush()
        {
            CheckIfDisposed();
            if (bufferedBytes > 0)
                WriteFromBuffer();
            stream.Flush();
        }

        private void CheckIfDisposed()
        {
            if (stream == null)
                throw new ObjectDisposedException(nameof(CargoStreamWriter));
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            ICargoStream stream = this.stream;
            if (stream != null)
            {
                stream.Dispose();
                this.stream = null;
            }
            PooledBuffer buffer = this.buffer;
            if (buffer == null)
                return;
            buffer.Dispose();
            this.buffer = null;
        }
    }
}
