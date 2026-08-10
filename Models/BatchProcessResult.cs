using System;
using System.Collections.Generic;

namespace TsCPToolKit.Models
{
    /// <summary>
    /// JSONファイル1件分の一括処理状態です。
    /// </summary>
    public enum BatchFileStatus
    {
        /// <summary>
        /// インデントを変更し、ファイルを書き換えました。
        /// </summary>
        Changed,

        /// <summary>
        /// すでに正しいインデントだったため変更しませんでした。
        /// </summary>
        Unchanged,

        /// <summary>
        /// 処理中にエラーが発生しました。
        /// </summary>
        Error
    }

    /// <summary>
    /// JSONファイル1件分の一括処理結果を保持します。
    /// </summary>
    public sealed class BatchFileResult
    {
        /// <summary>
        /// ファイル1件分の処理結果を作成します。
        /// </summary>
        public BatchFileResult(
            string filePath,
            BatchFileStatus status,
            string message,
            string? backupPath = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                filePath);

            ArgumentNullException.ThrowIfNull(message);

            FilePath = filePath;
            Status = status;
            Message = message;
            BackupPath = backupPath;
        }

        /// <summary>
        /// 処理対象ファイルのパスを取得します。
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// 処理状態を取得します。
        /// </summary>
        public BatchFileStatus Status { get; }

        /// <summary>
        /// 処理結果の説明を取得します。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 作成されたバックアップファイルのパスを取得します。
        /// </summary>
        public string? BackupPath { get; }
    }

    /// <summary>
    /// フォルダ一括処理全体の結果を保持します。
    /// </summary>
    public sealed class BatchProcessResult
    {
        /// <summary>
        /// 一括処理結果を作成します。
        /// </summary>
        public BatchProcessResult(
            IReadOnlyList<BatchFileResult> files)
        {
            ArgumentNullException.ThrowIfNull(files);

            Files = files;
        }

        /// <summary>
        /// ファイルごとの処理結果を取得します。
        /// </summary>
        public IReadOnlyList<BatchFileResult> Files { get; }

        /// <summary>
        /// 処理対象となったファイル数を取得します。
        /// </summary>
        public int TotalCount =>
            Files.Count;

        /// <summary>
        /// 書き換えたファイル数を取得します。
        /// </summary>
        public int ChangedCount =>
            CountStatus(BatchFileStatus.Changed);

        /// <summary>
        /// 変更不要だったファイル数を取得します。
        /// </summary>
        public int UnchangedCount =>
            CountStatus(BatchFileStatus.Unchanged);

        /// <summary>
        /// エラーになったファイル数を取得します。
        /// </summary>
        public int ErrorCount =>
            CountStatus(BatchFileStatus.Error);

        /// <summary>
        /// 指定された状態のファイル数を取得します。
        /// </summary>
        private int CountStatus(
            BatchFileStatus status)
        {
            int count = 0;

            foreach (BatchFileResult file in Files)
            {
                if (file.Status == status)
                {
                    count++;
                }
            }

            return count;
        }
    }
}