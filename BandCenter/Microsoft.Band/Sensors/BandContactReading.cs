// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandContactReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandContactReading : BandSensorReadingBase, IBandContactReading, IBandSensorReading
    {
        private const int serializedByteCount = 3;

        private BandContactReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public BandContactState State { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.contact == null)
                return;
            client.contact.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 3;

        internal static BandContactReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            BandContactReading bandContactReading = new(timestamp);
            bandContactReading.State = (BandContactState)reader.ReadByte();
            reader.ReadExactAndDiscard(2);
            return bandContactReading;
        }
    }
}
