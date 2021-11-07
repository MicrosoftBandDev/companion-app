// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ByteArrayProxyStream
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Band
{
    [ExcludeFromTestCodeCoverage]
    internal class ByteArrayProxyStream : ImageConversionStreamBase
    {
        private byte[] buffer;
        private int offset;
        private int length;
        private int position;

        public void SetBuffer(byte[] buffer, int offset, int length)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.length = length;
            position = 0;
        }

        public void ForgetBuffer() => buffer = null;

        public override bool CanRead => false;

        public override long Length => length;

        public override long Position
        {
            get => position;
            set => throw new InvalidOperationException();
        }

        public override int Read(byte[] argb32Array, int offset, int count) => throw new InvalidOperationException();

        public override void Write(byte[] argb32Array, int offset, int count) => throw new InvalidOperationException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value) => buffer[offset + position++] = value;
    }
}
