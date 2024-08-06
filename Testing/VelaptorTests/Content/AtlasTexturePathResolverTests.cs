// <copyright file="AtlasTexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System.IO.Abstractions;
using FluentAssertions;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="AtlasTexturePathResolver"/> class.
/// </summary>
public class AtlasTexturePathResolverTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsFileDirectoryNameToCorrectResult()
    {
        // Arrange
        var mockAppService = Substitute.For<IAppService>();
        var mockFile = Substitute.For<IFile>();
        var mockPath = Substitute.For<IPath>();
        var mockPlatform = Substitute.For<IPlatform>();

        // Act
        var sut = new AtlasTexturePathResolver(mockAppService, mockFile, mockPath, mockPlatform);
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.Should().Be("Atlas");
    }
    #endregion
}
