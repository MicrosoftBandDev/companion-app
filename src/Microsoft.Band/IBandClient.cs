// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.IBandClient
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Microsoft.Band.Notifications;
using Microsoft.Band.Personalization;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band
{
    public interface IBandClient : IDisposable
    {
        IBandNotificationManager NotificationManager { get; }

        IBandPersonalizationManager PersonalizationManager { get; }

        IBandTileManager TileManager { get; }

        IBandSensorManager SensorManager { get; }

        Task<string> GetFirmwareVersionAsync();

        Task<string> GetFirmwareVersionAsync(CancellationToken token);

        Task<string> GetHardwareVersionAsync();

        Task<string> GetHardwareVersionAsync(CancellationToken token);
    }
}
