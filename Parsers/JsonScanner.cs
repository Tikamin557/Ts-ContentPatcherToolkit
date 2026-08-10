using TsCPToolKit.Models;

namespace TsCPToolKit.Parsers
{
    /// <summary>
    /// JSONまたはJSONC形式のテキストを1行ずつ解析し、
    /// JSONの階層に関係する括弧だけを検出します。
    /// </summary>
    public sealed class JsonScanner
    {
        /// <summary>
        /// 現在、複数行コメントの内部にいるかどうかです。
        /// </summary>
        private bool isInsideBlockComment;

        /// <summary>
        /// 現在、複数行文字列の内部にいるかどうかです。
        /// </summary>
        private bool isInsideString;

        /// <summary>
        /// 複数行文字列の続きの行を揃える列位置です。
        /// </summary>
        /// <remarks>
        /// 複数行文字列ではない場合は-1です。
        /// </remarks>
        private int stringContentColumn =
            -1;

        /// <summary>
        /// スキャナーの状態を初期化します。
        /// </summary>
        public void Reset()
        {
            isInsideBlockComment = false;
            isInsideString = false;

            stringContentColumn =
                -1;
        }

        /// <summary>
        /// 1行分のテキストを解析します。
        /// </summary>
        /// <param name="line">
        /// 行頭のインデントを除いた1行分の本文。
        /// </param>
        /// <returns>
        /// 検出した括弧、コメント状態、
        /// 文字列状態に関する情報。
        /// </returns>
        public JsonScanResult ScanLine(
            string line)
        {
            ArgumentNullException.ThrowIfNull(line);

            int openingBracketCount = 0;
            int closingBracketCount = 0;
            int leadingClosingBracketCount = 0;

            bool wasInsideBlockComment =
                isInsideBlockComment;

            bool wasInsideString =
                isInsideString;

            bool isEscaped = false;

            /*
             * この行が複数行文字列の続きから始まる場合は、
             * 以前記録した揃え位置をそのまま使用します。
             */
            int lineStringContentColumn =
                wasInsideString
                    ? stringContentColumn
                    : -1;

            /*
             * 行頭から閉じ括弧だけが続いているかを表します。
             *
             * コメントや文字列が先に現れた場合、
             * その後にある閉じ括弧は「行頭の閉じ括弧」として
             * 扱いません。
             */
            bool canCountLeadingClosingBrackets =
                !isInsideBlockComment
                && !isInsideString;

            for (int index = 0;
                index < line.Length;
                index++)
            {
                char current =
                    line[index];

                char next =
                    index + 1 < line.Length
                        ? line[index + 1]
                        : '\0';

                /*
                 * 複数行コメントの内部です。
                 *
                 * コメント終了記号以外は、
                 * すべて無視します。
                 */
                if (isInsideBlockComment)
                {
                    canCountLeadingClosingBrackets =
                        false;

                    if (current == '*'
                        && next == '/')
                    {
                        isInsideBlockComment = false;
                        index++;
                    }

                    continue;
                }

                /*
                 * JSON文字列の内部です。
                 */
                if (isInsideString)
                {
                    canCountLeadingClosingBrackets =
                        false;

                    if (isEscaped)
                    {
                        isEscaped = false;
                        continue;
                    }

                    if (current == '\\')
                    {
                        isEscaped = true;
                        continue;
                    }

                    if (current == '"')
                    {
                        isInsideString = false;

                        stringContentColumn =
                            -1;
                    }

                    continue;
                }

                /*
                 * 行コメントの開始です。
                 *
                 * 以降はすべてコメントなので、
                 * この行の解析を終了します。
                 */
                if (current == '/'
                    && next == '/')
                {
                    break;
                }

                /*
                 * 複数行コメントの開始です。
                 */
                if (current == '/'
                    && next == '*')
                {
                    isInsideBlockComment = true;

                    canCountLeadingClosingBrackets =
                        false;

                    index++;

                    continue;
                }

                /*
                 * JSON文字列の開始です。
                 */
                if (current == '"')
                {
                    isInsideString = true;

                    canCountLeadingClosingBrackets =
                        false;

                    /*
                     * この文字列が行末まで閉じなかった場合に、
                     * 次の行を文字列内容の開始位置へ揃えるため、
                     * 開始位置を記録します。
                     *
                     * index + 1 は開始引用符の直後です。
                     */
                    stringContentColumn =
                        index + 1;

                    lineStringContentColumn =
                        stringContentColumn;

                    continue;
                }

                /*
                 * 空白は読み飛ばします。
                 */
                if (char.IsWhiteSpace(current))
                {
                    continue;
                }

                /*
                 * 開き括弧です。
                 */
                if (current is '{' or '[')
                {
                    openingBracketCount++;

                    canCountLeadingClosingBrackets =
                        false;

                    continue;
                }

                /*
                 * 閉じ括弧です。
                 */
                if (current is '}' or ']')
                {
                    closingBracketCount++;

                    if (canCountLeadingClosingBrackets)
                    {
                        leadingClosingBracketCount++;
                    }

                    continue;
                }

                /*
                 * 括弧以外の通常文字が出現しました。
                 */
                canCountLeadingClosingBrackets =
                    false;
            }

            return new JsonScanResult(
                openingBracketCount,
                closingBracketCount,
                leadingClosingBracketCount,
                wasInsideBlockComment,
                isInsideBlockComment,
                wasInsideString,
                isInsideString,
                lineStringContentColumn);
        }
    }
}