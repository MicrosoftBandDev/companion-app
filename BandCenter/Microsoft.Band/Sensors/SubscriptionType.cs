// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.SubscriptionType
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Sensors
{
    internal enum SubscriptionType
    {
        Accelerometer128MS = 0,
        Accelerometer32MS = 1,
        AccelerometerGyroscope128MS = 4,
        AccelerometerGyroscope32MS = 5,
        Distance = 13, // 0x0000000D
        Gsr = 15, // 0x0000000F
        HeartRate = 16, // 0x00000010
        Pedometer = 19, // 0x00000013
        SkinTemperature = 20, // 0x00000014
        UV = 21, // 0x00000015
        AmbientLight = 25, // 0x00000019
        RRInterval = 26, // 0x0000001A
        DeviceContact = 35, // 0x00000023
        BatteryGauge = 38, // 0x00000026
        UVFast = 40, // 0x00000028
        Calories1S = 46, // 0x0000002E
        Accelerometer16MS = 48, // 0x00000030
        AccelerometerGyroscope16MS = 49, // 0x00000031
        Barometer = 58, // 0x0000003A
        Elevation = 71, // 0x00000047
        CaloriesWithDailyValues = 107, // 0x0000006B
        DistanceWithDailyValues = 108, // 0x0000006C
        PedometerWithDailyValues = 109, // 0x0000006D
        ElevationWithDailyValues = 110, // 0x0000006E
        UVWithDailyValues = 111, // 0x0000006F
        AmbientLightWithDailyValues = 112, // 0x00000070
        Gsr200MS = 113, // 0x00000071
        LogEntry = 124, // 0x0000007C
        Max = 124, // 0x0000007C
    }
}
