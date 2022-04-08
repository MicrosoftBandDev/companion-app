// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Notifications.IBandNotificationManager
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band.Notifications
{
    public interface IBandNotificationManager
    {
        Task ShowDialogAsync(Guid tileId, string title, string body);

        Task ShowDialogAsync(Guid tileId, string title, string body, CancellationToken token);

        Task SendMessageAsync(
          Guid tileId,
          string title,
          string body,
          DateTimeOffset timestamp,
          MessageFlags flags = MessageFlags.None);

        Task SendMessageAsync(
          Guid tileId,
          string title,
          string body,
          DateTimeOffset timestamp,
          MessageFlags flags,
          CancellationToken token);

        Task VibrateAsync(VibrationType vibrationType);

        Task VibrateAsync(VibrationType vibrationType, CancellationToken token);
    }
}
