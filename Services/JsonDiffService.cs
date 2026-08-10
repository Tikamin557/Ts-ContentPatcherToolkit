using TsCPToolKit.Models;

namespace TsCPToolKit.Services
{
    /// <summary>
    /// 2つのテキストを行単位で比較します。
    /// </summary>
    public sealed class JsonDiffService
    {
        /// <summary>
        /// 最初に異なる行を取得します。
        /// </summary>
        /// <param name="expected">
        /// 正解として扱うExpected側のテキスト。
        /// </param>
        /// <param name="actual">
        /// 実際に生成されたOutput側のテキスト。
        /// </param>
        /// <returns>
        /// 最初に見つかった差分。
        /// </returns>
        public JsonDiffResult Compare(
            string expected,
            string actual)
        {
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(actual);

            string[] expectedLines =
                NormalizeLineEndings(expected);

            string[] actualLines =
                NormalizeLineEndings(actual);

            int lineCount =
                Math.Max(
                    expectedLines.Length,
                    actualLines.Length);

            for (int index = 0; index < lineCount; index++)
            {
                string expectedLine =
                    index < expectedLines.Length
                        ? expectedLines[index]
                        : "<EOF>";

                string actualLine =
                    index < actualLines.Length
                        ? actualLines[index]
                        : "<EOF>";

                if (string.Equals(
                    expectedLine,
                    actualLine,
                    StringComparison.Ordinal))
                {
                    continue;
                }

                return new JsonDiffResult(
                    hasDifference: true,
                    lineNumber: index + 1,
                    expectedLine,
                    actualLine);
            }

            return JsonDiffResult.NoDifference;
        }

        /// <summary>
        /// 改行コードをLFへ統一して行単位に分割します。
        /// </summary>
        private static string[] NormalizeLineEndings(
            string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n');
        }
    }
}