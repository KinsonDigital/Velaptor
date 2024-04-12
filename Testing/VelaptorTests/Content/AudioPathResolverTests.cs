// <copyright file="AudioPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
using Xunit;

/// <summary>
/// Tests the <see cref="AudioPathResolver"/> class.
/// </summary>
public class AudioPathResolverTests
{
    private const string ContentName = "test-content";
    private readonly string contentFilePath;
    private readonly string baseDir;
    private readonly string atlasContentDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPathResolverTests"/> class.
    /// </summary>
    public AudioPathResolverTests()
    {
        this.baseDir = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
            .ToCrossPlatPath();
        this.atlasContentDir = $"{this.baseDir}/Content/Audio";
        this.contentFilePath = $"{this.atlasContentDir}/{ContentName}";
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioPathResolver(null);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();

        // Act
        var source = new AudioPathResolver(mockDirectory.Object);
        var actual = source.ContentDirectoryName;

        // Assert
        actual.Should().Be("Audio");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.baseDir}/other-file-A.png",
                    $"{this.baseDir}/other-file-B.txt",
                };
            });

        var resolver = new AudioPathResolver(mockDirectory.Object);

        // Act
        var act = () => resolver.ResolveFilePath(ContentName);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The audio file '{this.contentFilePath}' does not exist.");
    }

    [Theory]
    [InlineData(".ogg", ".OGG")]
    [InlineData(".ogg", ".ogg")]
    [InlineData(".OGG", ".ogg")]
    [InlineData(".mp3", ".ogg")]
    [InlineData(".mp3", ".OGG")]
    [InlineData(".MP3", ".ogg")]
    [InlineData(".MP3", ".OGG")]
    public void ResolveFilePath_WhenBothOggAndMp3FilesExist_ResolvesToOggFile(string resolvePathExtension, string actualFileExtension)
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.atlasContentDir}/other-file.txt",
                    $"{this.contentFilePath}{actualFileExtension}",
                    $"{this.contentFilePath}.mp3",
                };
            });

        var resolver = new AudioPathResolver(mockDirectory.Object);

        // Act
        var actual = resolver.ResolveFilePath($"{ContentName}{resolvePathExtension}");

        // Assert
        actual.Should().Be($"{this.contentFilePath}{actualFileExtension}");
    }

    [Fact]
    public void ResolveFilePath_WhenOnlyMp3FilesExist_ResolvesToMp3File()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.atlasContentDir}other-file.txt",
                    $"{this.contentFilePath}.mp3",
                };
            });

        var resolver = new AudioPathResolver(mockDirectory.Object);

        // Act
        var actual = resolver.ResolveFilePath($"{ContentName}.mp3");

        // Assert
        actual.Should().Be($"{this.contentFilePath}.mp3");
    }
    #endregion
}
