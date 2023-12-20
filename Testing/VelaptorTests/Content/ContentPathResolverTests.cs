// <copyright file="ContentPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.Reflection;
using Fakes;
using FluentAssertions;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Xunit;

/// <summary>
/// Tests the <see cref="ContentPathResolver"/> class.
/// </summary>
public class ContentPathResolverTests
{
    private const string ContentName = "test-content.png";
    private static readonly string BaseDir = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
        .ToCrossPlatPath();

    /// <summary>
    /// Gets test data for the <see cref="RootDirectoryPath_WhenSettingValue_ReturnsCorrectResult"/> test.
    /// </summary>
    public static TheoryData<string, string> ContentRootPaths =>
        new ()
        {
            { null, $"{BaseDir}/Content" },
            { @"C:\base-content\", "C:/base-content" },
            { @"C:\base-content", "C:/base-content" },
        };

    #region Prop Tests
    [Theory]
    [MemberData(nameof(ContentRootPaths))]
    public void RootDirectoryPath_WhenSettingValue_ReturnsCorrectResult(string rootDirectory, string expected)
    {
        // Arrange
        var resolver = new ContentPathResolverFake();

        // Act
        resolver.RootDirectoryPath = rootDirectory;
        var actual = resolver.RootDirectoryPath;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(@"C:\temp\test-dir-name", "test-dir-name")]
    [InlineData(@"C:\temp\test-dir-name\", "test-dir-name")]
    [InlineData("C:/temp/test-dir-name", "test-dir-name")]
    [InlineData("C:/temp/test-dir-name/", "test-dir-name")]
    public void ContentDirectoryName_WhenSettingWithDirectoryPath_CorrectlySetsResult(
        string contentDirName,
        string expected)
    {
        // Arrange
        var resolver = new ContentPathResolverFake();

        // Act
        resolver.ContentDirectoryName = contentDirName;
        var actual = resolver.ContentDirectoryName;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("test-content.png")]
    public void ResolveFilePath_WhenInvoked_ResolvesContentFilePath(string contentName)
    {
        // Arrange
        var resolver = new ContentPathResolverFake();

        // Act
        var actual = resolver.ResolveFilePath(contentName);

        // Assert
        actual.Should().Be(ContentName);
    }

    [Theory]
    [InlineData(@"C:\temp\my-content\")]
    public void ResolveDirPath_WhenInvoked_ResolvesContentDirPath(string rootDirPath)
    {
        // Arrange
        var resolver = new ContentPathResolverFake();
        resolver.RootDirectoryPath = rootDirPath;
        resolver.ContentDirectoryName = "test-content";

        // Act
        var actual = resolver.ResolveDirPath();

        // Assert
        actual.Should().Be("C:/temp/my-content/test-content");
    }

    [Fact]
    public void ResolveFilePath_WhenContentNameIsNullOrEmpty_ThrowsException()
    {
        // Arrange
        var resolver = new ContentPathResolverFake();

        // Act
        var act = () => resolver.ResolveFilePath(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'contentName')");
    }

    [Theory]
    [InlineData(@"content.png\")]
    [InlineData("content.png/")]
    public void ResolveFilePath_WhenContentNameEndsWithDirSeparator_ThrowsException(string contentName)
    {
        // Arrange
        var resolver = new ContentPathResolverFake();

        // Act
        var act = () => resolver.ResolveFilePath(contentName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage($"The '{contentName}' cannot end with a folder. It must end with a file name with or without the extension. (Parameter 'contentName')");
    }
    #endregion
}
