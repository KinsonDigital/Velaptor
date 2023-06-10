// <copyright file="TexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Content;
using Xunit;

/// <summary>
/// Tests the <see cref="TexturePathResolver"/> class.
/// </summary>
public class TexturePathResolverTests
{
    private const string ContentName = "test-content";
    private readonly string contentFilePath;
    private readonly string baseDir;
    private readonly string atlasContentDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturePathResolverTests"/> class.
    /// </summary>
    public TexturePathResolverTests()
    {
        this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
            .ToCrossPlatPath();
        this.atlasContentDir = $@"{this.baseDir}/Content/Graphics";
        this.contentFilePath = $"{this.atlasContentDir}/{ContentName}.png";
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new TexturePathResolver(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        var resolver = new TexturePathResolver(mockDirectory.Object);

        // Act
        var actual = resolver.ContentDirectoryName;

        // Assert
        actual.Should().Be("Graphics");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.png"))
            .Returns(
                new[]
                {
                    $"{this.baseDir}/other-file-A.png",
                    $"{this.baseDir}/other-file-B.txt"
                }
            );

        var resolver = new TexturePathResolver(mockDirectory.Object);

        // Act
        var act = () => _ = resolver.ResolveFilePath(ContentName);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The texture image file '{this.contentFilePath}' does not exist.");
    }

    [Theory]
    [InlineData("test-content")]
    [InlineData("test-content.png")]
    [InlineData("TEST-CONTENT.png")]
    public void ResolveFilePath_WhenInvoked_ResolvesFilepath(string contentName)
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.png"))
            .Returns(
                new[]
                {
                    $"{this.atlasContentDir}/other-file.png",
                    this.contentFilePath,
                }
            );

        var resolver = new TexturePathResolver(mockDirectory.Object);

        // Act
        var actual = resolver.ResolveFilePath(contentName);

        // Assert
        actual.Should().Be(this.contentFilePath);
    }
    #endregion
}
