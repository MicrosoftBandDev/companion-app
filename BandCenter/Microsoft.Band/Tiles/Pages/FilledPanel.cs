// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.FilledPanel
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System.Collections.Generic;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class FilledPanel : PagePanel
    {
        public FilledPanel(params PageElement[] elements) : base(elements)
        {
        }

        public FilledPanel(IEnumerable<PageElement> elements) : base(elements)
        {
        }

        public ElementColorSource BackgroundColorSource
        {
            get => ColorSource;
            set => ColorSource = value;
        }

        public BandColor BackgroundColor
        {
            get => Color;
            set => Color = value;
        }

        internal override PageElementType TypeId => PageElementType.FilledQuad;
    }
}
