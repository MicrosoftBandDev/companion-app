// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandAmbientLightReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Sensors
{
    internal class BandAmbientLightReading : BandSensorReadingBase, IBandAmbientLightReading, IBandSensorReading
    {
        private const int serializedByteCount = 2;

        private BandAmbientLightReading(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        public int Brightness { get; private set; }

        internal override void Dispatch(BandClient client)
        {
            if (client.als == null)
                return;
            client.als.ProcessSensorReading(this);
        }

        internal static int GetSerializedByteCount(RemoteSubscriptionSampleHeader header) => 2;

        internal static BandAmbientLightReading DeserializeFromBand(
          ICargoReader reader,
          RemoteSubscriptionSampleHeader header,
          DateTimeOffset timestamp)
        {
            return new BandAmbientLightReading(timestamp)
            {
                Brightness = reader.ReadUInt16()
            };
        }
    }
}
