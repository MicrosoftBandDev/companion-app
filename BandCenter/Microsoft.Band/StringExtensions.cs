// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.StringExtensions
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

namespace Microsoft.Band
{
    internal static class StringExtensions
    {
        public static string Truncate(this string s, int length) => s.Length <= length ? s : s.Substring(0, length);

        public static string TruncateTrimDanglingHighSurrogate(this string s, int length)
        {
            length = s.LengthTruncatedTrimDanglingHighSurrogate(length);
            return s.Truncate(length);
        }

        public static int LengthTruncatedTrimDanglingHighSurrogate(this string s, int length)
        {
            if (s.Length <= length)
                return s.Length;
            if (length > 0 && char.IsHighSurrogate(s[length - 1]))
                --length;
            return length;
        }
    }
}
