// <copyright file="GpuBufferNameAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using Helpers;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="GpuBufferNameAttribute"/> class.
/// </summary>
public class GpuBufferNameAttributeTests
{
    #region Constructor Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNullOrEmptyNameParam_ThrowsException(string value)
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new GpuBufferNameAttribute(value);
        }, "The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsProperty()
    {
        // Arrange & Act
        var attribute = new GpuBufferNameAttribute("test-name");

        // Assert
        Assert.Equal("test-name", attribute.Name);
    }
    #endregion
}
