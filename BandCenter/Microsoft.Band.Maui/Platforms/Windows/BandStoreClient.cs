// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.BandStoreClient
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: .\netcore451\Microsoft.Band.Store.dll

using Microsoft.Band.Sensors;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Microsoft.UI.Xaml;

namespace Microsoft.Band.Windows
{
    internal class BandStoreClient : BandClient
    {
        public EventHandler Disconnected;
        private IReadableTransport pushServiceTransport;
        private readonly BluetoothDeviceInfo bluetoothBand;
        private static readonly TimeSpan PushServiceReconnectDelay = TimeSpan.FromSeconds(5.0);
        private static readonly Guid PushServiceGuid = Guid.Parse("{d8895bfd-0461-400d-bd52-dbe2a3c33021}");

        protected override void OnDisconnected(TransportDisconnectedEventArgs args)
        {
            if (args.Reason != TransportDisconnectedReason.TransportIssue)
                return;
            EventHandler disconnected = Disconnected;
            if (disconnected == null)
                return;
            disconnected(this, args);
        }

        protected BluetoothDeviceInfo BluetoothBand => bluetoothBand;

        internal BandStoreClient(
          BluetoothDeviceInfo deviceInfo,
          IDeviceTransport deviceTransport,
          IReadableTransport pushTransport,
          ILoggerProvider loggerProvider,
          IApplicationPlatformProvider applicationPlatformProvider)
          : base(deviceTransport, loggerProvider, applicationPlatformProvider)
        {
            bluetoothBand = deviceInfo;
            pushServiceTransport = pushTransport;
            ApplicationSuspensionManager.Instance.Suspending += new EventHandler(AppSuspending);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void AppSuspending(object sender, EventArgs e)
        {
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    loggerProvider.LogException(ProviderLogLevel.Error, ex);
                }
                catch
                {
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void Dispose(bool disposing)
        {
            bool flag = false;
            if (disposing && !Disposed)
            {
                flag = true;
                try
                {
                    ApplicationSuspensionManager.Instance.Suspending -= new EventHandler(AppSuspending);
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
            if (!flag || pushServiceTransport == null)
                return;
            pushServiceTransport.Dispose();
            pushServiceTransport = null;
        }

        protected override void ExecuteSensorSubscribeCommand(SubscriptionType type)
        {
            using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoRemoteSubscriptionSubscribeId, 21, CommandStatusHandling.ThrowOnlySeverityError);
            cargoCommandWriter.WriteByte((byte)type);
            cargoCommandWriter.WriteBool32(false);
            cargoCommandWriter.WriteGuid(PushServiceGuid);
        }

        protected override void ExecuteSensorUnsubscribeCommand(SubscriptionType type)
        {
            int dataSize = 17;
            using CargoCommandWriter cargoCommandWriter = ProtocolBeginWrite(DeviceCommands.CargoRemoteSubscriptionUnsubscribeId, dataSize, CommandStatusHandling.ThrowOnlySeverityError);
            cargoCommandWriter.WriteByte((byte)type);
            cargoCommandWriter.WriteGuid(PushServiceGuid);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void StreamBandData(ManualResetEvent started, CancellationToken stop)
        {
            loggerProvider.Log(ProviderLogLevel.Info, "Push service streaming task starting...");
            WaitHandle[] waitHandles = null;
            bool flag = true;
            int num1 = 0;
            while (!stop.IsCancellationRequested)
            {
                if (!flag)
                {
                    try
                    {
                        pushServiceTransport.Disconnect();
                    }
                    catch
                    {
                    }
                    if (waitHandles == null)
                        waitHandles = new[]
                        {
                            StreamingTaskAwake,
                            stop.WaitHandle
                        };
                    StreamingTaskAwake.Reset();
                    int num2;
                    if (num1 == 0)
                    {
                        ++num1;
                        num2 = WaitHandle.WaitAny(waitHandles, PushServiceReconnectDelay);
                    }
                    else
                        num2 = WaitHandle.WaitAny(waitHandles);
                    if (num2 == 1)
                        break;
                }
                try
                {
                    pushServiceTransport.Connect();
                    pushServiceTransport.CargoStream.Cancel = stop;
                    if (!stop.IsCancellationRequested)
                    {
                        if (!flag)
                        {
                            lock (SubscribedSensorTypes)
                            {
                                if (!stop.IsCancellationRequested)
                                {
                                    foreach (SubscriptionType subscribedSensorType in SubscribedSensorTypes)
                                    {
                                        ExecuteSensorSubscribeCommand(subscribedSensorType);
                                        if (stop.IsCancellationRequested)
                                            break;
                                    }
                                }
                                else
                                    break;
                            }
                        }
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    loggerProvider.LogException(ProviderLogLevel.Error, ex);
                    continue;
                }
                finally
                {
                    if (flag)
                    {
                        flag = false;
                        started.Set();
                    }
                }
                num1 = 0;
                try
                {
                    while (!stop.IsCancellationRequested)
                        ProcessPushServicePacket(pushServiceTransport.CargoReader);
                }
                catch (OperationCanceledException ex)
                {
                    break;
                }
                catch (Exception ex)
                {
                    loggerProvider.LogException(ProviderLogLevel.Error, ex);
                }
            }
            try
            {
                pushServiceTransport.Disconnect();
            }
            catch
            {
            }
        }

        private void ProcessPushServicePacket(CargoStreamReader reader)
        {
            int num = 0;
            PushServicePacketHeader servicePacketHeader = PushServicePacketHeader.DeserializeFromBand(reader);
            switch (servicePacketHeader.Type)
            {
                case PushServicePacketType.RemoteSubscription:
                    while (num < servicePacketHeader.Length)
                        num += ProcessSensorSubscriptionPayload(reader);
                    break;
                case PushServicePacketType.StrappEvent:
                    while (num < servicePacketHeader.Length)
                        num += ProcessTileEventPayload(reader);
                    break;
                default:
                    loggerProvider.Log(ProviderLogLevel.Warning, $"Unsupported push service type {servicePacketHeader.Type} received.");
                    pushServiceTransport.CargoReader.ReadExactAndDiscard(servicePacketHeader.Length);
                    break;
            }
        }

        private sealed class ApplicationSuspensionManager
        {
            public static readonly ApplicationSuspensionManager Instance = new();

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
            public ApplicationSuspensionManager()
            {
                try
                {
                    // FIXME: MAUI WinUI 3 apps don't use app lifecycle

                    //Application current = Application.Current;
                    //// ISSUE: method pointer
                    //WindowsRuntimeMarshal.AddEventHandler<SuspendingEventHandler>(new Func<SuspendingEventHandler, EventRegistrationToken>(current.add_Suspending), new Action<EventRegistrationToken>(current.remove_Suspending), new SuspendingEventHandler(this, __methodptr(OnCurrentApplicationSuspending)));
                }
                catch
                {
                }
            }

            public event EventHandler Suspending;

            private void OnCurrentApplicationSuspending(object sender, SuspendingEventArgs e) => NotifySuspending();

            private void NotifySuspending()
            {
                EventHandler suspending = Suspending;
                if (suspending == null)
                    return;
                suspending(this, EventArgs.Empty);
            }
        }
    }
}
