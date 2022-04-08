// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.BandIcon
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles
{
    public class BandIcon
    {
        internal BandIcon(int width, int height, byte[] iconData)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            if (iconData == null)
                throw new ArgumentNullException(nameof(iconData));
            long num = width * height;
            if (iconData.Length != num / 2L + num % 2L)
                throw new ArgumentException(BandResources.ImageDimensionPixelDataMismatch);
            Width = width;
            Height = height;
            IconData = iconData;
        }

        public int Height { get; private set; }

        public int Width { get; private set; }

        internal byte[] IconData { get; private set; }
    }
}
