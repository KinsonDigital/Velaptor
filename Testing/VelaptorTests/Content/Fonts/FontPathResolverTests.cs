// <copyright file="FontPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts;

using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using FluentAssertions;
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Content.Fonts;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="FontPathResolver"/> class.
/// </summary>
public class FontPathResolverTests
{
    private const string Extension = ".ttf";
    private readonly IAppService mockAppService;
    private readonly IFile mockFile;
    private readonly IPath mockPath;
    private readonly IPlatform mockPlatform;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPathResolverTests"/> class.
    /// </summary>
    public FontPathResolverTests()
    {
        this.mockPlatform = Substitute.For<IPlatform>();
        this.mockPlatform.CurrentPlatform.Returns(OSPlatform.Windows);

        this.mockAppService = Substitute.For<IAppService>();
        this.mockAppService.AppDirectory.Returns(_ => this.mockPlatform.CurrentPlatform == OSPlatform.Windows ? @"C:\app" : "/app");

        this.mockFile = Substitute.For<IFile>();
        this.mockFile.Exists(Arg.Any<string>()).Returns(true);

        this.mockPath = Substitute.For<IPath>();
        this.mockPath.DirectorySeparatorChar.Returns(Path.DirectorySeparatorChar);
        this.mockPath.AltDirectorySeparatorChar.Returns(Path.AltDirectorySeparatorChar);
    }

#pragma warning disable SA1514
    #region Test Data
    /// <summary>
    /// Provides test data for the <see cref="ResolveFilePath_WhenInvoked_ResolvesFilePath"/> test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string, string> ResolveFilePath_WhenContentNameDoesNotExist_Data()
    {
        return new TheoryData<string, string, string>
        {
            { string.Empty, $"test-content{Extension}", $@"C:\app\Content\Fonts\test-content{Extension}" },
            { string.Empty, $"test-content{Extension}", $@"C:\app\Content\Fonts\test-content{Extension}" },
            { string.Empty, $"TEST-CONTENT{Extension}", $@"C:\app\Content\Fonts\TEST-CONTENT{Extension}" },
            {
                $@"sub-dir\",
                $"test-content{Extension}",
                $@"C:\app\Content\Fonts\sub-dir\test-content{Extension}"
            },
        };
    }
    #endregion
#pragma warning restore SA1514

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var resolver = new FontPathResolver(this.mockAppService, this.mockFile, this.mockPath, this.mockPlatform);

        // Act
        var actual = resolver.ContentDirectoryName;

        // Assert
        actual.Should().Be("Fonts");
    }
    #endregion

    #region Methods Tests
    [Fact]
    public void ResolveFilePath_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => _ = sut.ResolveFilePath(null);

        // Assert
        act.Should().ThrowArgNullException()
            .WithMessage("Value cannot be null. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void ResolveFilePath_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => _ = sut.ResolveFilePath(string.Empty);

        // Assert
        act.Should().ThrowArgException()
            .WithMessage("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Theory]
    [MemberData(nameof(ResolveFilePath_WhenContentNameDoesNotExist_Data))]
    public void ResolveFilePath_WhenInvoked_ResolvesFilePath(
        string dirPath,
        string contentName,
        string expected)
    {
        // Arrange
        this.mockPath.HasExtension(Arg.Any<string>()).Returns((path) => Path.HasExtension(path.Arg<string>()));
        var contentFilePath = $@"C:\app\Content\Fonts" +
                              $@"\{dirPath}{contentName}{(Path.HasExtension(contentName) ? string.Empty : Extension)}";
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath($"{dirPath}{contentName}");

        // Assert
        actual.Should().Be(expected);
        this.mockFile.Received(1).Exists(contentFilePath);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontPathResolver"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontPathResolver CreateSystemUnderTest() => new (this.mockAppService, this.mockFile, this.mockPath, this.mockPlatform);
}
