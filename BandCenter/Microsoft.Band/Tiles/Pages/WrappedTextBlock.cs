// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.WrappedTextBlock
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class WrappedTextBlock : PageElement
    {
        private const int serializedCustomByteCount = 2;

        public WrappedTextBlock() : base(CommonElementColors.White)
        {
            AutoHeight = true;
        }

        public WrappedTextBlockFont Font { get; set; }

        public bool AutoHeight { get; set; }

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

        internal override PageElementType TypeId => PageElementType.WrappableText;

        protected override int SerializedCustomByteCount => 2;

        protected override uint AttributesToCustomStyleMask() => !AutoHeight ? 0U : 8192U;

        protected override void CustomStyleMaskToAttributes(uint mask) => AutoHeight = ((TextStyleMask)mask).HasFlag(TextStyleMask.AutoResize);

        internal override void SerializeCustomFieldsToBand(ICargoWriter writer) => writer.WriteUInt16((ushort)Font);

        internal override void DeserializeCustomFieldsFromBand(ICargoReader reader) => Font = (WrappedTextBlockFont)reader.ReadUInt16();

        [Flags]
        private enum TextStyleMask : uint
        {
            None = 0,
            AutoResize = 8192, // 0x00002000
        }
    }
}
