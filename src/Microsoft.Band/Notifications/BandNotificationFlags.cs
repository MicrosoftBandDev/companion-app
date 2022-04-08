// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Notifications.BandNotificationFlags
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Notifications
{
    [Flags]
    internal enum BandNotificationFlags : byte
    {
        UnmodifiedNotificationSettings = 0,
        ForceNotificationDialog = 1,
        SuppressNotificationDialog = 2,
        SuppressSmsReply = 4,
        MaxValue = SuppressSmsReply | SuppressNotificationDialog | ForceNotificationDialog, // 0x07
    }
}
