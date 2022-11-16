// <copyright file="AtlasTexturePathResolverTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.IO.Abstractions;
using Moq;
using Velaptor.Content;
using Xunit;

namespace VelaptorTests.Content;

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
        var resolver = new AtlasTexturePathResolver(mockDirectory.Object);
        var actual = resolver.ContentDirectoryName;

        // Assert
        Assert.Equal("Atlas", actual);
    }
    #endregion
}
