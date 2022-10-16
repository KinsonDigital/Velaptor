// <copyright file="FontMetaDataParserTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    using System;
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
        [InlineData(null,                                        false,              true,           "",                  "",                         0u)]
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
            failureMsg +=  $"{Environment.NewLine}Contains Metadata: {containsMetaData}";
            failureMsg += $"{Environment.NewLine}Is Valid: {isValid}";
            failureMsg += $"{Environment.NewLine}Metadata Prefix: {metaDataPrefix}";
            failureMsg += $"{Environment.NewLine}Metadata: {(string.IsNullOrEmpty(metaData) ? "Null Or Empty" : metaData)}";
            failureMsg += $"{Environment.NewLine}Font Size: {fontSize}";

            failureMsg += $"{Environment.NewLine}Actual Results:";
            failureMsg += $"{Environment.NewLine}Contains Metadata: {actual.ContainsMetaData}";
            failureMsg += $"{Environment.NewLine}Is Valid: {actual.IsValid}";
            failureMsg += $"{Environment.NewLine}Metadata Prefix: {actual.MetaDataPrefix}";
            failureMsg += $"{Environment.NewLine}Metadata: {(string.IsNullOrEmpty(actual.MetaData) ? "Null Or Empty" : actual.MetaData)}";
            failureMsg += $"{Environment.NewLine}Font Size: {actual.FontSize}";
            failureMsg += $"{Environment.NewLine}------------------------------------------------------------------------------------------";

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
