// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.DeviceCommands
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band
{
    internal static class DeviceCommands
    {
        internal const ushort IndexShift = 0;
        internal const ushort IndexBits = 7;
        internal const ushort IndexMask = 127;
        internal const ushort TXShift = 7;
        internal const ushort TXBits = 1;
        internal const ushort TXMask = 128;
        internal const ushort CategoryShift = 8;
        internal const ushort CategoryBits = 8;
        internal const ushort CategoryMask = 65280;
        internal static ushort CargoCoreModuleGetVersion = MakeCommand(Facility.LibraryJutil, TX.True, 1);
        internal static ushort CargoCoreModuleGetUniqueID = MakeCommand(Facility.LibraryJutil, TX.True, 2);
        internal static ushort CargoCoreModuleWhoAmI = MakeCommand(Facility.LibraryJutil, TX.True, 3);
        internal static ushort CargoCoreModuleGetLogVersion = MakeCommand(Facility.LibraryJutil, TX.True, 5);
        internal static ushort CargoCoreModuleGetApiVersion = MakeCommand(Facility.LibraryJutil, TX.True, 6);
        internal static ushort CargoCoreModuleSdkCheck = MakeCommand(Facility.LibraryJutil, TX.False, 7);
        internal static ushort CargoTimeGetUtcTime = MakeCommand(Facility.LibraryTime, TX.True, 0);
        internal static ushort CargoTimeSetUtcTime = MakeCommand(Facility.LibraryTime, TX.False, 1);
        internal static ushort CargoTimeGetLocalTime = MakeCommand(Facility.LibraryTime, TX.True, 2);
        internal static ushort CargoTimeSetTimeZoneFile = MakeCommand(Facility.LibraryTime, TX.False, 4);
        internal static ushort CargoTimeZoneFileGetVersion = MakeCommand(Facility.LibraryTime, TX.True, 6);
        internal static ushort CargoLoggerGetChunkData = MakeCommand(Facility.LibraryLogger, TX.True, 1);
        internal static ushort CargoLoggerEnableLogging = MakeCommand(Facility.LibraryLogger, TX.False, 3);
        internal static ushort CargoLoggerDisableLogging = MakeCommand(Facility.LibraryLogger, TX.False, 4);
        internal static ushort CargoLoggerGetChunkCounts = MakeCommand(Facility.LibraryLogger, TX.True, 9);
        internal static ushort CargoLoggerFlush = MakeCommand(Facility.LibraryLogger, TX.False, 13);
        internal static ushort CargoLoggerGetChunkRangeMetadata = MakeCommand(Facility.LibraryLogger, TX.True, 14);
        internal static ushort CargoLoggerGetChunkRangeData = MakeCommand(Facility.LibraryLogger, TX.True, 15);
        internal static ushort CargoLoggerDeleteChunkRange = MakeCommand(Facility.LibraryLogger, TX.False, 16);
        internal static ushort CargoProfileGetDataApp = MakeCommand(Facility.ModuleProfile, TX.True, 6);
        internal static ushort CargoProfileSetDataApp = MakeCommand(Facility.ModuleProfile, TX.False, 7);
        internal static ushort CargoProfileGetDataFW = MakeCommand(Facility.ModuleProfile, TX.True, 8);
        internal static ushort CargoProfileSetDataFW = MakeCommand(Facility.ModuleProfile, TX.False, 9);
        internal static ushort CargoRemoteSubscriptionSubscribe = MakeCommand(Facility.LibraryRemoteSubscription, TX.False, 0);
        internal static ushort CargoRemoteSubscriptionUnsubscribe = MakeCommand(Facility.LibraryRemoteSubscription, TX.False, 1);
        internal static ushort CargoRemoteSubscriptionGetDataLength = MakeCommand(Facility.LibraryRemoteSubscription, TX.True, 2);
        internal static ushort CargoRemoteSubscriptionGetData = MakeCommand(Facility.LibraryRemoteSubscription, TX.True, 3);
        internal static ushort CargoRemoteSubscriptionSubscribeId = MakeCommand(Facility.LibraryRemoteSubscription, TX.False, 7);
        internal static ushort CargoRemoteSubscriptionUnsubscribeId = MakeCommand(Facility.LibraryRemoteSubscription, TX.False, 8);
        internal static ushort CargoNotification = MakeCommand(Facility.ModuleNotification, TX.False, 0);
        internal static ushort CargoNotificationProtoBuf = MakeCommand(Facility.ModuleNotification, TX.False, 5);
        internal static ushort CargoDynamicAppRegisterApp = MakeCommand(Facility.ModuleFireballAppsManagement, TX.False, 0);
        internal static ushort CargoDynamicAppRemoveApp = MakeCommand(Facility.ModuleFireballAppsManagement, TX.False, 1);
        internal static ushort CargoDynamicAppRegisterAppIcons = MakeCommand(Facility.ModuleFireballAppsManagement, TX.False, 2);
        internal static ushort CargoDynamicAppSetAppTileIndex = MakeCommand(Facility.ModuleFireballAppsManagement, TX.False, 3);
        internal static ushort CargoDynamicAppSetAppBadgeTileIndex = MakeCommand(Facility.ModuleFireballAppsManagement, TX.False, 5);
        internal static ushort CargoDynamicAppSetAppNotificationTileIndex = MakeCommand(Facility.ModuleFireballAppsManagement, TX.False, 11);
        internal static ushort CargoDynamicPageLayoutSet = MakeCommand(Facility.ModuleFireballPageManagement, TX.False, 0);
        internal static ushort CargoDynamicPageLayoutRemove = MakeCommand(Facility.ModuleFireballPageManagement, TX.False, 1);
        internal static ushort CargoDynamicPageLayoutGet = MakeCommand(Facility.ModuleFireballPageManagement, TX.True, 2);
        internal static ushort CargoInstalledAppListGet = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 0);
        internal static ushort CargoInstalledAppListSet = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 1);
        internal static ushort CargoInstalledAppListStartStripSyncStart = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 2);
        internal static ushort CargoInstalledAppListStartStripSyncEnd = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 3);
        internal static ushort CargoInstalledAppListGetDefaults = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 4);
        internal static ushort CargoInstalledAppListSetTile = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 6);
        internal static ushort CargoInstalledAppListGetTile = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 7);
        internal static ushort CargoInstalledAppListGetSettingsMask = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 13);
        internal static ushort CargoInstalledAppListSetSettingsMask = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 14);
        internal static ushort CargoInstalledAppListEnableSetting = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 15);
        internal static ushort CargoInstalledAppListDisableSetting = MakeCommand(Facility.ModuleInstalledAppList, TX.False, 16);
        internal static ushort CargoInstalledAppListGetNoImages = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 18);
        internal static ushort CargoInstalledAppListGetDefaultsNoImages = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 19);
        internal static ushort CargoInstalledAppListGetMaxTileCount = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 21);
        internal static ushort CargoInstalledAppListGetMaxTileAllocatedCount = MakeCommand(Facility.ModuleInstalledAppList, TX.True, 22);
        internal static ushort CargoSystemSettingsOobeCompleteClear = MakeCommand(Facility.ModuleSystemSettings, TX.False, 0);
        internal static ushort CargoSystemSettingsOobeCompleteSet = MakeCommand(Facility.ModuleSystemSettings, TX.False, 1);
        internal static ushort CargoSystemSettingsFactoryReset = MakeCommand(Facility.ModuleSystemSettings, TX.True, 7);
        internal static ushort CargoSystemSettingsGetTimeZone = MakeCommand(Facility.ModuleSystemSettings, TX.True, 10);
        internal static ushort CargoSystemSettingsSetTimeZone = MakeCommand(Facility.ModuleSystemSettings, TX.False, 11);
        internal static ushort CargoSystemSettingsSetEphemerisFile = MakeCommand(Facility.ModuleSystemSettings, TX.False, 15);
        internal static ushort CargoSystemSettingsGetMeTileImageID = MakeCommand(Facility.ModuleSystemSettings, TX.True, 18);
        internal static ushort CargoSystemSettingsOobeCompleteGet = MakeCommand(Facility.ModuleSystemSettings, TX.True, 19);
        internal static ushort CargoSystemSettingsEnableDemoMode = MakeCommand(Facility.ModuleSystemSettings, TX.False, 25);
        internal static ushort CargoSystemSettingsDisableDemoMode = MakeCommand(Facility.ModuleSystemSettings, TX.False, 26);
        internal static ushort CargoSRAMFWUpdateLoadData = MakeCommand(Facility.LibrarySRAMFWUpdate, TX.False, 0);
        internal static ushort CargoSRAMFWUpdateBootIntoUpdateMode = MakeCommand(Facility.LibrarySRAMFWUpdate, TX.False, 1);
        internal static ushort CargoSRAMFWUpdateValidateAssets = MakeCommand(Facility.LibrarySRAMFWUpdate, TX.True, 2);
        internal static ushort CargoEFlashRead = MakeCommand(Facility.DriverEFlash, TX.True, 1);
        internal static ushort CargoGpsIsEnabled = MakeCommand(Facility.LibraryGps, TX.True, 6);
        internal static ushort CargoGpsEphemerisCoverageDates = MakeCommand(Facility.LibraryGps, TX.True, 13);
        internal static ushort CargoFireballUINavigateToScreen = MakeCommand(Facility.ModuleFireballUI, TX.False, 0);
        internal static ushort CargoFireballUIClearMeTileImage = MakeCommand(Facility.ModuleFireballUI, TX.False, 6);
        internal static ushort CargoFireballUISetSmsResponse = MakeCommand(Facility.ModuleFireballUI, TX.False, 7);
        internal static ushort CargoFireballUIGetAllSmsResponse = MakeCommand(Facility.ModuleFireballUI, TX.True, 11);
        internal static ushort CargoFireballUIReadMeTileImage = MakeCommand(Facility.ModuleFireballUI, TX.True, 14);
        internal static ushort CargoFireballUIWriteMeTileImageWithID = MakeCommand(Facility.ModuleFireballUI, TX.False, 17);
        internal static ushort CargoThemeColorSetFirstPartyTheme = MakeCommand(Facility.ModuleThemeColor, TX.False, 0);
        internal static ushort CargoThemeColorGetFirstPartyTheme = MakeCommand(Facility.ModuleThemeColor, TX.True, 1);
        internal static ushort CargoThemeColorSetCustomTheme = MakeCommand(Facility.ModuleThemeColor, TX.False, 2);
        internal static ushort CargoThemeColorReset = MakeCommand(Facility.ModuleThemeColor, TX.False, 4);
        internal static ushort CargoHapticPlayVibrationStream = MakeCommand(Facility.LibraryHaptic, TX.False, 0);
        internal static ushort CargoGoalTrackerSet = MakeCommand(Facility.ModuleGoalTracker, TX.False, 0);
        internal static ushort CargoFitnessPlansWriteFile = MakeCommand(Facility.LibraryFitnessPlans, TX.False, 4);
        internal static ushort CargoGolfCourseFileWrite = MakeCommand(Facility.LibraryGolf, TX.False, 0);
        internal static ushort CargoGolfCourseFileGetMaxSize = MakeCommand(Facility.LibraryGolf, TX.True, 1);
        internal static ushort CargoOobeSetStage = MakeCommand(Facility.ModuleOobe, TX.False, 0);
        internal static ushort CargoOobeGetStage = MakeCommand(Facility.ModuleOobe, TX.True, 1);
        internal static ushort CargoOobeFinalize = MakeCommand(Facility.ModuleOobe, TX.False, 2);
        internal static ushort CargoCortanaNotification = MakeCommand(Facility.ModuleCortana, TX.False, 0);
        internal static ushort CargoCortanaStart = MakeCommand(Facility.ModuleCortana, TX.False, 1);
        internal static ushort CargoCortanaStop = MakeCommand(Facility.ModuleCortana, TX.False, 2);
        internal static ushort CargoCortanaCancel = MakeCommand(Facility.ModuleCortana, TX.False, 3);
        internal static ushort CargoPersistedAppDataSetRunMetrics = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 0);
        internal static ushort CargoPersistedAppDataGetRunMetrics = MakeCommand(Facility.ModulePersistedApplicationData, TX.True, 1);
        internal static ushort CargoPersistedAppDataSetBikeMetrics = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 2);
        internal static ushort CargoPersistedAppDataGetBikeMetrics = MakeCommand(Facility.ModulePersistedApplicationData, TX.True, 3);
        internal static ushort CargoPersistedAppDataSetBikeSplitMult = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 4);
        internal static ushort CargoPersistedAppDataGetBikeSplitMult = MakeCommand(Facility.ModulePersistedApplicationData, TX.True, 5);
        internal static ushort CargoPersistedAppDataSetWorkoutActivities = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 9);
        internal static ushort CargoPersistedAppDataGetWorkoutActivities = MakeCommand(Facility.ModulePersistedApplicationData, TX.True, 16);
        internal static ushort CargoPersistedAppDataSetSleepNotification = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 17);
        internal static ushort CargoPersistedAppDataGetSleepNotification = MakeCommand(Facility.ModulePersistedApplicationData, TX.True, 18);
        internal static ushort CargoPersistedAppDataDisableSleepNotification = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 19);
        internal static ushort CargoPersistedAppDataSetLightExposureNotification = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 21);
        internal static ushort CargoPersistedAppDataGetLightExposureNotification = MakeCommand(Facility.ModulePersistedApplicationData, TX.True, 22);
        internal static ushort CargoPersistedAppDataDisableLightExposureNotification = MakeCommand(Facility.ModulePersistedApplicationData, TX.False, 23);
        internal static ushort CargoGetProductSerialNumber = MakeCommand(Facility.LibraryConfiguration, TX.True, 8);
        internal static ushort CargoKeyboardCmd = MakeCommand(Facility.LibraryKeyboard, TX.False, 0);
        internal static ushort CargoSubscriptionLoggerSubscribe = MakeCommand(Facility.ModuleLoggerSubscriptions, TX.False, 0);
        internal static ushort CargoSubscriptionLoggerUnsubscribe = MakeCommand(Facility.ModuleLoggerSubscriptions, TX.False, 1);
        internal static ushort CargoCrashDumpGetFileSize = MakeCommand(Facility.DriverCrashDump, TX.True, 1);
        internal static ushort CargoCrashDumpGetAndDeleteFile = MakeCommand(Facility.DriverCrashDump, TX.True, 2);
        internal static ushort CargoInstrumentationGetFileSize = MakeCommand(Facility.ModuleInstrumentation, TX.True, 4);
        internal static ushort CargoInstrumentationGetFile = MakeCommand(Facility.ModuleInstrumentation, TX.True, 5);
        internal static ushort CargoPersistedStatisticsRunGet = MakeCommand(Facility.ModulePersistedStatistics, TX.True, 2);
        internal static ushort CargoPersistedStatisticsWorkoutGet = MakeCommand(Facility.ModulePersistedStatistics, TX.True, 3);
        internal static ushort CargoPersistedStatisticsSleepGet = MakeCommand(Facility.ModulePersistedStatistics, TX.True, 4);
        internal static ushort CargoPersistedStatisticsGuidedWorkoutGet = MakeCommand(Facility.ModulePersistedStatistics, TX.True, 5);

        internal static ushort MakeCommand(Facility category, TX isTXCommand, byte index) => (ushort)((ushort)((uint)category << 8) | (uint)(ushort)((uint)isTXCommand << 7) | index);

        internal static void LookupCommand(
          ushort commandId,
          out Facility category,
          out TX isTXCommand,
          out byte index)
        {
            category = (Facility)((commandId & 65280) >> 8);
            isTXCommand = (TX)((commandId & 128) >> 7);
            index = (byte)(commandId & (uint)sbyte.MaxValue);
        }
    }
}
