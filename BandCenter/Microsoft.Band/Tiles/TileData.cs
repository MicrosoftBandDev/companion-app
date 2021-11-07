// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.TileData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Text;

namespace Microsoft.Band.Tiles
{
    internal class TileData
    {
        private Guid appID;
        private const int serializedByteCount = 88;

        public Guid AppID
        {
            get => appID;
            set => appID = value;
        }

        public uint StartStripOrder { get; set; }

        public uint ThemeColor { get; set; }

        public ushort FriendlyNameLength { get; set; }

        public TileSettings SettingsMask { get; set; }

        public byte[] NameAndOwnerId { get; set; }

        public string FriendlyName => NameAndOwnerId != null && FriendlyNameLength > 0 && NameAndOwnerId.Length >= FriendlyNameLength * 2 ? Encoding.Unicode.GetString(NameAndOwnerId, 0, FriendlyNameLength * 2) : string.Empty;

        public Guid OwnerId => BandClient.GetApplicationIdFromName(NameAndOwnerId, FriendlyNameLength);

        public BandIcon Icon { get; set; }

        public void SetNameAndOwnerId(string name, Guid ownerId)
        {
            if (name != null && name.Length > 29)
                throw new ArgumentException(string.Format(BandResources.GenericLengthExceeded, new[] { nameof(name) }));
            if (name != null && name.Length > 21 && ownerId != Guid.Empty)
                throw new ArgumentException(BandResources.BandTileOwnedTileNameExceedsLength, nameof(name));
            byte[] numArray = new byte[60];
            if (ownerId != Guid.Empty)
                BandBitConverter.GetBytes(ownerId, numArray, numArray.Length - 16);
            if (!string.IsNullOrEmpty(name))
            {
                int danglingHighSurrogate = Encoding.Unicode.GetBytesTrimDanglingHighSurrogate(name, name.Length, numArray, 0);
                NameAndOwnerId = numArray;
                FriendlyNameLength = (ushort)(danglingHighSurrogate / 2);
            }
            else
            {
                NameAndOwnerId = numArray;
                FriendlyNameLength = 0;
            }
        }

        public static int GetSerializedByteCount() => 88;

        public void SerializeToBand(ICargoWriter writer, uint? order = null)
        {
            writer.WriteGuid(appID);
            writer.WriteUInt32(order.HasValue ? order.Value : StartStripOrder);
            writer.WriteUInt32(ThemeColor);
            writer.WriteUInt16(FriendlyNameLength);
            writer.WriteUInt16((ushort)SettingsMask);
            writer.Write(NameAndOwnerId, 0, 60);
        }

        public static TileData DeserializeFromBand(ICargoReader reader) => new()
        {
            AppID = reader.ReadGuid(),
            StartStripOrder = reader.ReadUInt32(),
            ThemeColor = reader.ReadUInt32(),
            FriendlyNameLength = reader.ReadUInt16(),
            SettingsMask = (TileSettings)reader.ReadUInt16(),
            NameAndOwnerId = reader.ReadExact(60)
        };
    }
}
