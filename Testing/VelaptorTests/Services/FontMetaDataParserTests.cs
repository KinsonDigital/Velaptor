// <copyright file="FontMetaDataParserTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    using System.IO.Abstractions;
    using Moq;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="FontMetaDataParser"/> class.
    /// </summary>
    public class FontMetaDataParserTests
    {
        private const string DirPath = @"C:\Windows\Fonts\";
        private const string FontFilePath = @"C:\Windows\Fonts\my-font.ttf";
        private readonly Mock<IPath> mockPath;

        public FontMetaDataParserTests()
        {
            this.mockPath = new Mock<IPath>();
            this.mockPath.Setup(m => m.HasExtension($":{FontFilePath}")).Returns(true);
            this.mockPath.Setup(m => m.HasExtension(FontFilePath)).Returns(true);
            this.mockPath.Setup(m => m.GetExtension(FontFilePath)).Returns(".ttf");
            this.mockPath.Setup(m => m.GetExtension($":{FontFilePath}")).Returns(".ttf");
            this.mockPath.Setup(m => m.GetDirectoryName(FontFilePath)).Returns(DirPath);
            this.mockPath.Setup(m => m.GetDirectoryName($":{FontFilePath}")).Returns(DirPath);
        }

        #region Method Tests
        [Theory]
        /*                    stringToParse         |         containsMetaData,  | isValid,         |           filePath         |         metaData      |      fontSize */
        [InlineData(FontFilePath,                                false,          true,           "",                               "",                         -1)]
        [InlineData("|" + FontFilePath + "text-before-pipe",     true,           false,          "",                               "",     -1)]
        [InlineData(FontFilePath + "|",                          true,           false,          "",                               "",                         -1)]
        [InlineData(FontFilePath + "|size22",                    true,           false,          FontFilePath,                     "size22",                   -1)]
        [InlineData(FontFilePath + "||ize22",                    true,           false,          "",                               "",                         -1)]
        [InlineData(":" + FontFilePath + "|size22",              true,           false,          ":" + FontFilePath,               "",                         -1)]
        [InlineData(FontFilePath + "|size22:",                   true,           false,          FontFilePath,                     "",                         -1)]
        [InlineData(FontFilePath + "|si(ze:22",                  true,           false,          FontFilePath,                     "si(ze:22",                 -1)]
        [InlineData(FontFilePath + "|size:not-a-number",         true,           false,          FontFilePath,                     "size:not-a-number",        -1)]
        [InlineData(@"C:\Windows\Fonts\my-font|size:22",         true,           false,          @"C:\Windows\Fonts\my-font",      "size:22",                  -1)]
        [InlineData(@"C:\Windows\Fonts\my-image.jpg|size:22",    true,           false,          @"C:\Windows\Fonts\my-image.jpg", "size:22",                  -1)]
        [InlineData(@"my-image.ttf|size:22",                     true,           false,          @"my-image.ttf",                  "size:22",                  -1)]
        [InlineData(FontFilePath + "|size:22",                   true,           true,           FontFilePath,                     "size:22",                  22)]
        public void Parse_Invoke_ReturnsCorrectResult(
            string stringToParse,
            bool containsMetaData,
            bool isValid,
            string filePath,
            string metaData,
            int fontSize)
        {
            // Arrange
            var parser = new FontMetaDataParser(this.mockPath.Object);

            // Act
            var actual = parser.Parse(stringToParse);

            var failureMsg = "Expected Results:";
            failureMsg +=  $"\n\tContains Meta Data: {containsMetaData}";
            failureMsg += $"\n\tIs Valid: {isValid}";
            failureMsg += $"\n\tFile Path: {filePath}";
            failureMsg += $"\n\tMeta Data: {(string.IsNullOrEmpty(metaData) ? "Null Or Empty" : metaData)}";
            failureMsg += $"\n\tFont Size: {fontSize}";

            failureMsg += "\nActual Results:";
            failureMsg +=  $"\n\tContains Meta Data: {actual.ContainsMetaData}";
            failureMsg += $"\n\tIs Valid: {actual.IsValid}";
            failureMsg += $"\n\tFile Path: {actual.FilePath}";
            failureMsg += $"\n\tMeta Data: {(string.IsNullOrEmpty(actual.MetaData) ? "Null Or Empty" : actual.MetaData)}";
            failureMsg += $"\n\tFont Size: {actual.FontSize}";
            failureMsg += "\n------------------------------------------------------------------------------------------";

            // Assert
            AssertExtensions.EqualWithMessage(containsMetaData, actual.ContainsMetaData, failureMsg);
            AssertExtensions.EqualWithMessage(isValid, actual.IsValid, failureMsg);
            AssertExtensions.EqualWithMessage(filePath, actual.FilePath, failureMsg);
            AssertExtensions.EqualWithMessage(metaData, actual.MetaData, failureMsg);
            AssertExtensions.EqualWithMessage(fontSize, actual.FontSize, failureMsg);
        }
        #endregion
    }
}
