using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using TsCPToolKit.Helpers;
using TsCPToolKit.Models;
using TsCPToolKit.Localization;
using System.Diagnostics;

namespace TsCPToolKit
{
    /// <summary>
    /// MainWindowのフォルダ一括処理を担当します。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 一括処理するフォルダを選択します。
        /// </summary>
        private void BrowseBatchFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                Title =
                LanguageManager.GetString(
                    "Batch_BrowseDialogTitle"),
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            BatchFolderPathTextBox.Text =
                dialog.FolderName;

            BatchStatusTextBlock.Text =
                $"{LanguageManager.GetString("Status_SelectedFolder")} " +
                $"{dialog.FolderName}";
        }

        /// <summary>
        /// 選択されたフォルダ内のJSONファイルを
        /// 一括してインデント整理します。
        /// </summary>
        private void ProcessFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string folderPath =
                BatchFolderPathTextBox.Text;

            if (!ValidateBatchFolder(folderPath))
            {
                return;
            }

            if (!TryGetIndentSize(out int indentSize))
            {
                return;
            }

            bool includeSubfolders =
                IncludeSubfoldersCheckBox.IsChecked == true;

            bool createBackup =
                CreateBackupCheckBox.IsChecked == true;

            SearchOption searchOption =
                includeSubfolders
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

            int jsonFileCount;

            try
            {
                jsonFileCount =
                    Directory.GetFiles(
                        folderPath,
                        "*.json",
                        searchOption)
                    .Length;
            }
            catch (Exception exception)
            {
                MessageDialogHelper.ShowError(
                    LanguageManager.GetString(
                        "Batch_CheckFilesErrorMessage") +
                    "\n\n" +
                    exception.Message,
                    LanguageManager.GetString(
                        "Batch_CheckFilesErrorTitle"));

                return;
            }

            if (jsonFileCount == 0)
            {
                MessageDialogHelper.ShowInformation(
                    LanguageManager.GetString(
                        "Batch_NoJsonMessage"),
                    LanguageManager.GetString(
                        "Batch_NoJsonTitle"));

                BatchStatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Batch_NoJsonStatus");

                return;
            }

            string subfolderText =
                includeSubfolders
                    ? LanguageManager.GetString(
                        "Batch_SubfoldersIncluded")
                    : LanguageManager.GetString(
                        "Batch_CurrentFolderOnly");

            string backupText =
                createBackup
                    ? LanguageManager.GetString(
                        "Batch_BackupCreate")
                    : LanguageManager.GetString(
                        "Batch_BackupDoNotCreate");

            bool confirmed =
                MessageDialogHelper.Confirm(
                    $"{LanguageManager.GetString("Batch_ConfirmIntro")}\n\n" +
                    $"{LanguageManager.GetString("Batch_TargetFolderLabel")}\n" +
                    $"{folderPath}\n\n" +
                    $"{LanguageManager.GetString("Batch_TargetCountLabel")} " +
                    $"{jsonFileCount}\n" +
                    $"{LanguageManager.GetString("Batch_SearchRangeLabel")} " +
                    $"{subfolderText}\n" +
                    $"{LanguageManager.GetString("Batch_BackupLabel")} " +
                    $"{backupText}\n\n" +
                    $"{LanguageManager.GetString("Batch_OverwriteWarning")}\n" +
                    LanguageManager.GetString(
                        "Batch_ContinueQuestion"),
                    LanguageManager.GetString(
                        "Batch_ConfirmTitle"));

            if (!confirmed)
            {
                return;
            }

            try
            {
                SetProcessingState(
                    isProcessing: true,
                    statusText: null);

                BatchStatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Batch_ProcessingStatus");

                IndentOptions options =
                    CreateIndentOptions(indentSize);

                BatchProcessResult result =
                    jsonBatchProcessService.ProcessFolder(
                        folderPath,
                        options,
                        createBackup,
                        includeSubfolders);

                BatchStatusTextBlock.Text =
                    BuildBatchResultText(
                        folderPath,
                        result);

                ShowBatchCompletionMessage(result);
            }
            catch (Exception exception)
            {
                string message =
                    LanguageManager.GetString(
                        "Batch_ProcessErrorMessage");

                BatchStatusTextBlock.Text =
                    message + "\n\n" +
                    exception.Message;

                MessageDialogHelper.ShowError(
                    message + "\n\n" +
                    exception.Message,
                    LanguageManager.GetString(
                        "Batch_ProcessErrorTitle"));
            }
            finally
            {
                SetProcessingState(
                    isProcessing: false,
                    statusText: null);
            }
        }

        /// <summary>
        /// 一括処理用に選択されたフォルダを検証します。
        /// </summary>
        private bool ValidateBatchFolder(
     string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                MessageDialogHelper.ShowInformation(
                    LanguageManager.GetString(
                        "Batch_SelectFolderMessage"),
                    LanguageManager.GetString(
                        "Batch_SelectFolderTitle"));

                BatchStatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Batch_SelectFolderStatus");

                return false;
            }

            if (!Directory.Exists(folderPath))
            {
                MessageDialogHelper.ShowError(
                    LanguageManager.GetString(
                        "Batch_FolderNotFoundMessage"),
                    LanguageManager.GetString(
                        "Batch_FolderErrorTitle"));

                BatchStatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Batch_FolderNotFoundStatus");

                return false;
            }

            return true;
        }

        /// <summary>
        /// フォルダ一括処理の結果表示を作成します。
        /// </summary>
        private static string BuildBatchResultText(
            string folderPath,
            BatchProcessResult result)
        {
            StringBuilder text = new();

            text.AppendLine(
                LanguageManager.GetString(
                    "Batch_CompleteText"));

            text.AppendLine();

            text.AppendLine(
                $"{LanguageManager.GetString("Batch_TargetFolderLabel")} " +
                $"{folderPath}");

            text.AppendLine(
                $"{LanguageManager.GetString("Batch_TotalLabel")} " +
                $"{result.TotalCount}");

            text.AppendLine(
                $"{LanguageManager.GetString("Batch_ChangedLabel")} " +
                $"{result.ChangedCount}");

            text.AppendLine(
                $"{LanguageManager.GetString("Batch_UnchangedLabel")} " +
                $"{result.UnchangedCount}");

            text.AppendLine(
                $"{LanguageManager.GetString("Batch_ErrorLabel")} " +
                $"{result.ErrorCount}");

            if (result.ErrorCount == 0)
            {
                return text.ToString();
            }

            text.AppendLine();

            text.AppendLine(
                LanguageManager.GetString(
                    "Batch_ErrorFilesHeader"));

            foreach (BatchFileResult file in result.Files)
            {
                if (file.Status != BatchFileStatus.Error)
                {
                    continue;
                }

                text.AppendLine();
                text.AppendLine(file.FilePath);
                text.AppendLine($"  {file.Message}");
            }

            return text.ToString();
        }

        /// <summary>
        /// フォルダ一括処理の完了結果を表示します。
        /// </summary>
        private static void ShowBatchCompletionMessage(
            BatchProcessResult result)
        {
            string message =
                $"{LanguageManager.GetString("Batch_CompleteText")}\n\n" +
                $"{LanguageManager.GetString("Batch_TotalLabel")} " +
                $"{result.TotalCount}\n" +
                $"{LanguageManager.GetString("Batch_ChangedLabel")} " +
                $"{result.ChangedCount}\n" +
                $"{LanguageManager.GetString("Batch_UnchangedLabel")} " +
                $"{result.UnchangedCount}\n" +
                $"{LanguageManager.GetString("Batch_ErrorLabel")} " +
                $"{result.ErrorCount}";

            if (result.ErrorCount > 0)
            {
                message +=
                    "\n\n" +
                    LanguageManager.GetString(
                        "Batch_ErrorDetailsNotice");

                MessageDialogHelper.ShowWarning(
                    message,
                    LanguageManager.GetString(
                        "Batch_CompleteTitle"));

                return;
            }

            MessageDialogHelper.ShowInformation(
                message,
                LanguageManager.GetString(
                    "Batch_CompleteTitle"));
        }

        /// <summary>
        /// 現在参照している一括処理フォルダを
        /// エクスプローラーで開きます。
        /// </summary>
        private void OpenBatchFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string folderPath =
                BatchFolderPathTextBox.Text;

            if (string.IsNullOrWhiteSpace(
                folderPath)
                || !Directory.Exists(
                    folderPath))
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Batch_OpenFolderMissing"),
                    LanguageManager.GetString(
                        "Batch_OpenFolderErrorTitle"));

                return;
            }

            Process.Start(
                new ProcessStartInfo
                {
                    FileName =
                        folderPath,

                    UseShellExecute =
                        true
                });
        }
    }
}