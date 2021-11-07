// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandException
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    public class BandException : Exception
    {
        internal BandException()
        {
        }

        internal BandException(string message) : base(message)
        {
        }

        internal BandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
