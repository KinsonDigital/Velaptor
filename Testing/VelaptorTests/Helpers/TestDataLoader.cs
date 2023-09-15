// <copyright file="TestDataLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
namespace VelaptorTests.Helpers;

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
    private const char WinDirSeparatorChar = '\\';
    private const char CrossPlatDirSeparatorChar = '/';
    private const string TestDataFolderName = "SampleTestData";
    private static readonly string RootDirPath = @$"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
                                                     .Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar) +
                                                 $"{CrossPlatDirSeparatorChar}{TestDataFolderName}";

    /// <summary>
    /// Loads JSON formatted test data and returns it as type a list of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="relativeDirPath">The relative directory path of the test data.</param>
    /// <param name="fileName">The name of the test data file to load.</param>
    /// <typeparam name="T">The type of data to return.</typeparam>
    /// <returns>The list of data items of type <typeparamref name="T"/> to return.</returns>
    /// <remarks>
    /// <para>
    ///     The <paramref name="relativeDirPath"/> is the directory path relative to the root directory.
    ///     This directory is the same as the directory location as the test assembly.
    /// </para>
    ///
    /// <para>
    ///     If the <paramref name="relativeDirPath"/> is null or empty, then the default root test data directory path will be used.
    /// </para>
    /// </remarks>
    public static T LoadTestData<T>(string? relativeDirPath, string fileName)
    {
        var loadTestDataPrefix = $"Loading test data error:{Environment.NewLine}\t";

        relativeDirPath = string.IsNullOrEmpty(relativeDirPath)
            ? string.Empty
            : relativeDirPath;

        if (string.IsNullOrEmpty(fileName))
        {
            Assert.Fail($"{loadTestDataPrefix}The parameter '{nameof(fileName)}' must not be null or empty.");
        }

        if (!Path.HasExtension(fileName))
        {
            Assert.Fail($"{loadTestDataPrefix}The file name '{fileName}' must be a file name with an extension.");
        }

        relativeDirPath = relativeDirPath.Replace(WinDirSeparatorChar, CrossPlatDirSeparatorChar);
        relativeDirPath = relativeDirPath.TrimStart(CrossPlatDirSeparatorChar);

        relativeDirPath = Path.HasExtension(relativeDirPath)
            ? Path.GetDirectoryName(relativeDirPath)
            : relativeDirPath;

        // Add a directory separator if one does not exist
        relativeDirPath = Path.EndsInDirectorySeparator(relativeDirPath)
            ? relativeDirPath
            : @$"{relativeDirPath}";

        var fullDirPath = $"{RootDirPath}{CrossPlatDirSeparatorChar}{relativeDirPath}";

        if (!Directory.Exists(fullDirPath))
        {
            Assert.Fail($"{loadTestDataPrefix}The directory path '{fullDirPath}' does not exist.");
        }

        var fullTestDataFilePath = $"{fullDirPath}{CrossPlatDirSeparatorChar}{fileName}";

        if (!File.Exists(fullTestDataFilePath))
        {
            Assert.Fail($"{loadTestDataPrefix}The test data file path '{fullTestDataFilePath}' does not exist.");
        }

        var testJSONData = File.ReadAllText(fullTestDataFilePath);

        var settings = new JsonSerializerSettings
        {
            Error = (_, args) =>
            {
                Assert.Fail(args.ErrorContext.Error.Message);
            },
        };

        var result = JsonConvert.DeserializeObject<T>(testJSONData, settings);

        if (result is null)
        {
            throw new Exception("Test data failed to load.");
        }

        return result;
    }
}
