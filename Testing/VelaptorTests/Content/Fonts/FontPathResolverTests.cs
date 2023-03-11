// <copyright file="FontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Xunit;

/// <summary>
/// Tests the <see cref="FontPathResolver"/> class.
/// </summary>
public class FontPathResolverTests
{
    private const string OnlyWindowsSupportMessage = "Currently loading system fonts is only supported on Windows.";
    private const string RootDirInContentLocation = @"C:\app-dir\Content\";
    private const string ContentDirNameInContentLocation = "ContentFonts";
    private const string RootDirInWindowsLocation = @"C:\Windows\";
    private const string ContentDirNameInWindowsLocation = "WinFonts";
    private readonly Mock<IContentPathResolver> mockContentPathResolver;
    private readonly Mock<IContentPathResolver> mockWindowsPathResolver;
    private readonly Mock<IFile> mockFile;
    private readonly Mock<IDirectory> mockDirectory;
    private readonly Mock<IPlatform> mockPlatform;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPathResolverTests"/> class.
    /// </summary>
    public FontPathResolverTests()
    {
        this.mockContentPathResolver = new Mock<IContentPathResolver>();
        this.mockContentPathResolver.Setup(p => p.RootDirectoryPath).Returns(RootDirInContentLocation);
        this.mockContentPathResolver.Setup(p => p.ContentDirectoryName).Returns(ContentDirNameInContentLocation);

        this.mockWindowsPathResolver = new Mock<IContentPathResolver>();
        this.mockWindowsPathResolver.Setup(p => p.RootDirectoryPath).Returns(RootDirInWindowsLocation);
        this.mockWindowsPathResolver.Setup(p => p.ContentDirectoryName).Returns(ContentDirNameInWindowsLocation);

        this.mockFile = new Mock<IFile>();
        this.mockDirectory = new Mock<IDirectory>();

        this.mockPlatform = new Mock<IPlatform>();
        this.mockPlatform.SetupGet(p => p.CurrentPlatform)
            .Returns(() => throw new Exception("Platform mock not setup for test."));
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullContentFontPathResolverParam_ThrowsException()
    {
        // Act & Assert
        var act = () =>
        {
            _ = new FontPathResolver(
                null,
                this.mockWindowsPathResolver.Object,
                this.mockFile.Object,
                this.mockDirectory.Object,
                this.mockPlatform.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'contentFontPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullWindowsFontPathResolverParam_ThrowsException()
    {
        // Act
        var act = () =>
        {
            _ = new FontPathResolver(
                this.mockContentPathResolver.Object,
                null,
                this.mockFile.Object,
                this.mockDirectory.Object,
                this.mockPlatform.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'windowsFontPathResolver')");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Act
        var act = () =>
        {
            _ = new FontPathResolver(
                this.mockContentPathResolver.Object,
                this.mockWindowsPathResolver.Object,
                null,
                this.mockDirectory.Object,
                this.mockPlatform.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Act
        var act = () =>
        {
            _ = new FontPathResolver(
                this.mockContentPathResolver.Object,
                this.mockWindowsPathResolver.Object,
                this.mockFile.Object,
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
            _ = new FontPathResolver(
                this.mockContentPathResolver.Object,
                this.mockWindowsPathResolver.Object,
                this.mockFile.Object,
                this.mockDirectory.Object,
                null);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'platform')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void RootDirectory_WhenFontExistsInContentDirectory_ReturnsContentRootDirectoryForContentLocation()
    {
        // Arrange
        MockWindowsPlatform();
        this.mockDirectory.Setup(m => m.Exists(RootDirInContentLocation)).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.RootDirectoryPath;

        // Assert
        actual.Should().Be(RootDirInContentLocation);
    }

    [Fact]
    public void RootDirectory_WhenFontDoesNotExistInContentDirectory_ReturnsWindowsRootDirectoryForWindowsLocation()
    {
        // Arrange
        MockWindowsPlatform();
        this.mockDirectory.Setup(m => m.Exists(RootDirInContentLocation)).Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.RootDirectoryPath;

        // Assert
        actual.Should().Be(RootDirInWindowsLocation);
    }

    [Fact]
    public void RootDirectory_WhenNotOnWindowsAndFontDoesNotExistInContentDirectory_ThrowsException()
    {
        // Arrange
        MockOSXPlatform();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.RootDirectoryPath;

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage(OnlyWindowsSupportMessage);
    }

    [Fact]
    public void ContentDirectoryName_WhenFontExistsInContentDirectory_ReturnsContentDirectoryNameForContentLocation()
    {
        // Arrange
        MockWindowsPlatform();
        this.mockDirectory.Setup(m => m.Exists(RootDirInContentLocation)).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.Should().Be(ContentDirNameInContentLocation);
    }

    [Fact]
    public void ContentDirectoryName_WhenFontDoesNotExistInContentDirectory_ReturnsWindowsDirectoryNameForContentLocation()
    {
        // Arrange
        MockWindowsPlatform();
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.Should().Be(ContentDirNameInWindowsLocation);
    }

    [Fact]
    public void ContentDirectoryName_WhenNotOnWindowsAndFontDoesNotExistInContentDirectory_ThrowsException()
    {
        // Arrange
        MockOSXPlatform();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ContentDirectoryName;

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage(OnlyWindowsSupportMessage);
    }
    #endregion

    #region Public Methods
    [Fact]
    public void ResolveFilePath_IfNotWindows_ThrowsException()
    {
        // Arrange
        MockOSXPlatform();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () =>
        {
            sut.ResolveFilePath("test-content");
        };

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage(OnlyWindowsSupportMessage);
    }

    [Fact]
    public void ResolveFilePath_WhenFontExistsInContentDirectory_ReturnsCorrectResult()
    {
        // Arrange
        const string expected = @"C:\app-dir\test-content.ttf";
        MockWindowsPlatform();
        this.mockFile.Setup(m => m.Exists(expected)).Returns(true);
        this.mockContentPathResolver.Setup(m => m.ResolveFilePath("test-content"))
            .Returns(expected);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath("test-content");

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ResolveFilePath_WhenFontDoesNotExistInContentDirectory_ReturnsCorrectResult()
    {
        // Arrange
        const string expected = @"C:\Windows\Fonts\test-content.ttf";
        MockWindowsPlatform();
        this.mockFile.Setup(m => m.Exists(expected)).Returns(false);
        this.mockWindowsPathResolver.Setup(m => m.ResolveFilePath("test-content"))
            .Returns(expected);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath("test-content");

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ResolveDirPath_IfNotWindows_ThrowsException()
    {
        // Arrange
        MockOSXPlatform();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveDirPath();

        // Assert
        act.Should().Throw<NotSupportedException>()
            .WithMessage(OnlyWindowsSupportMessage);
    }

    [Fact]
    public void ResolveDirPath_WhenFontExistsInContentDirectory_ReturnsCorrectResult()
    {
        // Arrange
        const string expected = @"C:\app-dir\";
        MockWindowsPlatform();
        this.mockDirectory.Setup(m => m.Exists(expected)).Returns(true);
        this.mockContentPathResolver.Setup(m => m.ResolveDirPath())
            .Returns(expected);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveDirPath();

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ResolveDirPath_WhenFontDoesNotExistInContentDirectory_ReturnsCorrectResult()
    {
        // Arrange
        const string expected = @"C:\Windows\Fonts\";
        MockWindowsPlatform();
        this.mockDirectory.Setup(m => m.Exists(expected)).Returns(false);
        this.mockWindowsPathResolver.Setup(m => m.ResolveDirPath())
            .Returns(expected);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveDirPath();

        // Assert
        actual.Should().Be(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontPathResolver"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontPathResolver CreateSystemUnderTest()
    {
        return new FontPathResolver(
            this.mockContentPathResolver.Object,
            this.mockWindowsPathResolver.Object,
            this.mockFile.Object,
            this.mockDirectory.Object,
            this.mockPlatform.Object);
    }

    /// <summary>
    /// Mocks the platform to be Windows.
    /// </summary>
    private void MockWindowsPlatform() =>
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.Windows);

    /// <summary>
    /// Mocks the platform to be Mac OSX.
    /// </summary>
    private void MockOSXPlatform() =>
        this.mockPlatform.SetupGet(p => p.CurrentPlatform).Returns(OSPlatform.OSX);
}
