// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BufferServer
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal static class BufferServer
    {
        public static readonly BufferPool Pool_0016 = new(16, 25);
        public static readonly BufferPool Pool_0064 = new(64, 25);
        public static readonly BufferPool Pool_0256 = new(256, 25);
        public static readonly BufferPool Pool_1024 = new(1024, 25);
        public static readonly BufferPool Pool_4096 = new(4096, 25);
        public static readonly BufferPool Pool_8192 = new(8192, 25);
        public const int MaxBufferSize = 8192;

        public static BufferPool GetPool(int size) => GetPoolInternal(size) ?? throw new ArgumentOutOfRangeException(nameof(size));

        public static PooledBuffer GetBuffer(int size, bool zero = false)
        {
            BufferPool bufferPool = size >= 0 ? GetPoolInternal(size) : throw new ArgumentOutOfRangeException(nameof(size));
            return bufferPool == null ? new PooledBuffer(new byte[size], size) : bufferPool.GetBuffer(size, zero);
        }

        private static BufferPool GetPoolInternal(int size)
        {
            if (size <= 16)
                return Pool_0016;
            if (size <= 64)
                return Pool_0064;
            if (size <= 256)
                return Pool_0256;
            if (size <= 1024)
                return Pool_1024;
            if (size <= 4096)
                return Pool_4096;
            return size <= 8192 ? Pool_8192 : null;
        }
    }
}
