// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandIconRleCodec
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Microsoft.Band.Tiles;

namespace Microsoft.Band
{
    internal static class BandIconRleCodec
    {
        public static PooledBuffer EncodeTileIconRle(BandIcon icon)
        {
            ValidateIconSize(icon);
            PooledBuffer buffer = BufferServer.GetBuffer(1024);
            try
            {
                EncodeTileIconRle(icon, buffer.Buffer);
            }
            catch
            {
                buffer.Dispose();
                throw;
            }
            return buffer;
        }

        public static byte[] EncodeTileIconRleToArray(BandIcon icon)
        {
            ValidateIconSize(icon);
            byte[] rleArray = new byte[1024];
            EncodeTileIconRle(icon, rleArray);
            return rleArray;
        }

        private static void ValidateIconSize(BandIcon icon)
        {
            if (icon.Width * icon.Height > 15270)
                throw new BandException("Input icon has too many pixels for Run Length Encoding.");
        }

        private static void EncodeTileIconRle(BandIcon icon, byte[] rleArray)
        {
            byte[] iconData = icon.IconData;
            int num1 = 0;
            int rleIndex = 6;
            rleArray[0] = (byte)(icon.Width >> 8);
            rleArray[1] = (byte)icon.Width;
            rleArray[2] = (byte)(icon.Height >> 8);
            rleArray[3] = (byte)icon.Height;
            for (int index1 = 0; index1 < icon.Height; ++index1)
            {
                byte num2 = 0;
                int num3 = 0;
                switch (num1 % 2)
                {
                    case 0:
                        num2 = (byte)(iconData[num1++ / 2] >> 4 & 15);
                        break;
                    case 1:
                        num2 = (byte)(iconData[num1++ / 2] & 15U);
                        break;
                }
                int num4 = num3 + 1;
                for (int index2 = 1; index2 < icon.Width; ++index2)
                {
                    RleLengthCheck(rleIndex);
                    byte num5 = 0;
                    switch (num1 % 2)
                    {
                        case 0:
                            num5 = (byte)(iconData[num1++ / 2] >> 4 & 15);
                            break;
                        case 1:
                            num5 = (byte)(iconData[num1++ / 2] & 15U);
                            break;
                    }
                    if (num2 != num5)
                    {
                        if (num4 > 0)
                            rleArray[rleIndex++] = (byte)((uint)(num4 << 4) | num2);
                        num2 = num5;
                        num4 = 0;
                    }
                    ++num4;
                    if (num4 == 15)
                    {
                        RleLengthCheck(rleIndex);
                        rleArray[rleIndex++] = (byte)((uint)(num4 << 4) | num2);
                        num4 = 0;
                    }
                }
                if (num4 > 0)
                {
                    RleLengthCheck(rleIndex);
                    rleArray[rleIndex++] = (byte)((uint)(num4 << 4) | num2);
                }
            }
            rleArray[4] = (byte)(rleIndex >> 8);
            rleArray[5] = (byte)rleIndex;
        }

        private static void RleLengthCheck(int rleIndex)
        {
            if (rleIndex >= 1024)
                throw new BandException("Run Length Encoding Failure.");
        }

        public static BandIcon DecodeTileIconRle(PooledBuffer rleIcon) => DecodeTileIconRle(rleIcon.Buffer);

        public static BandIcon DecodeTileIconRle(byte[] rleArray)
        {
            ushort num1 = (ushort)((uint)rleArray[0] << 8 | rleArray[1]);
            ushort num2 = (ushort)((uint)rleArray[2] << 8 | rleArray[3]);
            if (num1 == 0 || num2 == 0 || num1 * (uint)num2 > 15270U)
                return new BandIcon(0, 0, new byte[0]);
            int num3 = num1 * num2;
            byte[] iconData = new byte[num3 / 2 + num3 % 2];
            int num4 = 0;
            ushort num5 = (ushort)((uint)rleArray[4] << 8 | rleArray[5]);
            for (int index = 6; index < num5; ++index)
            {
                byte num6 = (byte)((uint)rleArray[index] >> 4);
                byte num7 = (byte)(rleArray[index] & 15U);
                if (num7 == 0)
                {
                    num4 += num6;
                }
                else
                {
                    byte num8 = 0;
                    while (num8 < num6)
                    {
                        switch (num4 % 2)
                        {
                            case 0:
                                iconData[num4 / 2] = (byte)((uint)num7 << 4);
                                break;
                            case 1:
                                iconData[num4 / 2] |= num7;
                                break;
                        }
                        ++num8;
                        ++num4;
                    }
                }
            }
            return new BandIcon(num1, num2, iconData);
        }
    }
}
