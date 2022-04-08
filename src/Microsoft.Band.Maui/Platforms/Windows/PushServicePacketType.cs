// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.PushServicePacketType
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: .\netcore451\Microsoft.Band.Store.dll

namespace Microsoft.Band.Windows
{
    internal enum PushServicePacketType : ushort
    {
        WakeApp = 0,
        RemoteSubscription = 1,
        Sms = 100, // 0x0064
        DismissCall = 101, // 0x0065
        VoicePacketBegin = 200, // 0x00C8
        VoicePacketData = 201, // 0x00C9
        VoicePacketEnd = 202, // 0x00CA
        VoicePacketCancel = 203, // 0x00CB
        StrappEvent = 204, // 0x00CC
        StrappSyncRequest = 205, // 0x00CD
        CortanaContext = 206, // 0x00CE
        Keyboard = 220, // 0x00DC
        KeyboardSetContext = 222, // 0x00DE
    }
}
