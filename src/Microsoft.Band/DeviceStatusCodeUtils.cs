// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.DeviceStatusCodeUtils
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band
{
    internal static class DeviceStatusCodeUtils
    {
        private const ushort CodeShift = 0;
        private const byte CodeBits = 16;
        private const uint CodeMask = 65535;
        private const ushort FacilityShift = 16;
        private const byte FacilityBits = 11;
        private const uint FacilityMask = 134152192;
        private const ushort ReservedShift = 27;
        private const byte ReservedBits = 4;
        private const uint ReservedMask = 2013265920;
        private const uint CustomerBit = 536870912;
        private const ushort SeverityShift = 31;
        private const byte SeverityBits = 1;
        private const uint SeverityMask = 2147483648;
        internal static uint Success;

        internal static uint MakeStatus(bool isError, Facility facility, ushort code)
        {
            int num1 = (isError ? 1 : 0) << 31;
            uint num2 = 0;
            uint num3 = (uint)facility << 16;
            uint num4 = code;
            int num5 = (int)num2;
            return (uint)(num1 | num5) | num3 | num4;
        }

        internal static uint MakeStatusCust(bool isError, Facility facility, ushort code) => (uint)((isError ? 1 : 0) << 31 | 536870912) | (uint)facility << 16 | code;

        internal static bool IsSeverityError(uint status) => (status & 2147483648U) > 0U;
    }
}
