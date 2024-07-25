// <copyright file="TexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using FluentAssertions;
using NSubstitute;
using Velaptor.Content;
using Velaptor.ExtensionMethods;
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
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TexturePathResolverTests"/> class.
    /// </summary>
    public TexturePathResolverTests()
    {
        this.baseDir = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
            .ToCrossPlatPath();
        this.atlasContentDir = $"{this.baseDir}/Content/Graphics";
        this.contentFilePath = $"{this.atlasContentDir}/{ContentName}.png";

        this.mockPath = Substitute.For<IPath>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new TexturePathResolver(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var mockDirectory = Substitute.For<IDirectory>();
        var resolver = new TexturePathResolver(mockDirectory);

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
        var mockDirectory = Substitute.For<IDirectory>();
        mockDirectory.GetFiles(this.atlasContentDir, "*.png")
            .Returns(
            [
                $"{this.baseDir}/other-file-A.png",
                    $"{this.baseDir}/other-file-B.txt"
            ]);

        var resolver = new TexturePathResolver(mockDirectory);

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
        var mockDirectory = Substitute.For<IDirectory>();
        mockDirectory.GetFiles(this.atlasContentDir, "*.png")
            .Returns(
            [
                $"{this.atlasContentDir}/other-file.png",
                    this.contentFilePath
            ]);

        var resolver = new TexturePathResolver(mockDirectory);

        // Act
        var actual = resolver.ResolveFilePath(contentName);

        // Assert
        actual.Should().Be(this.contentFilePath);
    }
    #endregion
}
