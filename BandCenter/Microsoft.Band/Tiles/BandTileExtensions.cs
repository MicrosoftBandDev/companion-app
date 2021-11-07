// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.BandTileExtensions
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles
{
    internal static class BandTileExtensions
    {
        public static TileData ToTileData(this BandTile tile, int startStripOrder, Guid applicationId)
        {
            TileSettings tileSettings1 = TileSettings.EnableNotification;
            if (tile.IsBadgingEnabled)
                tileSettings1 |= TileSettings.EnableBadging;
            if (tile.IsScreenTimeoutDisabled)
                tileSettings1 |= TileSettings.ScreenTimeoutDisabled;
            TileSettings tileSettings2 = tileSettings1 | TileSettings.EnableAutoUpdate;
            TileData tileData = new();
            tileData.AppID = tile.TileId;
            tileData.StartStripOrder = (uint)startStripOrder;
            tileData.ThemeColor = 0U;
            tileData.SettingsMask = tileSettings2;
            tileData.SetNameAndOwnerId(tile.Name, applicationId);
            return tileData;
        }
    }
}
