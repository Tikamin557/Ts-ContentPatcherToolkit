using System;
using System.Text;
using TsCPToolKit.Models;

namespace TsCPToolKit.Utilities
{
    /// <summary>
    /// バイト列のBOMを確認し、文字コードを判定します。
    /// </summary>
    public static class EncodingDetector
    {
        /// <summary>
        /// バイト列から文字コード情報を取得します。
        /// </summary>
        public static EncodingDetectionResult Detect(
            byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);

            // UTF-32 Little Endian
            //
            // UTF-16 LEのBOMと先頭2バイトが同じなので、
            // UTF-16より先に確認する必要があります。
            if (bytes.Length >= 4
                && bytes[0] == 0xFF
                && bytes[1] == 0xFE
                && bytes[2] == 0x00
                && bytes[3] == 0x00)
            {
                return new EncodingDetectionResult(
                    new UTF32Encoding(
                        bigEndian: false,
                        byteOrderMark: true,
                        throwOnInvalidCharacters: true),
                    hasByteOrderMark: true,
                    preambleLength: 4);
            }

            // UTF-32 Big Endian
            if (bytes.Length >= 4
                && bytes[0] == 0x00
                && bytes[1] == 0x00
                && bytes[2] == 0xFE
                && bytes[3] == 0xFF)
            {
                return new EncodingDetectionResult(
                    new UTF32Encoding(
                        bigEndian: true,
                        byteOrderMark: true,
                        throwOnInvalidCharacters: true),
                    hasByteOrderMark: true,
                    preambleLength: 4);
            }

            // UTF-8 BOM
            if (bytes.Length >= 3
                && bytes[0] == 0xEF
                && bytes[1] == 0xBB
                && bytes[2] == 0xBF)
            {
                return new EncodingDetectionResult(
                    new UTF8Encoding(
                        encoderShouldEmitUTF8Identifier: true,
                        throwOnInvalidBytes: true),
                    hasByteOrderMark: true,
                    preambleLength: 3);
            }

            // UTF-16 Little Endian
            if (bytes.Length >= 2
                && bytes[0] == 0xFF
                && bytes[1] == 0xFE)
            {
                return new EncodingDetectionResult(
                    new UnicodeEncoding(
                        bigEndian: false,
                        byteOrderMark: true,
                        throwOnInvalidBytes: true),
                    hasByteOrderMark: true,
                    preambleLength: 2);
            }

            // UTF-16 Big Endian
            if (bytes.Length >= 2
                && bytes[0] == 0xFE
                && bytes[1] == 0xFF)
            {
                return new EncodingDetectionResult(
                    new UnicodeEncoding(
                        bigEndian: true,
                        byteOrderMark: true,
                        throwOnInvalidBytes: true),
                    hasByteOrderMark: true,
                    preambleLength: 2);
            }

            /*
             * BOMがない場合はUTF-8として扱います。
             *
             * Content PatcherのJSONでは、
             * UTF-8 BOMなしが一般的です。
             */
            return new EncodingDetectionResult(
                new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true),
                hasByteOrderMark: false,
                preambleLength: 0);
        }
    }
}