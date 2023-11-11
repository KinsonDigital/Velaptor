// <copyright file="ContentFontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using FluentAssertions;
using Moq;
using Velaptor.Content.Fonts;
using Velaptor.ExtensionMethods;
using Xunit;

/// <summary>
/// Tests the <see cref="ContentFontPathResolver"/> class.
/// </summary>
public class ContentFontPathResolverTests
{
    private const string ContentName = "test-content";
    private readonly string contentFilePath;
    private readonly string atlasContentDir;
    private readonly Mock<IDirectory> mockDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentFontPathResolverTests"/> class.
    /// </summary>
    public ContentFontPathResolverTests()
    {
        this.mockDirectory = new Mock<IDirectory>();
        var baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}"
            .ToCrossPlatPath();
        var baseContentDir = $@"{baseDir}/Content";
        this.atlasContentDir = $@"{baseContentDir}/Fonts";
        this.contentFilePath = $"{this.atlasContentDir}/{ContentName}.ttf";
    }

    #region Constructor Tests

    [Fact]
    public void Ctor_WithNullDirectoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ContentFontPathResolver(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'Directory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
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
    [InlineData("test-content")]
    [InlineData("test-content.ttf")]
    [InlineData("TEST-CONTENT.ttf")]
    public void ResolveFilePath_WhenInvoked_ResolvesFilePath(string contentName)
    {
        // Arrange
        this.mockDirectory.Setup(m => m.GetFiles(this.atlasContentDir, "*.ttf"))
            .Returns(() =>
            {
                return new[]
                {
                    $"{this.atlasContentDir}/other-file.txt",
                    this.contentFilePath,
                };
            });

        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath(contentName);

        // Assert
        actual.Should().Be(this.contentFilePath);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ContentFontPathResolver"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ContentFontPathResolver CreateSystemUnderTest() => new (this.mockDirectory.Object);
}
