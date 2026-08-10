using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TsCPToolKit.Models;
using TsCPToolKit.Helpers;
using TsCPToolKit.Localization;
using System.Diagnostics;

namespace TsCPToolKit
{
    /// <summary>
    /// MainWindowのテスト実行処理を担当します。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Testsフォルダを選択します。
        /// </summary>
        private void BrowseTestsFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                Title =
                    LanguageManager.GetString(
                        "Test_BrowseDialogTitle"),
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            TestsFolderPathTextBox.Text =
                dialog.FolderName;

            TestResultsTextBox.Text =
                $"{LanguageManager.GetString("Status_SelectedTestsFolder")} " +
                $"{dialog.FolderName}";
        }

        /// <summary>
        /// Input内のJSONを一括処理し、
        /// OutputとExpectedを比較します。
        /// </summary>
        private void RunTestsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string testsRootPath =
                TestsFolderPathTextBox.Text;

            if (!ValidateTestsFolder(testsRootPath))
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
                    statusText: null);

                TestResultsTextBox.Text =
                    LanguageManager.GetString(
                        "Test_RunningStatus");

                IndentOptions options =
                    CreateIndentOptions(indentSize);

                IReadOnlyList<TestCaseResult> results =
                    jsonIndentTestService.RunTests(
                        testsRootPath,
                        options);

                TestResultsTextBox.Text =
                    BuildTestResultText(results);

                ShowTestCompletionMessage(results);
            }
            catch (Exception exception)
            {
                string message =
                    LanguageManager.GetString(
                        "Test_RunErrorMessage");

                TestResultsTextBox.Text =
                    message + "\r\n\r\n" +
                    exception.Message;

                MessageDialogHelper.ShowError(
                    message + "\n\n" +
                    exception.Message,
                    LanguageManager.GetString(
                        "Test_RunErrorTitle"));
            }
            finally
            {
                SetProcessingState(
                    isProcessing: false,
                    statusText: null);
            }
        }

        /// <summary>
        /// Outputフォルダ内のJSONを
        /// Expectedフォルダへコピーします。
        /// </summary>
        private void UpdateExpectedButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string testsRootPath =
                TestsFolderPathTextBox.Text;

            if (!ValidateTestsFolder(testsRootPath))
            {
                return;
            }

            string outputDirectory =
                Path.Combine(
                    testsRootPath,
                    "Output");

            if (!Directory.Exists(outputDirectory))
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Test_OutputFolderMissingMessage") +
                    "\n\n" +
                    LanguageManager.GetString(
                        "Test_RunFirstMessage"),
                    LanguageManager.GetString(
                        "Test_OutputFolderMissingTitle"));

                return;
            }

            int outputFileCount =
                Directory.GetFiles(
                    outputDirectory,
                    "*.json",
                    SearchOption.TopDirectoryOnly)
                .Length;

            if (outputFileCount == 0)
            {
                MessageDialogHelper.ShowInformation(
                    LanguageManager.GetString(
                        "Test_NoOutputFilesMessage") +
                    "\n\n" +
                    LanguageManager.GetString(
                        "Test_RunFirstMessage"),
                    LanguageManager.GetString(
                        "Test_NoFilesToCopyTitle"));

                return;
            }

            bool confirmed =
                MessageDialogHelper.Confirm(
                    LanguageManager.GetString(
                        "Test_UpdateExpectedConfirmIntro") +
                    "\n\n" +
                    LanguageManager.GetString(
                        "Test_TargetFileCountLabel") +
                    " " +
                    outputFileCount +
                    "\n\n" +
                    LanguageManager.GetString(
                        "Test_UpdateExpectedOverwriteWarning") +
                    "\n" +
                    LanguageManager.GetString(
                        "Test_ContinueQuestion"),
                    LanguageManager.GetString(
                        "Test_UpdateExpectedConfirmTitle"));

            if (!confirmed)
            {
                return;
            }

            try
            {
                SetProcessingState(
                    isProcessing: true,
                    statusText: null);

                int copiedCount =
                    jsonIndentTestService.UpdateExpectedFiles(
                        testsRootPath);

                TestResultsTextBox.Text =
                    $"{copiedCount}" +
                    LanguageManager.GetString(
                        "Test_UpdateExpectedResultPart1") +
                    "\r\n\r\n" +
                    LanguageManager.GetString(
                        "Test_UpdateExpectedResultPart2");

                MessageDialogHelper.ShowInformation(
                    $"{copiedCount}" +
                    LanguageManager.GetString(
                        "Test_UpdateExpectedCompletePart"),
                    LanguageManager.GetString(
                        "Test_UpdateExpectedCompleteTitle"));
            }
            catch (Exception exception)
            {
                string message =
                    LanguageManager.GetString(
                        "Test_UpdateExpectedErrorMessage");

                TestResultsTextBox.Text =
                    message + "\r\n\r\n" +
                    exception.Message;

                MessageDialogHelper.ShowError(
                    message + "\n\n" +
                    exception.Message,
                    LanguageManager.GetString(
                        "Test_UpdateExpectedErrorTitle"));
            }
            finally
            {
                SetProcessingState(
                    isProcessing: false,
                    statusText: null);
            }
        }

        /// <summary>
        /// テスト結果の表示テキストを作成します。
        /// </summary>
        private static string BuildTestResultText(
            IReadOnlyList<TestCaseResult> results)
        {
            if (results.Count == 0)
            {
                return LanguageManager.GetString(
                    "Test_NoInputFiles");
            }

            int passedCount =
                results.Count(
                    result => result.Status == TestCaseStatus.Passed);

            int failedCount =
                results.Count(
                    result => result.Status == TestCaseStatus.Failed);

            int missingCount =
                results.Count(
                    result => result.Status == TestCaseStatus.ExpectedMissing);

            int errorCount =
                results.Count(
                    result => result.Status == TestCaseStatus.Error);

            StringBuilder text = new();

            text.AppendLine(
                $"{LanguageManager.GetString("Test_CountLabel")} " +
                $"{results.Count}");

            text.AppendLine(
                $"{LanguageManager.GetString("Test_PassLabel")} " +
                $"{passedCount}");

            text.AppendLine(
                $"{LanguageManager.GetString("Test_FailLabel")} " +
                $"{failedCount}");

            text.AppendLine(
                $"{LanguageManager.GetString("Test_MissingLabel")} " +
                $"{missingCount}");

            text.AppendLine(
                $"{LanguageManager.GetString("Test_ErrorLabel")} " +
                $"{errorCount}");

            text.AppendLine();
            text.AppendLine("----------------------------------------");
            text.AppendLine();

            foreach (TestCaseResult result in results)
            {
                string statusText =
                    result.Status switch
                    {
                        TestCaseStatus.Passed =>
                            "PASS",

                        TestCaseStatus.Failed =>
                            "FAIL",

                        TestCaseStatus.ExpectedMissing =>
                            "MISSING",

                        TestCaseStatus.Error =>
                            "ERROR",

                        _ =>
                            "UNKNOWN"
                    };

                text.AppendLine(
                    $"[{statusText}] {result.FileName}");

                text.AppendLine(
                    $"  {result.Message}");

                text.AppendLine();
            }

            return text.ToString();
        }

        /// <summary>
        /// テスト実行の完了結果をウィンドウで表示します。
        /// </summary>
        private static void ShowTestCompletionMessage(
            IReadOnlyList<TestCaseResult> results)
        {
            int passedCount =
                results.Count(
                    result => result.Status == TestCaseStatus.Passed);

            int failedCount =
                results.Count(
                    result => result.Status == TestCaseStatus.Failed);

            int missingCount =
                results.Count(
                    result => result.Status == TestCaseStatus.ExpectedMissing);

            int errorCount =
                results.Count(
                    result => result.Status == TestCaseStatus.Error);

            StringBuilder message = new();

            message.AppendLine(
                LanguageManager.GetString(
                    "Test_CompleteMessage"));

            message.AppendLine();

            message.AppendLine(
                $"{LanguageManager.GetString("Test_CountLabel")} " +
                $"{results.Count}");

            message.AppendLine(
                $"{LanguageManager.GetString("Test_PassLabel")} " +
                $"{passedCount}");

            message.AppendLine(
                $"{LanguageManager.GetString("Test_FailLabel")} " +
                $"{failedCount}");

            message.AppendLine(
                $"{LanguageManager.GetString("Test_MissingLabel")} " +
                $"{missingCount}");

            message.AppendLine(
                $"{LanguageManager.GetString("Test_ErrorLabel")} " +
                $"{errorCount}");

            /*
             * FAILがある場合は、
             * 最初に失敗したテストの差分を表示します。
             */
            TestCaseResult? firstFailed =
                results.FirstOrDefault(
                    result =>
                        result.Status == TestCaseStatus.Failed);

            if (firstFailed is not null)
            {
                message.AppendLine();

                message.AppendLine(
                    "----------------------------------------");

                message.AppendLine();

                message.AppendLine(
                    $"{LanguageManager.GetString("Test_FailPrefix")} " +
                    $"{firstFailed.FileName}");

                message.AppendLine();

                message.Append(
                    firstFailed.Message);

                if (failedCount > 1)
                {
                    message.AppendLine();
                    message.AppendLine();

                    message.AppendLine(
                        LanguageManager.GetString(
                            "Test_MoreFailuresPart1") +
                        $"{failedCount - 1}" +
                        LanguageManager.GetString(
                            "Test_MoreFailuresPart2"));

                    message.Append(
                        LanguageManager.GetString(
                            "Test_SeeResults"));
                }
            }

            /*
             * FAILがなくERRORだけある場合は、
             * 最初のエラー内容を表示します。
             */
            if (firstFailed is null)
            {
                TestCaseResult? firstError =
                    results.FirstOrDefault(
                        result =>
                            result.Status == TestCaseStatus.Error);

                if (firstError is not null)
                {
                    message.AppendLine();

                    message.AppendLine(
                        "----------------------------------------");

                    message.AppendLine();

                    message.AppendLine(
                        $"{LanguageManager.GetString("Test_ErrorPrefix")} " +
                        $"{firstError.FileName}");

                    message.AppendLine();

                    message.Append(
                        firstError.Message);

                    if (errorCount > 1)
                    {
                        message.AppendLine();
                        message.AppendLine();

                        message.AppendLine(
                            LanguageManager.GetString(
                                "Test_MoreErrorsPart1") +
                            $"{errorCount - 1}" +
                            LanguageManager.GetString(
                                "Test_MoreErrorsPart2"));

                        message.Append(
                            LanguageManager.GetString(
                                "Test_SeeResults"));
                    }
                }
            }

            string title =
                LanguageManager.GetString(
                    "Test_CompleteTitle");

            if (failedCount > 0 || errorCount > 0)
            {
                MessageDialogHelper.ShowWarning(
                    message.ToString(),
                    title);
            }
            else
            {
                MessageDialogHelper.ShowInformation(
                    message.ToString(),
                    title);
            }
        }

        /// <summary>
        /// 選択されたTestsフォルダを検証します。
        /// </summary>
        private bool ValidateTestsFolder(
     string testsRootPath)
        {
            if (string.IsNullOrWhiteSpace(testsRootPath))
            {
                MessageDialogHelper.ShowInformation(
                    LanguageManager.GetString(
                        "Test_SelectFolderMessage"),
                    LanguageManager.GetString(
                        "Test_SelectFolderTitle"));

                return false;
            }

            if (!Directory.Exists(testsRootPath))
            {
                MessageDialogHelper.ShowError(
                    LanguageManager.GetString(
                        "Test_FolderNotFoundMessage"),
                    LanguageManager.GetString(
                        "Test_FolderErrorTitle"));

                return false;
            }

            string inputDirectory =
                Path.Combine(
                    testsRootPath,
                    "Input");

            if (!Directory.Exists(inputDirectory))
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Test_InputFolderMissingMessage") +
                    "\n\n" +
                    LanguageManager.GetString(
                        "Test_InputFolderHelp"),
                    LanguageManager.GetString(
                        "Test_InputFolderMissingTitle"));

                return false;
            }

            return true;
        }

        /// <summary>
        /// 現在参照しているTestsフォルダを
        /// エクスプローラーで開きます。
        /// </summary>
        private void OpenTestsFolderButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string testsFolderPath =
                TestsFolderPathTextBox.Text;

            if (string.IsNullOrWhiteSpace(
                testsFolderPath)
                || !Directory.Exists(
                    testsFolderPath))
            {
                MessageDialogHelper.ShowWarning(
                    LanguageManager.GetString(
                        "Test_OpenFolderMissing"),
                    LanguageManager.GetString(
                        "Test_OpenFolderErrorTitle"));

                return;
            }

            Process.Start(
                new ProcessStartInfo
                {
                    FileName =
                        testsFolderPath,

                    UseShellExecute =
                        true
                });
        }
    }
}