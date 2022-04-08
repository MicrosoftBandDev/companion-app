// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ImageConversionStreamBase
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.IO;

namespace Microsoft.Band
{
    [ExcludeFromTestCodeCoverage]
    internal abstract class ImageConversionStreamBase : Stream
    {
        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanSeek => false;

        public override bool CanTimeout => false;

        public override void SetLength(long value) => throw new InvalidOperationException();

        public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException();

        public override void Flush()
        {
        }
    }
}
