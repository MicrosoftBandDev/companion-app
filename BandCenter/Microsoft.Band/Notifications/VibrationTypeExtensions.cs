// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.Notifications.VibrationTypeExtensions
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System;

namespace Microsoft.Band.Notifications
{
    internal static class VibrationTypeExtensions
    {
        public static BandVibrationType ToBandVibrationType(
          this VibrationType vibrationType)
        {
            switch (vibrationType)
            {
                case VibrationType.NotificationOneTone:
                    return BandVibrationType.ToastTextMessage;
                case VibrationType.NotificationTwoTone:
                    return BandVibrationType.AlertIncomingCall;
                case VibrationType.NotificationAlarm:
                    return BandVibrationType.AlertAlarm;
                case VibrationType.NotificationTimer:
                    return BandVibrationType.AlertTimer;
                case VibrationType.OneToneHigh:
                    return BandVibrationType.ExerciseGuidedWorkoutTimer;
                case VibrationType.TwoToneHigh:
                    return BandVibrationType.ExerciseGuidedWorkoutCircuitComplete;
                case VibrationType.ThreeToneHigh:
                    return BandVibrationType.ExerciseGuidedWorkoutComplete;
                case VibrationType.RampUp:
                    return BandVibrationType.SystemStartUp;
                case VibrationType.RampDown:
                    return BandVibrationType.SystemShutDown;
                default:
                    throw new ArgumentException("Unknown VibrationType value.");
            }
        }
    }
}
