// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandGyroscopeReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal sealed class BandGyroscopeReading : BandAccelerometerReading, IBandGyroscopeReading, IBandAccelerometerReading, IBandSensorReading
    {
        private const double ConversionFactor = 0.0304878048780488;
        private new const int serializedByteCount = 12;

        private BandGyroscopeReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public double AngularVelocityX { get; private set; }

        public double AngularVelocityY { get; private set; }

        public double AngularVelocityZ { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.gyroscope == null)
                return;
            client.gyroscope.ProcessSensorReading(this);
        }

        internal new static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 12;

        internal static BandGyroscopeReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandGyroscopeReading gyroscopeReading = new(timestamp);
            gyroscopeReading.DeserializeFromBand(reader);
            return gyroscopeReading;
        }

        private new void DeserializeFromBand(ICargoReader reader)
        {
            base.DeserializeFromBand(reader);
            AngularVelocityX = reader.ReadInt16() * (5.0 / 164.0);
            AngularVelocityY = reader.ReadInt16() * (5.0 / 164.0);
            AngularVelocityZ = reader.ReadInt16() * (5.0 / 164.0);
        }
    }
}
