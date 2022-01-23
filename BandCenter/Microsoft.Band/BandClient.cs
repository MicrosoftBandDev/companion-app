// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.BandClient
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using Google.Protobuf;
using Microsoft.Band.Notifications;
using Microsoft.Band.Personalization;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Band
{
    internal abstract class BandClient : IBandClient, IDisposable, IBandNotificationManager, IBandPersonalizationManager, IBandSensorManager, IBandTileManager
    {
        internal readonly ILoggerProvider loggerProvider;
        internal readonly IApplicationPlatformProvider applicationPlatformProvider;
        protected bool disposed;
        internal object protocolLock;
        private IDeviceTransport deviceTransport;
        internal FirmwareApp runningFirmwareApp;
        private CachedThings cachedThings;
        private const int MaxSensorSubscriptionQueueItemCount = 1000;
        private const int MaxTileEventQueueItemCount = 200;
        private readonly object streamingLock = new();
        private Task streamingTask;
        private ManualResetEvent streamingTaskAwake;
        private AutoResetEvent streamingDataReceivedEvent;
        private CancellationTokenSource streamingTaskCancel;
        private HashSet<byte> subscribedSensorTypes = new();
        private Queue<BandSensorReadingBase> sensorEventQueue = new();
        private bool eventingIsSubscribed;
        private Queue<BandTileEventBase> tileEventQueue = new();
        private Guid? currentAppId;
        private Dictionary<Guid, bool> tileIdOwnership = new();
        private static readonly BandSensorSampleDeserializer[] BandSensorSampleDeserializerTable = BandSensorSampleDeserializer.InitDeserializerTable();
        internal AccelerometerSensor accelerometer;
        internal GyroscopeSensor gyroscope;
        internal DistanceSensor distance;
        internal HeartRateSensor heartRate;
        internal ContactSensor contact;
        internal SkinTemperatureSensor skinTemperature;
        internal UVSensor uv;
        internal PedometerSensor pedometer;
        internal CaloriesSensor calories;
        internal GsrSensor gsr;
        internal RRIntervalSensor rrInterval;
        internal AmbientLightSensor als;
        internal BarometerSensor barometer;
        internal AltimeterSensor altimeter;
        private object tileEventLock = new();

        protected IDeviceTransport DeviceTransport => deviceTransport;

        protected bool Disposed => disposed;

        internal BandClient(IDeviceTransport deviceTransport, ILoggerProvider loggerProvider, IApplicationPlatformProvider applicationPlatformProvider)
        {
            this.deviceTransport = deviceTransport;
            this.loggerProvider = loggerProvider;
            this.applicationPlatformProvider = applicationPlatformProvider ?? throw new ArgumentNullException(nameof(applicationPlatformProvider));
            disposed = false;
            protocolLock = new object();
            cachedThings = new CachedThings(this);
            if (this.deviceTransport == null)
                return;
            this.deviceTransport.Disconnected += new EventHandler<TransportDisconnectedEventArgs>(DeviceTransport_Disconnected);
        }

        internal FirmwareApp FirmwareApp => runningFirmwareApp;

        internal CargoVersions FirmwareVersions { get; private set; }

        internal BandTypeConstants BandTypeConstants => FirmwareVersions.ApplicationVersion.PCBId < 20 ? BandTypeConstants.Cargo : BandTypeConstants.Envoy;

        public IBandNotificationManager NotificationManager => this;

        public IBandPersonalizationManager PersonalizationManager => this;

        public IBandTileManager TileManager => this;

        public IBandSensorManager SensorManager => this;

        internal void InitializeCachedProperties()
        {
            runningFirmwareApp = GetRunningFirmwareAppFromBand();
            FirmwareVersions = GetFirmwareVersionsFromBand();
        }

        public Task<string> GetFirmwareVersionAsync() => GetFirmwareVersionAsync(CancellationToken.None);

        public Task<string> GetFirmwareVersionAsync(CancellationToken token) => Task.FromResult(string.Format("{0:0}.{1:0}.{2:0}.{3:0}", FirmwareVersions.ApplicationVersion.VersionMajor, FirmwareVersions.ApplicationVersion.VersionMinor, FirmwareVersions.ApplicationVersion.BuildNumber, FirmwareVersions.ApplicationVersion.Revision));

        public Task<string> GetHardwareVersionAsync() => GetHardwareVersionAsync(CancellationToken.None);

        public Task<string> GetHardwareVersionAsync(CancellationToken token) => Task.FromResult(FirmwareVersions.ApplicationVersion.PCBId.ToString());

        protected abstract void OnDisconnected(TransportDisconnectedEventArgs args);

        private void DeviceTransport_Disconnected(object sender, TransportDisconnectedEventArgs args)
        {
            if (cachedThings != null)
                cachedThings.Clear();
            OnDisconnected(args);
        }

        internal FirmwareApp GetRunningFirmwareAppFromBand()
        {
            loggerProvider.Log(ProviderLogLevel.Verbose, "Retrieving running firmware app");
            using CargoCommandReader cargoCommandReader = ProtocolBeginRead(DeviceCommands.CargoCoreModuleWhoAmI, 1, CommandStatusHandling.DoNotCheck);
            int num = cargoCommandReader.ReadByte();
            CheckStatus(cargoCommandReader.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
            return (FirmwareApp)num;
        }

        internal CargoVersions GetFirmwareVersionsFromBand()
        {
            CheckIfDisposed();
            if (deviceTransport == null)
                throw new InvalidOperationException(BandResources.OperationRequiredConnectedDevice);
            CargoVersions cargoVersions = new();
            int bytesToRead = CargoVersion.GetSerializedByteCount() * 3;
            try
            {
                using CargoCommandReader cargoCommandReader = ProtocolBeginRead(DeviceCommands.CargoCoreModuleGetVersion, bytesToRead, CommandStatusHandling.DoNotCheck);
                for (int index = 0; index < 3; ++index)
                {
                    CargoVersion cargoVersion = CargoVersion.DeserializeFromBand(cargoCommandReader);
                    if (string.IsNullOrWhiteSpace(cargoVersion.AppName) || cargoVersion.VersionMajor == 0)
                    {
                        InvalidDataException invalidDataException = new(BandResources.InvalidAppAmount);
                        loggerProvider.LogException(ProviderLogLevel.Error, invalidDataException);
                        throw invalidDataException;
                    }
                    string appName = cargoVersion.AppName;
                    if (appName != "1BL")
                    {
                        if (appName != "2UP")
                        {
                            if (appName == "App")
                                cargoVersions.ApplicationVersion = cargoVersion;
                            else
                                throw new InvalidDataException($"Firmware version name \"{cargoVersion.AppName}\" read from the device was not recognized.");
                        }
                        else
                            cargoVersions.UpdaterVersion = cargoVersion;
                    }
                    else
                        cargoVersions.BootloaderVersion = cargoVersion;
                }
                CheckStatus(cargoCommandReader.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
            return cargoVersions;
        }

        internal void CheckFirmwareSdkBit(FirmwareSdkCheckPlatform platform, byte reserved)
        {
            CheckIfDisposed();
            CheckIfDisconnected();
            void writeArgBuf(ICargoWriter w)
            {
                w.WriteByte((byte)platform);
                w.WriteByte(reserved);
                w.WriteUInt16(3);
            }
            if ((int)ProtocolWriteWithArgs(DeviceCommands.CargoCoreModuleSdkCheck, 4, writeArgBuf, statusHandling: CommandStatusHandling.DoNotThrow).Status != (int)DeviceStatusCodeUtils.Success)
                throw new BandException(BandResources.SdkVersionNotSupported);
        }

        internal void CheckIfDisposed()
        {
            if (disposed)
            {
                ObjectDisposedException disposedException = new(nameof(BandClient));
                loggerProvider.LogException(ProviderLogLevel.Error, disposedException);
                throw disposedException;
            }
        }

        internal void CheckIfDisconnected()
        {
            if (deviceTransport == null)
            {
                InvalidOperationException operationException = new(BandResources.OperationRequiredConnectedDevice);
                loggerProvider.LogException(ProviderLogLevel.Error, operationException);
                throw operationException;
            }
        }

        internal void CheckIfDisconnectedOrUpdateMode()
        {
            CheckIfDisconnected();
            if (runningFirmwareApp != FirmwareApp.App)
            {
                BandIOException bandIoException = new(string.Format(BandResources.DeviceInNonAppMode, runningFirmwareApp));
                loggerProvider.LogException(ProviderLogLevel.Error, bandIoException);
                throw bandIoException;
            }
        }

        internal void CheckIfNotEnvoy()
        {
            if (BandTypeConstants.BandType != BandType.Envoy)
                throw new InvalidOperationException("Envoy required");
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || disposed)
                return;
            IDeviceTransport deviceTransport = this.deviceTransport;
            if (deviceTransport != null)
            {
                lock (streamingLock)
                    StopStreamingSubscriptionTasks();
                deviceTransport.Dispose();
                this.deviceTransport = null;
                loggerProvider.Log(ProviderLogLevel.Info, "BandClient Transport disposed.");
            }
            streamingDataReceivedEvent?.Dispose();
            disposed = true;
        }

        public Task ShowDialogAsync(Guid tileId, string title, string body) => ShowDialogAsync(tileId, title, body, CancellationToken.None);

        public Task ShowDialogAsync(
          Guid tileId,
          string title,
          string body,
          CancellationToken cancel)
        {
            if (tileId == Guid.Empty)
                throw new ArgumentException(BandResources.NotificationInvalidTileId, nameof(tileId));
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
                throw new ArgumentException(BandResources.NotificationFieldsEmpty);
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            return Task.Run(() => ShowDialogWithOwnerValidation(tileId, title, body, cancel), cancel);
        }

        internal void ShowDialogWithOwnerValidation(
          Guid tileId,
          string title,
          string body,
          CancellationToken cancel)
        {
            if (!TileInstalledAndOwned(tileId, cancel))
                return;
            ShowDialogHelper(tileId, title ?? string.Empty, body ?? string.Empty, cancel);
        }

        public Task SendMessageAsync(
          Guid tileId,
          string title,
          string body,
          DateTimeOffset timestamp,
          MessageFlags flags = MessageFlags.None)
        {
            return SendMessageAsync(tileId, title, body, timestamp, flags, CancellationToken.None);
        }

        public Task SendMessageAsync(
          Guid tileId,
          string title,
          string body,
          DateTimeOffset timestamp,
          MessageFlags flags,
          CancellationToken cancel)
        {
            if (tileId == Guid.Empty)
                throw new ArgumentException(BandResources.NotificationInvalidTileId, nameof(tileId));
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(body))
                throw new ArgumentException(BandResources.NotificationFieldsEmpty);
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            return Task.Run(() => SendMessageWithOwnerValidation(tileId, title, body, timestamp, flags, cancel), cancel);
        }

        internal void SendMessageWithOwnerValidation(
          Guid tileId,
          string title,
          string body,
          DateTimeOffset timestamp,
          MessageFlags flags,
          CancellationToken cancel)
        {
            if (!TileInstalledAndOwned(tileId, cancel))
                return;
            SendMessage(tileId, title ?? string.Empty, body ?? string.Empty, timestamp, flags, cancel);
        }

        public Task VibrateAsync(VibrationType vibrationType) => VibrateAsync(vibrationType, CancellationToken.None);

        public Task VibrateAsync(VibrationType vibrationType, CancellationToken cancel)
        {
            BandVibrationType bandVibrationType = vibrationType.ToBandVibrationType();
            return Task.Run(() => VibrateHelper(bandVibrationType, cancel), cancel);
        }

        internal void SendMessage(
          Guid tileId,
          string title,
          string body,
          DateTimeOffset timestamp,
          MessageFlags flags,
          CancellationToken token)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            token.ThrowIfCancellationRequested();
            NotificationMessaging notificationMessaging = new(tileId)
            {
                Timestamp = timestamp,
                Title = title,
                Body = body
            };
            if (!flags.HasFlag(MessageFlags.ShowDialog))
                notificationMessaging.Flags = 2;
            token.ThrowIfCancellationRequested();
            SendNotification(NotificationID.Messaging, NotificationPBMessageType.Messaging, notificationMessaging);
        }

        protected void ShowDialogHelper(
          Guid tileId,
          string title,
          string body,
          CancellationToken token,
          BandNotificationFlags flagbits = BandNotificationFlags.UnmodifiedNotificationSettings)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            token.ThrowIfCancellationRequested();
            NotificationGenericDialog notificationGenericDialog = new(tileId)
            {
                Title = title,
                Body = body
            };
            if (flagbits.HasFlag(BandNotificationFlags.ForceNotificationDialog))
                notificationGenericDialog.Flags |= 1;
            else
                notificationGenericDialog.Flags &= 254;
            token.ThrowIfCancellationRequested();
            SendNotification(NotificationID.GenericDialog, NotificationPBMessageType.GenericDialog, notificationGenericDialog);
        }

        protected void SendNotification(
          NotificationID notificationId,
          NotificationPBMessageType notificationPbType,
          NotificationBase notification)
        {
            int argBufSize = 0;
            Action<ICargoWriter> writeArgBuf = null;
            ushort commandId;
            int byteCount;
            switch (BandTypeConstants.BandType)
            {
                case BandType.Cargo:
                    commandId = DeviceCommands.CargoNotification;
                    byteCount = 2 + notification.GetSerializedByteCount();
                    break;
                case BandType.Envoy:
                    commandId = DeviceCommands.CargoNotificationProtoBuf;
                    byteCount = notification.GetSerializedProtobufByteCount();
                    argBufSize = 4;
                    writeArgBuf = w =>
                    {
                        w.WriteUInt16((ushort)byteCount);
                        w.WriteUInt16((ushort)notificationPbType);
                    };
                    break;
                default:
                    throw new InvalidOperationException();
            }
            using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(commandId, argBufSize, byteCount, writeArgBuf, CommandStatusHandling.DoNotCheck);
            switch (BandTypeConstants.BandType)
            {
                case BandType.Cargo:
                    cargoCommandWriter.WriteUInt16((ushort)notificationId);
                    notification.SerializeToBand(cargoCommandWriter);
                    break;
                case BandType.Envoy:
                    CodedOutputStream output = new(cargoCommandWriter, byteCount);
                    notification.SerializeProtobufToBand(output);
                    output.Flush();
                    break;
            }
            CheckStatus(cargoCommandWriter.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
        }

        protected void VibrateHelper(BandVibrationType bandVibrationType, CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            cancel.ThrowIfCancellationRequested();
            void writeData(ICargoWriter w) => w.WriteByte((byte)bandVibrationType);
            ProtocolWriteWithData(DeviceCommands.CargoHapticPlayVibrationStream, 1, writeData);
        }

        public Task<BandImage> GetMeTileImageAsync() => GetMeTileImageAsync(CancellationToken.None);

        public Task<BandImage> GetMeTileImageAsync(CancellationToken cancel) => Task.Run(() => GetMeTileImageInternal(cancel), cancel);

        public Task SetMeTileImageAsync(BandImage image) => SetMeTileImageAsync(image, CancellationToken.None);

        public Task SetMeTileImageAsync(BandImage image, CancellationToken cancel) => image != null ? Task.Run(() => SetMeTileImageInternal(image, uint.MaxValue, cancel), cancel) : Task.Run(() => ClearMeTileImageInternal(cancel), cancel);

        public Task<BandTheme> GetThemeAsync() => GetThemeAsync(CancellationToken.None);

        public Task<BandTheme> GetThemeAsync(CancellationToken cancel) => Task.Run(() => GetThemeInternal(cancel), cancel);

        public Task SetThemeAsync(BandTheme theme) => SetThemeAsync(theme, CancellationToken.None);

        public Task SetThemeAsync(BandTheme theme, CancellationToken cancel) => theme == null ? Task.Run(() => ResetThemeInternal(cancel), cancel) : Task.Run(() => SetThemeInternal(theme, cancel), cancel);

        protected BandImage GetMeTileImageInternal(CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            cancel.ThrowIfCancellationRequested();
            uint imageId = 0;
            void readData1(ICargoReader r) => imageId = r.ReadUInt32();
            ProtocolRead(DeviceCommands.CargoSystemSettingsGetMeTileImageID, 4, readData1);
            if (imageId == 0U)
                return null;
            cancel.ThrowIfCancellationRequested();
            int byteCount = BandTypeConstants.MeTileWidth * BandTypeConstants.MeTileHeight * 2;
            byte[] pixelData = null;
            void readData2(ICargoReader r) => pixelData = r.ReadExact(byteCount);
            ProtocolRead(DeviceCommands.CargoFireballUIReadMeTileImage, byteCount, readData2, 60000);
            return new BandImage(BandTypeConstants.MeTileWidth, BandTypeConstants.MeTileHeight, pixelData);
        }

        protected void SetMeTileImageInternal(BandImage image, uint imageId, CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            ValidateMeTileImage(image, imageId);
            cancel.ThrowIfCancellationRequested();
            RunUsingSynchronizedFirmwareUI(() => SetMeTileImageInternal(image, imageId));
        }

        protected void ClearMeTileImageInternal(CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            cancel.ThrowIfCancellationRequested();
            ProtocolWrite(DeviceCommands.CargoFireballUIClearMeTileImage, 60000);
        }

        protected void SetMeTileImageInternal(BandImage image, uint imageId)
        {
            void writeArgBuf(ICargoWriter w) => w.WriteUInt32(imageId);
            void writeData(ICargoWriter w) => w.Write(image.PixelData);
            ProtocolWrite(DeviceCommands.CargoFireballUIWriteMeTileImageWithID, 4, image.PixelData.Length, writeArgBuf, writeData, 60000);
        }

        protected BandTheme GetThemeInternal(CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            cancel.ThrowIfCancellationRequested();
            using CargoCommandReader cargoCommandReader = ProtocolBeginRead(DeviceCommands.CargoThemeColorGetFirstPartyTheme, 24, CommandStatusHandling.ThrowOnlySeverityError);
            return new BandTheme()
            {
                Base = new BandColor(cargoCommandReader.ReadUInt32()),
                Highlight = new BandColor(cargoCommandReader.ReadUInt32()),
                Lowlight = new BandColor(cargoCommandReader.ReadUInt32()),
                SecondaryText = new BandColor(cargoCommandReader.ReadUInt32()),
                HighContrast = new BandColor(cargoCommandReader.ReadUInt32()),
                Muted = new BandColor(cargoCommandReader.ReadUInt32())
            };
        }

        protected void ResetThemeInternal(CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            cancel.ThrowIfCancellationRequested();
            RunUsingSynchronizedFirmwareUI(() => ProtocolWrite(DeviceCommands.CargoThemeColorReset));
        }

        protected void SetThemeInternal(BandTheme theme, CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            cancel.ThrowIfCancellationRequested();
            RunUsingSynchronizedFirmwareUI(() => SetThemeInternal(theme));
        }

        protected void SetThemeInternal(BandTheme theme)
        {
            using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoThemeColorSetFirstPartyTheme, 24, CommandStatusHandling.ThrowOnlySeverityError);
            cargoCommandWriter.WriteUInt32(theme.Base.ToRgb());
            cargoCommandWriter.WriteUInt32(theme.Highlight.ToRgb());
            cargoCommandWriter.WriteUInt32(theme.Lowlight.ToRgb());
            cargoCommandWriter.WriteUInt32(theme.SecondaryText.ToRgb());
            cargoCommandWriter.WriteUInt32(theme.HighContrast.ToRgb());
            cargoCommandWriter.WriteUInt32(theme.Muted.ToRgb());
        }

        protected void ValidateMeTileImage(BandImage image, uint imageId = 4294967295)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (image.Width != BandTypeConstants.MeTileWidth)
                throw new ArgumentException(string.Format(BandResources.MeTileImageWidthError, BandTypeConstants.MeTileWidth));
            switch (BandTypeConstants.BandType)
            {
                case BandType.Cargo:
                    if (image.Height != BandTypeConstants.Cargo.MeTileHeight)
                        throw new ArgumentException(string.Format(BandResources.MeTileHeightHeightError, BandTypeConstants.Cargo.MeTileHeight));
                    break;
                case BandType.Envoy:
                    if (image.Height != BandTypeConstants.Cargo.MeTileHeight && image.Height != BandTypeConstants.Envoy.MeTileHeight)
                        throw new ArgumentException(string.Format(BandResources.MeTileHeightHeightError, BandTypeConstants.Envoy.MeTileHeight));
                    break;
                default:
                    throw new InvalidOperationException("Internal error: BandClass unrecognized");
            }
            if (imageId == 0U)
                throw new ArgumentOutOfRangeException(nameof(imageId));
        }

        internal CargoStatus ProtocolRead(
          ushort commandId,
          int dataSize,
          Action<ICargoReader> readData,
          int timeout = 5000,
          CommandStatusHandling statusHandling = CommandStatusHandling.ThrowOnlySeverityError)
        {
            return ProtocolRead(commandId, 0, dataSize, null, readData, timeout, statusHandling);
        }

        internal CargoStatus ProtocolRead(
          ushort commandId,
          int argBufSize,
          int dataSize,
          Action<ICargoWriter> writeArgBuf,
          Action<ICargoReader> readData,
          int timeout = 5000,
          CommandStatusHandling statusHandling = CommandStatusHandling.ThrowOnlySeverityError)
        {
            if (argBufSize < 0)
                throw new ArgumentOutOfRangeException(nameof(argBufSize));
            if (dataSize < 0)
                throw new ArgumentOutOfRangeException(nameof(dataSize));
            if (argBufSize > 0 && writeArgBuf == null)
                throw new ArgumentNullException(nameof(writeArgBuf));
            if (dataSize > 0 && readData == null)
                throw new ArgumentNullException(nameof(readData));
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, (uint)argBufSize, (uint)dataSize, writeArgBuf, true);
                    if (dataSize > 0)
                    {
                        deviceTransport.CargoStream.ReadTimeout = timeout;
                        readData(deviceTransport.CargoReader);
                    }
                    deviceTransport.CargoStream.ReadTimeout = 5000;
                    CargoStatus status = deviceTransport.CargoReader.ReadStatusPacket();
                    CheckStatus(status, statusHandling, loggerProvider);
                    return status;
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal CargoCommandReader ProtocolBeginRead(
          ushort commandId,
          int bytesToRead,
          CommandStatusHandling statusHandling)
        {
            return ProtocolBeginRead(commandId, 0, bytesToRead, null, statusHandling);
        }

        internal CargoCommandReader ProtocolBeginRead(
          ushort commandId,
          int argBufSize,
          int bytesToRead,
          Action<ICargoWriter> writeArgBuf,
          CommandStatusHandling statusHandling)
        {
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.ReadTimeout = 5000;
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, (uint)argBufSize, (uint)bytesToRead, writeArgBuf, true);
                    return new CargoCommandReader(deviceTransport, bytesToRead, protocolLock, loggerProvider, statusHandling);
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal CargoStatus ProtocolWrite(
          ushort commandId,
          int timeout = 5000,
          bool swallowStatusReadException = false,
          CommandStatusHandling statusHandling = CommandStatusHandling.ThrowOnlySeverityError)
        {
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, 0U, 0U, null, true);
                    CargoStatus status;
                    try
                    {
                        deviceTransport.CargoStream.ReadTimeout = timeout;
                        status = deviceTransport.CargoReader.ReadStatusPacket();
                    }
                    catch
                    {
                        if (swallowStatusReadException)
                            return new CargoStatus();
                        throw;
                    }
                    CheckStatus(status, statusHandling, loggerProvider);
                    return status;
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal CargoStatus ProtocolWriteWithArgs(
          ushort commandId,
          int argBufSize,
          Action<ICargoWriter> writeArgBuf,
          int timeout = 5000,
          CommandStatusHandling statusHandling = CommandStatusHandling.ThrowOnlySeverityError)
        {
            if (argBufSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(argBufSize));
            if (writeArgBuf == null)
                throw new ArgumentNullException(nameof(writeArgBuf));
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, (uint)argBufSize, 0U, writeArgBuf, true);
                    deviceTransport.CargoStream.ReadTimeout = timeout;
                    CargoStatus status = deviceTransport.CargoReader.ReadStatusPacket();
                    CheckStatus(status, statusHandling, loggerProvider);
                    return status;
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal CargoStatus ProtocolWriteWithData(
          ushort commandId,
          int dataSize,
          Action<ICargoWriter> writeData,
          int timeout = 5000,
          CommandStatusHandling statusHandling = CommandStatusHandling.ThrowOnlySeverityError)
        {
            if (dataSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(dataSize));
            if (writeData == null)
                throw new ArgumentNullException(nameof(writeData));
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, 0U, (uint)dataSize, null, false);
                    writeData(deviceTransport.CargoWriter);
                    deviceTransport.CargoWriter.Flush();
                    deviceTransport.CargoStream.ReadTimeout = timeout;
                    CargoStatus status = deviceTransport.CargoReader.ReadStatusPacket();
                    CheckStatus(status, statusHandling, loggerProvider);
                    return status;
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal CargoStatus ProtocolWrite(
          ushort commandId,
          int argBufSize,
          int dataSize,
          Action<ICargoWriter> writeArgBuf,
          Action<ICargoWriter> writeData,
          int timeout = 5000,
          CommandStatusHandling statusHandling = CommandStatusHandling.ThrowOnlySeverityError)
        {
            if (argBufSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(argBufSize));
            if (writeArgBuf == null)
                throw new ArgumentNullException(nameof(writeArgBuf));
            if (dataSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(dataSize));
            if (writeData == null)
                throw new ArgumentNullException(nameof(writeData));
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, (uint)argBufSize, (uint)dataSize, writeArgBuf, false);
                    writeData(deviceTransport.CargoWriter);
                    deviceTransport.CargoWriter.Flush();
                    deviceTransport.CargoStream.ReadTimeout = timeout;
                    CargoStatus status = deviceTransport.CargoReader.ReadStatusPacket();
                    CheckStatus(status, statusHandling, loggerProvider);
                    return status;
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal CargoCommandWriter ProtocolBeginWrite(
          ushort commandId,
          int dataSize,
          CommandStatusHandling statusHandling)
        {
            return ProtocolBeginWrite(commandId, 0, dataSize, null, statusHandling);
        }

        internal CargoCommandWriter ProtocolBeginWrite(
          ushort commandId,
          int argBufSize,
          int dataSize,
          Action<ICargoWriter> writeArgBuf,
          CommandStatusHandling statusHandling)
        {
            if (dataSize == 0)
            {
                ArgumentException argumentException = new("dataSize may not be zero");
                loggerProvider.LogException(ProviderLogLevel.Error, argumentException);
                throw argumentException;
            }
            try
            {
                lock (protocolLock)
                {
                    deviceTransport.CargoStream.ReadTimeout = 5000;
                    deviceTransport.CargoStream.WriteTimeout = 5000;
                    deviceTransport.WriteCommandPacket(commandId, (uint)argBufSize, (uint)dataSize, writeArgBuf, false);
                    return new CargoCommandWriter(deviceTransport, dataSize, protocolLock, loggerProvider, statusHandling);
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        internal static void CheckStatus(CargoStatus status, CommandStatusHandling statusHandling, ILoggerProvider loggerProvider)
        {
            if (statusHandling == CommandStatusHandling.DoNotCheck || (int)status.Status == (int)DeviceStatusCodeUtils.Success)
                return;
            string message = string.Format(BandResources.CommandStatusError, status.Status);
            switch (statusHandling)
            {
                case CommandStatusHandling.ThrowAnyNonZero:
                    loggerProvider.Log(ProviderLogLevel.Error, message);
                    throw new BandOperationException(status.Status, message);
                case CommandStatusHandling.ThrowOnlySeverityError:
                    if (DeviceStatusCodeUtils.IsSeverityError(status.Status))
                    {
                        loggerProvider.Log(ProviderLogLevel.Error, message);
                        throw new BandOperationException(status.Status, message);
                    }
                    break;
            }
            loggerProvider.Log(ProviderLogLevel.Verbose, message);
        }

        protected Task StreamingTask
        {
            get => streamingTask;
            set => streamingTask = value;
        }

        protected CancellationTokenSource StreamingTaskCancel
        {
            get => streamingTaskCancel;
            set => streamingTaskCancel = value;
        }

        protected object StreamingLock => streamingLock;

        protected ManualResetEvent StreamingTaskAwake => streamingTaskAwake;

        protected HashSet<byte> SubscribedSensorTypes => subscribedSensorTypes;

        protected bool EventingIsSubscribed => eventingIsSubscribed;

        public IBandSensor<IBandAccelerometerReading> Accelerometer
        {
            get
            {
                if (accelerometer == null)
                    lock (streamingLock)
                        if (accelerometer == null)
                            accelerometer = new AccelerometerSensor(this);
                return accelerometer;
            }
        }

        public IBandSensor<IBandGyroscopeReading> Gyroscope
        {
            get
            {
                if (gyroscope == null)
                    lock (streamingLock)
                        if (gyroscope == null)
                            gyroscope = new GyroscopeSensor(this);
                return gyroscope;
            }
        }

        public IBandSensor<IBandDistanceReading> Distance
        {
            get
            {
                if (distance == null)
                    lock (streamingLock)
                        if (distance == null)
                            distance = new DistanceSensor(this);
                return distance;
            }
        }

        public IBandSensor<IBandHeartRateReading> HeartRate
        {
            get
            {
                if (heartRate == null)
                    lock (streamingLock)
                        if (heartRate == null)
                            heartRate = new HeartRateSensor(this);
                return heartRate;
            }
        }

        public IBandContactSensor Contact
        {
            get
            {
                if (contact == null)
                    lock (streamingLock)
                        if (contact == null)
                            contact = new ContactSensor(this);
                return contact;
            }
        }

        public IBandSensor<IBandSkinTemperatureReading> SkinTemperature
        {
            get
            {
                if (skinTemperature == null)
                    lock (streamingLock)
                        if (skinTemperature == null)
                            skinTemperature = new SkinTemperatureSensor(this);
                return skinTemperature;
            }
        }

        public IBandSensor<IBandUVReading> UV
        {
            get
            {
                if (uv == null)
                    lock (streamingLock)
                        if (uv == null)
                            uv = new UVSensor(this);
                return uv;
            }
        }

        public IBandSensor<IBandPedometerReading> Pedometer
        {
            get
            {
                if (pedometer == null)
                    lock (streamingLock)
                        if (pedometer == null)
                            pedometer = new PedometerSensor(this);
                return pedometer;
            }
        }

        public IBandSensor<IBandCaloriesReading> Calories
        {
            get
            {
                if (calories == null)
                    lock (streamingLock)
                        if (calories == null)
                            calories = new CaloriesSensor(this);
                return calories;
            }
        }

        public IBandSensor<IBandGsrReading> Gsr
        {
            get
            {
                if (gsr == null)
                    lock (streamingLock)
                        if (gsr == null)
                            gsr = new GsrSensor(this);
                return gsr;
            }
        }

        public IBandSensor<IBandRRIntervalReading> RRInterval
        {
            get
            {
                if (rrInterval == null)
                    lock (streamingLock)
                        if (rrInterval == null)
                            rrInterval = new RRIntervalSensor(this);
                return rrInterval;
            }
        }

        public IBandSensor<IBandAmbientLightReading> AmbientLight
        {
            get
            {
                if (als == null)
                    lock (streamingLock)
                        if (als == null)
                            als = new AmbientLightSensor(this);
                return als;
            }
        }

        public IBandSensor<IBandBarometerReading> Barometer
        {
            get
            {
                if (barometer == null)
                    lock (streamingLock)
                        if (barometer == null)
                            barometer = new BarometerSensor(this);
                return barometer;
            }
        }

        public IBandSensor<IBandAltimeterReading> Altimeter
        {
            get
            {
                if (altimeter == null)
                    lock (streamingLock)
                        if (altimeter == null)
                            altimeter = new AltimeterSensor(this);
                return altimeter;
            }
        }

        internal bool IsSensorSubscribed(SubscriptionType type) => SubscribedSensorTypes.Contains((byte)type);

        internal void EventingSubscribe()
        {
            CheckIfDisposed();
            lock (streamingLock)
            {
                StartOrAwakeStreamingSubscriptionTasks();
                eventingIsSubscribed = true;
            }
        }

        internal void SensorSubscribe(SubscriptionType type)
        {
            CheckIfDisposed();
            lock (streamingLock)
            {
                StartOrAwakeStreamingSubscriptionTasks();
                if (IsSensorSubscribed(type))
                    return;
                ExecuteSensorSubscribeCommand(type);
                lock (SubscribedSensorTypes)
                    SubscribedSensorTypes.Add((byte)type);
            }
        }

        internal void EventingUnsubscribe()
        {
            CheckIfDisposed();
            lock (streamingLock)
            {
                if (!EventingIsSubscribed)
                    return;
                eventingIsSubscribed = false;
                if (SubscribedSensorTypes.Count != 0)
                    return;
                StopStreamingSubscriptionTasks();
            }
        }

        internal void SensorUnsubscribe(SubscriptionType type)
        {
            CheckIfDisposed();
            lock (streamingLock)
            {
                if (!IsSensorSubscribed(type))
                    return;
                ExecuteSensorUnsubscribeCommand(type);
                bool flag = false;
                lock (SubscribedSensorTypes)
                {
                    SubscribedSensorTypes.Remove((byte)type);
                    flag = SubscribedSensorTypes.Count == 0 && !EventingIsSubscribed;
                }
                if (!flag)
                    return;
                StopStreamingSubscriptionTasks();
            }
        }

        protected virtual void ExecuteSensorSubscribeCommand(SubscriptionType type)
        {
            void writeArgBuf(ICargoWriter w)
            {
                w.WriteByte((byte)type);
                w.WriteBool32(false);
            }
            loggerProvider.Log(ProviderLogLevel.Info, $"Remote subscribing to {type} sensor.");
            ProtocolWriteWithArgs(DeviceCommands.CargoRemoteSubscriptionSubscribe, 5, writeArgBuf);
        }

        protected virtual void ExecuteSensorUnsubscribeCommand(SubscriptionType type)
        {
            loggerProvider.Log(ProviderLogLevel.Info, $"Remote unsubscribing to {type} sensor.");
            void writeArgBuf(ICargoWriter w) => w.WriteByte((byte)type);
            ProtocolWriteWithArgs(DeviceCommands.CargoRemoteSubscriptionUnsubscribe, 1, writeArgBuf);
        }

        protected virtual void StartOrAwakeStreamingSubscriptionTasks()
        {
            if (!currentAppId.HasValue)
                currentAppId = new Guid?(applicationPlatformProvider.GetApplicationIdAsync(CancellationToken.None).Result);
            if (streamingTask == null)
            {
                streamingDataReceivedEvent = new AutoResetEvent(false);
                streamingTaskAwake = new ManualResetEvent(false);
                streamingTaskCancel = new CancellationTokenSource();
                loggerProvider.Log(ProviderLogLevel.Info, "Starting the streaming tasks...");
                Task.Run(() => FireSubscribedEvents(streamingDataReceivedEvent, streamingTaskCancel.Token));
                using ManualResetEvent started = new(false);
                streamingTask = Task.Run(() => StreamBandData(started, streamingTaskCancel.Token));
                started.WaitOne();
            }
            else
                streamingTaskAwake.Set();
        }

        protected virtual void StopStreamingSubscriptionTasks()
        {
            if (streamingTask == null)
                return;
            loggerProvider.Log(ProviderLogLevel.Info, "Signaling the streaming tasks to stop...");
            streamingTaskCancel.Cancel();
            streamingTask.Wait();
            streamingTaskCancel.Dispose();
            streamingTaskCancel = null;
            streamingTask = null;
            streamingTaskAwake.Dispose();
            streamingTaskAwake = null;
            streamingDataReceivedEvent = null;
            lock (sensorEventQueue)
                sensorEventQueue.Clear();
            lock (tileEventQueue)
                tileEventQueue.Clear();
            loggerProvider.Log(ProviderLogLevel.Info, "Streaming task has stopped");
        }

        private void FireSubscribedEvents(AutoResetEvent awake, CancellationToken stop)
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Starting task that fires events for streaming data...");
            WaitHandle[] waitHandles = new[]
            {
                awake,
                stop.WaitHandle
            };
            while (!stop.IsCancellationRequested && WaitHandle.WaitAny(waitHandles) == 0)
            {
                int num1 = 0;
                int num2 = 0;
                do
                {
                    BandSensorReadingBase sensorReadingBase = null;
                    BandTileEventBase bandTileEventBase = null;
                    lock (sensorEventQueue)
                    {
                        if ((num1 = sensorEventQueue.Count) > 0)
                            sensorReadingBase = sensorEventQueue.Dequeue();
                    }
                    if (!stop.IsCancellationRequested)
                    {
                        if (num1 > 0)
                            sensorReadingBase.Dispatch(this);
                        lock (tileEventQueue)
                        {
                            if ((num2 = tileEventQueue.Count) > 0)
                                bandTileEventBase = tileEventQueue.Dequeue();
                        }
                        if (!stop.IsCancellationRequested)
                        {
                            if (num2 > 0)
                                bandTileEventBase.Dispatch(this);
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                while (!stop.IsCancellationRequested && (num1 > 0 || num2 > 0));
            }
            awake.Dispose();
            loggerProvider.Log(ProviderLogLevel.Info, "Stopping task that fires events for streaming data...");
        }

        protected abstract void StreamBandData(ManualResetEvent started, CancellationToken stop);

        protected int ProcessSensorSubscriptionPayload(ICargoReader reader)
        {
            RemoteSubscriptionSampleHeader sampleHeader = RemoteSubscriptionSampleHeader.DeserializeFromBand(reader);
            switch (sampleHeader.SubscriptionType)
            {
                case SubscriptionType.Accelerometer32MS:
                case SubscriptionType.AccelerometerGyroscope32MS:
                case SubscriptionType.Accelerometer16MS:
                case SubscriptionType.AccelerometerGyroscope16MS:
                    int num = 0;
                    BandSensorSampleDeserializer sampleDeserializer = TryGetBandSensorSampleDeserializer(sampleHeader);
                    if (sampleDeserializer == null)
                    {
                        loggerProvider.Log(ProviderLogLevel.Warning, $"Unsupported subscription type {sampleHeader.SubscriptionType} received.");
                    }
                    else
                    {
                        num = sampleDeserializer.GetSerializeByteCount(sampleHeader);
                        if (sampleHeader.SampleSize % num != 0)
                        {
                            loggerProvider.Log(ProviderLogLevel.Error, $"Subscription type {sampleHeader.SubscriptionType} sample array size is not multiple of sample size.");
                            num = 0;
                        }
                    }
                    if (num == 0)
                    {
                        reader.ReadExactAndDiscard(sampleHeader.SampleSize);
                    }
                    else
                    {
                        DateTimeOffset now = DateTimeOffset.Now;
                        lock (sensorEventQueue)
                        {
                            for (int index = sampleHeader.SampleSize / num; index > 0; --index)
                            {
                                sensorEventQueue.Enqueue(sampleDeserializer.DeserializeFromBand(reader, sampleHeader, now));
                                if (sensorEventQueue.Count > 1000)
                                    sensorEventQueue.Dequeue();
                                streamingDataReceivedEvent.Set();
                            }
                        }
                    }
                    return RemoteSubscriptionSampleHeader.GetSerializedByteCount() + sampleHeader.SampleSize;
                default:
                    loggerProvider.Log(ProviderLogLevel.Info, $"QueueSensorSubscriptionPayload(): Type: {sampleHeader.SubscriptionType}, Missed Samples: {sampleHeader.NumMissedSamples}, Sample Size: {sampleHeader.SampleSize}");
                    goto case SubscriptionType.Accelerometer32MS;
            }
        }

        private static BandSensorSampleDeserializer TryGetBandSensorSampleDeserializer(RemoteSubscriptionSampleHeader sampleHeader)
        {
            return sampleHeader.SubscriptionType >= (SubscriptionType)BandSensorSampleDeserializerTable.Length ? null : BandSensorSampleDeserializerTable[(int)sampleHeader.SubscriptionType];
        }

        protected int ProcessTileEventPayload(ICargoReader reader)
        {
            BandTileEventBase bandTileEventBase = BandTileEventBase.DeserializeFromBand(reader, DateTimeOffset.Now, out byte[] tileFriendlyName);
            if (bandTileEventBase != null)
            {
                loggerProvider.Log(ProviderLogLevel.Info, $"QueueTileEventPayload(): Type: {bandTileEventBase.GetType().Name}, TileId: {bandTileEventBase.TileId}");
                if (!tileIdOwnership.TryGetValue(bandTileEventBase.TileId, out bool flag))
                {
                    Guid? currentAppId = this.currentAppId;
                    Guid applicationIdFromName = GetApplicationIdFromName(tileFriendlyName, 0);
                    flag = currentAppId.HasValue && (!currentAppId.HasValue || currentAppId.GetValueOrDefault() == applicationIdFromName);
                    tileIdOwnership.Add(bandTileEventBase.TileId, flag);
                }
                if (flag)
                {
                    lock (tileEventQueue)
                    {
                        tileEventQueue.Enqueue(bandTileEventBase);
                        if (tileEventQueue.Count > 200)
                            tileEventQueue.Dequeue();
                        streamingDataReceivedEvent.Set();
                    }
                }
            }
            return BandTileEventBase.GetSerializedByteCount();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void DispatchTileOpenedEvent(BandTileOpenedEvent tileEvent)
        {
            EventHandler<BandTileEventArgs<IBandTileOpenedEvent>> tileOpened = TileOpened;
            if (tileOpened == null)
                return;
            try
            {
                tileOpened(this, new BandTileEventArgs<IBandTileOpenedEvent>(tileEvent));
            }
            catch (Exception ex)
            {
                loggerProvider.LogException(ProviderLogLevel.Error, ex);
                Environment.FailFast("BandClient.TileOpened event handler threw an exception that was not handled by the application.", ex);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void DispatchTileButtonPressedEvent(BandTileButtonPressedEvent tileEvent)
        {
            EventHandler<BandTileEventArgs<IBandTileButtonPressedEvent>> tileButtonPressed = TileButtonPressed;
            if (tileButtonPressed == null)
                return;
            try
            {
                tileButtonPressed(this, new BandTileEventArgs<IBandTileButtonPressedEvent>(tileEvent));
            }
            catch (Exception ex)
            {
                loggerProvider.LogException(ProviderLogLevel.Error, ex);
                Environment.FailFast("BandClient.TileButtonPressed event handler threw an exception that was not handled by the application.", ex);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal void DispatchTileClosedEvent(BandTileClosedEvent tileEvent)
        {
            EventHandler<BandTileEventArgs<IBandTileClosedEvent>> tileClosed = TileClosed;
            if (tileClosed == null)
                return;
            try
            {
                tileClosed(this, new BandTileEventArgs<IBandTileClosedEvent>(tileEvent));
            }
            catch (Exception ex)
            {
                loggerProvider.LogException(ProviderLogLevel.Error, ex);
                Environment.FailFast("BandClient.TileClosed event handler threw an exception that was not handled by the application.", ex);
            }
        }

        public static Guid GetApplicationIdFromName(byte[] nameAndOwnerId, ushort friendlyNameLength)
        {
            return nameAndOwnerId != null && nameAndOwnerId.Length >= 16 && friendlyNameLength <= 21 ? BandBitConverter.ToGuid(nameAndOwnerId, nameAndOwnerId.Length - 16) : Guid.Empty;
        }

        public event EventHandler<BandTileEventArgs<IBandTileOpenedEvent>> TileOpened;

        public event EventHandler<BandTileEventArgs<IBandTileButtonPressedEvent>> TileButtonPressed;

        public event EventHandler<BandTileEventArgs<IBandTileClosedEvent>> TileClosed;

        public Task StartReadingsAsync() => StartReadingsAsync(CancellationToken.None);

        public virtual Task StartReadingsAsync(CancellationToken token) => Task.Run(() =>
        {
            lock (tileEventLock)
                EventingSubscribe();
        }, token);

        public Task StopReadingsAsync() => StopReadingsAsync(CancellationToken.None);

        public virtual Task StopReadingsAsync(CancellationToken token) => Task.Run(() =>
        {
            lock (tileEventLock)
                EventingUnsubscribe();
        }, token);

        public Task<IEnumerable<BandTile>> GetTilesAsync() => GetTilesAsync(CancellationToken.None);

        public Task<IEnumerable<BandTile>> GetTilesAsync(CancellationToken token)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            return Task.Run<IEnumerable<BandTile>>(async () =>
            {
                Guid applicationId = await applicationPlatformProvider.GetApplicationIdAsync(token).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
                BandTile[] array = GetInstalledTiles().Where(tile => tile.OwnerId == applicationId).Select(tile => tile.ToBandTile()).ToArray();
                foreach (BandTile bandTile in array)
                {
                    for (uint layoutIndex = 0; layoutIndex < 5U; ++layoutIndex)
                    {
                        token.ThrowIfCancellationRequested();
                        PageLayout layout = DynamicPageLayoutGetLayout(bandTile.TileId, layoutIndex);
                        if (layout != null)
                            bandTile.PageLayouts.Add(layout);
                        else
                            break;
                    }
                }
                return array;
            }, token);
        }

        public Task<bool> AddTileAsync(BandTile tile) => AddTileAsync(tile, CancellationToken.None);

        public async Task<bool> AddTileAsync(BandTile tile, CancellationToken token)
        {
            if (tile == null)
                throw new ArgumentNullException(nameof(tile));
            if (string.IsNullOrWhiteSpace(tile.Name))
                throw new ArgumentException(BandResources.BandTileEmptyName, nameof(tile));
            if (tile.SmallIcon == null)
                throw new ArgumentException(BandResources.BandTileNoSmallIcon, nameof(tile));
            if (tile.TileIcon == null)
                throw new ArgumentException(BandResources.BandTileNoTileIcon, nameof(tile));
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            if (tile.AdditionalIcons.Count + 2 > BandTypeConstants.MaxIconsPerTile)
                throw new ArgumentException(BandResources.BandTileTooManyIcons, nameof(tile));
            if (tile.PageLayouts.Count > 5)
                throw new ArgumentException(BandResources.BandTileTooManyTemplates, nameof(tile));
            foreach (PageLayout pageLayout in tile.PageLayouts)
            {
                if (pageLayout == null)
                    throw new InvalidOperationException(BandResources.BandTileNullTemplateEncountered);
                if (pageLayout.GetSerializedByteCountAndValidate() > 768)
                    throw new ArgumentException(BandResources.BandTilePageTemplateBlobTooBig);
                if (BandTypeConstants.BandType == BandType.Cargo && pageLayout.Elements.Any(element => element is IconButton))
                    throw new ArgumentException(BandResources.IconButtonsAreNotSupportedOnCargo);
            }
            token.ThrowIfCancellationRequested();
            IList<TileData> installedTiles = null;
            if (!await Task.Run(() =>
            {
                if (KnownTiles.AllTileGuids.Contains(tile.TileId))
                    return false;
                token.ThrowIfCancellationRequested();
                installedTiles = GetInstalledTilesNoIcons();
                if (installedTiles.Any(installedTile => installedTile.AppID == tile.TileId))
                    throw new InvalidOperationException(BandResources.BandTileIdAlreadyInstalled);
                token.ThrowIfCancellationRequested();
                if ((int)GetTileCapacity() == installedTiles.Count)
                    throw new InvalidOperationException(BandResources.BandAtMaxTileCapacity);
                token.ThrowIfCancellationRequested();
                return !GetDefaultTilesNoIconsInternal().Any(defaultTile => defaultTile.AppID == tile.TileId);
            }))
                return false;
            if (!await applicationPlatformProvider.GetAddTileConsentAsync(tile, token))
                return false;
            token.ThrowIfCancellationRequested();
            await Task.Run(async () =>
            {
                Guid applicationIdAsync = await applicationPlatformProvider.GetApplicationIdAsync(token);
                token.ThrowIfCancellationRequested();
                AddTile(tile, applicationIdAsync, installedTiles);
            }, token);
            return true;
        }

        public Task<bool> SetPagesAsync(Guid tileId, params PageData[] pages) => SetPagesAsync(tileId, CancellationToken.None, (IEnumerable<PageData>)pages);

        public Task<bool> SetPagesAsync(Guid tileId, IEnumerable<PageData> pages) => SetPagesAsync(tileId, CancellationToken.None, pages);

        public Task<bool> SetPagesAsync(Guid tileId, CancellationToken token, params PageData[] pages)
        {
            return SetPagesAsync(tileId, token, (IEnumerable<PageData>)pages);
        }

        public Task<bool> SetPagesAsync(Guid tileId, CancellationToken token, IEnumerable<PageData> pages)
        {
            if (tileId == Guid.Empty)
                throw new ArgumentException(string.Format(BandResources.SetPagesEmptyGuid, tileId));
            PageData[] pageList = pages != null ? pages.ToArray() : throw new ArgumentNullException(nameof(pages));
            if (pageList.Length == 0)
                throw new ArgumentException(string.Format(BandResources.GenericCountZero, new[] { nameof(pages) }));
            foreach (PageData pageData in pageList)
            {
                if (pageData == null)
                    throw new ArgumentException(BandResources.GenericNullCollectionElement, nameof(pages));
            }
            if (KnownTiles.AllTileGuids.Contains(tileId))
                return Task.FromResult(false);
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            return Task.Run(() => SetPagesWithOwnerValidation(tileId, token, pageList));
        }

        internal bool SetPagesWithOwnerValidation(Guid tileId, CancellationToken token, PageData[] pages)
        {
            if (!TileInstalledAndOwned(tileId, token))
                return false;
            SetPages(tileId, token, pages);
            return true;
        }

        internal void SetPages(Guid tileId, CancellationToken token, IEnumerable<PageData> pages)
        {
            foreach (PageData page in pages)
            {
                token.ThrowIfCancellationRequested();
                int lengthAndValidate = page.GetSerializedLengthAndValidate(BandTypeConstants);
                int dataSize = 40 + lengthAndValidate;
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoNotification, dataSize, CommandStatusHandling.DoNotCheck);
                cargoCommandWriter.WriteUInt16(101);
                cargoCommandWriter.WriteGuid(tileId);
                cargoCommandWriter.WriteUInt16((ushort)lengthAndValidate);
                cargoCommandWriter.WriteUInt16((ushort)page.PageLayoutIndex);
                cargoCommandWriter.WriteGuid(page.PageId);
                cargoCommandWriter.WriteByte(0);
                cargoCommandWriter.WriteByte(0);
                page.SerializeToBand(cargoCommandWriter);
                CheckStatus(cargoCommandWriter.CommandStatus, CommandStatusHandling.ThrowAnyNonZero, loggerProvider);
            }
        }

        public Task<bool> RemovePagesAsync(Guid tileId) => RemovePagesAsync(tileId, CancellationToken.None);

        public Task<bool> RemovePagesAsync(Guid tileId, CancellationToken token)
        {
            if (tileId == Guid.Empty)
                throw new ArgumentException(string.Format(BandResources.RemovePagesEmptyGuid, tileId));
            if (KnownTiles.AllTileGuids.Contains(tileId))
                return Task.FromResult(false);
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            return Task.Run(() => RemovePagesWithOwnerValidation(tileId, token));
        }

        private bool RemovePagesWithOwnerValidation(Guid tileId, CancellationToken token)
        {
            if (!TileInstalledAndOwned(tileId, token))
                return false;
            RemovePages(tileId, token);
            return true;
        }

        protected void RemovePages(Guid tileId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            SendNotification(NotificationID.GenericClearTile, NotificationPBMessageType.TileManagement, new NotificationGenericClearTile(tileId));
        }

        public Task<bool> RemoveTileAsync(BandTile tile) => RemoveTileAsync(tile, CancellationToken.None);

        public Task<bool> RemoveTileAsync(BandTile tile, CancellationToken token)
        {
            if (tile == null)
                throw new ArgumentNullException(nameof(tile));
            return RemoveTileAsync(tile.TileId, token);
        }

        public Task<bool> RemoveTileAsync(Guid tileId) => RemoveTileAsync(tileId, CancellationToken.None);

        public Task<bool> RemoveTileAsync(Guid tileId, CancellationToken token)
        {
            if (tileId == Guid.Empty)
                throw new ArgumentException(string.Format(BandResources.RemoveTileEmptyTileId, tileId));
            if (KnownTiles.AllTileGuids.Contains(tileId))
                return Task.FromResult(false);
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            Func<TileData, bool> func = null;
            return Task.Run(async () =>
            {
                Guid applicationId;
                applicationId = await applicationPlatformProvider.GetApplicationIdAsync(token).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
                IList<TileData> installedTilesNoIcons = GetInstalledTilesNoIcons();
                if (installedTilesNoIcons.FirstOrDefault(tile => tile.OwnerId == applicationId && tile.AppID == tileId) == null)
                    return false;
                token.ThrowIfCancellationRequested();
                RemoveTile(tileId, installedTilesNoIcons);
                token.ThrowIfCancellationRequested();
                if (GetInstalledTilesNoIcons().Any(func ??= (tile) => tile.AppID == tileId))
                    throw new BandException(string.Format(CultureInfo.CurrentCulture, BandResources.RemoveTileFailed, tileId));
                return true;
            }, token);
        }

        public Task<int> GetRemainingTileCapacityAsync() => GetRemainingTileCapacityAsync(CancellationToken.None);

        public Task<int> GetRemainingTileCapacityAsync(CancellationToken token)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            return Task.Run(() => GetRemainingTileCapacity(token));
        }

        private int GetRemainingTileCapacity(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            int tileCapacity = (int)GetTileCapacity();
            token.ThrowIfCancellationRequested();
            int count = GetInstalledTilesNoIcons().Count;
            return tileCapacity - count;
        }

        protected uint GetTileCapacity() => cachedThings.GetTileCapacity();

        protected uint GetTileMaxAllocatedCapacity() => cachedThings.GetTileMaxAllocatedCapacity();

        internal IList<TileData> GetInstalledTiles()
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Obtaining the AppList from the KDevice");
            return GetTilesHelper(DeviceCommands.CargoInstalledAppListGet, true).OrderBy(t => t.StartStripOrder).ToList();
        }

        protected IList<TileData> GetInstalledTilesNoIcons()
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Obtaining the AppList from the KDevice without images");
            return GetTilesHelper(DeviceCommands.CargoInstalledAppListGetNoImages, false).OrderBy(t => t.StartStripOrder).ToList();
        }

        internal IList<TileData> GetDefaultTilesInternal()
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Obtaining the default AppList from the KDevice");
            return GetTilesHelper(DeviceCommands.CargoInstalledAppListGetDefaults, true);
        }

        protected IList<TileData> GetDefaultTilesNoIconsInternal()
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Obtaining the default AppList from the KDevice without images");
            return GetTilesHelper(DeviceCommands.CargoInstalledAppListGetDefaultsNoImages, false);
        }

        private TileData GetInstalledTileNoIcons(Guid applicationId, Guid tileId) => GetInstalledTilesNoIcons().FirstOrDefault(tile => tile.OwnerId == applicationId && tile.AppID == tileId);

        private IList<TileData> GetTilesHelper(ushort commandId, bool withIcons)
        {
            DisposableList<PooledBuffer> disposableList = null;
            if (withIcons)
                disposableList = new DisposableList<PooledBuffer>();
            using (disposableList)
            {
                try
                {
                    int allocatedCapacity = (int)GetTileMaxAllocatedCapacity();
                    int num1 = 0;
                    List<TileData> tileDataList = new(allocatedCapacity);
                    if (withIcons)
                        num1 += 1024 * allocatedCapacity;
                    int bytesToRead = num1 + (4 + TileData.GetSerializedByteCount() * allocatedCapacity);
                    using (CargoCommandReader cargoCommandReader = ProtocolBeginRead(commandId, bytesToRead, CommandStatusHandling.DoNotCheck))
                    {
                        if (withIcons)
                        {
                            for (int index = 0; index < allocatedCapacity; ++index)
                            {
                                PooledBuffer buffer = BufferServer.GetBuffer(1024);
                                disposableList.Add(buffer);
                                cargoCommandReader.ReadExact(buffer.Buffer, 0, buffer.Length);
                            }
                        }
                        uint num2 = cargoCommandReader.ReadUInt32();
                        for (int index = 0; index < num2; ++index)
                            tileDataList.Add(TileData.DeserializeFromBand(cargoCommandReader));
                        if (cargoCommandReader.BytesRemaining > 0)
                            cargoCommandReader.ReadToEndAndDiscard();
                        CheckStatus(cargoCommandReader.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
                    }
                    if (withIcons)
                    {
                        for (int index = 0; index < tileDataList.Count; ++index)
                            tileDataList[index].Icon = BandIconRleCodec.DecodeTileIconRle(disposableList[index]);
                    }
                    return tileDataList;
                }
                catch (BandIOException ex)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new BandIOException(ex.Message, ex);
                }
            }
        }

        private void AddTile(BandTile tile, Guid applicationId, IEnumerable<TileData> installedTiles)
        {
            loggerProvider.Log(ProviderLogLevel.Verbose, "Adding new tile");
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            installedTiles = installedTiles.OrderBy(t => t.StartStripOrder);
            if (tile.Theme != null)
                ValidateTileTheme(tile.Theme, tile.TileId);
            RunUsingSynchronizedFirmwareUI(() =>
            {
                AddTileInsideSync(tile, applicationId, installedTiles);
                if (tile.Theme == null)
                    return;
                SetTileThemeInternal(tile.Theme, tile.TileId);
            }, () => AddTileOutsideSync(tile));
        }

        private void AddTileInsideSync(BandTile tile, Guid applicationId, IEnumerable<TileData> orderedInstalledTiles)
        {
            IEnumerable<BandIcon> icons = EnumerableExtensions.Concat(tile.TileIcon, tile.SmallIcon).Concat(tile.AdditionalIcons);
            RegisterTileIcons(tile.TileId, tile.Name, icons, false);
            SetTileIconIndexes(tile.TileId, 0U, 1U, 1U);
            int startStripOrder = orderedInstalledTiles.Count();
            SetStartStripData(orderedInstalledTiles.Concat(tile.ToTileData(startStripOrder, applicationId)), startStripOrder + 1);
        }

        private void AddTileOutsideSync(BandTile tile)
        {
            for (int index = 0; index < tile.PageLayouts.Count; ++index)
                DynamicPageLayoutSetLayout(tile.TileId, (uint)index, tile.PageLayouts[index]);
            for (uint count = (uint)tile.PageLayouts.Count; count < 5U; ++count)
                DynamicPageLayoutRemoveLayout(tile.TileId, count);
        }

        protected void RegisterTileIcons(
          Guid tileId,
          string friendlyName,
          IEnumerable<BandIcon> icons,
          bool iconsAlreadyRegistered)
        {
            using DisposableList<PooledBuffer> disposableList = new();
            int dataSize = 20;
            foreach (BandIcon icon in icons)
            {
                PooledBuffer pooledBuffer = BandIconRleCodec.EncodeTileIconRle(icon);
                disposableList.Add(pooledBuffer);
                dataSize += pooledBuffer.Length;
            }
            ushort commandId;
            if (iconsAlreadyRegistered)
            {
                commandId = DeviceCommands.CargoDynamicAppRegisterAppIcons;
                loggerProvider.Log(ProviderLogLevel.Verbose, $"Invoking DynamicAppUpdateStrappIcons for strapp: {friendlyName}");
            }
            else
            {
                commandId = DeviceCommands.CargoDynamicAppRegisterApp;
                loggerProvider.Log(ProviderLogLevel.Verbose, $"Invoking DynamicAppRegisterStrapp for strapp: {friendlyName}");
            }
            try
            {
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(commandId, dataSize, CommandStatusHandling.ThrowOnlySeverityError);
                cargoCommandWriter.WriteGuid(tileId);
                cargoCommandWriter.WriteInt32(disposableList.Count);
                foreach (PooledBuffer pooledBuffer in disposableList)
                    cargoCommandWriter.Write(pooledBuffer.Buffer, 0, pooledBuffer.Length);
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        private void RemoveTile(Guid guid, IEnumerable<TileData> installedTiles)
        {
            loggerProvider.Log(ProviderLogLevel.Verbose, "Removing tile");
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            installedTiles = installedTiles.OrderBy(t => t.StartStripOrder);
            RunUsingSynchronizedFirmwareUI(() => RemoveTileInsideSync(guid, installedTiles));
        }

        private void RemoveTileInsideSync(Guid guid, IEnumerable<TileData> orderedInstalledTiles)
        {
            UnregisterTileIcons(guid);
            TileData[] array = orderedInstalledTiles.Where(tile => tile.AppID != guid).ToArray();
            SetStartStripData(array, array.Length);
        }

        protected void UnregisterTileIcons(Guid guid)
        {
            loggerProvider.Log(ProviderLogLevel.Verbose, $"Invoking DynamicAppRemoveStrapp for strapp: {guid}");
            void writeData(ICargoWriter w) => w.WriteGuid(guid);
            ProtocolWriteWithData(DeviceCommands.CargoDynamicAppRemoveApp, 16, writeData, 60000);
        }

        protected void RunUsingSynchronizedFirmwareUI(Action insideSync, Action afterSync = null)
        {
            if (insideSync == null)
                throw new ArgumentNullException(nameof(insideSync));
            bool flag = false;
            try
            {
                FirmwareUiSyncStart();
                insideSync();
                flag = true;
            }
            finally
            {
                try
                {
                    FirmwareUiSyncEnd();
                }
                catch
                {
                    if (flag)
                        throw;
                }
            }
            if (afterSync == null)
                return;
            afterSync();
        }

        private void FirmwareUiSyncStart()
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Starting the startStrip sync");
            ProtocolWrite(DeviceCommands.CargoInstalledAppListStartStripSyncStart, 60000);
        }

        private void FirmwareUiSyncEnd()
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Ending the startStrip sync");
            ProtocolWrite(DeviceCommands.CargoInstalledAppListStartStripSyncEnd, 60000);
        }

        protected void SetTileIconIndexes(
          Guid tileId,
          uint tileIconIndex,
          uint badgeIconIndex,
          uint notificationIconIndex)
        {
            SetMainIconIndex(tileId, tileIconIndex);
            SetBadgeIconIndex(tileId, badgeIconIndex);
            SetNotificationIconIndex(tileId, notificationIconIndex);
        }

        protected void SetMainIconIndex(Guid tileId, uint iconIndex)
        {
            loggerProvider.Log(ProviderLogLevel.Verbose, $"Invoking DynamicAppSetTileIconIndex for tile: {tileId}");
            SetTileIconIndex(tileId, DeviceCommands.CargoDynamicAppSetAppTileIndex, iconIndex);
        }

        protected void SetBadgeIconIndex(Guid tileId, uint iconIndex)
        {
            loggerProvider.Log(ProviderLogLevel.Verbose, $"Invoking DynamicAppSetBadgeIconIndex for tile: {tileId}");
            SetTileIconIndex(tileId, DeviceCommands.CargoDynamicAppSetAppBadgeTileIndex, iconIndex);
        }

        protected void SetNotificationIconIndex(Guid tileId, uint iconIndex)
        {
            if (BandTypeConstants.BandType == BandType.Envoy)
            {
                loggerProvider.Log(ProviderLogLevel.Verbose, $"Invoking DynamicAppSetNotificationIconIndex for tile: {tileId}");
                SetTileIconIndex(tileId, DeviceCommands.CargoDynamicAppSetAppNotificationTileIndex, iconIndex);
            }
            else
                loggerProvider.Log(ProviderLogLevel.Verbose, $"Silently ignoring SetNotificationIconIndex() for Cargo device, tile: {tileId}");
        }

        private void SetTileIconIndex(Guid guid, ushort iconIndexCommandId, uint iconIndex)
        {
            int dataSize = 20;
            try
            {
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(iconIndexCommandId, dataSize, CommandStatusHandling.ThrowOnlySeverityError);
                cargoCommandWriter.WriteGuid(guid);
                cargoCommandWriter.WriteUInt32(iconIndex);
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        protected void SetStartStripData(IEnumerable<TileData> orderedList, int count)
        {
            void writeArgBuf(ICargoWriter w) => w.WriteInt32(count);
            int num = 0;
            loggerProvider.Log(ProviderLogLevel.Info, "Setting the installed AppList");
            int dataSize = 4 + count * TileData.GetSerializedByteCount();
            try
            {
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoInstalledAppListSet, 4, dataSize, writeArgBuf, CommandStatusHandling.DoNotCheck);
                deviceTransport.CargoStream.ReadTimeout = 60000;
                cargoCommandWriter.WriteUInt32((uint)count);
                foreach (TileData tileData in orderedList.Take(count))
                    tileData.SerializeToBand(cargoCommandWriter, new uint?((uint)num++));
                cargoCommandWriter.Flush();
                CheckStatus(cargoCommandWriter.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        protected void SetTileThemeInternal(BandTheme theme, Guid id, CancellationToken cancel)
        {
            CheckIfDisposed();
            CheckIfDisconnectedOrUpdateMode();
            ValidateTileTheme(theme, id);
            cancel.ThrowIfCancellationRequested();
            RunUsingSynchronizedFirmwareUI(() => SetTileThemeInternal(theme, id));
        }

        protected void SetTileThemeInternal(BandTheme theme, Guid id)
        {
            int dataSize = 40;
            try
            {
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoThemeColorSetCustomTheme, dataSize, CommandStatusHandling.ThrowOnlySeverityError);
                cargoCommandWriter.WriteUInt32(theme.Base.ToRgb());
                cargoCommandWriter.WriteUInt32(theme.Highlight.ToRgb());
                cargoCommandWriter.WriteUInt32(theme.Lowlight.ToRgb());
                cargoCommandWriter.WriteUInt32(theme.SecondaryText.ToRgb());
                cargoCommandWriter.WriteUInt32(theme.HighContrast.ToRgb());
                cargoCommandWriter.WriteUInt32(theme.Muted.ToRgb());
                cargoCommandWriter.WriteGuid(id);
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        protected void ValidateTileTheme(BandTheme theme, Guid id)
        {
        }

        protected void DynamicPageLayoutRemoveLayout(Guid appId, uint layoutIndex)
        {
            int dataSize = 20;
            using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoDynamicPageLayoutRemove, dataSize, CommandStatusHandling.ThrowOnlySeverityError);
            cargoCommandWriter.WriteGuid(appId);
            cargoCommandWriter.WriteUInt32(layoutIndex);
        }

        protected void DynamicPageLayoutSetSerializedLayout(Guid appId, uint layoutIndex, byte[] layoutBlob)
        {
            if (layoutBlob.Length > 768)
                throw new ArgumentException(BandResources.BandTilePageTemplateBlobTooBig);
            int dataSize = 24 + layoutBlob.Length;
            try
            {
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoDynamicPageLayoutSet, dataSize, CommandStatusHandling.DoNotCheck);
                ICargoStream cargoStream = deviceTransport.CargoStream;
                int readTimeout = cargoStream.ReadTimeout;
                int writeTimeout = cargoStream.WriteTimeout;
                try
                {
                    cargoStream.ReadTimeout *= 2;
                    cargoStream.WriteTimeout *= 2;
                    cargoCommandWriter.WriteGuid(appId);
                    cargoCommandWriter.WriteUInt32(layoutIndex);
                    cargoCommandWriter.WriteInt32(layoutBlob.Length);
                    cargoCommandWriter.Write(layoutBlob);
                    CheckStatus(cargoCommandWriter.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
                }
                finally
                {
                    cargoStream.ReadTimeout = readTimeout;
                    cargoStream.WriteTimeout = writeTimeout;
                }
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        protected void DynamicPageLayoutSetLayout(Guid appId, uint layoutIndex, PageLayout layout)
        {
            int countAndValidate = layout.GetSerializedByteCountAndValidate();
            if (countAndValidate > 768)
                throw new ArgumentException(BandResources.BandTilePageTemplateBlobTooBig);
            int dataSize = 24 + countAndValidate;
            try
            {
                using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoDynamicPageLayoutSet, dataSize, CommandStatusHandling.DoNotCheck);
                cargoCommandWriter.WriteGuid(appId);
                cargoCommandWriter.WriteUInt32(layoutIndex);
                cargoCommandWriter.WriteInt32(countAndValidate);
                layout.SerializeToBand(cargoCommandWriter);
                CheckStatus(cargoCommandWriter.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
            }
            catch (BandIOException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BandIOException(ex.Message, ex);
            }
        }

        protected byte[] DynamicPageLayoutGetSerializedLayout(Guid appId, uint layoutIndex)
        {
            int serializedByteCount = GetPageLayoutArgs.GetSerializedByteCount();
            void writeArgBuf(ICargoWriter w) => GetPageLayoutArgs.SerializeToBand(w, appId, layoutIndex);
            using CargoCommandReader cargoCommandReader = ProtocolBeginRead(DeviceCommands.CargoDynamicPageLayoutGet, serializedByteCount, 768, writeArgBuf, CommandStatusHandling.DoNotCheck);
            byte[] numArray = cargoCommandReader.ReadExact(768);
            CheckStatus(cargoCommandReader.CommandStatus, CommandStatusHandling.ThrowOnlySeverityError, loggerProvider);
            return numArray;
        }

        protected PageLayout DynamicPageLayoutGetLayout(Guid appId, uint layoutIndex)
        {
            int serializedByteCount = GetPageLayoutArgs.GetSerializedByteCount();
            void writeArgBuf(ICargoWriter w) => GetPageLayoutArgs.SerializeToBand(w, appId, layoutIndex);
            PageLayout pageLayout = null;
            using (CargoCommandReader cargoCommandReader = ProtocolBeginRead(DeviceCommands.CargoDynamicPageLayoutGet, serializedByteCount, 768, writeArgBuf, CommandStatusHandling.DoNotCheck))
            {
                try
                {
                    pageLayout = PageLayout.DeserializeFromBand(cargoCommandReader);
                }
                finally
                {
                    cargoCommandReader.ReadToEndAndDiscard();
                }
                CheckStatus(cargoCommandReader.CommandStatus, CommandStatusHandling.ThrowAnyNonZero, loggerProvider);
            }
            return pageLayout;
        }

        public bool TileInstalledAndOwned(Guid tileId, CancellationToken token)
        {
            Guid result = applicationPlatformProvider.GetApplicationIdAsync(token).Result;
            TileData installedTileNoIcons = GetInstalledTileNoIcons(result, tileId);
            return installedTileNoIcons != null && !(installedTileNoIcons.OwnerId != result);
        }

        protected class CachedThings
        {
            private BandClient bandClient;
            private uint apiVersion;
            private uint tileCapacity;
            private uint tileMaxAllocatedCapacity;

            public CachedThings(BandClient bandClient) => this.bandClient = bandClient;

            public uint GetApiVersion()
            {
                if (apiVersion == 0U)
                {
                    CheckCanUseProtocol();
                    apiVersion = ReadApiVersion();
                }
                return apiVersion;
            }

            public uint GetTileCapacity()
            {
                if (tileCapacity == 0U)
                {
                    CheckCanUseProtocol();
                    tileCapacity = ReadTileCapacity();
                }
                return tileCapacity;
            }

            public uint GetTileMaxAllocatedCapacity()
            {
                if (tileMaxAllocatedCapacity == 0U)
                {
                    if (GetApiVersion() >= 32U)
                    {
                        CheckCanUseProtocol();
                        tileMaxAllocatedCapacity = ReadTileMaxAllocatedCapacity();
                    }
                    else
                        tileMaxAllocatedCapacity = 15U;
                }
                return tileMaxAllocatedCapacity;
            }

            public void Clear()
            {
                apiVersion = 0U;
                tileCapacity = 0U;
                tileMaxAllocatedCapacity = 0U;
            }

            private uint ReadApiVersion()
            {
                uint version = 0;
                void readData(ICargoReader r) => version = r.ReadUInt32();
                bandClient.ProtocolRead(DeviceCommands.CargoCoreModuleGetApiVersion, 4, readData);
                return version;
            }

            private uint ReadTileCapacity()
            {
                uint capacity = 0;
                void readData(ICargoReader r) => capacity = r.ReadUInt32();
                bandClient.ProtocolRead(DeviceCommands.CargoInstalledAppListGetMaxTileCount, 4, readData);
                return capacity;
            }

            private uint ReadTileMaxAllocatedCapacity()
            {
                uint capacity = 0;
                void readData(ICargoReader r) => capacity = r.ReadUInt32();
                bandClient.ProtocolRead(DeviceCommands.CargoInstalledAppListGetMaxTileAllocatedCount, 4, readData);
                return capacity;
            }

            private void CheckCanUseProtocol()
            {
                bandClient.CheckIfDisposed();
                bandClient.CheckIfDisconnectedOrUpdateMode();
            }
        }

        private static class KnownTiles
        {
            public const string Workouts = "2af008a7-cd03-a04d-bb33-be904e6a2924";
            public const string Run = "65bd93db-4293-46af-9a28-bdd6513b4677";
            public const string Bike = "96430fcb-0060-41cb-9de2-e00cac97f85d";
            public const string Sleep = "23e7bc94-f90d-44e0-843f-250910fdf74e";
            public const string Exercise = "a708f02a-03cd-4da0-bb33-be904e6a2924";
            public const string AlarmStopwatch = "d36a92ea-3e85-4aed-a726-2898a6f2769b";
            public const string UV = "59976cf5-15c8-4799-9e31-f34c765a6bd1";
            public const string Weather = "69a39b4e-084b-4b53-9a1b-581826df9e36";
            public const string Finance = "5992928a-bd79-4bb5-9678-f08246d03e68";
            public const string Starbucks = "64a29f65-70bb-4f32-99a2-0f250a05d427";
            public const string GuidedWorkouts = "0281c878-afa8-40ff-acfd-bca06c5c4922";
            public const string Email = "823ba55a-7c98-4261-ad5e-929031289c6e";
            public const string Facebook = "fd06b486-bbda-4da5-9014-124936386237";
            public const string Twitter = "2e76a806-f509-4110-9c03-43dd2359d2ad";
            public const string Cortana = "d7fb5ff5-906a-4f2c-8269-dde6a75138c4";
            public const string Lync = "c06dc40e-61d2-485c-99de-20bf991a504d";
            public const string FBMessenger = "76b08699-2f2e-9041-96c2-1f4bfc7eab10";
            public const string Feed = "4076b009-0455-4af7-a705-6d4acd45a556";
            public const string Whatsapp = "73942f52-23dc-464a-a7e1-b3a6ba95321f";
            public const string SMS = "b4edbc35-027b-4d10-a797-1099cd2ad98a";
            public const string Calls = "22B1C099-F2BE-4BAC-8ED8-2D6B0B3C25D1";
            public const string Calendar = "ec149021-ce45-40e9-aeee-08f86e4746a7";
            public static readonly string[] AllTileIds = new[]
            {
                "2af008a7-cd03-a04d-bb33-be904e6a2924",
                "65bd93db-4293-46af-9a28-bdd6513b4677",
                "96430fcb-0060-41cb-9de2-e00cac97f85d",
                "23e7bc94-f90d-44e0-843f-250910fdf74e",
                "a708f02a-03cd-4da0-bb33-be904e6a2924",
                "d36a92ea-3e85-4aed-a726-2898a6f2769b",
                "59976cf5-15c8-4799-9e31-f34c765a6bd1",
                "69a39b4e-084b-4b53-9a1b-581826df9e36",
                "5992928a-bd79-4bb5-9678-f08246d03e68",
                "64a29f65-70bb-4f32-99a2-0f250a05d427",
                "0281c878-afa8-40ff-acfd-bca06c5c4922",
                "823ba55a-7c98-4261-ad5e-929031289c6e",
                "fd06b486-bbda-4da5-9014-124936386237",
                "2e76a806-f509-4110-9c03-43dd2359d2ad",
                "d7fb5ff5-906a-4f2c-8269-dde6a75138c4",
                "c06dc40e-61d2-485c-99de-20bf991a504d",
                "76b08699-2f2e-9041-96c2-1f4bfc7eab10",
                "4076b009-0455-4af7-a705-6d4acd45a556",
                "73942f52-23dc-464a-a7e1-b3a6ba95321f",
                "b4edbc35-027b-4d10-a797-1099cd2ad98a",
                "22B1C099-F2BE-4BAC-8ED8-2D6B0B3C25D1",
                "ec149021-ce45-40e9-aeee-08f86e4746a7"
            };
            public static readonly HashSet<Guid> AllTileGuids = new(((IEnumerable<string>)AllTileIds).Select(id => Guid.Parse(id)));
        }
    }
}
