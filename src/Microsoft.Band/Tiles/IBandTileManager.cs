// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.IBandTileManager
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Microsoft.Band.Tiles.Pages;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band.Tiles
{
    public interface IBandTileManager
    {
        event EventHandler<BandTileEventArgs<IBandTileOpenedEvent>> TileOpened;

        event EventHandler<BandTileEventArgs<IBandTileButtonPressedEvent>> TileButtonPressed;

        event EventHandler<BandTileEventArgs<IBandTileClosedEvent>> TileClosed;

        Task StartReadingsAsync();

        Task StartReadingsAsync(CancellationToken token);

        Task StopReadingsAsync();

        Task StopReadingsAsync(CancellationToken token);

        Task<IEnumerable<BandTile>> GetTilesAsync();

        Task<IEnumerable<BandTile>> GetTilesAsync(CancellationToken token);

        Task<bool> AddTileAsync(BandTile tile);

        Task<bool> AddTileAsync(BandTile tile, CancellationToken token);

        Task<bool> SetPagesAsync(Guid tileId, params PageData[] pages);

        Task<bool> SetPagesAsync(Guid tileId, CancellationToken token, params PageData[] pages);

        Task<bool> SetPagesAsync(Guid tileId, IEnumerable<PageData> pages);

        Task<bool> SetPagesAsync(Guid tileId, CancellationToken token, IEnumerable<PageData> pages);

        Task<bool> RemovePagesAsync(Guid tileId);

        Task<bool> RemovePagesAsync(Guid tileId, CancellationToken token);

        Task<bool> RemoveTileAsync(BandTile tile);

        Task<bool> RemoveTileAsync(BandTile tile, CancellationToken token);

        Task<bool> RemoveTileAsync(Guid tileId);

        Task<bool> RemoveTileAsync(Guid tileId, CancellationToken token);

        Task<int> GetRemainingTileCapacityAsync();

        Task<int> GetRemainingTileCapacityAsync(CancellationToken token);

        bool TileInstalledAndOwned(Guid tileId, CancellationToken token);
    }
}
