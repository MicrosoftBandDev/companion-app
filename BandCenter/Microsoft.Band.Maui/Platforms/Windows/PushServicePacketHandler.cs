// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.PushServicePacketHeader
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: .\netcore451\Microsoft.Band.Store.dll

namespace Microsoft.Band.Windows
{
    internal class PushServicePacketHeader
    {
        private PushServicePacketHeader()
        {
        }

        public PushServicePacketType Type { get; private set; }

        public int Length { get; private set; }

        internal static PushServicePacketHeader DeserializeFromBand(ICargoReader reader)
        {
            return new PushServicePacketHeader()
            {
                Type = (PushServicePacketType)reader.ReadUInt16(),
                Length = reader.ReadInt32()
            };
        }
    }
}
