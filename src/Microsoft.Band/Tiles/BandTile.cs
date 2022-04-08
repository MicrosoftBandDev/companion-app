// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Tiles.BandTile
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Microsoft.Band.Tiles.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Band.Tiles
{
    public sealed class BandTile
    {
        internal const short NameMaxLength = 21;
        internal const short MaxIcons = 10;
        internal const short MaxLayouts = 5;
        internal const short MaxPages = 8;
        internal const int MaxBinaryLayoutSize = 768;
        private readonly IList<BandIcon> additionalIcons = new List<BandIcon>();
        private string name;
        private BandIcon tileIcon;
        private BandIcon smallIcon;
        private readonly Guid tileId;

        public BandTile(Guid tileId)
        {
            this.tileId = !(tileId == Guid.Empty) ? tileId : throw new ArgumentException(BandResources.BandTileEmptyTileId, nameof(tileId));
            IsBadgingEnabled = true;
            PageLayouts = new List<PageLayout>();
        }

        public Guid TileId => tileId;

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));
                name = value.Length <= 21
                    ? value
                    : throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, BandResources.BandTileNameLengthExceeded, new[] { (ushort)21 }), nameof(value));
            }
        }

        public BandIcon TileIcon
        {
            get => tileIcon;
            set => tileIcon = value ?? throw new ArgumentNullException(nameof(value));
        }

        public BandIcon SmallIcon
        {
            get => smallIcon;
            set => smallIcon = value ?? throw new ArgumentNullException(nameof(value));
        }

        public BandTheme Theme { get; set; }

        public IList<PageLayout> PageLayouts { get; private set; }

        public bool IsBadgingEnabled { get; set; }

        public IList<BandIcon> AdditionalIcons => additionalIcons;

        public bool IsScreenTimeoutDisabled { get; set; }
    }
}
