// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.FilledButtonData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class FilledButtonData : PageElementData
    {
        private BandColor color;

        public FilledButtonData(short elementId, BandColor pressedColor)
          : base(PageElementType.InteractiveButtonWithBorder, elementId)
        {
            color = pressedColor;
        }

        public BandColor PressedColor
        {
            get => color;
            set => color = value;
        }

        internal override int GetSerializedLength() => base.GetSerializedLength() + 4;

        internal override void SerializeToBand(ICargoWriter writer)
        {
            base.SerializeToBand(writer);
            writer.WriteUInt32(PressedColor.ToRgb(1));
        }
    }
}
