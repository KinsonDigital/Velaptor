// <copyright file="ShaderNameAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using Velaptor.OpenGL;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="ShaderNameAttribute"/> class.
/// </summary>
public class ShaderNameAttributeTests
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
            _ = new ShaderNameAttribute(value);
        }, "The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsProperty()
    {
        // Arrange & Act
        var attribute = new ShaderNameAttribute("test-name");

        // Assert
        Assert.Equal("test-name", attribute.Name);
    }
    #endregion
}
