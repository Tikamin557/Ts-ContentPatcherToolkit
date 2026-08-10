using System;
using System.Text;

namespace TsCPToolKit.Helpers
{
    /// <summary>
    /// テキストファイルの書き込みに使用する
    /// 文字コードを作成します。
    /// </summary>
    public static class EncodingHelper
    {
        /// <summary>
        /// 元の文字コードとBOMの有無を維持した
        /// 書き込み用文字コードを作成します。
        /// </summary>
        public static Encoding CreateOutputEncoding(
            Encoding sourceEncoding,
            bool hasByteOrderMark)
        {
            ArgumentNullException.ThrowIfNull(
                sourceEncoding);

            // UTF-8
            if (sourceEncoding.CodePage
                == Encoding.UTF8.CodePage)
            {
                return new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier:
                        hasByteOrderMark,
                    throwOnInvalidBytes: true);
            }

            // UTF-16 Little Endian
            if (sourceEncoding.CodePage
                == Encoding.Unicode.CodePage)
            {
                return new UnicodeEncoding(
                    bigEndian: false,
                    byteOrderMark: hasByteOrderMark,
                    throwOnInvalidBytes: true);
            }

            // UTF-16 Big Endian
            if (sourceEncoding.CodePage
                == Encoding.BigEndianUnicode.CodePage)
            {
                return new UnicodeEncoding(
                    bigEndian: true,
                    byteOrderMark: hasByteOrderMark,
                    throwOnInvalidBytes: true);
            }

            // UTF-32 Little Endian
            if (sourceEncoding.CodePage == 12000)
            {
                return new UTF32Encoding(
                    bigEndian: false,
                    byteOrderMark: hasByteOrderMark,
                    throwOnInvalidCharacters: true);
            }

            // UTF-32 Big Endian
            if (sourceEncoding.CodePage == 12001)
            {
                return new UTF32Encoding(
                    bigEndian: true,
                    byteOrderMark: hasByteOrderMark,
                    throwOnInvalidCharacters: true);
            }

            return sourceEncoding;
        }
    }
}