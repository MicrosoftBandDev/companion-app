// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.PageLayout
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Band.Tiles.Pages
{
    public sealed class PageLayout
    {
        internal const int SerializedByteCount = 16;

        public PageLayout(PagePanel root = null) => Root = root;

        public PagePanel Root { get; set; }

        internal IEnumerable<PageElement> Elements
        {
            get
            {
                if (Root != null)
                {
                    Queue<PageElement> queue = new();
                    queue.Enqueue(Root);
                    while (queue.Count > 0)
                    {
                        PageElement element = queue.Dequeue();
                        yield return element;
                        if (element is PagePanel pagePanel4)
                        {
                            foreach (PageElement element1 in pagePanel4.Elements)
                                queue.Enqueue(element1);
                        }
                        element = null;
                    }
                    queue = null;
                }
            }
        }

        internal int GetSerializedByteCountAndValidate()
        {
            if (Root == null)
                throw new InvalidOperationException(BandResources.BandTilePageTemplateNullElementEncountered);
            return 16 + Root.GetSerializedByteCountAndValidate(new HashSet<short>(), new HashSet<PageElement>());
        }

        private int GetElementCountAndGenerateMissingIDs()
        {
            HashSet<short> existingIDs = new();
            short nextId = 1;
            int num = 0 + 1 + Root.GetElementCountAndIDs(existingIDs);
            Root.GenerateMissingIDs(existingIDs, ref nextId);
            return num;
        }

        internal void SerializeToBand(ICargoWriter writer)
        {
            ushort generateMissingIds = (ushort)GetElementCountAndGenerateMissingIDs();
            writer.WriteUInt16(1);
            writer.WriteUInt16(0);
            writer.WriteUInt16(0);
            writer.WriteUInt16(1);
            writer.WriteUInt16(0);
            writer.WriteUInt16(Root != null ? (ushort)1 : (ushort)0);
            writer.WriteUInt16(generateMissingIds);
            writer.WriteUInt16(PageElement.GetCheckNumber(PageElementType.PageHeader));
            if (Root == null)
                return;
            Root.SerializeToBand(writer);
        }

        internal static PageLayout DeserializeFromBand(ICargoReader reader)
        {
            if (reader.ReadUInt16() == 0)
                return null;
            int num1 = reader.ReadUInt16();
            int num2 = reader.ReadUInt16();
            PageElement pageElement = null;
            int num3 = reader.ReadUInt16() == 1 ? reader.ReadUInt16() : throw new SerializationException(BandResources.BandTilePageTemplateUnexpectedElementType);
            int num4 = reader.ReadUInt16();
            if (num4 > 1)
                throw new SerializationException(BandResources.BandTilePageTemplateInvalidElementChildCount);
            int num5 = reader.ReadUInt16();
            if (reader.ReadUInt16() != PageElement.GetCheckNumber(PageElementType.PageHeader))
                throw new SerializationException(BandResources.BandTilePageTemplateInvalidCheckDigit);
            if (num4 == 1)
            {
                pageElement = PageElement.DeserializeFromBand(reader);
                if (!(pageElement is PagePanel))
                    throw new SerializationException(BandResources.BandTilePageTemplateUnexpectedElementType);
            }
            return new PageLayout(pageElement as PagePanel);
        }
    }
}
