// <copyright file="AtlasJSONDataPathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using Shouldly;
using NSubstitute;
using Velaptor.Content;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasJSONDataPathResolver"/> class.
/// </summary>
public class AtlasJSONDataPathResolverTests
{
    private const string Extension = ".json";
    private readonly IAppService mockAppService;
    private readonly IFile mockFile;
    private readonly IPath mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasJSONDataPathResolverTests"/> class.
    /// </summary>
    public AtlasJSONDataPathResolverTests()
    {
        this.mockAppService = Substitute.For<IAppService>();

        this.mockAppService.AppDirectory.Returns("AppHome");
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
            { string.Empty, $"test-content{Extension}", $"AppHome/Content/Atlas/test-content{Extension}" },
            { string.Empty, $"TEST-CONTENT{Extension}", $"AppHome/Content/Atlas/TEST-CONTENT{Extension}" },
            { string.Empty, "test-content", $"AppHome/Content/Atlas/test-content{Extension}" },
            { "sub-dir", $"test-content{Extension}", $"AppHome/Content/Atlas/sub-dir/test-content{Extension}" },
        };
    }
    #endregion
#pragma warning restore SA1514

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var resolver = new AtlasJSONDataPathResolver(this.mockAppService, this.mockFile, this.mockPath);

        // Act
        var actual = resolver.ContentDirectoryName;

        // Assert
        actual.ShouldBe("Atlas");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ResolveFilePath_WithNullParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => _ = sut.ResolveFilePath(null);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'contentPathOrName')");
    }

    [Fact]
    public void ResolveFilePath_WithEmptyParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => _ = sut.ResolveFilePath(string.Empty);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'contentPathOrName')");
    }

    [Theory]
    [MemberData(nameof(ResolveFilePath_WhenContentNameDoesNotExist_Data))]
    public void ResolveFilePath_WhenInvoked_ResolvesFilePath(
        string subDir,
        string contentName,
        string expected)
    {
        // Arrange
        this.mockPath.HasExtension(Arg.Any<string>()).Returns((path) => Path.HasExtension(path.Arg<string>()));
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ResolveFilePath(string.IsNullOrEmpty(subDir) ? contentName : $"{subDir}/{contentName}");

        // Assert
        actual.ShouldBe(expected);
        this.mockFile.Received(1).Exists(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AtlasJSONDataPathResolver"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AtlasJSONDataPathResolver CreateSystemUnderTest() => new (this.mockAppService, this.mockFile, this.mockPath);
}
