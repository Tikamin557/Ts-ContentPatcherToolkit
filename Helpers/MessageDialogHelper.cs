using System.Windows;

namespace TsCPToolKit.Helpers
{
    /// <summary>
    /// アプリケーション内で使用する
    /// メッセージダイアログを表示します。
    /// </summary>
    public static class MessageDialogHelper
    {
        /// <summary>
        /// 情報メッセージを表示します。
        /// </summary>
        public static void ShowInformation(
            string message,
            string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// 警告メッセージを表示します。
        /// </summary>
        public static void ShowWarning(
            string message,
            string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        public static void ShowError(
            string message,
            string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        /// <summary>
        /// Yes / Noの確認ダイアログを表示します。
        /// </summary>
        /// <returns>
        /// Yesが選択された場合はtrue。
        /// </returns>
        public static bool Confirm(
            string message,
            string title)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No);

            return result == MessageBoxResult.Yes;
        }
    }
}