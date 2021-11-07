// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.PooledBuffer
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal sealed class PooledBuffer : IDisposable
    {
        private BufferPool pool;
        private byte[] buffer;
        private int length;
        private bool disposed;

        internal PooledBuffer(byte[] buffer, int length) : this(null, buffer, length)
        {
        }

        internal PooledBuffer(BufferPool pool, byte[] buffer, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (length < 0 || length > buffer.Length)
                throw new ArgumentOutOfRangeException("size");
            if (pool != null && buffer.Length != pool.BufferSize)
                throw new ArgumentException("The provided buffer does not belong to the provided pool");
            this.pool = pool;
            this.buffer = buffer;
            this.length = length;
        }

        internal byte[] Buffer
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(PooledBuffer));
                return buffer;
            }
        }

        internal int Length
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(PooledBuffer));
                return length;
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(PooledBuffer));
                if (value < 0 || value > buffer.Length)
                    throw new ArgumentOutOfRangeException();
                length = value;
            }
        }

        public void Dispose()
        {
            if (pool == null || disposed)
                return;
            pool.ReleaseBuffer(this);
            disposed = true;
        }

        internal void Undispose(int length)
        {
            if (length < 0 || length > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(length));
            disposed = false;
            this.length = length;
        }
    }
}
