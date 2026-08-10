using System.IO;
using TsCPToolKit.Helpers;
using TsCPToolKit.Models;
using TsCPToolKit.Localization;

namespace TsCPToolKit.Services
{
    /// <summary>
    /// JSONインデント処理のテストを一括実行します。
    /// </summary>
    public sealed class JsonIndentTestService
    {
        private readonly JsonIndentService jsonIndentService = new();

        private readonly JsonDiffService jsonDiffService = new();

        /// <summary>
        /// Testsフォルダ内のテストを実行します。
        /// </summary>
        /// <param name="testsRootPath">
        /// Input、Output、Expectedフォルダを含む
        /// Testsフォルダのパス。
        /// </param>
        /// <param name="options">
        /// インデント整理に使用する設定。
        /// </param>
        /// <returns>
        /// 各JSONファイルのテスト結果。
        /// </returns>
        public IReadOnlyList<TestCaseResult> RunTests(
            string testsRootPath,
            IndentOptions options)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                testsRootPath);

            ArgumentNullException.ThrowIfNull(options);

            string inputDirectory =
                Path.Combine(testsRootPath, "Input");

            string outputDirectory =
                Path.Combine(testsRootPath, "Output");

            string expectedDirectory =
                Path.Combine(testsRootPath, "Expected");

            ValidateInputDirectory(inputDirectory);

            Directory.CreateDirectory(outputDirectory);
            Directory.CreateDirectory(expectedDirectory);

            ClearGeneratedOutputFiles(outputDirectory);

            string[] inputFiles =
                Directory.GetFiles(
                    inputDirectory,
                    "*.json",
                    SearchOption.TopDirectoryOnly)
                .OrderBy(
                    path => Path.GetFileName(path),
                    StringComparer.OrdinalIgnoreCase)
                .ToArray();

            List<TestCaseResult> results = new();

            foreach (string inputPath in inputFiles)
            {
                TestCaseResult result =
                    RunSingleTest(
                        inputPath,
                        outputDirectory,
                        expectedDirectory,
                        options);

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Outputフォルダ内のJSONファイルを
        /// Expectedフォルダへコピーします。
        /// </summary>
        /// <param name="testsRootPath">
        /// Input、Output、Expectedフォルダを含む
        /// Testsフォルダのパス。
        /// </param>
        /// <returns>
        /// コピーしたJSONファイルの数。
        /// </returns>
        public int UpdateExpectedFiles(
            string testsRootPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                testsRootPath);

            string outputDirectory =
                Path.Combine(testsRootPath, "Output");

            string expectedDirectory =
                Path.Combine(testsRootPath, "Expected");

            if (!Directory.Exists(outputDirectory))
            {
                throw new DirectoryNotFoundException(
                    LanguageManager.GetString(
                        "TestService_OutputFolderNotFound") +
                    "\n\n" +
                    outputDirectory);
            }

            Directory.CreateDirectory(expectedDirectory);

            string[] outputFiles =
                Directory.GetFiles(
                    outputDirectory,
                    "*.json",
                    SearchOption.TopDirectoryOnly);

            foreach (string outputPath in outputFiles)
            {
                string fileName =
                    Path.GetFileName(outputPath);

                string expectedPath =
                    Path.Combine(
                        expectedDirectory,
                        fileName);

                File.Copy(
                    outputPath,
                    expectedPath,
                    overwrite: true);
            }

            return outputFiles.Length;
        }

        /// <summary>
        /// JSONファイル1件分のテストを実行します。
        /// </summary>
        private TestCaseResult RunSingleTest(
            string inputPath,
            string outputDirectory,
            string expectedDirectory,
            IndentOptions options)
        {
            string fileName =
                Path.GetFileName(inputPath);

            string outputPath =
                Path.Combine(outputDirectory, fileName);

            string expectedPath =
                Path.Combine(expectedDirectory, fileName);

            try
            {
                TextFileData inputFile =
                    FileHelper.ReadTextFile(inputPath);

                string formattedContent =
                    jsonIndentService.Format(
                        inputFile.Content,
                        options);

                /*
                 * Inputファイルは上書きせず、
                 * Outputフォルダへ結果を書き出します。
                 */
                FileHelper.WriteTextFile(
                    outputPath,
                    formattedContent,
                    inputFile);

                if (!File.Exists(expectedPath))
                {
                    return new TestCaseResult(
                        fileName,
                        inputPath,
                        outputPath,
                        expectedPath,
                        TestCaseStatus.ExpectedMissing,
                        LanguageManager.GetString(
                            "TestService_ExpectedMissing"));
                }

                TextFileData expectedFile =
                    FileHelper.ReadTextFile(expectedPath);

                JsonDiffResult diff =
                    jsonDiffService.Compare(
                        expectedFile.Content,
                        formattedContent);

                if (!diff.HasDifference)
                {
                    return new TestCaseResult(
                        fileName,
                        inputPath,
                        outputPath,
                        expectedPath,
                        TestCaseStatus.Passed,
                        LanguageManager.GetString(
                            "TestService_Passed"));
                }

                return new TestCaseResult(
                    fileName,
                    inputPath,
                    outputPath,
                    expectedPath,
                    TestCaseStatus.Failed,
                    $"{LanguageManager.GetString("TestService_FirstDifference")}" +
                    $"{diff.LineNumber}" +
                    $"{LanguageManager.GetString("TestService_LineSuffix")}\n" +
                    $"Expected : {diff.ExpectedLine}\n" +
                    $"Output   : {diff.ActualLine}");
            }
            catch (Exception exception)
            {
                return new TestCaseResult(
                    fileName,
                    inputPath,
                    outputPath,
                    expectedPath,
                    TestCaseStatus.Error,
                    exception.Message);
            }
        }

        /// <summary>
        /// Inputフォルダが存在することを確認します。
        /// </summary>
        private static void ValidateInputDirectory(
            string inputDirectory)
        {
            if (!Directory.Exists(inputDirectory))
            {
                throw new DirectoryNotFoundException(
                    LanguageManager.GetString(
                        "TestService_InputFolderNotFound") +
                    "\n\n" +
                    inputDirectory);
            }
        }

        /// <summary>
        /// 前回生成されたOutput内のJSONを削除します。
        /// </summary>
        private static void ClearGeneratedOutputFiles(
            string outputDirectory)
        {
            string[] oldOutputFiles =
                Directory.GetFiles(
                    outputDirectory,
                    "*.json",
                    SearchOption.TopDirectoryOnly);

            foreach (string filePath in oldOutputFiles)
            {
                File.Delete(filePath);
            }
        }
    }
}