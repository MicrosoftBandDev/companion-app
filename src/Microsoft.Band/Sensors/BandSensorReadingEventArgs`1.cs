// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandSensorReadingEventArgs`1
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    public class BandSensorReadingEventArgs<T> : EventArgs where T : IBandSensorReading
    {
        private readonly T sensorReading;

        public BandSensorReadingEventArgs(T reading) => sensorReading = reading;

        public T SensorReading => sensorReading;
    }
}
