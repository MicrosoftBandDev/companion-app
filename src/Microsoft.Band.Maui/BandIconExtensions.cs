using Microsoft.Band.Tiles;
using Microsoft.Maui.Controls;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using SLImage = SixLabors.ImageSharp.Image;

namespace Microsoft.Band
{
    public static class BandIconExtensions
    {
        public static BandIcon BandIconFromMauiImage(this ImageSource source)
        {
            SLImage agnosticImage = null;
            if (source is FileImageSource fileSource)
            {
#if WINDOWS
                agnosticImage = SLImage.Load(fileSource.File);
#else
                // Other platforms don't have free access to the file system,
                // so attempting to read the file directly is likely to fail.
                // TODO: Find a workaround for this.
                throw new PlatformNotSupportedException();
#endif
            }
            else if (source is UriImageSource uriSource)
            {
                // TODO: Handle downloading images
                throw new NotImplementedException();
            }
            else if (source is StreamImageSource streamSource)
            {
                var cancelToken = new System.Threading.CancellationTokenSource();
                agnosticImage = SLImage.Load(streamSource.Stream(cancelToken.Token).Result);
            }
            else if (source is FontImageSource fontSource)
            {
                Image<Rgba32> fontImage = new(256, 256);

                string glyph = fontSource.Glyph;

                FontCollection collection = new();
                FontFamily fontFam = collection.Find(fontSource.FontFamily);
                Font font = new(fontFam, (float)fontSource.Size);

                Color glyphColor = new(new Rgba32((uint)fontSource.Color.ToInt()));

                fontImage.Mutate(x => x.DrawText(glyph, font, glyphColor, PointF.Empty));
                agnosticImage = fontImage;
            }

            if (agnosticImage == null)
                throw new Exception("Failed to retrieve image data from ImageSource.");
            Image<Rgba32> convertedImage = agnosticImage.CloneAs<Rgba32>();
            byte[] bgr565Bytes;
            Rgba32[] pixelArray;
            if (convertedImage.TryGetSinglePixelSpan(out var pixelSpan))
                pixelArray = pixelSpan.ToArray();
            else
                throw new Exception("Failed to convert image to BGR565.");

            bgr565Bytes = new byte[pixelArray.Length * 2];
            int i = 0, w = 0;
            while (i < pixelArray.Length)
            {
                Rgba32 pixel = pixelArray[i++];
                ushort packed = 0;

                // Convert from 1 byte per channel to 5 or 6 as appropriate
                byte B = (byte)((double)pixel.B / 255 * 0b011111);
                byte G = (byte)((double)pixel.G / 255 * 0b111111);
                byte R = (byte)((double)pixel.R / 255 * 0b011111);

                packed = (ushort)((pixel.B << 11) | (pixel.G << 5) | pixel.R);

                bgr565Bytes[w++] = (byte)(packed >> sizeof(byte));      // Upper byte
                bgr565Bytes[w++] = (byte)(packed & byte.MaxValue);      // Lower byte
            }

            const int bytesPerPixel = 2;
            return new(convertedImage.Width * bytesPerPixel, convertedImage.Height * bytesPerPixel, bgr565Bytes);
        }
    }
}
