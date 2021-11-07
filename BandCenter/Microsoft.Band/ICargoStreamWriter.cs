// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ICargoStreamWriter
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal interface ICargoStreamWriter : ICargoWriter, IDisposable
    {
        void WriteCommandPacket(
          ushort commandId,
          uint argBufSize,
          uint dataStageSize,
          Action<ICargoWriter> writeArgBuf,
          bool prependPacketSize,
          bool flush);
    }
}
