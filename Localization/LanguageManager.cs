using System;
using System.Linq;
using System.Windows;

namespace TsCPToolKit.Localization
{
    /// <summary>
    /// アプリケーションで使用する表示言語を管理します。
    /// </summary>
    public static class LanguageManager
    {
        /// <summary>
        /// 英語の言語コードです。
        /// </summary>
        public const string English =
            "en";

        /// <summary>
        /// 日本語の言語コードです。
        /// </summary>
        public const string Japanese =
            "ja";

        /// <summary>
        /// デフォルト言語です。
        /// </summary>
        public const string DefaultLanguage =
            English;

        /// <summary>
        /// 現在使用している言語コードを取得します。
        /// </summary>
        public static string CurrentLanguage { get; private set; } =
            DefaultLanguage;

        /// <summary>
        /// 指定された言語へ表示言語を切り替えます。
        /// </summary>
        public static void SetLanguage(
            string language)
        {
            string normalizedLanguage =
                NormalizeLanguage(language);

            ResourceDictionary dictionary = new()
            {
                Source = new Uri(
                    $"/Localization/Strings.{normalizedLanguage}.xaml",
                    UriKind.Relative)
            };

            ResourceDictionary? currentDictionary =
                Application.Current.Resources
                    .MergedDictionaries
                    .FirstOrDefault(
                        IsLanguageDictionary);

            if (currentDictionary is not null)
            {
                Application.Current.Resources
                    .MergedDictionaries
                    .Remove(currentDictionary);
            }

            Application.Current.Resources
                .MergedDictionaries
                .Add(dictionary);

            CurrentLanguage =
                normalizedLanguage;
        }

        /// <summary>
        /// 指定された言語コードが利用可能かを取得します。
        /// </summary>
        public static bool IsSupported(
            string? language)
        {
            return string.Equals(
                    language,
                    English,
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    language,
                    Japanese,
                    StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 未対応の言語コードをデフォルト言語へ補正します。
        /// </summary>
        private static string NormalizeLanguage(
            string? language)
        {
            if (string.Equals(
                language,
                Japanese,
                StringComparison.OrdinalIgnoreCase))
            {
                return Japanese;
            }

            return English;
        }

        /// <summary>
        /// ResourceDictionaryが言語辞書かを判定します。
        /// </summary>
        private static bool IsLanguageDictionary(
            ResourceDictionary dictionary)
        {
            string? source =
                dictionary.Source?.OriginalString;

            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }

            return source.Contains(
                "/Localization/Strings.",
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 現在の表示言語から、指定されたキーの文字列を取得します。
        /// </summary>
        /// <param name="key">
        /// ResourceDictionaryに登録されているキー。
        /// </param>
        /// <returns>
        /// 対応する文字列。
        /// 見つからない場合はキー名をそのまま返します。
        /// </returns>
        public static string GetString(
            string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            object? resource =
                Application.Current.TryFindResource(key);

            if (resource is string text)
            {
                return text;
            }

            return key;
        }
    }
}