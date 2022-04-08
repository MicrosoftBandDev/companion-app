// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandOperationException
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    public sealed class BandOperationException : BandIOException
    {
        internal BandOperationException(uint hResult) => HResult = hResult;

        internal BandOperationException(uint hResult, string message) : base(message)
        {
            HResult = hResult;
        }

        internal BandOperationException(uint hResult, string message, Exception innerException) : base(message, innerException)
        {
            HResult = hResult;
        }

        public uint HResult { get; private set; }
    }
}
