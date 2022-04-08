// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandAccelerometerReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandAccelerometerReading : BandSensorReadingBase, IBandAccelerometerReading, IBandSensorReading
    {
        private const double ConversionFactor = 0.000244140625;
        protected const int serializedByteCount = 6;

        protected BandAccelerometerReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public double AccelerationX { get; private set; }

        public double AccelerationY { get; private set; }

        public double AccelerationZ { get; private set; }

        internal override void Dispatch(BandClient client) => client.accelerometer.ProcessSensorReading(this);

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 6;

        internal static BandAccelerometerReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandAccelerometerReading accelerometerReading = new(timestamp);
            accelerometerReading.DeserializeFromBand(reader);
            return accelerometerReading;
        }

        protected void DeserializeFromBand(ICargoReader reader)
        {
            AccelerationX = reader.ReadInt16() * 0.000244140625;
            AccelerationY = reader.ReadInt16() * 0.000244140625;
            AccelerationZ = reader.ReadInt16() * 0.000244140625;
        }
    }
}
