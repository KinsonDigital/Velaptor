// <copyright file="GraphicsContentSourceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System.IO.Abstractions;
using FluentAssertions;
using Moq;
using Velaptor.Content;
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
        var mockDirectory = new Mock<IDirectory>();

        // Act
        var source = new TexturePathResolver(mockDirectory.Object);
        var actual = source.ContentDirectoryName;

        // Assert
        actual.Should().Be("Graphics");
    }
    #endregion
}
