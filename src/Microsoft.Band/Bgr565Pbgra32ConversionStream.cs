// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Bgr565Pbgra32ConversionStream
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;

namespace Microsoft.Band
{
    internal class Bgr565Pbgra32ConversionStream : ImageConversionStreamBase
    {
        private const int pixelWidthFactor = 2;
        private int argb32Index;
        private ByteArrayProxyStream writeProxy;

        public Bgr565Pbgra32ConversionStream(int argb32ByteCount) => Bgr565Array = new byte[argb32ByteCount / 2];

        public Bgr565Pbgra32ConversionStream(byte[] bgr565Array) => Bgr565Array = bgr565Array;

        public override long Length => Bgr565Array.Length * 2;

        public override long Position
        {
            get => argb32Index;
            set => throw new InvalidOperationException();
        }

        public byte[] Bgr565Array { get; private set; }

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
            int index = argb32Index / 2;
            for (; argb32Index < Length && count > 0; --count)
            {
                switch (argb32Index % 4)
                {
                    case 0:
                        Bgr565Array[index] |= (byte)((argb32Array[offset] & 248U) >> 3);
                        break;
                    case 1:
                        Bgr565Array[index++] |= (byte)((argb32Array[offset] & 28) << 3);
                        Bgr565Array[index] |= (byte)((argb32Array[offset] & 224U) >> 5);
                        break;
                    case 2:
                        Bgr565Array[index++] |= (byte)(argb32Array[offset] & 248U);
                        break;
                }
                ++offset;
                ++argb32Index;
            }
        }

        public new void CopyTo(Stream dest) => CopyToInternal(dest, (int)Length - argb32Index);

        public new void CopyTo(Stream dest, int bufferSize) => CopyToInternal(dest, (int)Length - argb32Index);

        private void CopyToInternal(Stream dest, int count)
        {
            int index = argb32Index / 2;
            for (; argb32Index < Length && count > 0; --count)
            {
                switch (argb32Index % 4)
                {
                    case 0:
                        dest.WriteByte((byte)((Bgr565Array[index] & 31) * byte.MaxValue / 31));
                        break;
                    case 1:
                        dest.WriteByte((byte)(((Bgr565Array[index++] & 224) >> 5 | (Bgr565Array[index] & 7) << 3) * byte.MaxValue / 63));
                        break;
                    case 2:
                        dest.WriteByte((byte)(((Bgr565Array[index] & 248) >> 3) * byte.MaxValue / 31));
                        break;
                    case 3:
                        dest.WriteByte(byte.MaxValue);
                        ++index;
                        break;
                }
                ++argb32Index;
            }
        }
    }
}
