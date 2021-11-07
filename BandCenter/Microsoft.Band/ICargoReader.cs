// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ICargoReader
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal interface ICargoReader : IDisposable
    {
        int Read(byte[] buffer, int offset, int count);

        int Read(byte[] buffer);

        void ReadExact(byte[] buffer, int offset, int count);

        byte[] ReadExact(int count);

        void ReadExactAndDiscard(int count);

        byte ReadByte();

        short ReadInt16();

        ushort ReadUInt16();

        int ReadInt32();

        uint ReadUInt32();

        long ReadInt64();

        ulong ReadUInt64();

        bool ReadBool32();

        Guid ReadGuid();

        string ReadString(int length);
    }
}
