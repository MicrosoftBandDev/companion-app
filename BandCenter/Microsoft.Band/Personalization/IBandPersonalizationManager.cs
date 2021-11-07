// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Personalization.IBandPersonalizationManager
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band.Personalization
{
    public interface IBandPersonalizationManager
    {
        Task<BandImage> GetMeTileImageAsync();

        Task<BandImage> GetMeTileImageAsync(CancellationToken cancel);

        Task SetMeTileImageAsync(BandImage image);

        Task SetMeTileImageAsync(BandImage image, CancellationToken cancel);

        Task<BandTheme> GetThemeAsync();

        Task<BandTheme> GetThemeAsync(CancellationToken cancel);

        Task SetThemeAsync(BandTheme theme);

        Task SetThemeAsync(BandTheme theme, CancellationToken cancel);
    }
}
