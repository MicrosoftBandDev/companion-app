// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.FlowPanel
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Band.Tiles.Pages
{
    public class FlowPanel : PagePanel
    {
        public FlowPanel(params PageElement[] elements) : this(CommonElementColors.Black, elements)
        {
        }

        public FlowPanel(IEnumerable<PageElement> elements) : this(CommonElementColors.Black, elements)
        {
        }

        protected FlowPanel(BandColor color, IEnumerable<PageElement> elements) : base(color, elements)
        {
        }

        public FlowPanelOrientation Orientation { get; set; }

        internal override PageElementType TypeId => PageElementType.FlowList;

        protected override uint AttributesToCustomStyleMask() => Orientation != FlowPanelOrientation.Horizontal ? 4096U : 2048U;

        protected override void CustomStyleMaskToAttributes(uint mask) => Orientation = ((FlowPanelOrientation)mask).HasFlag(FlowPanelOrientation.Vertical) ? FlowPanelOrientation.Vertical : FlowPanelOrientation.Horizontal;

        [Flags]
        private enum FlowlistStyleMask : uint
        {
            Horizontal = 2048, // 0x00000800
            Vertical = 4096, // 0x00001000
        }
    }
}
