// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.TextButton
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class TextButton : PageElement
    {
        public TextButton() : base(CommonElementColors.Grey)
        {
        }

        public BandColor PressedColor
        {
            get => Color;
            set => Color = value;
        }

        internal override PageElementType TypeId => PageElementType.InteractiveButtonWithText;

        protected override uint AttributesToCustomStyleMask() => 1073741824;
    }
}
