// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.IBandAltimeterReading
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Sensors
{
    public interface IBandAltimeterReading : IBandSensorReading
    {
        long TotalGain { get; }

        long TotalLoss { get; }

        long SteppingGain { get; }

        long SteppingLoss { get; }

        long StepsAscended { get; }

        long StepsDescended { get; }

        float Rate { get; }

        long FlightsAscended { get; }

        long FlightsDescended { get; }

        long FlightsAscendedToday { get; }

        long TotalGainToday { get; }
    }
}
