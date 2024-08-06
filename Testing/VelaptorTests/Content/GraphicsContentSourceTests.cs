// <copyright file="GraphicsContentSourceTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="TexturePathResolver"/> class.
/// </summary>
public class GraphicsContentSourceTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsContentDirectoryNameToCorrectValue()
    {
        // Arrange
        var mockAppService = Substitute.For<IAppService>();
        var mockFile = Substitute.For<IFile>();
        var mockPath = Substitute.For<IPath>();
        var mockPlatform = Substitute.For<IPlatform>();

        // Act
        var source = new TexturePathResolver(mockAppService, mockFile, mockPath, mockPlatform);
        var actual = source.ContentDirectoryName;

        // Assert
        actual.Should().Be("Graphics");
    }
    #endregion
}
