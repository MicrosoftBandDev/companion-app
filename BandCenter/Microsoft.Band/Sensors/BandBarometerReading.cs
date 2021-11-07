// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandBarometerReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandBarometerReading : BandSensorReadingBase, IBandBarometerReading, IBandSensorReading
    {
        private const double AirPressureConversionFactor = 4096.0;
        private const double TemperatureConversionFactor = 480.0;
        private const double TemperatureConversionOffset = 42.5;
        private const int serializedByteCount = 6;

        private BandBarometerReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public double AirPressure { get; private set; }

        public double Temperature { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.barometer == null)
                return;
            client.barometer.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 6;

        internal static BandBarometerReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            return new BandBarometerReading(timestamp)
            {
                AirPressure = reader.ReadInt32() / 4096.0,
                Temperature = reader.ReadInt16() / 480.0 + 42.5
            };
        }
    }
}
