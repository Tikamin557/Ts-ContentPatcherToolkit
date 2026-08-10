using System;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Models
{
    /// <summary>
    /// 2つのテキストを比較した結果を保持します。
    /// </summary>
    public sealed class JsonDiffResult
    {
        /// <summary>
        /// 差分が存在しない比較結果を取得します。
        /// </summary>
        public static JsonDiffResult NoDifference { get; } =
            new(
                hasDifference: false,
                lineNumber: 0,
                expectedLine: string.Empty,
                actualLine: string.Empty);

        /// <summary>
        /// 差分比較結果を作成します。
        /// </summary>
        /// <param name="hasDifference">
        /// 差分が存在するかどうか。
        /// </param>
        /// <param name="lineNumber">
        /// 最初に差分が見つかった行番号。
        /// 差分がない場合は0。
        /// </param>
        /// <param name="expectedLine">
        /// Expected側の行内容。
        /// </param>
        /// <param name="actualLine">
        /// 実際に生成されたOutput側の行内容。
        /// </param>
        public JsonDiffResult(
            bool hasDifference,
            int lineNumber,
            string expectedLine,
            string actualLine)
        {
            if (lineNumber < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lineNumber),
                    LanguageManager.GetString(
                        "Internal_InvalidLineNumber"));
            }

            ArgumentNullException.ThrowIfNull(expectedLine);
            ArgumentNullException.ThrowIfNull(actualLine);

            HasDifference = hasDifference;
            LineNumber = lineNumber;
            ExpectedLine = expectedLine;
            ActualLine = actualLine;
        }

        /// <summary>
        /// 差分が存在するかどうかを取得します。
        /// </summary>
        public bool HasDifference { get; }

        /// <summary>
        /// 最初に差分が見つかった行番号を取得します。
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Expected側の行内容を取得します。
        /// </summary>
        public string ExpectedLine { get; }

        /// <summary>
        /// 実際に生成されたOutput側の行内容を取得します。
        /// </summary>
        public string ActualLine { get; }
    }
}