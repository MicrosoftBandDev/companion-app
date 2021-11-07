// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandDistanceReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal sealed class BandDistanceReading : BandSensorReadingBase, IBandDistanceReading, IBandSensorReading
    {
        private const int cargoSerializedByteCount = 22;
        private const int envoySerializedByteCount = 17;

        private BandDistanceReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public long TotalDistance { get; private set; }

        public long DistanceToday { get; private set; }

        public double Speed { get; private set; }

        public double Pace { get; private set; }

        public MotionType CurrentMotion { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.distance == null)
                return;
            client.distance.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header)
        {
            switch (header.SubscriptionType)
            {
                case SubscriptionType.Distance:
                    return 22;
                case SubscriptionType.DistanceWithDailyValues:
                    return 17;
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static BandDistanceReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandDistanceReading bandDistanceReading = new(timestamp);
            if (header.SubscriptionType == SubscriptionType.Distance)
            {
                bandDistanceReading.TotalDistance = reader.ReadUInt32();
                reader.ReadExactAndDiscard(8);
                bandDistanceReading.Speed = reader.ReadUInt32();
                bandDistanceReading.Pace = reader.ReadUInt32();
                bandDistanceReading.CurrentMotion = MotionType.Idle;
                reader.ReadExactAndDiscard(2);
            }
            else if (header.SubscriptionType == SubscriptionType.DistanceWithDailyValues)
            {
                bandDistanceReading.TotalDistance = reader.ReadUInt32();
                bandDistanceReading.Speed = reader.ReadUInt32();
                bandDistanceReading.Pace = reader.ReadUInt32();
                reader.ReadExactAndDiscard(1);
                bandDistanceReading.DistanceToday = reader.ReadUInt32();
            }
            return bandDistanceReading;
        }
    }
}
