// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandSkinTemperatureReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandSkinTemperatureReading : BandSensorReadingBase, IBandSkinTemperatureReading, IBandSensorReading
    {
        private const int ConversionFactor = 100;
        private const int serializedByteCount = 10;

        private BandSkinTemperatureReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public double Temperature { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.skinTemperature == null)
                return;
            client.skinTemperature.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 10;

        internal static BandSkinTemperatureReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandSkinTemperatureReading temperatureReading = new(timestamp);
            temperatureReading.Temperature = reader.ReadInt16() / 100.0;
            reader.ReadExactAndDiscard(8);
            return temperatureReading;
        }
    }
}
