// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.GetPageLayoutArgs
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles
{
    internal static class GetPageLayoutArgs
    {
        private const int serializedByteCount = 24;

        internal static int GetSerializedByteCount() => 24;

        internal static void SerializeToBand(ICargoWriter writer, Guid tileId, uint layoutIndex)
        {
            writer.WriteGuid(tileId);
            writer.WriteUInt32(layoutIndex);
            writer.WriteUInt32(768U);
        }
    }
}
