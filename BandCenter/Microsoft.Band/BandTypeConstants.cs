// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandTypeConstants
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Band
{
  internal class BandTypeConstants
  {
    internal static readonly BandTypeConstants Cargo = new(BandType.Cargo);
    internal static readonly BandTypeConstants Envoy = new(BandType.Envoy);

    protected BandTypeConstants(BandType bandType) => BandType = bandType;

    public BandType BandType { get; private set; }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public ushort MeTileWidth => 310;

    public ushort MeTileHeight
    {
      get
      {
        switch (BandType)
        {
          case BandType.Cargo:
            return 102;
          case BandType.Envoy:
            return 128;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    public ushort TileIconPreferredSize
    {
      get
      {
        switch (BandType)
        {
          case BandType.Cargo:
            return 46;
          case BandType.Envoy:
            return 48;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    public ushort BadgeIconPreferredSize
    {
      get
      {
        switch (BandType)
        {
          case BandType.Cargo:
            return 24;
          case BandType.Envoy:
            return 24;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    public ushort NotificiationIconPreferredSize
    {
      get
      {
        switch (BandType)
        {
          case BandType.Cargo:
            return 36;
          case BandType.Envoy:
            return 36;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    public int MaxIconsPerTile
    {
      get
      {
        switch (BandType)
        {
          case BandType.Cargo:
            return 10;
          case BandType.Envoy:
            return 15;
          default:
            throw new InvalidOperationException();
        }
      }
    }

    internal T GetBandSpecificValue<T>(T cargo, T envoy)
    {
      switch (BandType)
      {
        case BandType.Cargo:
          return cargo;
        case BandType.Envoy:
          return envoy;
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
