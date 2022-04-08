// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.TextBlock
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class TextBlock : PageElement
    {
        private const int serializedCustomByteCount = 4;

        public TextBlock() : base(CommonElementColors.White)
        {
            AutoWidth = true;
        }

        public TextBlockFont Font { get; set; }

        public short Baseline { get; set; }

        public TextBlockBaselineAlignment BaselineAlignment { get; set; }

        public bool AutoWidth { get; set; }

        public new ElementColorSource ColorSource
        {
            get => base.ColorSource;
            set => base.ColorSource = value;
        }

        public new BandColor Color
        {
            get => base.Color;
            set => base.Color = value;
        }

        internal override PageElementType TypeId => PageElementType.Text;

        protected override int SerializedCustomByteCount => 4;

        protected override uint AttributesToCustomStyleMask() => AutoWidthToTextStyleMask() | BaselineAlignmentToTextStyleMask();

        private uint AutoWidthToTextStyleMask() => !AutoWidth ? 0U : 16384U;

        private uint BaselineAlignmentToTextStyleMask()
        {
            switch (BaselineAlignment)
            {
                case TextBlockBaselineAlignment.Absolute:
                    return 2048;
                case TextBlockBaselineAlignment.Relative:
                    return 4096;
                default:
                    return 0;
            }
        }

        protected override void CustomStyleMaskToAttributes(uint mask)
        {
            TextStyleMaskToAutoWidth(mask);
            TextStyleMaskToBaselineAlignment(mask);
        }

        private void TextStyleMaskToAutoWidth(uint mask) => AutoWidth = ((TextStyleMask)mask).HasFlag(TextStyleMask.AutoResizeWidth);

        private void TextStyleMaskToBaselineAlignment(uint mask)
        {
            if (((TextStyleMask)mask).HasFlag(TextStyleMask.VerticalBaselineAbsolute))
                BaselineAlignment = TextBlockBaselineAlignment.Absolute;
            else if (((TextStyleMask)mask).HasFlag(TextStyleMask.VerticalBaselineRelative))
                BaselineAlignment = TextBlockBaselineAlignment.Relative;
            else
                BaselineAlignment = TextBlockBaselineAlignment.Automatic;
        }

        internal override void SerializeCustomFieldsToBand(ICargoWriter writer)
        {
            writer.WriteUInt16((ushort)Font);
            writer.WriteInt16(Baseline);
        }

        internal override void DeserializeCustomFieldsFromBand(ICargoReader reader)
        {
            Font = (TextBlockFont)reader.ReadUInt16();
            Baseline = reader.ReadInt16();
        }

        [Flags]
        private enum TextStyleMask : uint
        {
            None = 0,
            VerticalBaselineAbsolute = 2048, // 0x00000800
            VerticalBaselineRelative = 4096, // 0x00001000
            AutoResizeWidth = 16384, // 0x00004000
        }
    }
}
