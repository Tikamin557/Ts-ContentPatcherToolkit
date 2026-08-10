namespace TsCPToolKit.Models
{
    /// <summary>
    /// アプリケーションの設定を保持します。
    /// </summary>
    public sealed class AppConfig
    {
        /// <summary>
        /// 前回選択したTestsフォルダのパスを取得または設定します。
        /// </summary>
        public string TestsFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// インデント1段あたりのスペース数を取得または設定します。
        /// </summary>
        public int IndentSize { get; set; } = 4;

        /// <summary>
        /// 処理前にバックアップを作成するかどうかを取得または設定します。
        /// </summary>
        public bool CreateBackup { get; set; } = true;

        /// <summary>
        /// アプリケーションで使用する表示言語を取得または設定します。
        /// </summary>
        public string Language { get; set; } = "en";
    }
}