// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.DisposableList`1
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Band
{
    internal class DisposableList<T> : List<T>, IDisposable where T : IDisposable
    {
        public DisposableList()
        {
        }

        public DisposableList(int capacity) : base(capacity)
        {
        }

        public DisposableList(IEnumerable<T> collection) : base(collection)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Dispose()
        {
            foreach (T obj in this)
            {
                IDisposable disposable = obj;
                try
                {
                    disposable?.Dispose();
                }
                catch
                {
                }
            }
            Clear();
        }
    }
}
