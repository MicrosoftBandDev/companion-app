// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Alpha4Pbgra32ConversionStream
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;

namespace Microsoft.Band
{
    internal class Alpha4Pbgra32ConversionStream : ImageConversionStreamBase
    {
        private const int pixelWidthFactor = 4;
        private int argb32ByteCount;
        private int argb32Index;
        private ByteArrayProxyStream writeProxy;

        public Alpha4Pbgra32ConversionStream(int argb32ByteCount)
        {
            int num = argb32ByteCount / 4;
            this.argb32ByteCount = argb32ByteCount;
            Alpha4Array = new byte[num / 2 + num % 2];
        }

        public Alpha4Pbgra32ConversionStream(byte[] alpha4Array, int argb32ByteCount)
        {
            this.argb32ByteCount = argb32ByteCount;
            Alpha4Array = alpha4Array;
        }

        public override long Length => argb32ByteCount;

        public override long Position
        {
            get => argb32Index;
            set => throw new InvalidOperationException();
        }

        public byte[] Alpha4Array { get; private set; }

        public override int Read(byte[] argb32Array, int offset, int count)
        {
            if (writeProxy == null)
                writeProxy = new ByteArrayProxyStream();
            writeProxy.SetBuffer(argb32Array, offset, count);
            try
            {
                CopyToInternal(writeProxy, count);
            }
            finally
            {
                writeProxy.ForgetBuffer();
            }
            return count;
        }

        public override void Write(byte[] argb32Array, int offset, int count)
        {
            int num = argb32Index / 4;
            for (; argb32Index < Length && count > 0; --count)
            {
                if (argb32Index % 4 == 3)
                {
                    switch (num % 2)
                    {
                        case 0:
                            Alpha4Array[num++ / 2] |= (byte)(argb32Array[offset] & 240U);
                            break;
                        case 1:
                            Alpha4Array[num++ / 2] |= (byte)((argb32Array[offset] & 240) >> 4);
                            break;
                    }
                }
                ++offset;
                ++argb32Index;
            }
        }

        public new void CopyTo(Stream dest) => CopyToInternal(dest, (int)Length - argb32Index);

        private void CopyToInternal(Stream dest, int count)
        {
            int num = argb32Index / 4;
            for (; argb32Index < Length && count > 0; --count)
            {
                switch (num % 2)
                {
                    case 0:
                        dest.WriteByte((byte)(((Alpha4Array[num / 2] & 240) >> 4) * byte.MaxValue / 15));
                        break;
                    case 1:
                        dest.WriteByte((byte)((Alpha4Array[num / 2] & 15) * byte.MaxValue / 15));
                        break;
                }
                if (argb32Index % 4 == 3)
                    ++num;
                ++argb32Index;
            }
        }
    }
}
