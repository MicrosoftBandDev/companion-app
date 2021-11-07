// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandSensorReadingBase
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal abstract class BandSensorReadingBase : IBandSensorReading
    {
        protected BandSensorReadingBase(DateTimeOffset timestamp) => Timestamp = timestamp;

        public DateTimeOffset Timestamp { get; private set; }

        internal abstract void Dispatch(BandClient client);
    }
}
