// <copyright file="AtlasTexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System.IO.Abstractions;
using FluentAssertions;
using Moq;
using Velaptor.Content;
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
        var mockDirectory = new Mock<IDirectory>();

        // Act
        var sut = new AtlasTexturePathResolver(mockDirectory.Object);
        var actual = sut.ContentDirectoryName;

        // Assert
        actual.Should().Be("Atlas");
    }
    #endregion
}
