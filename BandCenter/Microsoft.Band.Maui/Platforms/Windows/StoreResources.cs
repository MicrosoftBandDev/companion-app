// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Store.StoreResources
// Assembly: Microsoft.Band.Store, Version=1.3.20628.2, Culture=neutral, PublicKeyToken=608d7da3159f502b
// MVID: 91750BE8-70C6-4542-841C-664EE611AF0B
// Assembly location: C:\Users\jjask\AppData\Local\Temp\Xiwoxyt\b1d4237fe8\lib\netcore451\Microsoft.Band.Store.dll

namespace Microsoft.Band.Windows
{
    internal static class StoreResources
    {
        public static string DeviceInfoNotBluetooth => "BandInfo must be for a Bluetooth connection.";

        public static string DeviceInfoNotUsb => "BandInfo must be for a USB connection.";

        public static string PushServiceNotFound => "Band does not support push service.";

        public static string RfComm_FromId_ReturnedNull => "A non-specific error occurred while attempting to acquire the Bluetooth device service. This error can occur if the application manifest does not have the required permissions for opening the Bluetooth connection to the Microsoft Band, or if the user denies access.";

        public static string RfComm_FromId_Threw => "An error occurred while attempting to acquire the Bluetooth device service. This error can occur if the paired device is unreachable or has become unpaired from the current host.";
    }
}
