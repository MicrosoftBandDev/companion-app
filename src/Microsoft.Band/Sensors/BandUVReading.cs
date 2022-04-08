// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandUVReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandUVReading : BandSensorReadingBase, IBandUVReading, IBandSensorReading
    {
        private const int cargoSerializedByteCount = 6;
        private const int envoySerializedByteCount = 5;

        private BandUVReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public UVIndexLevel IndexLevel { get; private set; }

        public long ExposureToday { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.uv == null)
                return;
            client.uv.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header)
        {
            switch (header.SubscriptionType)
            {
                case SubscriptionType.UV:
                    return 6;
                case SubscriptionType.UVWithDailyValues:
                    return 5;
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static BandUVReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandUVReading bandUvReading = new(timestamp);
            if (header.SubscriptionType == SubscriptionType.UV)
            {
                reader.ReadExactAndDiscard(2);
                bandUvReading.IndexLevel = (UVIndexLevel)reader.ReadUInt16();
                reader.ReadExactAndDiscard(2);
            }
            else
            {
                bandUvReading.IndexLevel = (UVIndexLevel)reader.ReadByte();
                bandUvReading.ExposureToday = reader.ReadUInt32();
            }
            return bandUvReading;
        }
    }
}
