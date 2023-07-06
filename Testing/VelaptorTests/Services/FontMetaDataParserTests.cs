// <copyright file="FontMetaDataParserTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using FluentAssertions;
using Velaptor.Services;
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

        // Assert
        actual.ContainsMetaData.Should().Be(containsMetaData);
        actual.IsValid.Should().Be(isValid);
        actual.MetaDataPrefix.Should().Be(metaDataPrefix);
        actual.MetaData.Should().Be(metaData);
        actual.FontSize.Should().Be(fontSize);
    }
    #endregion
}
