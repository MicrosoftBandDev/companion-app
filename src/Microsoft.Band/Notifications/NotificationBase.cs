// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Notifications.NotificationBase
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Google.Protobuf;
using System;

namespace Microsoft.Band.Notifications
{
    internal abstract class NotificationBase
    {
        protected NotificationBase(Guid tileId) => TileId = tileId;

        protected NotificationBase(ref Guid tileId) => TileId = tileId;

        public Guid TileId { get; private set; }

        internal virtual int GetSerializedByteCount() => 16;

        internal virtual void SerializeToBand(ICargoWriter writer) => writer.WriteGuid(TileId);

        internal abstract int GetSerializedProtobufByteCount();

        internal abstract void SerializeProtobufToBand(CodedOutputStream output);
    }
}
