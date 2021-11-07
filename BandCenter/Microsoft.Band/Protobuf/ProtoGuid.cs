// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Protobuf.ProtoGuid
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Google.Protobuf;
using System;

namespace Microsoft.Band.Protobuf
{
    internal sealed class ProtoGuid
    {
        internal static int GetSerializedProtobufByteCount() => 18;

        internal static void SerializeProtobufToBand(CodedOutputStream output, Guid guid)
        {
            output.WriteRawTag(10);
            output.WriteBytes(guid.ToByteArray());
        }
    }
}
