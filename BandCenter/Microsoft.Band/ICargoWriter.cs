// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ICargoWriter
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal interface ICargoWriter : IDisposable
    {
        void Write(byte[] buffer, int offset, int count);

        void Write(byte[] buffer);

        void WriteByte(byte b);

        void WriteByte(byte b, int count);

        void WriteInt16(short i);

        void WriteUInt16(ushort i);

        void WriteInt32(int i);

        void WriteUInt32(uint i);

        void WriteInt64(long i);

        void WriteUInt64(ulong i);

        void WriteBool32(bool b);

        void WriteGuid(Guid guid);

        void WriteString(string s);

        void WriteStringWithPadding(string s, int exactLength);

        void WriteStringWithTruncation(string s, int maxLength);

        void Flush();
    }
}
