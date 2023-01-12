// <copyright file="TexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using Helpers;
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
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new TexturePathResolver(null);
        }, "The parameter must not be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();

        // Act
        var resolver = new TexturePathResolver(mockDirectory.Object);
        var actual = resolver.ContentDirectoryName;

        // Assert
        Assert.Equal("Graphics", actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.png"))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.baseDir}/other-file-A.png",
                    $"{this.baseDir}/other-file-B.txt",
                };
            });

        var resolver = new TexturePathResolver(mockDirectory.Object);

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<FileNotFoundException>(() =>
        {
            resolver.ResolveFilePath(ContentName);
        }, $"The texture image file '{this.contentFilePath}' does not exist.");
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
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.atlasContentDir}/other-file.png",
                    this.contentFilePath,
                };
            });

        var resolver = new TexturePathResolver(mockDirectory.Object);

        // Act
        var actual = resolver.ResolveFilePath(contentName);

        // Assert
        Assert.Equal(this.contentFilePath, actual);
    }
    #endregion
}
