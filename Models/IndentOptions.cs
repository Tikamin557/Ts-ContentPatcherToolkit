namespace TsCPToolKit.Models
{
    /// <summary>
    /// JSONのインデント整理に使用する設定です。
    /// </summary>
    public sealed class IndentOptions
    {
        /// <summary>
        /// インデント1段あたりのスペース数です。
        /// </summary>
        public int IndentSize { get; init; } = 4;

        /// <summary>
        /// 空行に含まれるスペースを削除するかどうかです。
        /// </summary>
        public bool TrimBlankLines { get; init; } = true;
    }
}