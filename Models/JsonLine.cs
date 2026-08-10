using System;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Models
{
    /// <summary>
    /// JSONファイル内の1行分の情報を保持します。
    /// </summary>
    public sealed class JsonLine
    {
        /// <summary>
        /// JSONファイル内の1行分の情報を作成します。
        /// </summary>
        /// <param name="lineNumber">
        /// 1から始まる行番号。
        /// </param>
        /// <param name="originalText">
        /// 改行コードを含まない、元の行内容。
        /// </param>
        /// <param name="lineEnding">
        /// 元の行末に付いていた改行コード。
        /// </param>
        public JsonLine(
            int lineNumber,
            string originalText,
            string lineEnding)
        {
            if (lineNumber < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(lineNumber),
                    LanguageManager.GetString(
                        "Internal_InvalidLineNumberOneBased"));
            }

            ArgumentNullException.ThrowIfNull(originalText);
            ArgumentNullException.ThrowIfNull(lineEnding);

            LineNumber =
                lineNumber;

            OriginalText =
                originalText;

            LineEnding =
                lineEnding;

            int contentStartIndex =
                FindContentStartIndex(
                    originalText);

            OriginalIndent =
                originalText[..contentStartIndex];

            Content =
                originalText[contentStartIndex..];
        }

        /// <summary>
        /// 1から始まる行番号を取得します。
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// 改行コードを含まない、
        /// 元の行内容を取得します。
        /// </summary>
        public string OriginalText { get; }

        /// <summary>
        /// 元の行末に付いていた改行コードを取得します。
        /// </summary>
        /// <remarks>
        /// CRLF、LF、CR、
        /// またはファイル末尾の場合は空文字列です。
        /// </remarks>
        public string LineEnding { get; }

        /// <summary>
        /// 元の行頭にあったスペースまたはタブを取得します。
        /// </summary>
        public string OriginalIndent { get; }

        /// <summary>
        /// 行頭のスペースとタブを除いた
        /// 本文を取得します。
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// 空行かどうかを取得します。
        /// </summary>
        public bool IsBlank =>
            Content.Length == 0;

        /// <summary>
        /// 整理後のインデント階層を取得または設定します。
        /// </summary>
        /// <remarks>
        /// スペース数ではなく、JSON上の階層数です。
        ///
        /// 例えばインデント幅が4で階層が2の場合、
        /// 実際の行頭には8個のスペースが付きます。
        /// </remarks>
        public int NewIndentLevel { get; set; }

        /// <summary>
        /// 整理後の行頭スペース数を
        /// 直接指定する値を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 通常のJSON行ではnullです。
        ///
        /// 複数行文字列の続きなど、
        /// JSON階層とは異なる位置へ揃える必要がある場合だけ
        /// 実際のスペース数を設定します。
        ///
        /// 値が設定されている場合は
        /// NewIndentLevelより優先して使用します。
        /// </remarks>
        public int? ExactIndentSpaces { get; set; }

        /// <summary>
        /// 行頭のスペースとタブが終わる位置を取得します。
        /// </summary>
        private static int FindContentStartIndex(
            string text)
        {
            int index = 0;

            while (index < text.Length)
            {
                char character =
                    text[index];

                if (character != ' '
                    && character != '\t')
                {
                    break;
                }

                index++;
            }

            return index;
        }
    }
}