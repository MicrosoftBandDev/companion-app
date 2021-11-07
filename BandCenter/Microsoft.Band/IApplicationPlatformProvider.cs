// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.IApplicationPlatformProvider
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Microsoft.Band.Tiles;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band
{
    internal interface IApplicationPlatformProvider
    {
        Task<Guid> GetApplicationIdAsync(CancellationToken token);

        Task<bool> GetAddTileConsentAsync(BandTile tile, CancellationToken token);

        UserConsent GetCurrentSensorConsent(Type sensorType);

        Task<bool> RequestSensorConsentAsync(Type sensorType, string prompt, CancellationToken token);
    }
}
