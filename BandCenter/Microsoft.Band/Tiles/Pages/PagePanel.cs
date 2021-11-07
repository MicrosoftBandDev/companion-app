// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.PagePanel
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Band.Tiles.Pages
{
    public abstract class PagePanel : PageElement
    {
        protected PagePanel(params PageElement[] elements) : this(CommonElementColors.Black, elements)
        {
        }

        protected PagePanel(IEnumerable<PageElement> elements) : this(CommonElementColors.Black, elements)
        {
        }

        protected PagePanel(BandColor color, IEnumerable<PageElement> elements) : base(color)
        {
            Elements = elements != null ? elements.ToList() : throw new ArgumentNullException(nameof(elements));
        }

        public IList<PageElement> Elements { get; private set; }

        public override PageElement FindElement(short elementIdToFind)
        {
            PageElement element1 = base.FindElement(elementIdToFind);
            if (element1 == null)
            {
                foreach (PageElement element2 in Elements)
                {
                    element1 = element2.FindElement(elementIdToFind);
                    if (element1 != null)
                        break;
                }
            }
            return element1;
        }

        protected override ushort ChildCount => (ushort)Elements.Count;

        internal override int GetSerializedByteCountAndValidate(HashSet<short> existingIDs, HashSet<PageElement> existingElements)
        {
            base.GetSerializedByteCountAndValidate(existingIDs, existingElements);
            int num = 36 + SerializedCustomByteCount;
            foreach (PageElement element in Elements)
            {
                if (element == null)
                    throw new InvalidOperationException(BandResources.BandTilePageTemplateNullElementEncountered);
                num += element.GetSerializedByteCountAndValidate(existingIDs, existingElements);
            }
            return num;
        }

        internal override int GetElementCountAndIDs(HashSet<short> existingIDs)
        {
            int elementCountAndIds = base.GetElementCountAndIDs(existingIDs);
            foreach (PageElement element in Elements)
            {
                ++elementCountAndIds;
                elementCountAndIds += element.GetElementCountAndIDs(existingIDs);
            }
            return elementCountAndIds;
        }

        internal override void GenerateMissingIDs(HashSet<short> existingIDs, ref short nextId)
        {
            base.GenerateMissingIDs(existingIDs, ref nextId);
            foreach (PageElement element in Elements)
                element.GenerateMissingIDs(existingIDs, ref nextId);
        }

        internal override void SerializeElementsToBand(ICargoWriter writer)
        {
            foreach (PageElement element in Elements)
                element.SerializeToBand(writer);
        }

        internal override void DeserializeElementsFromBand(ICargoReader reader, int childCount)
        {
            for (int index = 0; index < childCount; ++index)
                Elements.Add(DeserializeFromBand(reader));
        }
    }
}
