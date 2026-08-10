using System.Text;
using TsCPToolKit.Models;
using TsCPToolKit.Parsers;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Services
{
    /// <summary>
    /// 改行位置や行内の内容を変更せず、
    /// 各行の先頭のインデントだけを整理します。
    /// </summary>
    public sealed class JsonIndentService
    {
        private readonly JsonScanner scanner = new();

        /// <summary>
        /// JSONまたはJSONC形式のテキストについて、
        /// 各行の先頭のインデントだけを整理します。
        /// </summary>
        public string Format(
            string content,
            IndentOptions options)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(options);

            if (options.IndentSize < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.IndentSize),
                    LanguageManager.GetString(
                        "Internal_InvalidIndentSize"));
            }

            IReadOnlyList<JsonLine> lines =
                ParseLines(content);

            CalculateIndentLevels(
                lines,
                options);

            return BuildFormattedText(
                lines,
                options);
        }

        /// <summary>
        /// 改行コードを保持したまま、
        /// テキストを1行ずつのデータへ分割します。
        /// </summary>
        private static IReadOnlyList<JsonLine> ParseLines(
            string content)
        {
            List<JsonLine> lines = new();

            int position = 0;
            int lineNumber = 1;

            while (position < content.Length)
            {
                int lineStart = position;

                while (position < content.Length
                    && content[position] != '\r'
                    && content[position] != '\n')
                {
                    position++;
                }

                string lineText =
                    content[lineStart..position];

                string lineEnding =
                    ReadLineEnding(
                        content,
                        ref position);

                lines.Add(
                    new JsonLine(
                        lineNumber,
                        lineText,
                        lineEnding));

                lineNumber++;
            }

            if (content.Length == 0)
            {
                lines.Add(
                    new JsonLine(
                        lineNumber: 1,
                        originalText: string.Empty,
                        lineEnding: string.Empty));
            }

            return lines;
        }

        /// <summary>
        /// 各行に設定するJSON階層と、
        /// 必要に応じて特殊なインデント位置を計算します。
        /// </summary>
        private void CalculateIndentLevels(
            IReadOnlyList<JsonLine> lines,
            IndentOptions options)
        {
            scanner.Reset();

            int currentIndentLevel = 0;

            foreach (JsonLine line in lines)
            {
                JsonScanResult scanResult =
                    scanner.ScanLine(
                        line.Content);

                /*
                 * 通常は特殊なインデント指定を使用しません。
                 */
                line.ExactIndentSpaces =
                    null;

                /*
                 * この行が複数行文字列の続きの場合は、
                 * JSON階層ではなく、文字列本文の開始位置へ揃えます。
                 *
                 * 例えば、
                 *
                 *     "Target": "Maps/A,
                 *                Maps/B,
                 *                Maps/C",
                 *
                 * の Maps/B、Maps/C の位置を揃えます。
                 */
                if (scanResult.WasInsideString
                    && scanResult.StringContentColumn >= 0)
                {
                    int baseIndentSpaces =
                        currentIndentLevel
                        * options.IndentSize;

                    line.ExactIndentSpaces =
                        baseIndentSpaces
                        + scanResult.StringContentColumn;

                    line.NewIndentLevel =
                        currentIndentLevel;
                }
                else if (scanResult.WasInsideBlockComment
                    || scanResult.IsInsideBlockComment)
                {
                    /*
                     * 複数行コメントの開始行・内部・終了行は、
                     * 現在のJSON階層へ揃えます。
                     *
                     * コメント内の { } [ ] は
                     * JSON階層として扱いません。
                     */
                    line.NewIndentLevel =
                        currentIndentLevel;
                }
                else
                {
                    line.NewIndentLevel = Math.Max(
                        0,
                        currentIndentLevel
                            - scanResult.LeadingClosingBracketCount);
                }

                currentIndentLevel = Math.Max(
                    0,
                    currentIndentLevel
                        + scanResult.IndentLevelChange);
            }
        }

        /// <summary>
        /// 計算された階層を使い、
        /// 整理後のテキストを構築します。
        /// </summary>
        private static string BuildFormattedText(
            IReadOnlyList<JsonLine> lines,
            IndentOptions options)
        {
            StringBuilder result = new();

            foreach (JsonLine line in lines)
            {
                if (line.IsBlank)
                {
                    if (!options.TrimBlankLines)
                    {
                        result.Append(
                            line.OriginalText);
                    }

                    result.Append(
                        line.LineEnding);

                    continue;
                }

                int spaceCount =
                    line.ExactIndentSpaces
                    ?? line.NewIndentLevel
                        * options.IndentSize;

                result.Append(
                    ' ',
                    spaceCount);

                result.Append(
                    line.Content);

                result.Append(
                    line.LineEnding);
            }

            return result.ToString();
        }

        /// <summary>
        /// 現在位置にある改行コードを読み取り、
        /// 次の行の先頭へ進めます。
        /// </summary>
        private static string ReadLineEnding(
            string content,
            ref int position)
        {
            if (position >= content.Length)
            {
                return string.Empty;
            }

            if (content[position] == '\r')
            {
                position++;

                if (position < content.Length
                    && content[position] == '\n')
                {
                    position++;

                    return "\r\n";
                }

                return "\r";
            }

            if (content[position] == '\n')
            {
                position++;

                return "\n";
            }

            return string.Empty;
        }
    }
}