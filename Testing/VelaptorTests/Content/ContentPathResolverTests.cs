// <copyright file="ContentPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Fakes;
using FluentAssertions;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="ContentPathResolver"/> class.
/// </summary>
public class ContentPathResolverTests
{
    private static readonly char DirSepChar = Path.AltDirectorySeparatorChar;
    private static readonly string Root = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:/" : "/";
    private readonly IAppService mockAppService;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IPlatform mockPlatform;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentPathResolverTests"/> class.
    /// </summary>
    public ContentPathResolverTests()
    {
        this.mockAppService = Substitute.For<IAppService>();
        this.mockAppService.AppDirectory.Returns($"{Root}app");

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.DirectorySeparatorChar.Returns(Path.DirectorySeparatorChar);
        this.mockPath.AltDirectorySeparatorChar.Returns(Path.AltDirectorySeparatorChar);

        this.mockPlatform = Substitute.For<IPlatform>();
    }

#pragma warning disable SA1514
    #region Test Data
    /// <summary>
    /// Provides test data for the <see cref="ResolveFilePath_WhenInvoked_ResolvesContentFilePath"/> test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, bool, string, string> ResolveFilePath_WhenInvoked_ResolvesContentFilePath_Data()
    {
        const string contentName = "test-content.png";

        return new TheoryData<string, bool, string, string>
        {
            {
                $"{Root}root-dir{DirSepChar}{contentName}",
                true,
                "WINDOWS",
                $"{Root}root-dir{DirSepChar}{contentName}"
            },
            {
                "test-content.png",
                false,
                "WINDOWS",
                $"{Root}app{DirSepChar}Content{DirSepChar}Graphics{DirSepChar}{contentName}"
            },
            {
                "test-content.png",
                false,
                "OSX",
                $"{Root}app{DirSepChar}Content{DirSepChar}Graphics{DirSepChar}{contentName}"
            },
            {
                "test-content.png",
                false,
                "LINUX",
                $"{Root}app{DirSepChar}Content{DirSepChar}Graphics{DirSepChar}{contentName}"
            },
            {
                "test-content.png",
                false,
                "FREEBSD",
                $"{Root}app{DirSepChar}Content{DirSepChar}Graphics{DirSepChar}{contentName}"
            },
            {
                $"sub-dir{DirSepChar}test-content.png",
                false,
                "WINDOWS",
                $"{Root}app{DirSepChar}Content{DirSepChar}Graphics{DirSepChar}sub-dir{DirSepChar}{contentName}"
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
            _ = new ContentPathResolverFake(null, this.mockFile, this.mockPath, this.mockPlatform);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'appService')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ContentPathResolverFake(this.mockAppService, null, this.mockPath, this.mockPlatform);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ContentPathResolverFake(this.mockAppService, this.mockFile, null, this.mockPlatform);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'path')");
    }

    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioPathResolver(this.mockAppService, this.mockFile, this.mockPath, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'platform')");
    }
    #endregion

    #region Prop Tests
    [Theory]
    [InlineData(null, "C:/app/Content")]
    [InlineData("", "C:/app/Content")]
    [InlineData(@"C:\base-content\", "C:/base-content/")]
    [InlineData(@"C:\base-content", @"C:/base-content")]
    public void RootDirectoryPath_WhenSettingValue_ReturnsCorrectResult(string? rootDirectory, string expected)
    {
        // Arrange
        var resolver = CreateSystemUnderTest();

        // Act
        resolver.RootDirectoryPath = rootDirectory!;
        var actual = resolver.RootDirectoryPath;

        // Assert
        actual.Should().Be(expected);
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
        actual.Should().Be(expected);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(@"C:\temp\my-content\")]
    [InlineData(@"C:\temp\my-content/")]
    public void ResolveDirPath_WhenInvoked_ResolvesContentDirPath(string rootDirPath)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.RootDirectoryPath = rootDirPath;
        sut.ContentDirectoryName = "test-content";

        // Act
        var actual = sut.ResolveDirPath();

        // Assert
        actual.Should().Be(@"C:/temp/my-content/test-content");
    }

    [Fact]
    public void ResolveFilePath_WhenContentNameIsNull_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void ResolveFilePath_WhenContentNameIsEmpty_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(string.Empty);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'contentPathOrName')");
    }

    [Theory]
    [InlineData(@"content.png\")]
    [InlineData("content.png/")]
    public void ResolveFilePath_WhenContentNameEndsWithDirSeparator_ThrowsException(string contentPathOrName)
    {
        // Arrange
        var expectedMsg = $"The '{contentPathOrName}' cannot end with a folder. It must end with or without an extension." +
                          " (Parameter 'contentPathOrName')";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(contentPathOrName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage(expectedMsg);
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
        act.Should().Throw<ArgumentException>().WithMessage(expectedMsg);
    }

    [Fact]
    public void ResolveFilePath_WhenContentFileDoesNotExist_ThrowsException()
    {
        // Arrange
        var expectedContentFilePath = $"{Root}app{DirSepChar}test-content.png";
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(true);
        this.mockPath.HasExtension(Arg.Any<string>()).Returns(true);
        this.mockFile.Exists(Arg.Any<string>()).Returns(false);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(expectedContentFilePath);

        // Assert
        act.Should().Throw<FileNotFoundException>("The content file could not be found.")
            .And.FileName.Should().Be(expectedContentFilePath);
    }

    [Theory]
    [MemberData(nameof(ResolveFilePath_WhenInvoked_ResolvesContentFilePath_Data))]
    public void ResolveFilePath_WhenInvoked_ResolvesContentFilePath(
        string contentPathOrName,
        bool isPathRooted,
        string platform,
        string expected)
    {
        // Arrange
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        this.mockPath.IsPathRooted(Arg.Any<string>()).Returns(isPathRooted);
        this.mockPath.HasExtension(Arg.Any<string>()).Returns(true);
        this.mockPlatform.CurrentPlatform.Returns(OSPlatform.Create(platform));

        var sut = CreateSystemUnderTest();
        sut.ContentDirectoryName = "Graphics";

        // Act
        var actual = sut.ResolveFilePath(contentPathOrName);

        // Assert
        actual.Should().Be(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ContentPathResolverFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ContentPathResolverFake CreateSystemUnderTest() => new (this.mockAppService, this.mockFile, this.mockPath, this.mockPlatform);
}
