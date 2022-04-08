// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.PageData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class PageData
    {
        private int pageLayoutIndex;
        private readonly IList<PageElementData> values;

        public PageData(Guid pageId, int pageLayoutIndex, params PageElementData[] values)
          : this(pageId, pageLayoutIndex, (IEnumerable<PageElementData>)values)
        {
        }

        public PageData(Guid pageId, int pageLayoutIndex, IEnumerable<PageElementData> values)
        {
            if (pageLayoutIndex < 0 || pageLayoutIndex >= 5)
                throw new ArgumentOutOfRangeException(nameof(pageLayoutIndex));
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            PageId = pageId;
            PageLayoutIndex = pageLayoutIndex;
            this.values = values.ToList();
        }

        public Guid PageId { get; set; }

        public int PageLayoutIndex
        {
            get => pageLayoutIndex;
            set => pageLayoutIndex = value >= 0 && value < 5 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public IList<PageElementData> Values => values;

        internal int GetSerializedLengthAndValidate(BandTypeConstants constants)
        {
            if (values.Count == 0)
                throw new InvalidOperationException(string.Format(BandResources.BandTilePageDataInvalidElementChildCount));
            int num = 0;
            foreach (PageElementData pageElementData in values)
            {
                if (pageElementData == null)
                    throw new InvalidOperationException(string.Format(BandResources.BandTilePageDataNullElementEncountered));
                pageElementData.Validate(constants);
                num += pageElementData.GetSerializedLength();
            }
            return num;
        }

        internal void SerializeToBand(ICargoWriter writer)
        {
            foreach (PageElementData pageElementData in values)
                pageElementData.SerializeToBand(writer);
        }
    }
}
