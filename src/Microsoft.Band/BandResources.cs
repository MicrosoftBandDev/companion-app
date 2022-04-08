// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandResources
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band
{
    internal class BandResources
    {
        internal static string BadDeviceCommandStatusPacket => "Unexpected packet type encountered while reading command status packet.";

        internal static string BandAtMaxTileCapacity => "The Band already has its maximum number of Tiles.";

        internal static string BandTileIdAlreadyInstalled => "Tile already exists.";

        internal static string BandTileEmptyName => "A BandTile must have a non-empty Name property.";

        internal static string BandTileEmptyTileId => "A BandTile must have a non-empty Tile ID.";

        internal static string BandTileNameLengthExceeded => "The Name property must be no greater than {0} characters.";

        internal static string BandTileNoSmallIcon => "A BandTile must have a non-null SmallIcon property.";

        internal static string BandTileNoTileIcon => "A BandTile must have a non-null TileIcon property.";

        internal static string BandTileNullTemplateEncountered => "Null page templates not allowed.";

        internal static string BandTileOwnedTileNameExceedsLength => "A Band Tile name length exceeds that allowed by owned Tiles.";

        internal static string BandTilePageDataInvalidElementChildCount => "The data element count  is invalid.";

        internal static string BandTilePageDataNullElementEncountered => "Null page data elements are not allowed.";

        internal static string BandTilePageTemplateBlobTooBig => "Page template byte count exceeds the maximum.";

        internal static string BandTilePageTemplateDuplicateElementOrId => "A duplicate page element or element Id was encountered.";

        internal static string BandTilePageTemplateInvalidCheckDigit => "Page template check digit mismatch.";

        internal static string BandTilePageTemplateInvalidElementChildCount => "The page template element count is invalid.";

        internal static string BandTilePageTemplateNullElementEncountered => "Null page template elements not not allowed.";

        internal static string BandTilePageTemplateUnexpectedElementType => "Unexpected element type.";

        internal static string BandTilePageTemplateUnknownOrInvalidElementType => "Invalid or unknown element type.";

        internal static string BandTileTooManyIcons => "The page icon count exceeds the maximum.";

        internal static string BandTileTooManyTemplates => "The page template count exceeds the maximum.";

        internal static string BarcodeDataEmpty => "Barcode data cannot be empty.";

        internal static string BarcodeDataTooLong => "Value exceeds maximum barcode data length {0}.";

        internal static string ByteWriteFailure => "Cannot write the byte value to the stream.";

        internal static string CargoCommandStatusUnavailable => "CommandStatus is not available until all data has been transferred to/from the band.";

        internal static string CommandStatusError => "Device status code: 0x{0:X8} received.";

        internal static string ConnectionAttemptFailed => "Failed to connect to the target band.";

        internal static string ConsentDialogTitle => "Microsoft Band";

        internal static string DeviceInNonAppMode => "The band is not running in App mode. Current Device Mode = {0}.";

        internal static string EofExceptionOnWrite => "An attempt was made to write beyond the end of the stream.";

        internal static string GenericCountZero => "{0} must have at least one entry.";

        internal static string GenericLengthExceeded => "{0} is too long.";

        internal static string GenericNullCollectionElement => "A null element was encountered";

        internal static string HeartRateSensorConsentPrompt => "Allow this application to access the heart rate sensor on your Microsoft Band?";

        internal static string IconButtonsAreNotSupportedOnCargo => "Icon buttons are only available on Microsoft Band 2 and newer devices";

        internal static string ImageDimensionPixelDataMismatch => "Pixel data does not match width and height.";

        internal static string InvalidAppAmount => "Invalid amount of apps found on the device.";

        internal static string InvalidBarcodeCode39Data => "Invalid Code-39 data encountered.";

        internal static string InvalidBarcodePdf417Data => "Invalid Pdf417 data encountered. Some Band versions support a limited character set; see documentation for details.";

        internal static string MeTileHeightHeightError => "Me Tile image height must be {0}.";

        internal static string MeTileImageWidthError => "Me Tile image width must be {0}.";

        internal static string NotificationFieldsEmpty => "Both title and body are null or empty.";

        internal static string NotificationInvalidTileId => "Tile ID is invalid.";

        internal static string OperationRequiredConnectedDevice => "The attempted operation requires a connected band.";

        internal static string RemovePagesEmptyGuid => "Unable to remove pages from Tile '{0}'.";

        internal static string RemoveTileEmptyTileId => "Removing a tile requires a non-empty Tile ID.";

        internal static string RemoveTileFailed => "Unable to remove the Tile '{0}' from the Band.";

        internal static string SdkVersionNotSupported => "This version of the SDK is no longer supported.";

        internal static string SensorUserConsentNotQueried => "This sensor type requires explicit user consent. RequestUserConsentAsync() must be called one time prior to subscribing.";

        internal static string SetPagesEmptyGuid => "Unable to set pages on Tile '{0}'.";

        internal static string StreamReadFailure => "Cannot read data from the stream.";

        internal static string StreamWriteFailure => "Cannot write data to the stream.";

        internal static string UICommandLabelNo => "No";

        internal static string UICommandLabelYes => "Yes";

        internal static string UnsupportedSensor => "Use of an unsupported sensor.";

        internal static string UnsupportedSensorInterval => "Use of an unsupported reporting interval.";

        internal static string AddTileConsentPrompt => "Add {0} tile to your Band?";
    }
}
