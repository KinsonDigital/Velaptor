// <copyright file="AudioPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using NSubstitute;
using Shouldly;
using Velaptor;
using Velaptor.Content;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="AudioPathResolver"/> class.
/// </summary>
public class AudioPathResolverTests
{
    private readonly IAppService mockAppService;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IPlatform mockPlatform;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPathResolverTests"/> class.
    /// </summary>
    public AudioPathResolverTests()
    {
        this.mockPlatform = Substitute.For<IPlatform>();
        this.mockPlatform.CurrentPlatform.Returns(OSPlatform.Windows);

        this.mockAppService = Substitute.For<IAppService>();
        this.mockAppService.AppDirectory.Returns("AppHome");

        this.mockFile = Substitute.For<IFile>();

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.DirectorySeparatorChar.Returns(Path.DirectorySeparatorChar);
        this.mockPath.AltDirectorySeparatorChar.Returns(Path.AltDirectorySeparatorChar);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
    {
        // Arrange
        // Act
        var sut = CreateSystemUnderTest();
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.ShouldBe("Audio");
    }

    [Fact]
    public void Ctor_WithNullFileParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioPathResolver(
                this.mockAppService,
                null,
                this.mockPath,
                this.mockPlatform);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message
            .ShouldBe("Value cannot be null. (Parameter 'file')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioPathResolver(
                this.mockAppService,
                this.mockFile,
                null,
                this.mockPlatform);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message
            .ShouldBe("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveFilePath_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(null);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void ResolveFilePath_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath(string.Empty);

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Theory]
    [InlineData("WINDOWS", "")]
    [InlineData("LINUX", "\nNote: Linux and macOS are case-sensitive.")]
    [InlineData("OSX", "\nNote: Linux and macOS are case-sensitive.")]
    [InlineData("FREEBSD", "\nNote: Linux and macOS are case-sensitive.")]
    public void ResolveFilePath_WithUnsupportedExtension_ThrowsException(string platformValue, string expectedNote)
    {
        // Arrange
        var platform = OSPlatform.Create(platformValue);
        var expected = "The file extension '.other' is not supported.  Supported audio formats are '.ogg' and '.mp3'." +
                       expectedNote;

        this.mockPath.GetExtension(Arg.Any<string>()).Returns(".other");
        this.mockPlatform.CurrentPlatform.Returns(platform);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.ResolveFilePath("test-content.other");

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData(".ogg")]
    [InlineData(".mp3")]
    public void ResolveFilePath_WhenInvoked_ResolvesFilePath(string extension)
    {
        // Arrange
        var fullContentName = $"test-content{extension}";
        var expected = $"{this.mockAppService.AppDirectory}/Content/Audio/{fullContentName}";
        this.mockPath.HasExtension(Arg.Any<string>()).Returns(true);
        this.mockPath.GetExtension(Arg.Any<string>()).Returns(extension);
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath(fullContentName);

        // Assert
        actual.ShouldBe(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AudioPathResolver"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AudioPathResolver CreateSystemUnderTest() => new (this.mockAppService, this.mockFile, this.mockPath, this.mockPlatform);
}
