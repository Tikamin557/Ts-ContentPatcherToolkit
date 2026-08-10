using System.Windows;
using System.Windows.Controls;
using TsCPToolKit.Helpers;
using TsCPToolKit.Models;
using TsCPToolKit.Localization;

namespace TsCPToolKit
{
    /// <summary>
    /// MainWindowで共通して使用するUI処理を担当します。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 現在の設定値から、
        /// JSONインデント整理用の設定を作成します。
        /// </summary>
        private static IndentOptions CreateIndentOptions(
            int indentSize)
        {
            return new IndentOptions
            {
                IndentSize = indentSize,
                TrimBlankLines = true
            };
        }

        /// <summary>
        /// 画面で選択されているインデント幅を取得します。
        /// </summary>
        private bool TryGetIndentSize(
            out int indentSize)
        {
            indentSize = 0;

            if (IndentSizeComboBox.SelectedItem
                is not ComboBoxItem selectedItem)
            {
                ShowIndentSizeError();
                return false;
            }

            string? selectedText =
                selectedItem.Content?.ToString();

            if (!int.TryParse(
                selectedText,
                out indentSize))
            {
                ShowIndentSizeError();
                return false;
            }

            if (indentSize < 0)
            {
                ShowIndentSizeError();
                return false;
            }

            return true;
        }

        /// <summary>
        /// インデント幅の設定エラーを表示します。
        /// </summary>
        private void ShowIndentSizeError()
        {
            MessageDialogHelper.ShowWarning(
                LanguageManager.GetString(
                    "Common_InvalidIndentSizeMessage"),
                LanguageManager.GetString(
                    "Common_SettingsErrorTitle"));

            StatusTextBlock.Text =
                LanguageManager.GetString(
                    "Common_CheckIndentSizeStatus");
        }

        /// <summary>
        /// 処理中かどうかに応じて、
        /// 画面上の操作可否を切り替えます。
        /// </summary>
        private void SetProcessingState(
            bool isProcessing,
            string? statusText)
        {
            bool isEnabled =
                !isProcessing;

            BrowseButton.IsEnabled =
                isEnabled;

            ProcessButton.IsEnabled =
                isEnabled;

            OpenSingleFolderButton.IsEnabled =
                isEnabled;

            IndentSizeComboBox.IsEnabled =
                isEnabled;

            LanguageComboBox.IsEnabled =
                isEnabled;

            CreateBackupCheckBox.IsEnabled =
                isEnabled;

            BrowseBatchFolderButton.IsEnabled =
                isEnabled;

            ProcessFolderButton.IsEnabled =
                isEnabled;

            OpenBatchFolderButton.IsEnabled =
                isEnabled;

            IncludeSubfoldersCheckBox.IsEnabled =
                isEnabled;

            BrowseTestsFolderButton.IsEnabled =
                isEnabled;

            RunTestsButton.IsEnabled =
                isEnabled;

            UpdateExpectedButton.IsEnabled =
                isEnabled;

            OpenTestsFolderButton.IsEnabled =
                isEnabled;

            if (statusText is not null)
            {
                StatusTextBlock.Text =
                    statusText;
            }
        }
    }
}