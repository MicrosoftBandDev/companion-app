// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandAccessDeniedException
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: .\netcore451\Microsoft.Band.Store.dll

using System;

namespace Microsoft.Band
{
    public class BandAccessDeniedException : BandIOException
    {
        internal BandAccessDeniedException()
        {
        }

        internal BandAccessDeniedException(string message) : base(message)
        {
        }

        internal BandAccessDeniedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
