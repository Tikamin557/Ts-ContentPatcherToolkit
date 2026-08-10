using System;
using System.IO;
using System.Windows;
using TsCPToolKit.Helpers;
using TsCPToolKit.Localization;

namespace TsCPToolKit
{
    /// <summary>
    /// MainWindowのドラッグ＆ドロップ処理を担当します。
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// ドラッグ＆ドロップされたパスの種類です。
        /// </summary>
        private enum DroppedPathType
        {
            /// <summary>
            /// 処理対象外です。
            /// </summary>
            None,

            /// <summary>
            /// JSONファイルです。
            /// </summary>
            JsonFile,

            /// <summary>
            /// フォルダです。
            /// </summary>
            Folder
        }

        /// <summary>
        /// ファイルまたはフォルダがウィンドウ上へ
        /// ドラッグされたときに、ドロップ可能か判定します。
        /// </summary>
        private void Window_DragEnter(
            object sender,
            DragEventArgs e)
        {
            if (TryGetDroppedPath(
                e.Data,
                out _,
                out _))
            {
                e.Effects =
                    DragDropEffects.Copy;
            }
            else
            {
                e.Effects =
                    DragDropEffects.None;
            }

            e.Handled = true;
        }

        /// <summary>
        /// ウィンドウへドロップされたJSONファイルまたは
        /// フォルダを、対応する処理対象として設定します。
        /// </summary>
        private void Window_Drop(
            object sender,
            DragEventArgs e)
        {
            if (!TryGetDroppedPath(
                e.Data,
                out string? droppedPath,
                out DroppedPathType pathType))
            {
                MessageDialogHelper.ShowInformation(
                    LanguageManager.GetString(
                        "DragDrop_InvalidMessage"),
                    LanguageManager.GetString(
                        "DragDrop_InvalidTitle"));

                return;
            }

            if (pathType == DroppedPathType.JsonFile)
            {
                FilePathTextBox.Text =
                    droppedPath;

                StatusTextBlock.Text =
                    $"{LanguageManager.GetString("DragDrop_FileStatus")} " +
                    $"{Path.GetFileName(droppedPath)}";

                return;
            }

            if (pathType == DroppedPathType.Folder)
            {
                BatchFolderPathTextBox.Text =
                    droppedPath;

                BatchStatusTextBlock.Text =
                    $"{LanguageManager.GetString("DragDrop_FolderStatus")} " +
                    $"{droppedPath}";
            }
        }

        /// <summary>
        /// ドラッグ＆ドロップされたデータから、
        /// JSONファイルまたはフォルダのパスを取得します。
        /// </summary>
        private static bool TryGetDroppedPath(
            IDataObject data,
            out string? droppedPath,
            out DroppedPathType pathType)
        {
            droppedPath = null;
            pathType = DroppedPathType.None;

            if (!data.GetDataPresent(
                DataFormats.FileDrop))
            {
                return false;
            }

            if (data.GetData(
                DataFormats.FileDrop)
                is not string[] droppedPaths)
            {
                return false;
            }

            /*
             * 現在は1件ずつのドロップだけを受け付けます。
             */
            if (droppedPaths.Length != 1)
            {
                return false;
            }

            string path =
                droppedPaths[0];

            if (File.Exists(path))
            {
                string extension =
                    Path.GetExtension(path);

                if (!string.Equals(
                    extension,
                    ".json",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                droppedPath = path;
                pathType = DroppedPathType.JsonFile;

                return true;
            }

            if (Directory.Exists(path))
            {
                droppedPath = path;
                pathType = DroppedPathType.Folder;

                return true;
            }

            return false;
        }
    }
}