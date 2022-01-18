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
        private const string FontFilePath = "meta-data-prefix";

        #region Method Tests
        [Theory]
        /*                    stringToParse         |       containsMetaData,   |    isValid,    |     metaDataPrefix    |       metaData        |  fontSize */
        [InlineData(FontFilePath,                                false,              true,           "",                  "",                         0u)]
        [InlineData("|" + FontFilePath + "text-before-pipe",     true,               false,          "",                  "",                         0u)]
        [InlineData(FontFilePath + "|",                          true,               false,          "",                  "",                         0u)]
        [InlineData(FontFilePath + "|size22",                    true,               false,          FontFilePath,        "size22",                   0u)]
        [InlineData(FontFilePath + "||ize22",                    true,               false,          "",                  "",                         0u)]
        [InlineData(":" + FontFilePath + "|size22",              true,               false,          ":" + FontFilePath,  "",                         0u)]
        [InlineData(FontFilePath + "|size22:",                   true,               false,          FontFilePath,        "",                         0u)]
        [InlineData(FontFilePath + "|si(ze:22",                  true,               false,          FontFilePath,        "si(ze:22",                 0u)]
        [InlineData(FontFilePath + "|size:not-a-number",         true,               false,          FontFilePath,        "size:not-a-number",        0u)]
        [InlineData(FontFilePath + "|size:22",                   true,               true,           FontFilePath,        "size:22",                  22u)]
        public void Parse_Invoke_ReturnsCorrectResult(
            string stringToParse,
            bool containsMetaData,
            bool isValid,
            string metaDataPrefix,
            string metaData,
            uint fontSize)
        {
            // Arrange
            var parser = new FontMetaDataParser();

            // Act
            var actual = parser.Parse(stringToParse);

            var failureMsg = "Expected Results:";
            failureMsg +=  $"\n\tContains Metadata: {containsMetaData}";
            failureMsg += $"\n\tIs Valid: {isValid}";
            failureMsg += $"\n\tMetadata Prefix: {metaDataPrefix}";
            failureMsg += $"\n\tMetadata: {(string.IsNullOrEmpty(metaData) ? "Null Or Empty" : metaData)}";
            failureMsg += $"\n\tFont Size: {fontSize}";

            failureMsg += "\nActual Results:";
            failureMsg +=  $"\n\tContains Metadata: {actual.ContainsMetaData}";
            failureMsg += $"\n\tIs Valid: {actual.IsValid}";
            failureMsg += $"\n\tMetadata Prefix: {actual.MetaDataPrefix}";
            failureMsg += $"\n\tMetadata: {(string.IsNullOrEmpty(actual.MetaData) ? "Null Or Empty" : actual.MetaData)}";
            failureMsg += $"\n\tFont Size: {actual.FontSize}";
            failureMsg += "\n------------------------------------------------------------------------------------------";

            // Assert
            AssertExtensions.EqualWithMessage(containsMetaData, actual.ContainsMetaData, failureMsg);
            AssertExtensions.EqualWithMessage(isValid, actual.IsValid, failureMsg);
            AssertExtensions.EqualWithMessage(metaDataPrefix, actual.MetaDataPrefix, failureMsg);
            AssertExtensions.EqualWithMessage(metaData, actual.MetaData, failureMsg);
            AssertExtensions.EqualWithMessage(fontSize, actual.FontSize, failureMsg);
        }
        #endregion
    }
}
