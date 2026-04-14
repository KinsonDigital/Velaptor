// <copyright file="ContentPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable ConvertToLocalFunction
namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Fakes;
using Helpers;
using Shouldly;
using NSubstitute;
using Velaptor.Content;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="ContentPathResolver"/> class.
/// </summary>
public class ContentPathResolverTests
{
    private static readonly string RootDirPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\app" : "/app";
    private readonly IAppService mockAppService;
    private readonly IFile mockFile;
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentPathResolverTests"/> class.
    /// </summary>
    public ContentPathResolverTests()
    {
        this.mockAppService = Substitute.For<IAppService>();
        this.mockAppService.AppDirectory.Returns(_ => RootDirPath);

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.AltDirectorySeparatorChar.Returns(Path.AltDirectorySeparatorChar);
    }

#pragma warning disable SA1514
    #region Test Data
    /// <summary>
    /// Provides test data for the <see cref="ResolveFilePath_WhenInvokedOnWindows_ResolvesContentFilePath"/> test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, bool, string> ResolveFilePath_WhenInvokedOnWindows_ResolvesContentFilePath_Data()
    {
        const string contentName = "test-content.png";

        return new TheoryData<string, bool, string>
        {
            {
                $@"{RootDirPath}root-dir\{contentName}", // contentPathOrName - backslash rooted path input
                true, // isPathRooted
                $"{RootDirPath}root-dir/{contentName}" // expected - normalized to forward slashes
            },
            {
                "test-content.png",
                false,
                $"{RootDirPath}Content/Graphics/{contentName}"
            },
            {
                @"sub-dir\test-content.png",
                false,
                $"{RootDirPath}Content/Graphics/sub-dir/{contentName}"
            },
        };
    }

    /// <summary>
    /// Provides test data for the <see cref="ResolveFilePath_WhenInvokedOnPosix_ResolvesContentFilePath"/> test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, bool, string> ResolveFilePath_WhenInvokedOnPosix_ResolvesContentFilePath_Data()
    {
        const string contentName = "test-content.png";

        return new TheoryData<string, bool, string>
        {
            {
                "test-content.png",
                false,
                $"{RootDirPath}/Content/Graphics/{contentName}"
            },
            {
                @"sub-dir\test-content.png",
                false,
                $"{RootDirPath}/Content/Graphics/sub-dir/{contentName}"
            },
            {
                "test-content.png",
                false,
                $"{RootDirPath}/Content/Graphics/{contentName}"
            },
        };
    }
    #endregion
#pragma warning restore SA1514

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullAppServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ContentPathResolverFake(null, this.mockFile, this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'appService')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ContentPathResolverFake(this.mockAppService, null, this.mockPath);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ContentPathResolverFake(this.mockAppService, this.mockFile, null);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Prop Tests
    [TheoryForWindows]
    [InlineData(null, "C:/app/Content")]
    [InlineData("", "C:/app/Content")]
    [InlineData(@"C:\base-content\", "C:/base-content")]
    [InlineData("C:/base-content/", "C:/base-content")]
    [InlineData(@"\base-content\", "/base-content")]
    [InlineData("/base-content/", "/base-content")]
    public void RootDirectoryPath_WhenSettingValueOnWindows_ReturnsCorrectResult(string? rootDirectory, string expected)
    {
        // Arrange
        var resolver = CreateSystemUnderTest();

        // Act
        resolver.RootDirectoryPath = rootDirectory!;
        var actual = resolver.RootDirectoryPath;

        // Assert
        actual.ShouldBe(expected);
    }

    [TheoryForPosix]
    [InlineData(null, "/app/Content")]
    [InlineData("", "/app/Content")]
    [InlineData(@"C:\base-content\", "/base-content")]
    [InlineData("C:/base-content/", "/base-content")]
    [InlineData(@"\base-content\", "/base-content")]
    [InlineData(@"/base-content/", "/base-content")]
    public void RootDirectoryPath_WhenSettingValueOnPosix_ReturnsCorrectResult(string? rootDirectory, string expected)
    {
        // Arrange
        var resolver = CreateSystemUnderTest();

        // Act
        resolver.RootDirectoryPath = rootDirectory!;
        var actual = resolver.RootDirectoryPath;

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(@"C:\temp\test-dir-name", "test-dir-name")]
    [InlineData(@"C:\temp\test-dir-name\", "test-dir-name")]
    public void ContentDirectoryName_WhenSettingWithDirectoryPath_CorrectlySetsResult(
        string contentDirName,
        string expected)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.ContentDirectoryName = contentDirName;
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.ShouldBe(expected);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveDirPath_WhenInvoked_ResolvesContentDirPath()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.RootDirectoryPath = $@"{RootDirPath}temp\my-content\";
        sut.ContentDirectoryName = "test-content";

        // Act
        var actual = sut.ResolveDirPath();

        // Assert
        actual.ShouldBe($@"{RootDirPath}temp/my-content/test-content");
    }

    [Fact]
    public void ResolveFilePath_WhenContentNameIsNull_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(null);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void ResolveFilePath_WhenContentNameIsEmpty_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(string.Empty);

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Theory]
    [InlineData(@"content.png\", "content.png/")]
    [InlineData("content.png/", "content.png/")]
    public void ResolveFilePath_WhenContentNameEndsWithDirSeparator_ThrowsException(string contentPathOrName, string normalizedPathOrName)
    {
        // Arrange
        var expectedMsg = $"The '{normalizedPathOrName}' cannot end with a folder. It must end with or without an extension." +
                          " (Parameter 'contentPathOrName')";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(contentPathOrName);

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void ResolveFilePath_WithoutAnExtension_ThrowsException()
    {
        // Arrange
        const string expectedMsg = "The 'test-content' must end with an extension. (Parameter 'contentPathOrName')";

        this.mockPath.GetExtension(Arg.Any<string>()).Returns(string.Empty);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath("test-content");

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void ResolveFilePath_WhenContentFileDoesNotExist_ThrowsException()
    {
        // Arrange
        // Input uses backslashes; after NormalizeSeparators they become forward slashes.
        var inputFilePath = $@"{RootDirPath}\app\test-content.png";
        var expectedNormalizedFilePath = $"{RootDirPath}/app/test-content.png";
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        this.mockPath.HasExtension(Arg.Any<string>()).Returns(true);
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(inputFilePath);

        // Assert
        var exception = Should.Throw<FileNotFoundException>(act, "The content file could not be found.");
        exception.FileName.ShouldBe(expectedNormalizedFilePath);
    }

    [TheoryForWindows]
    [MemberData(nameof(ResolveFilePath_WhenInvokedOnWindows_ResolvesContentFilePath_Data))]
    public void ResolveFilePath_WhenInvokedOnWindows_ResolvesContentFilePath(
        string contentPathOrName,
        bool isPathRooted,
        string expected)
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(isPathRooted);
        this.mockPath.HasExtension(Arg.Any<string>()).Returns(true);

        var sut = CreateSystemUnderTest();
        sut.ContentDirectoryName = "Graphics";

        // Act
        var actual = sut.ResolveFilePath(contentPathOrName);

        // Assert
        actual.ShouldBe(expected);
    }

    [TheoryForPosix]
    [MemberData(nameof(ResolveFilePath_WhenInvokedOnPosix_ResolvesContentFilePath_Data))]
    public void ResolveFilePath_WhenInvokedOnPosix_ResolvesContentFilePath(
        string contentPathOrName,
        bool isPathRooted,
        string expected)
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(isPathRooted);
        this.mockPath.HasExtension(Arg.Any<string>()).Returns(true);

        var sut = CreateSystemUnderTest();
        sut.ContentDirectoryName = "Graphics";

        // Act
        var actual = sut.ResolveFilePath(contentPathOrName);

        // Assert
        actual.ShouldBe(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ContentPathResolverFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ContentPathResolverFake CreateSystemUnderTest() => new (this.mockAppService, this.mockFile, this.mockPath);
}
