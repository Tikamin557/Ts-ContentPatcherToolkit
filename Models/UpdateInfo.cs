namespace TsCPToolKit.Models
{
    /// <summary>
    /// 公開されている最新バージョン情報です。
    /// </summary>
    public sealed class UpdateInfo
    {
        /// <summary>
        /// 最新バージョンです。
        /// </summary>
        public string Version { get; set; } =
            string.Empty;

        /// <summary>
        /// Nexus ModsのページURLです。
        /// </summary>
        public string NexusUrl { get; set; } =
            string.Empty;
    }
}