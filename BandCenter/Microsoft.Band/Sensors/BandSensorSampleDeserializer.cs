// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandSensorSampleDeserializer
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandSensorSampleDeserializer
    {
        public BandSensorSampleDeserializer(
          Func<RemoteSubscriptionSampleHeader, int> getSerializeByteCount,
          Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase> deserializeFromBand)
        {
            GetSerializeByteCount = getSerializeByteCount;
            DeserializeFromBand = deserializeFromBand;
        }

        public Func<RemoteSubscriptionSampleHeader, int> GetSerializeByteCount { get; private set; }

        public Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase> DeserializeFromBand { get; private set; }

        internal static BandSensorSampleDeserializer[] InitDeserializerTable()
        {
            BandSensorSampleDeserializer[] sampleDeserializerArray = new BandSensorSampleDeserializer[125];
            BandSensorSampleDeserializer sampleDeserializer1 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandAccelerometerReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandAccelerometerReading.DeserializeFromBand));
            sampleDeserializerArray[0] = sampleDeserializer1;
            sampleDeserializerArray[1] = sampleDeserializer1;
            sampleDeserializerArray[48] = sampleDeserializer1;
            BandSensorSampleDeserializer sampleDeserializer2 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandGyroscopeReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandGyroscopeReading.DeserializeFromBand));
            sampleDeserializerArray[4] = sampleDeserializer2;
            sampleDeserializerArray[5] = sampleDeserializer2;
            sampleDeserializerArray[49] = sampleDeserializer2;
            sampleDeserializerArray[35] = new BandSensorSampleDeserializer(new Func<RemoteSubscriptionSampleHeader, int>(BandContactReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandContactReading.DeserializeFromBand));
            BandSensorSampleDeserializer sampleDeserializer3 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandDistanceReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandDistanceReading.DeserializeFromBand));
            sampleDeserializerArray[13] = sampleDeserializer3;
            sampleDeserializerArray[108] = sampleDeserializer3;
            sampleDeserializerArray[16] = new BandSensorSampleDeserializer(new Func<RemoteSubscriptionSampleHeader, int>(BandHeartRateReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandHeartRateReading.DeserializeFromBand));
            BandSensorSampleDeserializer sampleDeserializer4 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandPedometerReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandPedometerReading.DeserializeFromBand));
            sampleDeserializerArray[19] = sampleDeserializer4;
            sampleDeserializerArray[109] = sampleDeserializer4;
            BandSensorSampleDeserializer sampleDeserializer5 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandCaloriesReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandCaloriesReading.DeserializeFromBand));
            sampleDeserializerArray[46] = sampleDeserializer5;
            sampleDeserializerArray[107] = sampleDeserializer5;
            sampleDeserializerArray[20] = new BandSensorSampleDeserializer(new Func<RemoteSubscriptionSampleHeader, int>(BandSkinTemperatureReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandSkinTemperatureReading.DeserializeFromBand));
            BandSensorSampleDeserializer sampleDeserializer6 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandUVReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandUVReading.DeserializeFromBand));
            sampleDeserializerArray[21] = sampleDeserializer6;
            sampleDeserializerArray[40] = sampleDeserializer6;
            sampleDeserializerArray[111] = sampleDeserializer6;
            BandSensorSampleDeserializer sampleDeserializer7 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandGsrReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandGsrReading.DeserializeFromBand));
            sampleDeserializerArray[15] = sampleDeserializer7;
            sampleDeserializerArray[113] = sampleDeserializer7;
            sampleDeserializerArray[26] = new BandSensorSampleDeserializer(new Func<RemoteSubscriptionSampleHeader, int>(BandRRIntervalReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandRRIntervalReading.DeserializeFromBand));
            sampleDeserializerArray[25] = new BandSensorSampleDeserializer(new Func<RemoteSubscriptionSampleHeader, int>(BandAmbientLightReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandAmbientLightReading.DeserializeFromBand));
            sampleDeserializerArray[58] = new BandSensorSampleDeserializer(new Func<RemoteSubscriptionSampleHeader, int>(BandBarometerReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandBarometerReading.DeserializeFromBand));
            BandSensorSampleDeserializer sampleDeserializer8 = new(new Func<RemoteSubscriptionSampleHeader, int>(BandAltimeterReading.GetSerializedByteCount), new Func<ICargoReader, RemoteSubscriptionSampleHeader, DateTimeOffset, BandSensorReadingBase>(BandAltimeterReading.DeserializeFromBand));
            sampleDeserializerArray[71] = sampleDeserializer8;
            sampleDeserializerArray[110] = sampleDeserializer8;
            return sampleDeserializerArray;
        }
    }
}
