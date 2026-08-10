using System;
using System.IO;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Helpers
{
    /// <summary>
    /// 処理対象ファイルのバックアップを作成します。
    /// </summary>
    public static class BackupHelper
    {
        /// <summary>
        /// 元ファイルと同じフォルダーに
        /// 重複しない名前でバックアップを作成します。
        /// </summary>
        /// <returns>
        /// 作成されたバックアップファイルのパス。
        /// </returns>
        public static string CreateBackup(
            string filePath)
        {
            ValidateFilePath(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    LanguageManager.GetString(
                        "Internal_BackupSourceNotFound"),
                    filePath);
            }

            string backupPath =
                GetAvailableBackupPath(filePath);

            File.Copy(
                filePath,
                backupPath,
                overwrite: false);

            return backupPath;
        }

        /// <summary>
        /// 既存ファイルと重複しない
        /// バックアップ先を取得します。
        /// </summary>
        private static string GetAvailableBackupPath(
            string filePath)
        {
            string firstBackupPath =
                filePath + ".bak";

            if (!File.Exists(firstBackupPath))
            {
                return firstBackupPath;
            }

            int number = 2;

            while (true)
            {
                string numberedBackupPath =
                    $"{filePath}.bak{number}";

                if (!File.Exists(numberedBackupPath))
                {
                    return numberedBackupPath;
                }

                number++;
            }
        }

        /// <summary>
        /// ファイルパスの基本的な検証を行います。
        /// </summary>
        private static void ValidateFilePath(
            string filePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                filePath);
        }
    }
}