using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using TsCPToolKit.Models;
using TsCPToolKit.Services;
using TsCPToolKit.UI;

namespace TsCPToolKit
{
    /// <summary>
    /// アプリケーションのメインウィンドウです。
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly JsonIndentService jsonIndentService =
            new();

        private readonly JsonIndentTestService jsonIndentTestService =
            new();

        private readonly JsonBatchProcessService jsonBatchProcessService =
            new();

        private readonly AppConfigService appConfigService =
            new();

        private readonly UpdateCheckService updateCheckService =
            new();

        /// <summary>
        /// メインウィンドウを初期化します。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            LoadAppConfig();

            Loaded +=
                MainWindow_Loaded;
        }

        /// <summary>
        /// メインウィンドウ表示後に
        /// 更新情報を自動確認します。
        /// </summary>
        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await CheckForUpdateOnStartupAsync();
        }

        /// <summary>
        /// アプリ起動時に最新バージョンを確認し、
        /// 新しいバージョンがある場合だけ
        /// バージョン情報ウィンドウを表示します。
        /// </summary>
        private async Task CheckForUpdateOnStartupAsync()
        {
            try
            {
                UpdateInfo updateInfo =
                    await updateCheckService
                        .GetLatestVersionAsync();

                Version? assemblyVersion =
                    Assembly.GetExecutingAssembly()
                        .GetName()
                        .Version;

                if (assemblyVersion is null)
                {
                    return;
                }

                Version currentVersion =
                    new(
                        assemblyVersion.Major,
                        assemblyVersion.Minor,
                        Math.Max(
                            0,
                            assemblyVersion.Build));

                bool hasNewVersion =
                    UpdateCheckService.IsNewerVersion(
                        currentVersion,
                        updateInfo.Version);

                if (!hasNewVersion)
                {
                    return;
                }

                AboutUpdateWindow window =
                    new(updateInfo)
                    {
                        Owner = this
                    };

                window.ShowDialog();
            }
            catch
            {
                /*
                 * 起動時の更新確認に失敗しても、
                 * アプリ本体の使用には影響させません。
                 */
            }
        }

        /// <summary>
        /// バージョン情報・更新確認ウィンドウを開きます。
        /// </summary>
        private void AboutUpdateButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            AboutUpdateWindow window = new()
            {
                Owner = this
            };

            window.ShowDialog();
        }
    }
}