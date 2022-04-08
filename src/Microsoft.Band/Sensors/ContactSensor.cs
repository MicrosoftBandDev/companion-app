// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.ContactSensor
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Band.Sensors
{
    internal sealed class ContactSensor : BandSensorBase<IBandContactReading>, IBandContactSensor, IBandSensor<IBandContactReading>
    {
        private IBandContactReading lastReading;

        public ContactSensor(BandClient bandClient)
            : base(bandClient, new List<BandType>() { BandType.Cargo, BandType.Envoy },
                new Dictionary<TimeSpan, SubscriptionType>()
                {
                    {
                        TimeSpan.Zero,
                        SubscriptionType.DeviceContact
                    }
                })
        {
        }

        public event EventHandler<BandSensorReadingEventArgs<IBandContactReading>> InternalReadingChanged;

        public Task<IBandContactReading> GetCurrentStateAsync() => Task.Run(() =>
        {
            lock (ReadingsLock)
            {
                if (ClientHandle.IsSensorSubscribed(supportedReportingSubscriptions[ReportingInterval]))
                    return lastReading;
                TaskCompletionSource<IBandContactReading> contactTask = new();
                InternalReadingChanged += (sender, args) => contactTask.TrySetResult(args.SensorReading);
                ClientHandle.SensorSubscribe(SubscriptionType.DeviceContact);
                IBandContactReading result = contactTask.Task.Result;
                ClientHandle.SensorUnsubscribe(SubscriptionType.DeviceContact);
                return result;
            }
        });

        public override void ProcessSensorReading(IBandContactReading reading)
        {
            lastReading = reading ?? throw new ArgumentNullException(nameof(reading));
            if (InternalReadingChanged != null)
                InternalReadingChanged(this, new BandSensorReadingEventArgs<IBandContactReading>(reading));
            base.ProcessSensorReading(reading);
        }
    }
}
