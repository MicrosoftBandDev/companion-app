// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Notifications.NotificationMessaging
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Google.Protobuf;
using Microsoft.Band.Protobuf;
using System;

namespace Microsoft.Band.Notifications
{
    internal class NotificationMessaging : NotificationBase
    {
        public NotificationMessaging(Guid tileId) : base(tileId)
        {
        }

        public DateTimeOffset Timestamp { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public byte Flags { get; set; }

        internal override int GetSerializedByteCount()
        {
            int num1 = 0;
            int num2 = 0;
            if (!string.IsNullOrEmpty(Title))
                num1 = Title.LengthTruncatedTrimDanglingHighSurrogate(20);
            if (!string.IsNullOrEmpty(Body))
                num2 = Body.LengthTruncatedTrimDanglingHighSurrogate(160);
            return base.GetSerializedByteCount() + CargoFileTime.GetSerializedByteCount() + 6 + (num1 * 2) + (num2 * 2);
        }

        internal override void SerializeToBand(ICargoWriter writer)
        {
            int maxLength1 = 0;
            int maxLength2 = 0;
            if (!string.IsNullOrEmpty(Title))
                maxLength1 = Title.LengthTruncatedTrimDanglingHighSurrogate(20);
            if (!string.IsNullOrEmpty(Body))
                maxLength2 = Body.LengthTruncatedTrimDanglingHighSurrogate(160);
            base.SerializeToBand(writer);
            writer.WriteUInt16((ushort)(maxLength1 * 2));
            writer.WriteUInt16((ushort)(maxLength2 * 2));
            CargoFileTime.SerializeToBandFromDateTimeOffset(writer, Timestamp);
            writer.WriteByte(Flags);
            writer.WriteByte(0);
            if (maxLength1 > 0)
                writer.WriteStringWithTruncation(Title, maxLength1);
            if (maxLength2 <= 0)
                return;
            writer.WriteStringWithTruncation(Body, maxLength2);
        }

        internal override int GetSerializedProtobufByteCount()
        {
            int num = 0 + 2 + ProtoFileTime.GetSerializedProtobufByteCount() + 2 + ProtoGuid.GetSerializedProtobufByteCount();
            if (!string.IsNullOrEmpty(Title))
                num += 1 + CodedOutputStreamExtensions.ComputeStringSize(Title, 40);
            if (!string.IsNullOrEmpty(Body))
                num += 1 + CodedOutputStreamExtensions.ComputeStringSize(Body, 320);
            if (Flags != 0)
                num += 1 + CodedOutputStream.ComputeUInt32Size(Flags);
            return num;
        }

        internal override void SerializeProtobufToBand(CodedOutputStream output)
        {
            output.WriteRawTag(10);
            output.WriteLength(ProtoFileTime.GetSerializedProtobufByteCount());
            ProtoFileTime.SerializeProtobufToBand(output, Timestamp);
            output.WriteRawTag(18);
            output.WriteLength(ProtoGuid.GetSerializedProtobufByteCount());
            ProtoGuid.SerializeProtobufToBand(output, TileId);
            if (!string.IsNullOrEmpty(Title))
            {
                output.WriteRawTag(42);
                output.WriteString(Title, 40);
            }
            if (!string.IsNullOrEmpty(Body))
            {
                output.WriteRawTag(50);
                output.WriteString(Body, 320);
            }
            if (Flags == 0)
                return;
            output.WriteRawTag(80);
            output.WriteUInt32(Flags);
        }
    }
}
