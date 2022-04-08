// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Personalization.BandImage
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Personalization
{
    public class BandImage
    {
        internal BandImage(int width, int height, byte[] pixelData)
        {
            if (pixelData == null)
                throw new ArgumentNullException(nameof(pixelData));
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            if (pixelData.Length != width * height * 2)
                throw new ArgumentException(BandResources.ImageDimensionPixelDataMismatch);
            Width = width;
            Height = height;
            PixelData = pixelData;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal byte[] PixelData { get; private set; }
    }
}
