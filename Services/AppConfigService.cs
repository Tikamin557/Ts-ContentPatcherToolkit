using System;
using System.IO;
using System.Text.Json;
using TsCPToolKit.Models;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Services
{
    /// <summary>
    /// アプリケーション設定の読み込みと保存を担当します。
    /// </summary>
    public sealed class AppConfigService
    {
        private const string ApplicationFolderName =
            "T's Content Patcher Toolkit";

        private const string ConfigFileName =
            "config.json";

        private static readonly JsonSerializerOptions SerializerOptions =
            new()
            {
                WriteIndented = true
            };

        /// <summary>
        /// 設定ファイルの保存先を取得します。
        /// </summary>
        public string ConfigFilePath { get; } =
            BuildConfigFilePath();

        /// <summary>
        /// 保存されている設定を読み込みます。
        /// </summary>
        /// <remarks>
        /// 設定ファイルが存在しない場合や、
        /// 読み込みに失敗した場合は初期設定を返します。
        /// </remarks>
        public AppConfig Load()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    return new AppConfig();
                }

                string json =
                    File.ReadAllText(ConfigFilePath);

                AppConfig? config =
                    JsonSerializer.Deserialize<AppConfig>(
                        json,
                        SerializerOptions);

                return ValidateConfig(
                    config ?? new AppConfig());
            }
            catch
            {
                /*
                 * 設定ファイルの破損などで、
                 * アプリ本体が起動できなくなるのを防ぎます。
                 */
                return new AppConfig();
            }
        }

        /// <summary>
        /// 現在の設定をconfig.jsonへ保存します。
        /// </summary>
        public void Save(AppConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            string? directoryPath =
                Path.GetDirectoryName(ConfigFilePath);

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new InvalidOperationException(
                    LanguageManager.GetString(
                        "Internal_ConfigPathUnavailable"));
            }

            Directory.CreateDirectory(directoryPath);

            string json =
                JsonSerializer.Serialize(
                    config,
                    SerializerOptions);

            File.WriteAllText(
                ConfigFilePath,
                json);
        }

        /// <summary>
        /// 設定値を利用可能な範囲へ補正します。
        /// </summary>
        private static AppConfig ValidateConfig(
            AppConfig config)
        {
            if (config.IndentSize is not (2 or 3 or 4 or 8))
            {
                config.IndentSize = 4;
            }

            config.TestsFolderPath ??=
                string.Empty;

            if (!LanguageManager.IsSupported(config.Language))
            {
                config.Language =
                    LanguageManager.DefaultLanguage;
            }

            return config;
        }

        /// <summary>
        /// config.jsonの保存先を作成します。
        /// </summary>
        private static string BuildConfigFilePath()
        {
            string documentsPath =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);

            return Path.Combine(
                documentsPath,
                ApplicationFolderName,
                ConfigFileName);
        }
    }
}