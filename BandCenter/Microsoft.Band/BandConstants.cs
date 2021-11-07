// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandConstants
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band
{
    internal static class BandConstants
    {
        public const ushort PacketTypeCommand = 12025;
        public const int CommandPacketSize = 8;
        public const ushort MaxCommandRelatedDataSize = 55;
        public const ushort UsbMaxDataPayloadSize = 64;
        public const ushort BlueToothMaxDataPayloadSize = 329;
        public const ushort PacketTypeStatus = 42750;
        public const int StatusPacketSize = 6;
        public const ushort IDStringSize = 66;
        public const ushort DeviceNameMaxLength = 16;
        public const ushort NotificationTileMessageTitleMaxLengthV1 = 40;
        public const ushort NotificationTileMessageBodyMaxLengthV1 = 320;
        public const ushort NotificationTileMessageTitleMaxLengthV2 = 40;
        public const ushort NotificationTileMessageBodyMaxLengthV2 = 320;
        public const ushort NotificationSmartResponseMaxLengthV2 = 160;
        public const ushort NotificationGenericDialogLineOneMaxLengthV1 = 40;
        public const ushort NotificationGenericDialogLineTwoMaxLengthV1 = 320;
        public const ushort NotificationGenericDialogLineOneMaxLengthV2 = 40;
        public const ushort NotificationGenericDialogLineTwoMaxLengthV2 = 320;
        public const ushort NotificationGenericUpdateMaxLength = 390;
        public const ushort TilePageShortTextDataMaxLength = 20;
        public const ushort TilePageLongTextDataMaxLength = 160;
        public const ushort TilePageLongTextDataMaxCount = 1;
        public const ushort SampleHeaderSize = 4;
        public const ushort AppCount = 3;
        public const ushort DeviceIconSize = 1024;
        public const ushort MaxIconPixelCount = 15270;
        public const ushort OldMaxTilesToTransfer = 15;
        public const ushort ApiVersionSupportsMaxTilesToTransfer = 32;
        public const ushort TileDataNameFieldLength = 30;
        public const ushort TileDataMaxNameLength = 29;
        public const ushort OwnedTileMaxNameLength = 21;
        public const ushort MaxSubscriptions = 64;
        public const uint OneBLVersionIndex = 0;
        public const uint TwoUpVersionIndex = 1;
        public const uint AppVersionIndex = 2;
        public const uint BuildNumberIndexInFirmwareVersionString = 2;
        public const int StreamingDataRequestIntervalMS = 5000;
        public const int PushServiceReconnectTimeout = 5000;
        public const int DeviceProductSerialNumSizeInBytes = 12;
        public const int DevicePermanentSerialNumSizeInBytes = 16;
        public const int DeviceBTMACAddressSizeInBytes = 6;
        public const int DefaultIOTimeout = 5000;
        public const int SyncUITimeout = 60000;
        public const ushort SdkFirmwareBitCheckVersion = 3;
        public const int GuidByteCount = 16;
        public const long MaxFileTimeForDateTimeValue = 2650467743999990000;
        public static readonly DateTime MinDateTimeForFileTime = DateTime.FromFileTimeUtc(0L);
        public static readonly DateTimeOffset MinDateTimeOffsetForFileTime = DateTimeOffset.FromFileTime(0L);
        public static string Band1UsbInterfaceGuid = "5F359E4EAFC94DC3B15B1B6F383B020C";
        public static string Band2UsbInterfaceGuid = "47683994FBB94E21A0D7EEDC9F7DC3AC";
        public static readonly Guid[] UsbInterfaceGuids = new Guid[2]
        {
            new Guid(Band1UsbInterfaceGuid),
            new Guid(Band2UsbInterfaceGuid)
        };
        public const int UserProfileFirmwareBytesCount = 256;
    }
}
