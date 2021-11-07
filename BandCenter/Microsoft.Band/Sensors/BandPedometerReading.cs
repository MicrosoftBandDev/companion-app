// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandPedometerReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandPedometerReading : BandSensorReadingBase, IBandPedometerReading, IBandSensorReading
    {
        private const int cargoSerializedByteCount = 13;
        private const int envoySerializedByteCount = 8;

        private BandPedometerReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public long TotalSteps { get; private set; }

        public long StepsToday { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.pedometer == null)
                return;
            client.pedometer.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header)
        {
            switch (header.SubscriptionType)
            {
                case SubscriptionType.Pedometer:
                    return 13;
                case SubscriptionType.PedometerWithDailyValues:
                    return 8;
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static BandPedometerReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandPedometerReading pedometerReading = new(timestamp);
            if (header.SubscriptionType == SubscriptionType.Pedometer)
            {
                pedometerReading.TotalSteps = reader.ReadUInt32();
                reader.ReadExactAndDiscard(9);
            }
            else if (header.SubscriptionType == SubscriptionType.PedometerWithDailyValues)
            {
                pedometerReading.TotalSteps = reader.ReadUInt32();
                pedometerReading.StepsToday = reader.ReadUInt32();
            }
            return pedometerReading;
        }
    }
}
