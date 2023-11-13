// <copyright file="AtlasJSONDataPathResolverTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="AtlasJSONDataPathResolver"/> class.
/// </summary>
public class AtlasJSONDataPathResolverTests
{
    private const string ContentName = "test-content";
    private readonly string contentFilePath;
    private readonly string baseDir;
    private readonly string atlasContentDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasJSONDataPathResolverTests"/> class.
    /// </summary>
    public AtlasJSONDataPathResolverTests()
    {
        this.baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
            .ToCrossPlatPath();
        this.atlasContentDir = $@"{this.baseDir}/Content/Atlas";
        this.contentFilePath = $"{this.atlasContentDir}/{ContentName}.json";
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AtlasJSONDataPathResolver(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'directory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();

        // Act
        var sut = new AtlasJSONDataPathResolver(mockDirectory.Object);
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.Should().Be("Atlas");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveFilePath_WhenContentItemDoesNotExist_ThrowsException()
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.json"))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.baseDir}/other-file-A.json",
                    $"{this.baseDir}/other-file-B.txt",
                };
            });

        var sut = new AtlasJSONDataPathResolver(mockDirectory.Object);

        // Act
        var act = () => sut.ResolveFilePath(ContentName);

        // Assert
        act.Should().Throw<FileNotFoundException>()
            .WithMessage($"The texture atlas data file '{this.contentFilePath}' does not exist.");
    }

    [Theory]
    [InlineData("test-content")]
    [InlineData("test-content.json")]
    [InlineData("TEST-CONTENT.json")]
    public void ResolveFilePath_WhenInvoked_ResolvesFilepath(string contentName)
    {
        // Arrange
        var mockDirectory = new Mock<IDirectory>();
        mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.json"))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.atlasContentDir}/other-file.json",
                    this.contentFilePath,
                };
            });

        var sut = new AtlasJSONDataPathResolver(mockDirectory.Object);

        // Act
        var actual = sut.ResolveFilePath(contentName);

        // Assert
        actual.Should().Be(this.contentFilePath);
    }
    #endregion
}
