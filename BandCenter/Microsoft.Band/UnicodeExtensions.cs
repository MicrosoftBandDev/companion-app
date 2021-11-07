// Decompiled with JetBrains decompiler
// Type: Microsoft.Band.UnicodeExtensions
// Assembly: Microsoft.Band, Version=1.3.20517.1, Culture=neutral, PublicKeyToken=null
// MVID: AFCBFE03-E2CF-481D-86F4-92C60C36D26A
// Assembly location: C:\Users\Pdawg\Downloads\Microsoft Band Sync Setup\Microsoft_Band.dll

using System.Text;

namespace Microsoft.Band
{
    internal static class UnicodeExtensions
    {
        public static int GetBytesTrimDanglingHighSurrogate(
          this Encoding encoding,
          string s,
          int charCount,
          byte[] bytes,
          int byteIndex)
        {
            charCount = s.LengthTruncatedTrimDanglingHighSurrogate(charCount);
            return encoding.GetBytes(s, 0, charCount, bytes, byteIndex);
        }
    }
}
