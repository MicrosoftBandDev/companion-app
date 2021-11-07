// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.Pages.TextData
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles.Pages
{
    public abstract class TextData : PageElementData
    {
        private string text;

        internal TextData(PageElementType typeId, short elementId, string text) : base(typeId, elementId)
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text
        {
            get => text;
            set => text = value ?? throw new ArgumentNullException(nameof(value));
        }

        private string SerializationText
        {
            get
            {
                if (string.IsNullOrEmpty(text))
                    return " ";
                string str = text.TruncateTrimDanglingHighSurrogate(160);
                return string.IsNullOrEmpty(str) ? " " : str;
            }
        }

        internal override int GetSerializedLength() => base.GetSerializedLength() + 2 + SerializationText.Length * 2;

        internal override void SerializeToBand(ICargoWriter writer)
        {
            base.SerializeToBand(writer);
            writer.WriteUInt16((ushort)SerializationText.Length);
            writer.WriteString(SerializationText);
        }
    }
}
