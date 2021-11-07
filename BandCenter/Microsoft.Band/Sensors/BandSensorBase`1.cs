// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.BandSensorBase`1
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band.Sensors
{
    internal abstract class BandSensorBase<T> : IBandSensor<T> where T : IBandSensorReading
    {
        private object readingsLock;
        private BandClient clientHandle;
        private bool isSupported;
        protected Dictionary<TimeSpan, SubscriptionType> supportedReportingSubscriptions;
        private TimeSpan reportingInterval;

        protected BandSensorBase(BandClient bandClient, IEnumerable<BandType> supportedBandClasses, Dictionary<TimeSpan, SubscriptionType> supportedReportingSubscriptions)
        {
            if (supportedBandClasses == null)
                throw new ArgumentNullException(nameof(supportedBandClasses));
            clientHandle = bandClient ?? throw new ArgumentNullException(nameof(bandClient));
            isSupported = supportedBandClasses.Contains(bandClient.BandTypeConstants.BandType);
            this.supportedReportingSubscriptions = supportedReportingSubscriptions ?? throw new ArgumentNullException(nameof(supportedReportingSubscriptions));
            reportingInterval = this.supportedReportingSubscriptions.Keys.FirstOrDefault();
            readingsLock = new object();
        }

        protected object ReadingsLock => readingsLock;

        protected BandClient ClientHandle => clientHandle;

        public bool IsSupported => isSupported;

        public IEnumerable<TimeSpan> SupportedReportingIntervals => supportedReportingSubscriptions.Keys;

        public TimeSpan ReportingInterval
        {
            get => reportingInterval;
            set
            {
                if (!supportedReportingSubscriptions.Keys.Contains(value))
                    throw new ArgumentOutOfRangeException(BandResources.UnsupportedSensorInterval);
                reportingInterval = value;
            }
        }

        public event EventHandler<BandSensorReadingEventArgs<T>> ReadingChanged;

        public virtual UserConsent GetCurrentUserConsent() => UserConsent.Granted;

        public Task<bool> RequestUserConsentAsync() => RequestUserConsentAsync(CancellationToken.None);

        public virtual Task<bool> RequestUserConsentAsync(CancellationToken token) => Task.FromResult(true);

        public Task<bool> StartReadingsAsync() => StartReadingsAsync(CancellationToken.None);

        public virtual async Task<bool> StartReadingsAsync(CancellationToken token)
        {
            if (!isSupported)
                throw new InvalidOperationException(BandResources.UnsupportedSensor);
            switch (GetCurrentUserConsent())
            {
                case UserConsent.NotSpecified:
                    throw new InvalidOperationException(BandResources.SensorUserConsentNotQueried);
                case UserConsent.Declined:
                    return false;
                default:
                    SubscriptionType type = supportedReportingSubscriptions[reportingInterval];
                    await Task.Run(() =>
                    {
                        lock (readingsLock)
                            clientHandle.SensorSubscribe(type);
                    }, token);
                    return true;
            }
        }

        public Task StopReadingsAsync() => StopReadingsAsync(CancellationToken.None);

        public virtual Task StopReadingsAsync(CancellationToken token)
        {
            if (!isSupported)
                throw new InvalidOperationException(BandResources.UnsupportedSensor);
            SubscriptionType type = supportedReportingSubscriptions[reportingInterval];
            return Task.Run(() =>
            {
                lock (readingsLock)
                    clientHandle.SensorUnsubscribe(type);
            }, token);
        }

        public virtual void ProcessSensorReading(T reading)
        {
            if (reading == null)
                throw new ArgumentNullException(nameof(reading));
            EventHandler<BandSensorReadingEventArgs<T>> readingChanged = ReadingChanged;
            if (readingChanged == null)
                return;
            if (!clientHandle.IsSensorSubscribed(supportedReportingSubscriptions[reportingInterval]))
                return;
            try
            {
                readingChanged(this, new BandSensorReadingEventArgs<T>(reading));
            }
            catch (Exception ex)
            {
                clientHandle.loggerProvider.LogException(ProviderLogLevel.Error, ex);
                Environment.FailFast("BandSensorBase.ReadingChanged event handler threw an exception that was not handled by the application.", ex);
            }
        }
    }
}
