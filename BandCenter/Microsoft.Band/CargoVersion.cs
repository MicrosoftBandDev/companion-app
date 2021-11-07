// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.CargoVersion
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System.Text;

namespace Microsoft.Band
{
    internal class CargoVersion
    {
        private const int maxAppNameCharCount = 5;
        private const int serializedByteCount = 19;

        public string AppName { get; private set; }

        public byte PCBId { get; private set; }

        public ushort VersionMajor { get; private set; }

        public ushort VersionMinor { get; private set; }

        public uint Revision { get; private set; }

        public uint BuildNumber { get; private set; }

        public byte DebugBuild { get; private set; }

        internal static int GetSerializedByteCount() => 19;

        internal static CargoVersion DeserializeFromBand(ICargoReader reader)
        {
            CargoVersion cargoVersion = new();
            using (PooledBuffer buffer = BufferServer.GetBuffer(5))
            {
                reader.ReadExact(buffer.Buffer, 0, buffer.Length);
                int count = 0;
                while (count < buffer.Length && buffer.Buffer[count] != 0)
                    ++count;
                cargoVersion.AppName = Encoding.UTF8.GetString(buffer.Buffer, 0, count);
                cargoVersion.PCBId = reader.ReadByte();
                cargoVersion.VersionMajor = reader.ReadUInt16();
                cargoVersion.VersionMinor = reader.ReadUInt16();
                cargoVersion.Revision = reader.ReadUInt32();
                cargoVersion.BuildNumber = reader.ReadUInt32();
                cargoVersion.DebugBuild = reader.ReadByte();
            }
            return cargoVersion;
        }
    }
}
