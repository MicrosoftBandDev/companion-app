// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.BandTileOpenedEvent
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Band.Tiles
{
    internal class BandTileOpenedEvent : BandTileEventBase, IBandTileOpenedEvent, IBandTileEvent
    {
        public BandTileOpenedEvent(DateTimeOffset timestamp) : base(timestamp)
        {
        }

        internal override void Dispatch(BandClient client) => client.DispatchTileOpenedEvent(this);

        public BandTileOpenedEvent(IDictionary<string, object> valueSet) : base(valueSet)
        {
        }

        public override void EncodeToDictionary(IDictionary<string, object> valueSet)
        {
            valueSet["Type"] = "TileOpenedEvent";
            base.EncodeToDictionary(valueSet);
        }
    }
}
