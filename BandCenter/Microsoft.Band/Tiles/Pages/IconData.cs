// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.IconData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class IconData : PageElementData
    {
        public IconData(short elementId, ushort iconIndex) : base(PageElementType.Icon, elementId)
        {
            IconIndex = iconIndex;
        }

        public ushort IconIndex { get; set; }

        internal override int GetSerializedLength() => base.GetSerializedLength() + 2;

        internal override void SerializeToBand(ICargoWriter writer)
        {
            base.SerializeToBand(writer);
            writer.WriteUInt16(IconIndex);
        }
    }
}
