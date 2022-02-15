// <copyright file="TestDataLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// Loads test data from the unit test project's SampleTestData folder.
    /// </summary>
    public static class TestDataLoader
    {
        private const string TestDataFolderName = "SampleTestData";
        private static readonly string RootDirPath = @$"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\{TestDataFolderName}\";

        /// <summary>
        /// Loads JSON formatted test data and returns it as type a list of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="relativeDirPath">The relative directory path of the test data.</param>
        /// <param name="fileName">The name of the test data file to load.</param>
        /// <typeparam name="T">The type of data to return.</typeparam>
        /// <returns>The list of data items of type <typeparamref name="T"/> to return.</returns>
        /// <remarks>
        /// <para>
        ///     The <paramref name="relativeDirPath"/> is the directory path relative to the root directory
        ///     path which is the same directory as the test assembly.
        /// </para>
        ///
        /// <para>
        ///     If the <paramref name="relativeDirPath"/> is null or empty, then the default root test data directory path will be used.
        /// </para>
        /// </remarks>
        public static T[] LoadTestData<T>(string? relativeDirPath, string fileName)
        {
            const string loadTestDataPrefix = "Loading test data error:\n\t";

            relativeDirPath = string.IsNullOrEmpty(relativeDirPath)
                ? string.Empty
                : relativeDirPath;

            if (string.IsNullOrEmpty(fileName))
            {
                Assert.True(false, $"{loadTestDataPrefix}The parameter '{nameof(fileName)}' must not be null or empty.");
            }

            if (Path.HasExtension(fileName) is false)
            {
                Assert.True(false, $"{loadTestDataPrefix}The file name '{fileName}' must be a file name with an extension.");
            }

            relativeDirPath = relativeDirPath.Replace(@"\\", @"\");
            relativeDirPath = relativeDirPath.TrimStart('\\');

            relativeDirPath = Path.HasExtension(relativeDirPath)
                ? Path.GetDirectoryName(relativeDirPath)
                : relativeDirPath;

            // Add a directory separator if one does not exist
            relativeDirPath = Path.EndsInDirectorySeparator(relativeDirPath)
                ? relativeDirPath
                : @$"{relativeDirPath}\";

            var fullDirPath = $"{RootDirPath}{relativeDirPath}";

            if (Directory.Exists(fullDirPath) is false)
            {
                Assert.True(false, $"{loadTestDataPrefix}The directory path '{fullDirPath}' does not exist.");
            }

            var fullTestDataFilePath = $"{fullDirPath}{fileName}".Replace($@"\\", @"\");

            if (File.Exists(fullTestDataFilePath) is false)
            {
                Assert.True(false, $"{loadTestDataPrefix}The test data file path '{fullTestDataFilePath}' does not exist.");
            }

            var testJSONData = File.ReadAllText(fullTestDataFilePath);

            var result = JsonConvert.DeserializeObject<T[]>(testJSONData);

            if (result is null)
            {
                throw new Exception("Test data failed to load.");
            }

            return result;
        }
    }
}
