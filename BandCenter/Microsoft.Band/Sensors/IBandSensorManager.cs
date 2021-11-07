// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Sensors.IBandSensorManager
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band.Sensors
{
    public interface IBandSensorManager
    {
        IBandSensor<IBandAccelerometerReading> Accelerometer { get; }

        IBandSensor<IBandGyroscopeReading> Gyroscope { get; }

        IBandSensor<IBandDistanceReading> Distance { get; }

        IBandSensor<IBandHeartRateReading> HeartRate { get; }

        IBandContactSensor Contact { get; }

        IBandSensor<IBandSkinTemperatureReading> SkinTemperature { get; }

        IBandSensor<IBandUVReading> UV { get; }

        IBandSensor<IBandPedometerReading> Pedometer { get; }

        IBandSensor<IBandCaloriesReading> Calories { get; }

        IBandSensor<IBandGsrReading> Gsr { get; }

        IBandSensor<IBandRRIntervalReading> RRInterval { get; }

        IBandSensor<IBandAmbientLightReading> AmbientLight { get; }

        IBandSensor<IBandBarometerReading> Barometer { get; }

        IBandSensor<IBandAltimeterReading> Altimeter { get; }
    }
}
