// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.CargoStreamBase
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;

namespace Microsoft.Band
{
    internal abstract class CargoStreamBase : Stream
    {
        public override long Length => throw new InvalidOperationException();

        public override long Position
        {
            get => throw new InvalidOperationException();
            set => throw new InvalidOperationException();
        }

        public override bool CanRead => false;

        public override bool CanWrite => false;

        public override bool CanSeek => false;

        public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();

        public override void SetLength(long value) => throw new InvalidOperationException();

        public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException();

        public override void Flush() => throw new InvalidOperationException();
    }
}
