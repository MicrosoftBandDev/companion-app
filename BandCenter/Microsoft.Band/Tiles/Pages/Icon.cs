// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.Icon
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class Icon : PageElement
    {
        public Icon() : base(CommonElementColors.White)
        {
        }

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

        internal override PageElementType TypeId => PageElementType.Icon;
    }
}
