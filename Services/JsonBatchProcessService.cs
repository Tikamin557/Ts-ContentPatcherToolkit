using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TsCPToolKit.Helpers;
using TsCPToolKit.Models;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Services
{
    /// <summary>
    /// フォルダ内のJSONファイルを一括して処理します。
    /// </summary>
    public sealed class JsonBatchProcessService
    {
        private readonly JsonIndentService jsonIndentService =
            new();

        /// <summary>
        /// 指定されたフォルダ内のJSONファイルを
        /// 一括してインデント整理します。
        /// </summary>
        /// <param name="folderPath">
        /// 処理対象フォルダ。
        /// </param>
        /// <param name="options">
        /// インデント整理の設定。
        /// </param>
        /// <param name="createBackup">
        /// 変更前にバックアップを作成するかどうか。
        /// </param>
        /// <param name="includeSubfolders">
        /// サブフォルダ内のJSONも処理するかどうか。
        /// </param>
        public BatchProcessResult ProcessFolder(
            string folderPath,
            IndentOptions options,
            bool createBackup,
            bool includeSubfolders)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                folderPath);

            ArgumentNullException.ThrowIfNull(options);

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(
                    LanguageManager.GetString(
                        "BatchService_FolderNotFound") +
                    "\n\n" +
                    folderPath);
            }

            SearchOption searchOption =
                includeSubfolders
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

            string[] jsonFiles =
                Directory.GetFiles(
                    folderPath,
                    "*.json",
                    searchOption)
                .OrderBy(
                    path => path,
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();

            List<BatchFileResult> results =
                new();

            foreach (string filePath in jsonFiles)
            {
                BatchFileResult result =
                    ProcessSingleFile(
                        filePath,
                        options,
                        createBackup);

                results.Add(result);
            }

            return new BatchProcessResult(results);
        }

        /// <summary>
        /// JSONファイル1件を処理します。
        /// </summary>
        private BatchFileResult ProcessSingleFile(
            string filePath,
            IndentOptions options,
            bool createBackup)
        {
            try
            {
                TextFileData fileData =
                    FileHelper.ReadTextFile(filePath);

                string formattedContent =
                    jsonIndentService.Format(
                        fileData.Content,
                        options);

                if (formattedContent == fileData.Content)
                {
                    return new BatchFileResult(
                        filePath,
                        BatchFileStatus.Unchanged,
                        LanguageManager.GetString(
                            "BatchService_AlreadyFormatted"));
                }

                string? backupPath = null;

                if (createBackup)
                {
                    backupPath =
                        BackupHelper.CreateBackup(
                            filePath);
                }

                FileHelper.WriteTextFile(
                    filePath,
                    formattedContent,
                    fileData);

                return new BatchFileResult(
                    filePath,
                    BatchFileStatus.Changed,
                    LanguageManager.GetString(
                        "BatchService_Formatted"),
                    backupPath);
            }
            catch (Exception exception)
            {
                /*
                 * 1件でエラーが発生しても、
                 * 残りのJSONファイルは処理を続行します。
                 */
                return new BatchFileResult(
                    filePath,
                    BatchFileStatus.Error,
                    exception.Message);
            }
        }
    }
}