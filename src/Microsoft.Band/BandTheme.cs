// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandTheme
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band
{
    public sealed class BandTheme
    {
        internal const ushort ColorCount = 6;

        public BandColor Base { get; set; }

        public BandColor Highlight { get; set; }

        public BandColor Lowlight { get; set; }

        public BandColor SecondaryText { get; set; }

        public BandColor HighContrast { get; set; }

        public BandColor Muted { get; set; }
    }
}
