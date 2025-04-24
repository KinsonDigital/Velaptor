// <copyright file="GpuBufferNameAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using Shouldly;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="GpuBufferNameAttribute"/> class.
/// </summary>
public class GpuBufferNameAttributeTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GpuBufferNameAttribute(null);

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WithEmptyParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new GpuBufferNameAttribute(string.Empty);

        // Assert
        var exception = Should.Throw<ArgumentException>(act);
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsProperty()
    {
        // Arrange & Act
        var attribute = new GpuBufferNameAttribute("test-name");

        // Assert
        attribute.Name.ShouldBe("test-name");
    }
    #endregion
}
