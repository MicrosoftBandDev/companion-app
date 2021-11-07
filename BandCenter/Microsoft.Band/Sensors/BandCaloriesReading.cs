// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandCaloriesReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandCaloriesReading : BandSensorReadingBase, IBandCaloriesReading, IBandSensorReading
    {
        private const int cargoSerializedByteCount = 20;
        private const int envoySerializedByteCount = 8;

        private BandCaloriesReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public long Calories { get; private set; }

        public long CaloriesToday { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.calories == null)
                return;
            client.calories.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header)
        {
            switch (header.SubscriptionType)
            {
                case SubscriptionType.Calories1S:
                    return 20;
                case SubscriptionType.CaloriesWithDailyValues:
                    return 8;
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static BandCaloriesReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandCaloriesReading bandCaloriesReading = new(timestamp);
            if (header.SubscriptionType == SubscriptionType.Calories1S)
            {
                bandCaloriesReading.Calories = reader.ReadUInt32();
                reader.ReadExactAndDiscard(16);
            }
            else
            {
                bandCaloriesReading.Calories = reader.ReadUInt32();
                bandCaloriesReading.CaloriesToday = reader.ReadUInt32();
            }
            return bandCaloriesReading;
        }
    }
}
