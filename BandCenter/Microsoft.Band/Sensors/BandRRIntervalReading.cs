// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandRRIntervalReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandRRIntervalReading : BandSensorReadingBase, IBandRRIntervalReading, IBandSensorReading
    {
        private const double RRIntervalConversionFactor = 0.016592;
        private const int serializedByteCount = 6;

        private BandRRIntervalReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public double Interval { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.rrInterval == null)
                return;
            client.rrInterval.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 6;

        internal static BandRRIntervalReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            reader.ReadExactAndDiscard(4);
            return new BandRRIntervalReading(timestamp)
            {
                Interval = reader.ReadUInt16() * 0.016592
            };
        }
    }
}
