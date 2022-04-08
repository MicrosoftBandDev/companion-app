// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.Barcode
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class Barcode : PageElement
    {
        public Barcode(BarcodeType barcodeType) => BarcodeType = barcodeType;

        public BarcodeType BarcodeType { get; private set; }

        internal override PageElementType TypeId
        {
            get
            {
                switch (BarcodeType)
                {
                    case BarcodeType.Code39:
                        return PageElementType.BarcodeCode39;
                    default:
                        return PageElementType.BarcodePdf417;
                }
            }
        }
    }
}
