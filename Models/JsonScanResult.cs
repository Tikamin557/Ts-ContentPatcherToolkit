namespace TsCPToolKit.Models
{
    /// <summary>
    /// JSONの1行を解析した結果を保持します。
    /// </summary>
    public sealed class JsonScanResult
    {
        /// <summary>
        /// 解析結果を作成します。
        /// </summary>
        /// <param name="openingBracketCount">
        /// JSON階層に影響する開き括弧の数。
        /// </param>
        /// <param name="closingBracketCount">
        /// JSON階層に影響する閉じ括弧の数。
        /// </param>
        /// <param name="leadingClosingBracketCount">
        /// 行頭にある閉じ括弧の数。
        /// </param>
        /// <param name="wasInsideBlockComment">
        /// この行の開始時点で複数行コメント内だったかどうか。
        /// </param>
        /// <param name="isInsideBlockComment">
        /// この行の終了時点で複数行コメント内かどうか。
        /// </param>
        /// <param name="wasInsideString">
        /// この行の開始時点で複数行文字列内だったかどうか。
        /// </param>
        /// <param name="isInsideString">
        /// この行の終了時点で複数行文字列内かどうか。
        /// </param>
        /// <param name="stringContentColumn">
        /// 複数行文字列の続きの行を揃える列位置。
        /// 利用しない場合は-1。
        /// </param>
        public JsonScanResult(
            int openingBracketCount,
            int closingBracketCount,
            int leadingClosingBracketCount,
            bool wasInsideBlockComment,
            bool isInsideBlockComment,
            bool wasInsideString,
            bool isInsideString,
            int stringContentColumn)
        {
            OpeningBracketCount =
                openingBracketCount;

            ClosingBracketCount =
                closingBracketCount;

            LeadingClosingBracketCount =
                leadingClosingBracketCount;

            WasInsideBlockComment =
                wasInsideBlockComment;

            IsInsideBlockComment =
                isInsideBlockComment;

            WasInsideString =
                wasInsideString;

            IsInsideString =
                isInsideString;

            StringContentColumn =
                stringContentColumn;
        }

        /// <summary>
        /// JSON階層に影響する開き括弧の数を取得します。
        /// </summary>
        public int OpeningBracketCount { get; }

        /// <summary>
        /// JSON階層に影響する閉じ括弧の数を取得します。
        /// </summary>
        public int ClosingBracketCount { get; }

        /// <summary>
        /// 行頭にある閉じ括弧の数を取得します。
        /// </summary>
        public int LeadingClosingBracketCount { get; }

        /// <summary>
        /// この行の開始時点で
        /// 複数行コメント内だったかどうかを取得します。
        /// </summary>
        public bool WasInsideBlockComment { get; }

        /// <summary>
        /// この行の終了時点で
        /// 複数行コメント内かどうかを取得します。
        /// </summary>
        public bool IsInsideBlockComment { get; }

        /// <summary>
        /// この行の開始時点で
        /// 複数行文字列内だったかどうかを取得します。
        /// </summary>
        public bool WasInsideString { get; }

        /// <summary>
        /// この行の終了時点で
        /// 複数行文字列内かどうかを取得します。
        /// </summary>
        public bool IsInsideString { get; }

        /// <summary>
        /// 複数行文字列の続きの行を揃える
        /// 列位置を取得します。
        /// </summary>
        /// <remarks>
        /// 複数行文字列ではない場合は-1です。
        /// </remarks>
        public int StringContentColumn { get; }

        /// <summary>
        /// この行によるJSON階層の変化量を取得します。
        /// </summary>
        /// <remarks>
        /// 正の値なら次の行の階層が深くなり、
        /// 負の値なら浅くなります。
        /// </remarks>
        public int IndentLevelChange =>
            OpeningBracketCount
            - ClosingBracketCount;
    }
}