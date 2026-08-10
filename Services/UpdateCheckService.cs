using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TsCPToolKit.Models;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Services
{
    /// <summary>
    /// アプリケーションの最新バージョン情報を取得します。
    /// </summary>
    public sealed class UpdateCheckService
    {
        /*
         * GitHubの version.json のRawアドレス
         */
        private const string VersionInfoUrl =
            "https://raw.githubusercontent.com/Tikamin557/Ts-ContentPatcherToolkit/refs/heads/main/version.json";

        private static readonly HttpClient HttpClient =
            new()
            {
                Timeout =
                    TimeSpan.FromSeconds(10)
            };

        /// <summary>
        /// 公開されている最新バージョン情報を取得します。
        /// </summary>
        public async Task<UpdateInfo> GetLatestVersionAsync()
        {
            UpdateInfo? updateInfo =
                await HttpClient.GetFromJsonAsync<UpdateInfo>(
                    VersionInfoUrl);

            if (updateInfo is null)
            {
                throw new InvalidOperationException(
                    LanguageManager.GetString(
                        "Internal_UpdateInfoLoadFailed"));
            }

            if (string.IsNullOrWhiteSpace(
                updateInfo.Version))
            {
                throw new InvalidOperationException(
                    LanguageManager.GetString(
                        "Internal_UpdateInfoInvalid"));
            }

            return updateInfo;
        }

        /// <summary>
        /// 最新版が現在のバージョンより新しいか判定します。
        /// </summary>
        public static bool IsNewerVersion(
            Version currentVersion,
            string latestVersionText)
        {
            if (!Version.TryParse(
                latestVersionText,
                out Version? latestVersion))
            {
                throw new InvalidOperationException(
                    LanguageManager.GetString(
                        "Internal_UpdateVersionInvalid"));
            }

            return latestVersion >
                currentVersion;
        }
    }
}