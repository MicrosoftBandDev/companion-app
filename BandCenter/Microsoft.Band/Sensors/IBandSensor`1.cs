// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.IBandSensor`1
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band.Sensors
{
    public interface IBandSensor<T> where T : IBandSensorReading
    {
        bool IsSupported { get; }

        IEnumerable<TimeSpan> SupportedReportingIntervals { get; }

        TimeSpan ReportingInterval { get; set; }

        event EventHandler<BandSensorReadingEventArgs<T>> ReadingChanged;

        UserConsent GetCurrentUserConsent();

        Task<bool> RequestUserConsentAsync();

        Task<bool> RequestUserConsentAsync(CancellationToken token);

        Task<bool> StartReadingsAsync();

        Task<bool> StartReadingsAsync(CancellationToken token);

        Task StopReadingsAsync();

        Task StopReadingsAsync(CancellationToken token);
    }
}
