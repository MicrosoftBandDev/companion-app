// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandGsrReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandGsrReading : BandSensorReadingBase, IBandGsrReading, IBandSensorReading
    {
        private const int serializedByteCount = 7;

        private BandGsrReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public int Resistance { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.gsr == null)
                return;
            client.gsr.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 7;

        internal static BandGsrReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandGsrReading bandGsrReading = new(timestamp);
            reader.ReadExactAndDiscard(1);
            bandGsrReading.Resistance = (int)reader.ReadUInt32();
            reader.ReadExactAndDiscard(2);
            return bandGsrReading;
        }
    }
}
