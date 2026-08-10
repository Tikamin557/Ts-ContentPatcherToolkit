using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using TsCPToolKit.Localization;
using TsCPToolKit.Models;
using TsCPToolKit.Services;

namespace TsCPToolKit.UI
{
    /// <summary>
    /// アプリケーションのバージョン情報と
    /// 更新情報を表示するウィンドウです。
    /// </summary>
    public partial class AboutUpdateWindow : Window
    {
        /// <summary>
        /// 最新バージョン情報の取得を担当するサービスです。
        /// </summary>
        private readonly UpdateCheckService updateCheckService =
            new();

        /// <summary>
        /// 現在実行中のアプリケーションの
        /// バージョンです。
        /// </summary>
        private readonly Version currentVersion;

        /// <summary>
        /// ウィンドウ生成時に渡された
        /// 取得済みの更新情報です。
        /// </summary>
        /// <remarks>
        /// 起動時の自動更新確認ですでに情報を取得している場合に使用し、
        /// 同じ更新情報を再取得するのを防ぎます。
        /// (起動時チェック済みなら再通信しないため。)
        /// </remarks>
        private readonly UpdateInfo? initialUpdateInfo;

        /// <summary>
        /// Nexus Modsページを開くためのURLです。
        /// </summary>
        private string? nexusUrl;

        /// <summary>
        /// 更新情報を新たに取得して表示する
        /// バージョン情報ウィンドウを初期化します。
        /// </summary>
        public AboutUpdateWindow()
            : this(null)
        {
        }

        /// <summary>
        /// 取得済みの更新情報を使用して
        /// バージョン情報ウィンドウを初期化します。
        /// </summary>
        /// <param name="updateInfo">
        /// すでに取得済みの更新情報。
        /// nullの場合はウィンドウ表示後に更新情報を取得します。
        /// </param>
        public AboutUpdateWindow(
            UpdateInfo? updateInfo)
        {
            InitializeComponent();

            initialUpdateInfo =
                updateInfo;

            currentVersion =
                GetCurrentVersion();

            ShowCurrentVersion();

            Loaded +=
                AboutUpdateWindow_Loaded;
        }

        /// <summary>
        /// 現在実行中のアプリケーションの
        /// バージョン番号を取得します。
        /// </summary>
        private static Version GetCurrentVersion()
        {
            Version? assemblyVersion =
                Assembly.GetExecutingAssembly()
                    .GetName()
                    .Version;

            if (assemblyVersion is null)
            {
                return new Version(
                    0,
                    0,
                    0);
            }

            return new Version(
                assemblyVersion.Major,
                assemblyVersion.Minor,
                Math.Max(
                    0,
                    assemblyVersion.Build));
        }

        /// <summary>
        /// 現在のバージョン番号を表示します。
        /// </summary>
        private void ShowCurrentVersion()
        {
            CurrentVersionTextBlock.Text =
                $"{currentVersion.Major}." +
                $"{currentVersion.Minor}." +
                $"{currentVersion.Build}";
        }

        /// <summary>
        /// 公開されている最新バージョンを確認します。
        /// </summary>
        private async Task CheckForUpdatesAsync()
        {
            SetCheckingState(
                isChecking: true);

            LatestVersionTextBlock.Text =
                LanguageManager.GetString(
                    "AboutUpdate_NotChecked");

            UpdateStatusTextBlock.Text =
                LanguageManager.GetString(
                    "AboutUpdate_Checking");

            nexusUrl = null;

            OpenNexusButton.IsEnabled =
                false;

            try
            {
                UpdateInfo updateInfo =
                    await updateCheckService
                        .GetLatestVersionAsync();

                ShowUpdateInfo(
                    updateInfo);
            }
            catch (Exception exception)
            {
                LatestVersionTextBlock.Text =
                    LanguageManager.GetString(
                        "AboutUpdate_UnknownVersion");

                UpdateStatusTextBlock.Text =
                    LanguageManager.GetString(
                        "AboutUpdate_CheckFailed") +
                    "\n\n" +
                    exception.Message;
            }
            finally
            {
                SetCheckingState(
                    isChecking: false);
            }
        }

        /// <summary>
        /// 取得した更新情報を画面へ表示します。
        /// </summary>
        private void ShowUpdateInfo(
            UpdateInfo updateInfo)
        {
            LatestVersionTextBlock.Text =
                updateInfo.Version;

            bool hasNewVersion =
                UpdateCheckService.IsNewerVersion(
                    currentVersion,
                    updateInfo.Version);

            UpdateStatusTextBlock.Text =
                hasNewVersion
                    ? LanguageManager.GetString(
                        "AboutUpdate_NewVersionStatus")
                    : LanguageManager.GetString(
                        "AboutUpdate_LatestStatus");

            nexusUrl = null;

            OpenNexusButton.IsEnabled =
                false;

            if (!TryValidateNexusUrl(
                updateInfo.NexusUrl))
            {
                return;
            }

            nexusUrl =
                updateInfo.NexusUrl;

            OpenNexusButton.IsEnabled =
                true;
        }

        /// <summary>
        /// Nexus Modsページをブラウザで開きます。
        /// </summary>
        private void OpenNexusButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!TryValidateNexusUrl(
                nexusUrl))
            {
                return;
            }

            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            nexusUrl!,

                        UseShellExecute =
                            true
                    });
            }
            catch (Exception exception)
            {
                UpdateStatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Internal_UpdateUrlInvalid") +
                    "\n\n" +
                    exception.Message;
            }
        }

        /// <summary>
        /// Nexus ModsのURLとして利用可能かを確認します。
        /// </summary>
        private static bool TryValidateNexusUrl(
            string? url)
        {
            if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out Uri? uri))
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttps
                || uri.Scheme == Uri.UriSchemeHttp;
        }

        /// <summary>
        /// ウィンドウが表示されたときに
        /// 最新バージョンを自動確認します。
        /// </summary>
        /// <remarks>
        /// 起動時の更新確認ですでに更新情報を取得している場合は、
        /// その情報を再利用して通信を行いません。
        /// </remarks>
        private async void AboutUpdateWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (initialUpdateInfo is not null)
            {
                ShowUpdateInfo(
                    initialUpdateInfo);

                return;
            }

            await CheckForUpdatesAsync();
        }

        /// <summary>
        /// 更新確認中の操作状態を切り替えます。
        /// </summary>
        private void SetCheckingState(
            bool isChecking)
        {
            CloseButton.IsEnabled =
                !isChecking;
        }

        /// <summary>
        /// ウィンドウを閉じます。
        /// </summary>
        private void CloseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}