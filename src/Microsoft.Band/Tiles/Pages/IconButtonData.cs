// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.IconButtonData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class IconButtonData : PageElementData
    {
        public IconButtonData(
          short elementId,
          ushort iconIndex,
          ushort pressedIconIndex,
          BandColor iconColor,
          BandColor pressedIconColor,
          BandColor backgroundColor,
          BandColor pressedBackgroundColor)
          : base(PageElementType.InteractiveButtonWithIcon, elementId)
        {
            IconIndex = iconIndex;
            PressedIconIndex = pressedIconIndex;
            IconColor = iconColor;
            PressedIconColor = pressedIconColor;
            BackgroundColor = backgroundColor;
            PressedBackgroundColor = pressedBackgroundColor;
        }

        public ushort IconIndex { get; set; }

        public ushort PressedIconIndex { get; set; }

        public BandColor IconColor { get; set; }

        public BandColor PressedIconColor { get; set; }

        public BandColor BackgroundColor { get; set; }

        public BandColor PressedBackgroundColor { get; set; }

        internal override int GetSerializedLength() => base.GetSerializedLength() + 26;

        internal override void SerializeToBand(ICargoWriter writer)
        {
            base.SerializeToBand(writer);
            writer.WriteUInt16(IconIndex);
            writer.WriteUInt16(PressedIconIndex);
            writer.WriteUInt16(13001);
            writer.WriteUInt16((ushort)ElementId);
            writer.WriteUInt16(4);
            writer.WriteUInt32(BackgroundColor.ToRgb(1));
            writer.WriteUInt32(PressedBackgroundColor.ToRgb(1));
            writer.WriteUInt32(IconColor.ToRgb(1));
            writer.WriteUInt32(PressedIconColor.ToRgb(1));
        }
    }
}
