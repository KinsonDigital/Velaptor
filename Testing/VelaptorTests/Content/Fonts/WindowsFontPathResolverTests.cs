// <copyright file="WindowsFontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Content.Fonts;
using Xunit;

/// <summary>
/// Tests the <see cref="WindowsFontPathResolver"/> class.
/// </summary>
public class WindowsFontPathResolverTests
{
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IPlatform> mockPlatform;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsFontPathResolverTests"/> class.
    /// </summary>
    public WindowsFontPathResolverTests()
    {
        this.mockDirectory = new Mock<IDirectory>();

        this.mockPlatform = new Mock<IPlatform>();
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Act
        var act = () =>
        {
            _ = new WindowsFontPathResolver(
                null,
                this.mockPlatform.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WithNullPlatformParam_ThrowsException()
    {
        // Act
        var act = () =>
        {
            _ = new WindowsFontPathResolver(
                this.mockDirectory.Object,
                null);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'platform')");
    }

    [Fact]
    public void Ctor_WhenNotOnWindowsPlatform_ThrowsException()
    {
        // Arrange
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Linux);

        // Act
        var act = () => _ = CreateSystemUnderTest();

        // Assert
        act.Should().Throw<PlatformNotSupportedException>()
            .WithMessage($"The '{nameof(WindowsFontPathResolver)}' can only be used on the 'Windows' platform.");
    }
    #endregion

    #region Prop tests
    [Fact]
    public void RootDirectoryPath_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.RootDirectoryPath;

        // Assert
        actual.Should().Be(@"C:/Windows");
    }

    [Fact]
    public void ContentDirectoryName_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.Should().Be("Fonts");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ResolveFilePath_WhenNullOrEmpty_ThrowsException(string contentName)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(contentName);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'contentName')");
    }

    [Fact]
    public void ResolveFilePath_WhenContentNameEndsWithDirectorySeparator_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(@"test-content/");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage(@"The 'test-content/' cannot end with a folder.  It must end with a file name with or without the extension. (Parameter 'contentName')");
    }

    [Fact]
    public void ResolveFilePath_WhenInvoking_ReturnsCorrectResolvedFilePath()
    {
        // Arrange
        const string rootDir = @"C:/Windows";
        const string contentDirName = "Fonts";
        const string contentName = "test-content";
        const string extension = ".ttf";
        const string fullContentDirPath = $@"{rootDir}/{contentDirName}";
        const string expected = $@"{fullContentDirPath}/{contentName}{extension}";

        var files = new[]
        {
            $"{fullContentDirPath}/other-file.txt",
            expected,
        };

        this.mockDirectory.Setup(m => m.GetFiles(fullContentDirPath, $"*{extension}"))
            .Returns(files);

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath(contentName);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ResolveDirPath_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const string expected = "C:/Windows/Fonts";
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveDirPath();

        // Assert
        actual.Should().Be(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="WindowsFontPathResolver"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private WindowsFontPathResolver CreateSystemUnderTest() => new (this.mockDirectory.Object, this.mockPlatform.Object);
}
