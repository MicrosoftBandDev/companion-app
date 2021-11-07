// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.TileSettings
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Tiles
{
    [Flags]
    internal enum TileSettings : ushort
    {
        None = 0,
        EnableNotification = 1,
        EnableBadging = 2,
        UseCustomColorForTile = 4,
        EnableAutoUpdate = 8,
        ScreenTimeout30Seconds = 16, // 0x0010
        ScreenTimeoutDisabled = 32, // 0x0020
    }
}
