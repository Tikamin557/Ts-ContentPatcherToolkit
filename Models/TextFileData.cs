using System;
using System.Text;

namespace TsCPToolKit.Models
{
    /// <summary>
    /// 読み込んだテキストファイルの内容と
    /// 文字コード情報を保持します。
    /// </summary>
    public sealed class TextFileData
    {
        /// <summary>
        /// テキストファイルの情報を作成します。
        /// </summary>
        /// <param name="content">
        /// ファイルから読み込んだ本文。
        /// </param>
        /// <param name="encoding">
        /// ファイルの文字コード。
        /// </param>
        /// <param name="hasByteOrderMark">
        /// BOMが付いていたかどうか。
        /// </param>
        public TextFileData(
            string content,
            Encoding encoding,
            bool hasByteOrderMark)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(encoding);

            Content = content;
            Encoding = encoding;
            HasByteOrderMark = hasByteOrderMark;
        }

        /// <summary>
        /// ファイルから読み込んだ本文を取得します。
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// ファイルの文字コードを取得します。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// BOMが付いていたかどうかを取得します。
        /// </summary>
        public bool HasByteOrderMark { get; }
    }
}