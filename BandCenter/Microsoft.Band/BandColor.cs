// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandColor
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band
{
    public struct BandColor
    {
        private byte r;
        private byte g;
        private byte b;

        public BandColor(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
        }

        internal BandColor(uint rgb)
        {
            r = (byte)(rgb >> 16 & byte.MaxValue);
            g = (byte)(rgb >> 8 & byte.MaxValue);
            b = (byte)(rgb & byte.MaxValue);
        }

        public byte R => r;

        public byte G => g;

        public byte B => b;

        internal uint ToRgb(byte alpha = 255) => (uint)(alpha << 24 | R << 16 | G << 8) | B;
    }
}
