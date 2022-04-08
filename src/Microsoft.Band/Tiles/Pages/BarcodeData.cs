// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.BarcodeData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class BarcodeData : PageElementData
    {
        private static readonly byte[] validPdf417CharMapEnvoy = new byte[128]
        {
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           1,
           1,
           0,
           0,
           1,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           0
        };
        private static readonly byte[] validCode39CharMap = new byte[59]
        {
           1,
           0,
           0,
           0,
           1,
           1,
           0,
           0,
           0,
           0,
           0,
           1,
           0,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           0,
           0,
           0,
           0,
           0,
           0,
           0,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1,
           1
        };
        private string barcodeData;

        public BarcodeData(BarcodeType barcodeType, short elementId, string barcode)
          : base(BarcodeTypeToPageElementType(barcodeType), elementId)
        {
            if (barcode == null)
                throw new ArgumentNullException(nameof(barcode));
            BarcodeType = barcodeType;
            SetBarcode(barcode, nameof(barcode));
        }

        public static int MaximumDataLength => 39;

        private static PageElementType BarcodeTypeToPageElementType(
          BarcodeType barcodeType)
        {
            if (barcodeType == BarcodeType.Pdf417)
                return PageElementType.BarcodePdf417;
            if (barcodeType == BarcodeType.Code39)
                return PageElementType.BarcodeCode39;
            throw new InvalidOperationException();
        }

        private static void ValidatePDF417Length(string value, string paramName)
        {
            if (value.Length > MaximumDataLength)
                throw new ArgumentOutOfRangeException(paramName, string.Format(BandResources.BarcodeDataTooLong, new[] { MaximumDataLength }));
        }

        private static void ValidateCode39Length(string value, string paramName)
        {
            if (value.Length > MaximumDataLength)
                throw new ArgumentOutOfRangeException(paramName, string.Format(BandResources.BarcodeDataTooLong, new[] { MaximumDataLength }));
        }

        public BarcodeType BarcodeType { get; private set; }

        public string Barcode
        {
            get => barcodeData;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                SetBarcode(value, "Value");
            }
        }

        private void SetBarcode(string value, string paramName)
        {
            if (value.Length == 0)
                throw new ArgumentOutOfRangeException(paramName, BandResources.BarcodeDataEmpty);
            switch (BarcodeType)
            {
                case BarcodeType.Pdf417:
                    ValidatePDF417Length(value, paramName);
                    break;
                case BarcodeType.Code39:
                    ValidateCode39Length(value, paramName);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            barcodeData = value;
        }

        internal override void Validate(BandTypeConstants constants)
        {
            switch (BarcodeType)
            {
                case BarcodeType.Pdf417:
                    switch (constants.BandType)
                    {
                        case BandType.Cargo:
                            ValidatePdf417ForBandV1(Barcode);
                            return;
                        case BandType.Envoy:
                            ValidatePdf417ForBandV2(Barcode);
                            return;
                        default:
                            throw new InvalidOperationException();
                    }
                case BarcodeType.Code39:
                    ValidateCode39(Barcode);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        internal override int GetSerializedLength() => base.GetSerializedLength() + 2 + Barcode.Length * 2;

        private static void ValidatePdf417ForBandV1(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                    throw new SerializationException(BandResources.InvalidBarcodePdf417Data);
            }
        }

        private static void ValidatePdf417ForBandV2(string value)
        {
            foreach (char ch in value)
            {
                if (ch >= validPdf417CharMapEnvoy.Length || validPdf417CharMapEnvoy[ch] == 0)
                    throw new SerializationException(BandResources.InvalidBarcodePdf417Data);
            }
        }

        private static void ValidateCode39(string value)
        {
            foreach (int num in value)
            {
                int index = num - 32;
                if (index < 0 || index >= validCode39CharMap.Length || validCode39CharMap[index] == 0)
                    throw new SerializationException(BandResources.InvalidBarcodeCode39Data);
            }
        }

        internal override void SerializeToBand(ICargoWriter writer)
        {
            base.SerializeToBand(writer);
            writer.WriteUInt16((ushort)Barcode.Length);
            writer.WriteString(Barcode);
        }
    }
}
