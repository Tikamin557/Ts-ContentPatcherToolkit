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
    /// MainWindowの単一JSONファイル処理を担当します。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 処理対象のJSONファイルを選択します。
        /// </summary>
        private void BrowseButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Title =
                    LanguageManager.GetString(
                        "Single_BrowseDialogTitle"),

                Filter =
                    "JSON files (*.json)|*.json|" +
                    "All files (*.*)|*.*",

                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            FilePathTextBox.Text =
                dialog.FileName;

            StatusTextBlock.Text =
                $"{LanguageManager.GetString("Status_SelectedFile")} " +
                $"{Path.GetFileName(dialog.FileName)}";
        }

        /// <summary>
        /// 選択されたJSONファイルのインデントを整理します。
        /// </summary>
        private void ProcessButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string filePath =
                FilePathTextBox.Text;

            if (!ValidateSelectedFile(filePath))
            {
                return;
            }

            if (!TryGetIndentSize(out int indentSize))
            {
                return;
            }

            try
            {
                SetProcessingState(
                    isProcessing: true,
                    statusText: LanguageManager.GetString(
                        "Single_ProcessingStatus"));

                TextFileData fileData =
                    FileHelper.ReadTextFile(filePath);

                IndentOptions options =
                    CreateIndentOptions(indentSize);

                string formattedContent =
                    jsonIndentService.Format(
                        fileData.Content,
                        options);

                if (formattedContent == fileData.Content)
                {
                    StatusTextBlock.Text =
                        LanguageManager.GetString(
                            "Single_NoChangeStatus");

                    MessageDialogHelper.ShowInformation(
                        LanguageManager.GetString(
                            "Single_NoChangeMessage"),
                        LanguageManager.GetString(
                            "Single_NoChangeTitle"));

                    return;
                }

                string? backupPath = null;

                if (CreateBackupCheckBox.IsChecked == true)
                {
                    backupPath =
                        BackupHelper.CreateBackup(filePath);
                }

                FileHelper.WriteTextFile(
                    filePath,
                    formattedContent,
                    fileData);

                ShowCompletionMessage(
                    filePath,
                    backupPath);
            }
            catch (DecoderFallbackException exception)
            {
                ShowError(
                    LanguageManager.GetString(
                        "Single_EncodingErrorTitle"),
                    LanguageManager.GetString(
                        "Single_EncodingErrorMessage"),
                    exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                ShowError(
                    LanguageManager.GetString(
                        "Single_AccessErrorTitle"),
                    LanguageManager.GetString(
                        "Single_AccessErrorMessage"),
                    exception);
            }
            catch (IOException exception)
            {
                ShowError(
                    LanguageManager.GetString(
                        "Single_FileErrorTitle"),
                    LanguageManager.GetString(
                        "Single_ReadWriteErrorMessage"),
                    exception);
            }
            catch (Exception exception)
            {
                ShowError(
                    LanguageManager.GetString(
                        "Single_UnexpectedErrorTitle"),
                    LanguageManager.GetString(
                        "Single_UnexpectedErrorMessage"),
                    exception);
            }
            finally
            {
                SetProcessingState(
                    isProcessing: false,
                    statusText: null);
            }
        }

        /// <summary>
        /// 選択された通常処理用ファイルを検証します。
        /// </summary>
        private bool ValidateSelectedFile(
            string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageDialogHelper.ShowInformation(
                    LanguageManager.GetString(
                        "Single_SelectFileMessage"),
                    LanguageManager.GetString(
                        "Single_SelectFileTitle"));

                StatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Single_SelectFileStatus");

                return false;
            }

            if (!File.Exists(filePath))
            {
                MessageDialogHelper.ShowError(
                    LanguageManager.GetString(
                        "Single_FileNotFoundMessage"),
                    LanguageManager.GetString(
                        "Single_FileErrorTitle"));

                StatusTextBlock.Text =
                    LanguageManager.GetString(
                        "Single_FileNotFoundStatus");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 通常処理の完了メッセージを表示します。
        /// </summary>
        private void ShowCompletionMessage(
            string filePath,
            string? backupPath)
        {
            string fileName =
                Path.GetFileName(filePath);

            string completeTitle =
                LanguageManager.GetString(
                    "Single_CompleteTitle");

            string completeMessage =
                LanguageManager.GetString(
                    "Single_CompleteMessage");

            string processedFileLabel =
                LanguageManager.GetString(
                    "Single_ProcessedFileLabel");

            string backupLabel =
                LanguageManager.GetString(
                    "Single_BackupLabel");

            string completeStatus =
                LanguageManager.GetString(
                    "Single_CompleteStatus");

            if (backupPath is null)
            {
                StatusTextBlock.Text =
                    $"{completeStatus} {fileName}";

                MessageDialogHelper.ShowInformation(
                    $"{completeMessage}\n\n" +
                    $"{processedFileLabel} {fileName}",
                    completeTitle);

                return;
            }

            string backupFileName =
                Path.GetFileName(backupPath);

            StatusTextBlock.Text =
                $"{completeStatus} {fileName}\n" +
                $"{backupLabel} {backupFileName}";

            MessageDialogHelper.ShowInformation(
                $"{completeMessage}\n\n" +
                $"{processedFileLabel} {fileName}\n" +
                $"{backupLabel} {backupFileName}",
                completeTitle);
        }

        /// <summary>
        /// 通常処理のエラーを表示します。
        /// </summary>
        private void ShowError(
            string title,
            string message,
            Exception exception)
        {
            StatusTextBlock.Text =
                message;

            MessageDialogHelper.ShowError(
                message + "\n\n" + exception.Message,
                title);
        }

        /// <summary>
        /// 現在参照しているJSONファイルがあるフォルダを
        /// エクスプローラーで開きます。
        /// </summary>
        private void OpenSingleFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string filePath =
                FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(
                filePath))
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Single_OpenFolderMissing"),
                    LanguageManager.GetString(
                        "Single_OpenFolderErrorTitle"));

                return;
            }

            string? folderPath =
                Path.GetDirectoryName(
                    filePath);

            if (string.IsNullOrWhiteSpace(
                folderPath)
                || !Directory.Exists(
                    folderPath))
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Single_OpenFolderMissing"),
                    LanguageManager.GetString(
                        "Single_OpenFolderErrorTitle"));

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