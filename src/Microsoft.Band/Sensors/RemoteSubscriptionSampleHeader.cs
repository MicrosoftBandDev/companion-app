// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.RemoteSubscriptionSampleHeader
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Sensors
{
    internal class RemoteSubscriptionSampleHeader
    {
        private const int serializedByteCount = 4;

        private RemoteSubscriptionSampleHeader()
        {
        }

        public SubscriptionType SubscriptionType { get; private set; }

        public byte NumMissedSamples { get; private set; }

        public short SampleSize { get; private set; }

        internal static int GetSerializedByteCount() => 4;

        internal static RemoteSubscriptionSampleHeader DeserializeFromBand(ICargoReader reader)
        {
            return new RemoteSubscriptionSampleHeader()
            {
                SubscriptionType = (SubscriptionType)reader.ReadByte(),
                NumMissedSamples = reader.ReadByte(),
                SampleSize = reader.ReadInt16()
            };
        }
    }
}
