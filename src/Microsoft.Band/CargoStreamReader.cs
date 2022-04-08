// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.CargoStreamReader
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.Band
{
    internal sealed class CargoStreamReader : CargoStreamBase, ICargoReader, IDisposable
    {
        private ICargoStream stream;
        private BufferPool bufferPool;
        private PooledBuffer buffer;
        private int bufferedBytes;
        private int bufferOffset;

        public CargoStreamReader(ICargoStream source, BufferPool bufferPool)
        {
            stream = source;
            this.bufferPool = bufferPool;
        }

        public override bool CanRead => true;

        private int ReadToBuffer()
        {
            if (buffer == null)
                buffer = bufferPool.GetBuffer();
            int num = stream.Read(buffer.Buffer, bufferedBytes, bufferPool.BufferSize - bufferedBytes);
            bufferedBytes += num;
            return num;
        }

        private void ReadToBufferMinimum(int minCount)
        {
            int length = bufferedBytes - bufferOffset;
            if (length > 0 && length < minCount)
            {
                Array.Copy(this.buffer.Buffer, bufferOffset, this.buffer.Buffer, 0, length);
                bufferedBytes = length;
                bufferOffset = 0;
            }
            int buffer;
            for (; length < minCount; length += buffer)
            {
                buffer = ReadToBuffer();
                if (buffer == 0)
                    throw new EndOfStreamException();
            }
        }

        private void AdvanceBuffer(int count)
        {
            if (count == bufferedBytes - bufferOffset)
            {
                bufferedBytes = 0;
                bufferOffset = 0;
            }
            else
                bufferOffset += count;
        }

        public int Read(byte[] destination) => Read(destination, 0, destination.Length);

        public override int Read(byte[] destination, int offset, int count)
        {
            CheckIfDisposed();
            if (bufferedBytes == 0)
            {
                if (count >= bufferPool.BufferSize)
                    return stream.Read(destination, offset, bufferPool.BufferSize);
                ReadToBuffer();
            }
            int num = Math.Min(bufferedBytes - bufferOffset, count);
            Array.Copy(buffer.Buffer, bufferOffset, destination, offset, num);
            AdvanceBuffer(num);
            return num;
        }

        public void ReadExact(byte[] destination, int offset, int count)
        {
            CheckIfDisposed();
            int num;
            for (int index = 0; index < count; index += num)
            {
                num = Read(destination, offset + index, count - index);
                if (num == 0)
                    throw new EndOfStreamException();
            }
        }

        public byte[] ReadExact(int count)
        {
            CheckIfDisposed();
            byte[] destination = new byte[count];
            ReadExact(destination, 0, count);
            return destination;
        }

        public void ReadExactAndDiscard(int count)
        {
            CheckIfDisposed();
            int count1;
            for (int index = 0; index < count; index += count1)
            {
                if (bufferedBytes == 0)
                    ReadToBuffer();
                count1 = Math.Min(count - index, bufferedBytes - bufferOffset);
                if (count1 == 0)
                    throw new EndOfStreamException();
                AdvanceBuffer(count1);
            }
        }

        public byte ReadByte()
        {
            CheckIfDisposed();
            int num1 = 1;
            ReadToBufferMinimum(num1);
            int num2 = buffer.Buffer[bufferOffset];
            AdvanceBuffer(num1);
            return (byte)num2;
        }

        public short ReadInt16()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(2);
            int int16 = BitConverter.ToInt16(buffer.Buffer, bufferOffset);
            AdvanceBuffer(2);
            return (short)int16;
        }

        public ushort ReadUInt16()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(2);
            int uint16 = BitConverter.ToUInt16(buffer.Buffer, bufferOffset);
            AdvanceBuffer(2);
            return (ushort)uint16;
        }

        public int ReadInt32()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(4);
            int int32 = BitConverter.ToInt32(buffer.Buffer, bufferOffset);
            AdvanceBuffer(4);
            return int32;
        }

        public uint ReadUInt32()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(4);
            int uint32 = (int)BitConverter.ToUInt32(buffer.Buffer, bufferOffset);
            AdvanceBuffer(4);
            return (uint)uint32;
        }

        public long ReadInt64()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(8);
            long int64 = BitConverter.ToInt64(buffer.Buffer, bufferOffset);
            AdvanceBuffer(8);
            return int64;
        }

        public ulong ReadUInt64()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(8);
            long uint64 = (long)BitConverter.ToUInt64(buffer.Buffer, bufferOffset);
            AdvanceBuffer(8);
            return (ulong)uint64;
        }

        public bool ReadBool32()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(4);
            int num = (uint)BitConverter.ToInt32(buffer.Buffer, bufferOffset) > 0U ? 1 : 0;
            AdvanceBuffer(4);
            return num != 0;
        }

        public Guid ReadGuid()
        {
            CheckIfDisposed();
            ReadToBufferMinimum(16);
            Guid guid = BandBitConverter.ToGuid(buffer.Buffer, bufferOffset);
            AdvanceBuffer(16);
            return guid;
        }

        public string ReadString(int length)
        {
            int num = length * 2;
            if (num > buffer.Length)
                throw new BandIOException("Unsupported string length");
            CheckIfDisposed();
            ReadToBufferMinimum(num);
            int count = 0;
            while (count < num && buffer.Buffer[bufferOffset + count] != 0)
                count += 2;
            string str = Encoding.Unicode.GetString(buffer.Buffer, bufferOffset, count);
            AdvanceBuffer(num);
            return str;
        }

        public new void CopyTo(Stream stream, int count)
        {
            CheckIfDisposed();
            int num = 0;
            while (num < count)
            {
                if (bufferedBytes == 0)
                    ReadToBuffer();
                int count1 = Math.Min(count - num, bufferedBytes - bufferOffset);
                stream.Write(buffer.Buffer, bufferOffset, count1);
                num += count1;
                AdvanceBuffer(count1);
            }
        }

        public CargoStatus ReadStatusPacket()
        {
            CheckIfDisposed();
            for (int index = 0; index < 2; ++index)
            {
                if (ReadByte() != (42750 >> index * 8 & byte.MaxValue))
                    throw new IOException(BandResources.BadDeviceCommandStatusPacket);
            }
            return new CargoStatus()
            {
                PacketType = 42750,
                Status = ReadUInt32()
            };
        }

        private void CheckIfDisposed()
        {
            if (stream == null)
                throw new ObjectDisposedException(nameof(CargoStreamReader));
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
