using System;
using System.IO;
using System.Text;
using TsCPToolKit.Models;
using TsCPToolKit.Utilities;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Helpers
{
    /// <summary>
    /// テキストファイルの読み込みと書き込みを担当します。
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// ファイルを読み込み、
        /// 本文と文字コード情報を取得します。
        /// </summary>
        public static TextFileData ReadTextFile(
            string filePath)
        {
            ValidateFilePath(filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    LanguageManager.GetString(
                        "Internal_FileNotFound"),
                    filePath);
            }

            byte[] bytes =
                File.ReadAllBytes(filePath);

            EncodingDetectionResult encodingResult =
                EncodingDetector.Detect(bytes);

            string content =
                encodingResult.Encoding.GetString(
                    bytes,
                    encodingResult.PreambleLength,
                    bytes.Length
                        - encodingResult.PreambleLength);

            return new TextFileData(
                content,
                encodingResult.Encoding,
                encodingResult.HasByteOrderMark);
        }

        /// <summary>
        /// 元ファイルの文字コード情報を維持して、
        /// ファイルへ本文を書き込みます。
        /// </summary>
        public static void WriteTextFile(
            string filePath,
            string content,
            TextFileData fileData)
        {
            ValidateFilePath(filePath);
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(fileData);

            Encoding outputEncoding =
                EncodingHelper.CreateOutputEncoding(
                    fileData.Encoding,
                    fileData.HasByteOrderMark);

            byte[] contentBytes =
                outputEncoding.GetBytes(content);

            byte[] preamble =
                fileData.HasByteOrderMark
                    ? outputEncoding.GetPreamble()
                    : Array.Empty<byte>();

            using FileStream stream = new(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

            if (preamble.Length > 0)
            {
                stream.Write(preamble);
            }

            stream.Write(contentBytes);
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