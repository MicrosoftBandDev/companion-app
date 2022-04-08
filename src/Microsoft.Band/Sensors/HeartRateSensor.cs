// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.HeartRateSensor
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Band.Sensors
{
    internal sealed class HeartRateSensor : PulseSensor<IBandHeartRateReading>
    {
        public HeartRateSensor(BandClient bandClient)
            : base(bandClient, new List<BandType>() { BandType.Cargo, BandType.Envoy },
                new Dictionary<TimeSpan, SubscriptionType>()
                {
                    {
                        TimeSpan.FromSeconds(1.0),
                        SubscriptionType.HeartRate
                    }
                })
        {
        }
    }
}
