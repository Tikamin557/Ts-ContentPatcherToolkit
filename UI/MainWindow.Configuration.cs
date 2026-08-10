using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using TsCPToolKit.Helpers;
using TsCPToolKit.Localization;
using TsCPToolKit.Models;

namespace TsCPToolKit
{
    /// <summary>
    /// MainWindowの設定保存・復元処理を担当します。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 保存されているアプリケーション設定を画面へ反映します。
        /// </summary>
        private void LoadAppConfig()
        {
            AppConfig config =
                appConfigService.Load();

            LanguageManager.SetLanguage(
                config.Language);

            SelectLanguage(
                config.Language);

            /*
             * 通常処理のJSONファイルは、
             * 前回の選択を復元せず毎回空欄にします。
             */
            FilePathTextBox.Text =
                string.Empty;

            StatusTextBlock.Text =
                LanguageManager.GetString(
                    "Config_InitialFileStatus");

            if (!string.IsNullOrWhiteSpace(
                config.TestsFolderPath))
            {
                TestsFolderPathTextBox.Text =
                    config.TestsFolderPath;
            }

            SelectIndentSize(
                config.IndentSize);

            CreateBackupCheckBox.IsChecked =
                config.CreateBackup;

            if (!string.IsNullOrWhiteSpace(
                config.TestsFolderPath))
            {
                TestResultsTextBox.Text =
                    Directory.Exists(
                        config.TestsFolderPath)
                        ? $"{LanguageManager.GetString("Config_PreviousTestsFolderLabel")} " +
                          $"{config.TestsFolderPath}"
                        : LanguageManager.GetString(
                            "Config_PreviousTestsFolderMissing");
            }
        }

        /// <summary>
        /// 指定された言語を言語選択欄へ反映します。
        /// </summary>
        private void SelectLanguage(
            string language)
        {
            foreach (object item in LanguageComboBox.Items)
            {
                if (item is not ComboBoxItem comboBoxItem)
                {
                    continue;
                }

                string? itemLanguage =
                    comboBoxItem.Tag?.ToString();

                if (!string.Equals(
                    itemLanguage,
                    language,
                    StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                LanguageComboBox.SelectedItem =
                    comboBoxItem;

                return;
            }

            /*
             * 保存されている言語が見つからない場合は、
             * デフォルトの英語を選択します。
             */
            LanguageComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 言語選択が変更されたときに
        /// 表示言語を切り替えます。
        /// </summary>
        private void LanguageComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem
                is not ComboBoxItem selectedItem)
            {
                return;
            }

            string? language =
                selectedItem.Tag?.ToString();

            if (!LanguageManager.IsSupported(language))
            {
                return;
            }

            LanguageManager.SetLanguage(
                language!);
        }

        /// <summary>
        /// 指定されたインデント幅をコンボボックスで選択します。
        /// </summary>
        private void SelectIndentSize(
            int indentSize)
        {
            foreach (object item in IndentSizeComboBox.Items)
            {
                if (item is not ComboBoxItem comboBoxItem)
                {
                    continue;
                }

                string? itemText =
                    comboBoxItem.Content?.ToString();

                if (!int.TryParse(
                    itemText,
                    out int itemIndentSize))
                {
                    continue;
                }

                if (itemIndentSize == indentSize)
                {
                    IndentSizeComboBox.SelectedItem =
                        comboBoxItem;

                    return;
                }
            }

            /*
             * 設定に該当する値がない場合は、
             * 4スペースを選択します。
             */
            IndentSizeComboBox.SelectedIndex = 2;
        }

        /// <summary>
        /// メインウィンドウが閉じられるときに
        /// 現在の設定を保存します。
        /// </summary>
        private void Window_Closing(
            object? sender,
            CancelEventArgs e)
        {
            SaveAppConfig();
        }

        /// <summary>
        /// 現在の画面設定をconfig.jsonへ保存します。
        /// </summary>
        private void SaveAppConfig()
        {
            try
            {
                int indentSize = 4;

                if (!TryReadIndentSize(
                    out indentSize))
                {
                    indentSize = 4;
                }

                AppConfig config = new()
                {
                    TestsFolderPath =
                        TestsFolderPathTextBox.Text,

                    IndentSize =
                        indentSize,

                    CreateBackup =
                        CreateBackupCheckBox.IsChecked == true,

                    Language =
                        LanguageManager.CurrentLanguage
                };

                appConfigService.Save(
                    config);
            }
            catch (Exception exception)
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Config_SaveErrorMessage") +
                    "\n\n" +
                    exception.Message,
                    LanguageManager.GetString(
                        "Config_SaveErrorTitle"));
            }
        }

        /// <summary>
        /// 現在選択されているインデント幅を取得します。
        /// </summary>
        /// <remarks>
        /// 設定保存用なので、取得できなくても
        /// エラーメッセージは表示しません。
        /// </remarks>
        private bool TryReadIndentSize(
            out int indentSize)
        {
            indentSize = 0;

            if (IndentSizeComboBox.SelectedItem
                is not ComboBoxItem selectedItem)
            {
                return false;
            }

            string? selectedText =
                selectedItem.Content?.ToString();

            return int.TryParse(
                selectedText,
                out indentSize);
        }
    }
}