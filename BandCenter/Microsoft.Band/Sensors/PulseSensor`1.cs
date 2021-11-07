// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.PulseSensor`1
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band.Sensors
{
    internal abstract class PulseSensor<T> : BandSensorBase<T> where T : IBandSensorReading
    {
        protected PulseSensor(
          BandClient bandClient,
          IEnumerable<BandType> supportedBandClasses,
          Dictionary<TimeSpan, SubscriptionType> supportedReportingSubscriptions)
          : base(bandClient, supportedBandClasses, supportedReportingSubscriptions)
        {
        }

        public override UserConsent GetCurrentUserConsent() => ClientHandle.applicationPlatformProvider.GetCurrentSensorConsent(typeof(HeartRateSensor));

        public override async Task<bool> RequestUserConsentAsync(CancellationToken token)
        {
            int num = await ClientHandle.applicationPlatformProvider.RequestSensorConsentAsync(typeof(HeartRateSensor), BandResources.HeartRateSensorConsentPrompt, token) ? 1 : 0;
            if (num == 0)
            {
                ClientHandle.SensorUnsubscribe(SubscriptionType.HeartRate);
                ClientHandle.SensorUnsubscribe(SubscriptionType.RRInterval);
            }
            return num != 0;
        }
    }
}
