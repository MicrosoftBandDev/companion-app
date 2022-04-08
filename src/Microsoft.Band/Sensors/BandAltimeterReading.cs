// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandAltimeterReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandAltimeterReading : BandSensorReadingBase, IBandAltimeterReading, IBandSensorReading
    {
        private const int cargoSerializedByteCount = 50;
        private const int envoySerializedByteCount = 42;

        private BandAltimeterReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public long TotalGain { get; private set; }

        public long TotalLoss { get; private set; }

        public long SteppingGain { get; private set; }

        public long SteppingLoss { get; private set; }

        public long StepsAscended { get; private set; }

        public long StepsDescended { get; private set; }

        public float Rate { get; private set; }

        public long FlightsAscended { get; private set; }

        public long FlightsDescended { get; private set; }

        public long FlightsAscendedToday { get; private set; }

        public long TotalGainToday { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.altimeter == null)
                return;
            client.altimeter.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header)
        {
            switch (header.SubscriptionType)
            {
                case SubscriptionType.Elevation:
                    return 50;
                case SubscriptionType.ElevationWithDailyValues:
                    return 42;
                default:
                    throw new InvalidOperationException();
            }
        }

        internal static BandAltimeterReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandAltimeterReading altimeterReading = new(timestamp);
            if (header.SubscriptionType == SubscriptionType.ElevationWithDailyValues)
            {
                altimeterReading.TotalGain = reader.ReadUInt32();
                altimeterReading.TotalLoss = reader.ReadUInt32();
                altimeterReading.SteppingGain = reader.ReadUInt32();
                altimeterReading.SteppingLoss = reader.ReadUInt32();
                altimeterReading.StepsAscended = reader.ReadUInt32();
                altimeterReading.StepsDescended = reader.ReadUInt32();
                altimeterReading.Rate = reader.ReadInt16();
                altimeterReading.FlightsAscended = reader.ReadUInt32();
                altimeterReading.FlightsDescended = reader.ReadUInt32();
                altimeterReading.FlightsAscendedToday = reader.ReadUInt32();
                altimeterReading.TotalGainToday = reader.ReadUInt32();
            }
            else
            {
                reader.ReadExactAndDiscard(8);
                altimeterReading.TotalGain = reader.ReadUInt32();
                altimeterReading.TotalLoss = reader.ReadUInt32();
                altimeterReading.SteppingGain = reader.ReadUInt32();
                altimeterReading.SteppingLoss = reader.ReadUInt32();
                reader.ReadExactAndDiscard(8);
                altimeterReading.StepsAscended = reader.ReadUInt32();
                altimeterReading.StepsDescended = reader.ReadUInt32();
                altimeterReading.Rate = reader.ReadInt16();
                altimeterReading.FlightsAscended = reader.ReadUInt32();
                altimeterReading.FlightsDescended = reader.ReadUInt32();
            }
            return altimeterReading;
        }
    }
}
