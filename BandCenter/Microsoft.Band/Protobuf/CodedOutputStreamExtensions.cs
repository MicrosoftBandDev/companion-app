// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Protobuf.CodedOutputStreamExtensions
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Google.Protobuf;
using System;

namespace Microsoft.Band.Protobuf
{
    internal static class CodedOutputStreamExtensions
    {
        public static int ComputeStringSize(string value, int maxByteCount)
        {
            int danglingHighSurrogate = BandUtf8Encoding.GetUtf8ByteCountTrimDanglingHighSurrogate(value, maxByteCount);
            return CodedOutputStream.ComputeLengthSize(danglingHighSurrogate) + danglingHighSurrogate;
        }

        public static void WriteString(this CodedOutputStream output, string value, int maxByteCount)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (maxByteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxByteCount));
            output.WriteString(value.Truncate(BandUtf8Encoding.GetCharCountToMaxUtf8ByteCountTrimDanglingHighSurrogate(value, maxByteCount)));
        }

        public static void WriteBytes(this CodedOutputStream output, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            output.WriteBytes(ByteString.CopyFrom(value));
        }
    }
}
