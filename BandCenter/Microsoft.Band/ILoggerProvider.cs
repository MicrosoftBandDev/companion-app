// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ILoggerProvider
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Band
{
    internal interface ILoggerProvider
    {
        void Log(ProviderLogLevel level, string message, params object[] args);

        void LogException(ProviderLogLevel level, Exception e);

        void LogWebException(ProviderLogLevel level, WebException e);

        void LogException(ProviderLogLevel level, Exception e, string message, params object[] args);

        void PerfStart(string eventName);

        void PerfEnd(string eventName);

        void TelemetryEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics);
    }
}
