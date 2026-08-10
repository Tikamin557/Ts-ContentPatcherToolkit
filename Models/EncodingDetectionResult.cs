using System;
using System.Text;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Models
{
    /// <summary>
    /// 文字コードの判定結果を保持します。
    /// </summary>
    public sealed class EncodingDetectionResult
    {
        /// <summary>
        /// 文字コードの判定結果を作成します。
        /// </summary>
        /// <param name="encoding">
        /// 検出された文字コード。
        /// </param>
        /// <param name="hasByteOrderMark">
        /// BOMが存在するかどうか。
        /// </param>
        /// <param name="preambleLength">
        /// BOMのバイト数。
        /// </param>
        public EncodingDetectionResult(
            Encoding encoding,
            bool hasByteOrderMark,
            int preambleLength)
        {
            ArgumentNullException.ThrowIfNull(encoding);

            if (preambleLength < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(preambleLength),
                    LanguageManager.GetString(
                        "Internal_InvalidBomLength"));
            }

            Encoding = encoding;
            HasByteOrderMark = hasByteOrderMark;
            PreambleLength = preambleLength;
        }

        /// <summary>
        /// 検出された文字コードを取得します。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// BOMが存在するかどうかを取得します。
        /// </summary>
        public bool HasByteOrderMark { get; }

        /// <summary>
        /// BOMのバイト数を取得します。
        /// </summary>
        public int PreambleLength { get; }
    }
}