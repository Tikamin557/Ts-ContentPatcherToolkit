namespace TsCPToolKit.Models
{
    /// <summary>
    /// テスト1件分の実行状態です。
    /// </summary>
    public enum TestCaseStatus
    {
        /// <summary>
        /// 出力内容が期待結果と一致しました。
        /// </summary>
        Passed,

        /// <summary>
        /// 出力内容が期待結果と一致しませんでした。
        /// </summary>
        Failed,

        /// <summary>
        /// 比較対象となる期待結果がまだありません。
        /// </summary>
        ExpectedMissing,

        /// <summary>
        /// テストの実行中にエラーが発生しました。
        /// </summary>
        Error
    }

    /// <summary>
    /// テスト1件分の実行結果を保持します。
    /// </summary>
    public sealed class TestCaseResult
    {
        /// <summary>
        /// テスト結果を作成します。
        /// </summary>
        public TestCaseResult(
            string fileName,
            string inputPath,
            string outputPath,
            string expectedPath,
            TestCaseStatus status,
            string message)
        {
            FileName = fileName;
            InputPath = inputPath;
            OutputPath = outputPath;
            ExpectedPath = expectedPath;
            Status = status;
            Message = message;
        }

        /// <summary>
        /// テスト対象のファイル名を取得します。
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 入力ファイルのパスを取得します。
        /// </summary>
        public string InputPath { get; }

        /// <summary>
        /// 生成された出力ファイルのパスを取得します。
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// 期待結果ファイルのパスを取得します。
        /// </summary>
        public string ExpectedPath { get; }

        /// <summary>
        /// テストの実行状態を取得します。
        /// </summary>
        public TestCaseStatus Status { get; }

        /// <summary>
        /// 実行結果の説明を取得します。
        /// </summary>
        public string Message { get; }
    }
}