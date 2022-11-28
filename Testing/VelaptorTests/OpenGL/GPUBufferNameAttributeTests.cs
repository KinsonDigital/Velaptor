// <copyright file="GPUBufferNameAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using Velaptor.OpenGL;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="GPUBufferNameAttribute"/> class.
/// </summary>
public class GPUBufferNameAttributeTests
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
            _ = new GPUBufferNameAttribute(value);
        }, "The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsProperty()
    {
        // Arrange & Act
        var attribute = new GPUBufferNameAttribute("test-name");

        // Assert
        Assert.Equal("test-name", attribute.Name);
    }
    #endregion
}
