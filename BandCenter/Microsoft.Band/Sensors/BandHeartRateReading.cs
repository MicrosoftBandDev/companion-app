// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandHeartRateReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandHeartRateReading : BandSensorReadingBase, IBandHeartRateReading, IBandSensorReading
    {
        private const int HeartRateLockedMinimumQualityValue = 6;
        private const int serializedByteCount = 2;

        private BandHeartRateReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public int HeartRate { get; private set; }

        public HeartRateQuality Quality { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.heartRate == null)
                return;
            client.heartRate.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 2;

        internal static BandHeartRateReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            return new BandHeartRateReading(timestamp)
            {
                HeartRate = reader.ReadByte(),
                Quality = reader.ReadByte() >= 6 ? HeartRateQuality.Locked : HeartRateQuality.Acquiring
            };
        }
    }
}
