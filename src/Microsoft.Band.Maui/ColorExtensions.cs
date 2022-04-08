// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.ColorExtensions
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: .\netcore451\Microsoft.Band.Store.dll

using System.Drawing;

namespace Microsoft.Band
{
    public static partial class ColorExtensions
    {
        public static BandColor ToBandColor(this Color color) => new(color.R, color.G, color.B);

        public static Color ToColor(this BandColor color) => Color.FromArgb(byte.MaxValue, color.R, color.G, color.B);
    }
}
