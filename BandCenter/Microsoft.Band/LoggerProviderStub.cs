// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.LoggerProviderStub
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Band
{
    internal class LoggerProviderStub : ILoggerProvider
    {
        public static readonly LoggerProviderStub _default = new();

        public static LoggerProviderStub Default => _default;

        public void Log(ProviderLogLevel level, string message, params object[] args)
        {
        }

        public void LogException(ProviderLogLevel level, Exception e)
        {
        }

        public void LogWebException(ProviderLogLevel level, WebException e)
        {
        }

        public void LogException(ProviderLogLevel level, Exception e, string message, params object[] args)
        {
        }

        public void PerfStart(string eventName)
        {
        }

        public void PerfEnd(string eventName)
        {
        }

        public void TelemetryEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> metrics)
        {
        }
    }
}
