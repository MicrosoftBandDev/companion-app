// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.ScrollFlowPanel
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System.Collections.Generic;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class ScrollFlowPanel : FlowPanel
    {
        public ScrollFlowPanel(params PageElement[] elements) : base(CommonElementColors.White, elements)
        {
        }

        public ScrollFlowPanel(IEnumerable<PageElement> elements) : base(CommonElementColors.White, elements)
        {
        }

        public ElementColorSource ScrollBarColorSource
        {
            get => ColorSource;
            set => ColorSource = value;
        }

        public BandColor ScrollBarColor
        {
            get => Color;
            set => Color = value;
        }

        internal override PageElementType TypeId => PageElementType.ScrollFlowList;
    }
}
